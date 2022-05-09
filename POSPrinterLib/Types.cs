using POS.Devices;

namespace SharpRambo.POSPrinterLib
{
    public enum StatusType
    {
        Power,
        Usage,
        General,
        Journal,
        Receipt,
        Slip
    }

    public enum PowerStatus
    {
        Online = OPOS_Constants.OPOS_PS_ONLINE,
        Off = OPOS_Constants.OPOS_PS_OFF,
        Offline = OPOS_Constants.OPOS_PS_OFFLINE,
        OffOffline = OPOS_Constants.OPOS_PS_OFF_OFFLINE,
        Unknown = OPOS_Constants.OPOS_PS_UNKNOWN,
    }

    public enum JournalStatus
    {
        // Cartridge
        CartridgeEmpty = OPOSPOSPrinterConstants.PTR_SUE_JRN_CARTRIDGE_EMPTY,
        CartridgeRefill = OPOSPOSPrinterConstants.PTR_SUE_JRN_CARTRIDGE_NEAREMPTY,
        CartridgeCleanHead = OPOSPOSPrinterConstants.PTR_SUE_JRN_HEAD_CLEANING,
        CartridgeOK = OPOSPOSPrinterConstants.PTR_SUE_JRN_CARTRIDGE_OK,
        // Cover
        CoverOpen = OPOSPOSPrinterConstants.PTR_SUE_JRN_COVER_OPEN,
        CoverOK = OPOSPOSPrinterConstants.PTR_SUE_JRN_COVER_OK,
        // Paper
        PaperEmpty = OPOSPOSPrinterConstants.PTR_SUE_JRN_EMPTY,
        PaperRefill = OPOSPOSPrinterConstants.PTR_SUE_JRN_NEAREMPTY,
        PaperOK = OPOSPOSPrinterConstants.PTR_SUE_JRN_PAPEROK
    }

    public enum ReceiptStatus
    {
        // Cartridge
        CartridgeEmpty = OPOSPOSPrinterConstants.PTR_SUE_REC_CARTRIDGE_EMPTY,
        CartridgeRefill = OPOSPOSPrinterConstants.PTR_SUE_REC_CARTRIDGE_NEAREMPTY,
        CartridgeCleanHead = OPOSPOSPrinterConstants.PTR_SUE_REC_HEAD_CLEANING,
        CartridgeOK = OPOSPOSPrinterConstants.PTR_SUE_REC_CARTRIDGE_OK,
        // Cover
        CoverOpen = OPOSPOSPrinterConstants.PTR_SUE_REC_COVER_OPEN,
        CoverOK = OPOSPOSPrinterConstants.PTR_SUE_REC_COVER_OK,
        // Paper
        PaperEmpty = OPOSPOSPrinterConstants.PTR_SUE_REC_EMPTY,
        PaperRefill = OPOSPOSPrinterConstants.PTR_SUE_REC_NEAREMPTY,
        PaperOK = OPOSPOSPrinterConstants.PTR_SUE_REC_PAPEROK
    }

    public enum SlipStatus
    {
        // Cartridge
        CartridgeEmpty = OPOSPOSPrinterConstants.PTR_SUE_SLP_CARTRIDGE_EMPTY,
        CartridgeRefill = OPOSPOSPrinterConstants.PTR_SUE_SLP_CARTRIDGE_NEAREMPTY,
        CartridgeCleanHead = OPOSPOSPrinterConstants.PTR_SUE_SLP_HEAD_CLEANING,
        CartridgeOK = OPOSPOSPrinterConstants.PTR_SUE_SLP_CARTRIDGE_OK,
        // Cover
        CoverOpen = OPOSPOSPrinterConstants.PTR_SUE_SLP_COVER_OPEN,
        CoverOK = OPOSPOSPrinterConstants.PTR_SUE_SLP_COVER_OK,
        // Paper
        PaperEmpty = OPOSPOSPrinterConstants.PTR_SUE_SLP_EMPTY,
        PaperRefill = OPOSPOSPrinterConstants.PTR_SUE_SLP_NEAREMPTY,
        PaperOK = OPOSPOSPrinterConstants.PTR_SUE_SLP_PAPEROK
    }

    public enum GeneralStatus
    {
        // Processing
        Idle = OPOSPOSPrinterConstants.PTR_SUE_IDLE,
        Progress = OPOS_Constants.OPOS_SUE_UF_PROGRESS,
        Complete = OPOS_Constants.OPOS_SUE_UF_COMPLETE,

        // General
        CoverOpen = OPOSPOSPrinterConstants.PTR_SUE_COVER_OPEN,
        CoverOK = OPOSPOSPrinterConstants.PTR_SUE_COVER_OK,

        // Firmware
        FirmwareUpdateFailed = 2201,
    }

    public enum UsageStatus
    {
        Ready,
        Busy,
        Offline,
        Warning,
        Maintance,
        CoverOpen,
        PaperEmpty,
        PaperRefill,
        Error,
        Unknown
    }

