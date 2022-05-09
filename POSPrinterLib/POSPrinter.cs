#region System
using System;
using System.Text;
using System.Drawing;
#endregion

#region POS
using POS.Devices;
#endregion

using SharpRambo.ExtensionsLib;

namespace SharpRambo.POSPrinterLib
{
    public class POSPrinter
    {
        #region Events
        public class StatusUpdateEventArgs
        {
            public Enum Status { get; }
            public UsageStatus UsageStatus { get; }
            public StatusType Type { get; }

            public StatusUpdateEventArgs(Enum status, StatusType type, UsageStatus usageStatus = UsageStatus.Unknown)
            {
                Status = status;
                Type = type;
                UsageStatus = usageStatus;
            }
        }

        public delegate void StatusUpdateEventHandler(object sender, StatusUpdateEventArgs e);
        public event StatusUpdateEventHandler StatusUpdateEvent;
        #endregion

        private bool _CoverOpened = false;
        private bool _PaperEmpty = false;
        private bool _PaperRefill = false;

        public Station StationID { get; set; } = Station.Receipt;
        public bool ReadyToPrint { get; private set; } = false;

        private bool _TransactionMode = false;
        public bool TransactionMode
        {
            get => _TransactionMode;
            set {
                _TransactionMode = value;
                _Printer.TransactionPrint((int)StationID, value ? 11 : 12);
            }
        }

        public bool CheckCoverClose { get; set; } = true;
        public bool AsyncMode { get; set; } = true;
        public string[] CheckCoverCloseMessage { get; set; }

        #region OPosPosFields
        // Set Bondrucker
        private readonly OPOSPOSPrinter _Printer =
#if NET5_0_OR_GREATER
            new();
#else
            new OPOSPOSPrinter();
#endif

        public OPOSPOSPrinter OPosPrinter => _Printer;

        public string ModelName { get; }

        //printer.DeviceEnabled = true;
        public static readonly string BOLD = Encoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'b', (byte)'C' });
        public static readonly string UNDERLINE = Encoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'2', (byte)'u', (byte)'C' });
        public static readonly string ITALIC = Encoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'i', (byte)'C' });
        public static readonly string LEFT_ALIGN = Encoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'l', (byte)'A' });
        public static readonly string CENTER_ALIGN = Encoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'c', (byte)'A' });
        public static readonly string RIGHT_ALIGN = Encoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'r', (byte)'A' });
        public static readonly string DEFAULT_CHARACTERS = Encoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'1', (byte)'C' });
        public static readonly string DOUBLE_WIDE_CHARACTERS = Encoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'2', (byte)'C' });
        public static readonly string DOUBLE_HEIGHT_CHARACTERS = Encoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'3', (byte)'C' });
        public static readonly string DOUBLE_WIDE_AND_HEIGHT_CHARACTERS = Encoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'4', (byte)'C' });
        public static readonly string LINE_FEED = Encoding.ASCII.GetString(new byte[] { 27, (byte)'|', (byte)'d' });
        public static readonly string DIVIDER = "------------------------------------------";
#endregion

        public Enum Status { get; private set; }

        public PowerStatus PowerStatus { get; private set; }
        public GeneralStatus GeneralStatus { get; private set; }
        public ReceiptStatus ReceiptStatus { get; private set; }
        public JournalStatus JournalStatus { get; private set; }
        public SlipStatus SlipStatus { get; private set; }
        public UsageStatus UsageStatus { get; private set; }

        public POSPrinter(string modelName, bool asyncMode = true, uint timeout = 1000, Station stationID = Station.Receipt)
        {
            if (!modelName.IsNull()) {
                try {
                    StationID = stationID;
                    AsyncMode = asyncMode;
                    int open = _Printer.Open(modelName);

                    if (open == 0) {
                        _Printer.ClaimDevice(Convert.ToInt32(timeout));
                        _Printer.PowerNotify = 1;

                        if (_Printer.Claimed) {
                            _Printer.StatusUpdateEvent += onStatusUpdate;
                            _Printer.DeviceEnabled = true;
                            _Printer.AsyncMode = AsyncMode;
                            ModelName = modelName;
                            UsageStatus = UsageStatus.Ready;
                        } else
                            UsageStatus = UsageStatus.Offline;
                    } else
                        UsageStatus = UsageStatus.Offline;
                        
                } catch (Exception ex) {
                    _ = ex.Message;
                    Close();
                    _Printer = null;
                    UsageStatus = UsageStatus.Error;
                }
            }
        }

        public bool PrintLine(string data, NewLine newLine = NewLine.LF, int cutPaper = -1)
        {
            try {
                string nL = getLineFeed(newLine);
                _Printer.PrintNormal((int)StationID, data + nL);

#if NET5_0_OR_GREATER
                if (cutPaper is > 0 and <= 100)
#else
                if (cutPaper > 0 && cutPaper <= 100)
#endif
                    CutPaper(Convert.ToUInt32(cutPaper));
                else if (cutPaper < -1)
                    CutPaper();

                return true;
            } catch (Exception) {
                return false;
            }
        }

        public bool PrintLines(string[] lines, NewLine newLine = NewLine.LF, NewLine lastNewLine = NewLine.LF)
        {
            bool result = true;

            for (int i = 0; i < lines.Length; i++)
                result = PrintLine(lines[i], i == lines.Length - 1 ? lastNewLine : newLine) && result;

            return result;
        }

        public bool PrintLines(NewLine newLine, NewLine lastNewLine, params string[] lines)
            => PrintLines(lines, newLine, lastNewLine);

        public bool PrintLines(NewLine lastNewLine, params string[] lines)
            => PrintLines(lines, NewLine.LF, lastNewLine);

        public bool PrintLines(params string[] lines)
            => PrintLines(lines, NewLine.LF, NewLine.LF);

        public bool PrintDivider(NewLine newLine = NewLine.LF)
            => PrintLine(DIVIDER, NewLine.LF);

        public bool PrintBarCode(string data, BarCode barCodeType, int height, int width, Alignment alignment = Alignment.Center, TextPosition textPosition = TextPosition.Above)
        {
            try {
                _Printer.PrintBarCode(2, data, Convert.ToInt32(barCodeType), height, width, Convert.ToInt32(alignment), Convert.ToInt32(textPosition));
                return true;
            } catch (Exception) {
                return false;
            }
        }
        public bool PrintBarCode(string data, BarCode barCodeType, Size size, Alignment alignment = Alignment.Center, TextPosition textPosition = TextPosition.Above)
            => PrintBarCode(data, barCodeType, size.Height, size.Width, alignment, textPosition);

        public bool PrintBitmap(string filePath, int width, Alignment alignment = Alignment.Center)
        {
            try {
                _Printer.PrintBitmap((int)StationID, filePath, width, (int)alignment);
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public bool CutPaper(uint percentage = 95)
        {
            try {
                _Printer.CutPaper(percentage <= 100 ? Convert.ToInt32(percentage) : 0);
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public bool ReleasePaper(bool cutPaper = true, int cutPaperPercentage = -1)
            => PrintLine("\n\r\n\r\n\r\n\r\n\r\n\r\n\r", NewLine.None, cutPaper
#if NET5_0_OR_GREATER
                ? (cutPaperPercentage is >= 0 and <= 100 ? cutPaperPercentage : -2)
#else
                ? (cutPaperPercentage >= 0 && cutPaperPercentage <= 100 ? cutPaperPercentage : -2)
#endif
                : -1
            );

        public int Close()
        {
            if (_Printer != null) {
                _Printer.ReleaseDevice();
                return _Printer.Close();
            } else
                return 9999;
        }

        private static string getLineFeed(NewLine newLine = NewLine.LF)
#if NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            => newLine switch {
                NewLine.LF => "\n",
                NewLine.LF_Double => getLineFeed() + getLineFeed(),
                NewLine.CRLF => getLineFeed() + "\r",
                NewLine.CRLF_Double => getLineFeed(NewLine.CRLF) + getLineFeed(NewLine.CRLF),
                NewLine.System => LINE_FEED,
                _ => string.Empty, // Printer.NewLine.None
            };
#else
        {
            switch(newLine) {
                case NewLine.LF:
                    return "\n";

                case NewLine.LF_Double:
                    return getLineFeed() + getLineFeed();

                case NewLine.CRLF:
                    return getLineFeed() + "\r";

                case NewLine.CRLF_Double:
                    return getLineFeed(NewLine.CRLF) + getLineFeed(NewLine.CRLF);

                case NewLine.System:
                    return LINE_FEED;

                default: // Printer.NewLine.None
                    return string.Empty;
            }
        }
#endif

        private UsageStatus mapStatus(Enum statusEnum, StatusType type)
        {
            UsageStatus usage = UsageStatus.Unknown;

            switch (type) {
                case StatusType.General:
                    GeneralStatus general = (GeneralStatus)statusEnum;

                    if (general == GeneralStatus.CoverOpen) {
                        usage = UsageStatus.CoverOpen;
                        _CoverOpened = true;
                        ReadyToPrint = false;
                    } else {
                        usage = _PaperEmpty
                        ? UsageStatus.PaperEmpty
                        : (_PaperRefill
                            ? UsageStatus.PaperRefill
                            : (general == GeneralStatus.Progress
                                ? UsageStatus.Busy
                                : (general == GeneralStatus.FirmwareUpdateFailed
                                    ? UsageStatus.Warning
                                    : UsageStatus.Ready)));

                        ReadyToPrint = !_PaperEmpty && general != GeneralStatus.Progress && general != GeneralStatus.FirmwareUpdateFailed;
                    }

                    break;

                case StatusType.Power:
                    PowerStatus power = (PowerStatus)statusEnum;

                    if (GeneralStatus != GeneralStatus.CoverOpen && ReceiptStatus != ReceiptStatus.CoverOpen && JournalStatus != JournalStatus.CoverOpen && SlipStatus != SlipStatus.CoverOpen)
                    {
#if NET5_0_OR_GREATER
                        if (power is PowerStatus.Off or PowerStatus.Offline or PowerStatus.OffOffline) {
#else
                        if (power == PowerStatus.Off || power == PowerStatus.Offline || power == PowerStatus.OffOffline) {
#endif
                            usage = UsageStatus.Offline;
                            ReadyToPrint = false;
                        } else if (power == PowerStatus.Online) {
                            usage = !_PaperEmpty
                                ? (!_PaperRefill
                                    ? UsageStatus.Ready
                                    : UsageStatus.PaperRefill)
                                : UsageStatus.PaperEmpty;

                            ReadyToPrint = !_PaperEmpty;
                        }
                    } else {
                        usage = UsageStatus.CoverOpen;
                        _CoverOpened = true;
                        ReadyToPrint = false;
                    }

                    break;

                case StatusType.Journal:
                    JournalStatus journal = (JournalStatus)statusEnum;

#if NET5_0_OR_GREATER
                    if (journal is JournalStatus.CartridgeEmpty or JournalStatus.PaperEmpty or JournalStatus.CoverOpen) {
#else
                    if (journal == JournalStatus.CartridgeEmpty || journal == JournalStatus.PaperEmpty || journal == JournalStatus.CoverOpen) {
#endif
                        usage = UsageStatus.Warning;
                        ReadyToPrint = false;
#if NET5_0_OR_GREATER
                    } else if (journal is JournalStatus.CartridgeCleanHead or JournalStatus.CartridgeRefill) {
#else
                    } else if (journal == JournalStatus.CartridgeCleanHead || journal == JournalStatus.CartridgeRefill) {
#endif
                        usage = UsageStatus.Maintenence;
                        ReadyToPrint = false;
                    } else if (journal == JournalStatus.PaperRefill) {
                        usage = UsageStatus.PaperRefill;
                        ReadyToPrint = true;
                    } else if (journal == JournalStatus.CoverOpen) {
                        usage = UsageStatus.CoverOpen;
                        _CoverOpened = true;
                        ReadyToPrint = false;
                    } else {
                        usage = UsageStatus.Ready;
                        ReadyToPrint = true;
                    }

                    break;

                case StatusType.Receipt:
                    ReceiptStatus receipt = (ReceiptStatus)statusEnum;

#if NET5_0_OR_GREATER
                    if (receipt is ReceiptStatus.CartridgeEmpty) {
#else
                    if (receipt == ReceiptStatus.CartridgeEmpty || receipt == ReceiptStatus.CoverOpen) {
#endif
                        usage = UsageStatus.Warning;
                        ReadyToPrint = false;
#if NET5_0_OR_GREATER
                    } else if (receipt is ReceiptStatus.CartridgeCleanHead or ReceiptStatus.CartridgeRefill) {
#else
                    } else if (receipt == ReceiptStatus.CartridgeCleanHead || receipt == ReceiptStatus.CartridgeRefill) {
#endif
                        usage = UsageStatus.Maintenence;
                        ReadyToPrint = false;
                    } else if (receipt == ReceiptStatus.PaperEmpty) {
                        usage = UsageStatus.PaperEmpty;
                        _PaperEmpty = true;
                        ReadyToPrint = false;
                    } else if (receipt == ReceiptStatus.PaperRefill) {
                        usage = UsageStatus.PaperRefill;
                        _PaperRefill = true;
                        ReadyToPrint = true;
                    } else if (receipt == ReceiptStatus.PaperOK) {
                        usage = UsageStatus.Ready;
                        _PaperEmpty = false;
                        _PaperRefill = false;
                        ReadyToPrint = true;
                    } else if (receipt == ReceiptStatus.CoverOpen) {
                        usage = UsageStatus.CoverOpen;
                        _CoverOpened = true;
                        ReadyToPrint = false;
                    } else {
                        usage = !_PaperEmpty
                                ? (!_PaperRefill
                                    ? UsageStatus.Ready
                                    : UsageStatus.PaperRefill)
                                : UsageStatus.PaperEmpty;

                        ReadyToPrint = !_PaperEmpty;
                    }

                    break;

                case StatusType.Slip:
                    SlipStatus slip = (SlipStatus)statusEnum;

#if NET5_0_OR_GREATER
                    if (slip is SlipStatus.CartridgeEmpty or SlipStatus.PaperEmpty or SlipStatus.CoverOpen) {
#else
                    if (slip == SlipStatus.CartridgeEmpty || slip == SlipStatus.PaperEmpty || slip == SlipStatus.CoverOpen) {
#endif
                        usage = UsageStatus.Warning;
                        ReadyToPrint = false;
#if NET5_0_OR_GREATER
                    } else if (slip is SlipStatus.CartridgeCleanHead or SlipStatus.CartridgeRefill) {
#else
                    } else if (slip == SlipStatus.CartridgeCleanHead || slip == SlipStatus.CartridgeRefill) {
#endif
                        usage = UsageStatus.Maintenence;
                        ReadyToPrint = false;
                    } else if (slip == SlipStatus.PaperRefill) {
                        usage = UsageStatus.PaperRefill;
                        ReadyToPrint = true;
                    } else if (slip == SlipStatus.CoverOpen) {
                        usage = UsageStatus.CoverOpen;
                        _CoverOpened = true;
                        ReadyToPrint = false;
                    } else {
                        usage = UsageStatus.Ready;
                        ReadyToPrint = true;
                    }

                    break;
            }

            return usage;
        }

        private void onStatusUpdate(int data)
        {
            Type powerStatusType = typeof(PowerStatus);
            Type generalStatusType = typeof(GeneralStatus);
            Type receiptStatusType = typeof(ReceiptStatus);
            Type journalStatusType = typeof(JournalStatus);
            Type slipStatusType = typeof(SlipStatus);

            StatusType type = StatusType.Power;

            if (Enum.IsDefined(powerStatusType, data)) {
                PowerStatus = (PowerStatus)Enum.ToObject(powerStatusType, data);
                Status = PowerStatus;
            } else if (Enum.IsDefined(generalStatusType, data)) {
                GeneralStatus = (GeneralStatus)Enum.ToObject(generalStatusType, data);
                Status = GeneralStatus;
                type = StatusType.General;
            } else if (Enum.IsDefined(receiptStatusType, data)) {
                ReceiptStatus = (ReceiptStatus)Enum.ToObject(receiptStatusType, data);
                Status = ReceiptStatus;
                type = StatusType.Receipt;
            } else if (Enum.IsDefined(journalStatusType, data)) {
                JournalStatus = (JournalStatus)Enum.ToObject(journalStatusType, data);
                Status = JournalStatus;
                type = StatusType.Journal;
            } else if (Enum.IsDefined(slipStatusType, data)) {
                SlipStatus = (SlipStatus)Enum.ToObject(slipStatusType, data);
                Status = SlipStatus;
                type = StatusType.Slip;
            } else
                Status = PowerStatus.Unknown;

            UsageStatus = mapStatus(Status, type);

            if (GeneralStatus == GeneralStatus.CoverOK) {
                if (CheckCoverClose && _CoverOpened) {
                    _Printer.CheckHealth(1);

                    if (CheckCoverCloseMessage.IsNull<string>())
                        CheckCoverCloseMessage = new[] {
                            BOLD + UNDERLINE + DOUBLE_WIDE_AND_HEIGHT_CHARACTERS + CENTER_ALIGN + "System Message" + getLineFeed(),
                            DOUBLE_WIDE_CHARACTERS + CENTER_ALIGN + "Cover closed!" + getLineFeed(),
                            DIVIDER,
                            "Device: " + _Printer.DeviceDescription + getLineFeed(),
                            "ServiceObjectVersion: " + _Printer.ServiceObjectVersion.ToString(),
                            _Printer.ServiceObjectDescription + getLineFeed(),
                            "ControlObjectVersion: " + _Printer.ControlObjectVersion.ToString(),
                            _Printer.ControlObjectDescription + getLineFeed(),
                            _Printer.CheckHealthText
                        };

                    TransactionMode = true;
                    PrintLines(CheckCoverCloseMessage);
                    PrintDivider();
                    ReleasePaper();
                    TransactionMode = false;
                }

                _CoverOpened = false;
            }

            if (StatusUpdateEvent != null)
                StatusUpdateEvent.Invoke(this, new StatusUpdateEventArgs(Status, type, UsageStatus));
        }
    }
}