    public enum Station
    {
        Journal = OPOSPOSPrinterConstants.PTR_S_JOURNAL,
        Receipt = OPOSPOSPrinterConstants.PTR_S_RECEIPT,
        JournalReceipt = OPOSPOSPrinterConstants.PTR_S_JOURNAL_RECEIPT,
        Slip = OPOSPOSPrinterConstants.PTR_S_SLIP,
        JournalSlip = OPOSPOSPrinterConstants.PTR_S_JOURNAL_SLIP,
        ReceiptSLip = OPOSPOSPrinterConstants.PTR_S_RECEIPT_SLIP
    }

    public enum NewLine
    {
        LF,
        LF_Double,
        CRLF,
        CRLF_Double,
        System,
        None
    }

    public enum BarCode
    {
        UPCA = OPOSPOSPrinterConstants.PTR_BCS_UPCA,
        UPCE = OPOSPOSPrinterConstants.PTR_BCS_UPCE,
        EAN8 = OPOSPOSPrinterConstants.PTR_BCS_EAN8,
        JAN8 = OPOSPOSPrinterConstants.PTR_BCS_JAN8,
        EAN13 = OPOSPOSPrinterConstants.PTR_BCS_EAN13,
        JAN13 = OPOSPOSPrinterConstants.PTR_BCS_JAN13,
        TF = OPOSPOSPrinterConstants.PTR_BCS_TF,
        ITF = OPOSPOSPrinterConstants.PTR_BCS_ITF,
        Codabar = OPOSPOSPrinterConstants.PTR_BCS_Codabar,
        Code39 = OPOSPOSPrinterConstants.PTR_BCS_Code39,
        Code93 = OPOSPOSPrinterConstants.PTR_BCS_Code93,
        Code128 = OPOSPOSPrinterConstants.PTR_BCS_Code128,
        UPCA_S = OPOSPOSPrinterConstants.PTR_BCS_UPCA_S,
        UPCE_S = OPOSPOSPrinterConstants.PTR_BCS_UPCE_S,
        UPCD1 = OPOSPOSPrinterConstants.PTR_BCS_UPCD1,
        UPCD2 = OPOSPOSPrinterConstants.PTR_BCS_UPCD2,
        UPCD3 = OPOSPOSPrinterConstants.PTR_BCS_UPCD3,
        UPCD4 = OPOSPOSPrinterConstants.PTR_BCS_UPCD4,
        UPCD5 = OPOSPOSPrinterConstants.PTR_BCS_UPCD5,
        EAN8_S = OPOSPOSPrinterConstants.PTR_BCS_EAN8_S,
        EAN13_S = OPOSPOSPrinterConstants.PTR_BCS_EAN13_S,
        EAN128 = OPOSPOSPrinterConstants.PTR_BCS_EAN128,
        OCRA = OPOSPOSPrinterConstants.PTR_BCS_OCRA,
        OCRE = OPOSPOSPrinterConstants.PTR_BCS_OCRB,
        Code128_Parsed = OPOSPOSPrinterConstants.PTR_BCS_Code128_Parsed,
        RSS14 = OPOSPOSPrinterConstants.PTR_BCS_RSS14,
        RSS_Expanded = OPOSPOSPrinterConstants.PTR_BCS_RSS_EXPANDED,
        GS1DATABAR = OPOSPOSPrinterConstants.PTR_BCS_GS1DATABAR,
        GS1DATABAR_E = OPOSPOSPrinterConstants.PTR_BCS_GS1DATABAR_E,
        GS1DATABAR_S = OPOSPOSPrinterConstants.PTR_BCS_GS1DATABAR_S,
        GS1DATABAR_E_S = OPOSPOSPrinterConstants.PTR_BCS_GS1DATABAR_E_S,
        PDF417 = OPOSPOSPrinterConstants.PTR_BCS_PDF417,
        MAXICode = OPOSPOSPrinterConstants.PTR_BCS_MAXICODE,
        Datamatrix = OPOSPOSPrinterConstants.PTR_BCS_DATAMATRIX,
        QRCode = OPOSPOSPrinterConstants.PTR_BCS_QRCODE,
        MicroQRCode = OPOSPOSPrinterConstants.PTR_BCS_UQRCODE,
        Aztec = OPOSPOSPrinterConstants.PTR_BCS_AZTEC,
        MicroPDF417 = OPOSPOSPrinterConstants.PTR_BCS_UPDF417,
        Other = OPOSPOSPrinterConstants.PTR_BCS_OTHER
    }

    public enum Alignment
    {
        Left = OPOSPOSPrinterConstants.PTR_BC_LEFT,
        Center = OPOSPOSPrinterConstants.PTR_BC_CENTER,
        Right = OPOSPOSPrinterConstants.PTR_BC_RIGHT
    }

    public enum TextPosition
    {
        None = OPOSPOSPrinterConstants.PTR_BC_TEXT_NONE,
        Above = OPOSPOSPrinterConstants.PTR_BC_TEXT_ABOVE,
        Below = OPOSPOSPrinterConstants.PTR_BC_TEXT_BELOW
    }
}
