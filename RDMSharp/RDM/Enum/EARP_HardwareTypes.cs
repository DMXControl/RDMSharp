namespace RDMSharp
{
    public enum EARP_HardwareTypes : ushort
    {
        /// <summary>Reserved (RFC5494)</summary>
        Reserved = 0,

        /// <summary>Ethernet (10Mb) [Jon Postel]</summary>
        Ethernet = 1,

        /// <summary>Experimental Ethernet (3Mb) [Jon Postel]</summary>
        ExperimentalEthernet = 2,

        /// <summary>Amateur Radio AX.25 [Philip Koch]</summary>
        AmateurRadioAX25 = 3,

        /// <summary>Proteon ProNET Token Ring [Avri Doria]</summary>
        ProteonProNetTokenRing = 4,

        /// <summary>Chaos [Gill Pratt]</summary>
        Chaos = 5,

        /// <summary>IEEE 802 Networks [Jon Postel]</summary>
        IEEE802 = 6,

        /// <summary>ARCNET (RFC1201)</summary>
        Arcnet = 7,

        /// <summary>Hyperchannel [Jon Postel]</summary>
        Hyperchannel = 8,

        /// <summary>Lanstar [Tom Unger]</summary>
        Lanstar = 9,

        /// <summary>Autonet Short Address [Mike Burrows]</summary>
        AutonetShortAddress = 10,

        /// <summary>LocalTalk [Joyce K. Reynolds]</summary>
        LocalTalk = 11,

        /// <summary>LocalNet (IBM PCNet or SYTEK LocalNET) [Joseph Murdock]</summary>
        LocalNet = 12,

        /// <summary>Ultra link [Rajiv Dhingra]</summary>
        UltraLink = 13,

        /// <summary>SMDS [George Clapp]</summary>
        SMDS = 14,

        /// <summary>Frame Relay [Andy Malis]</summary>
        FrameRelay = 15,

        /// <summary>Asynchronous Transmission Mode (ATM) [JXB2]</summary>
        ATM1 = 16,

        /// <summary>HDLC [Jon Postel]</summary>
        HDLC = 17,

        /// <summary>Fibre Channel (RFC4338)</summary>
        FibreChannel = 18,

        /// <summary>Asynchronous Transmission Mode (ATM) (RFC2225)</summary>
        ATM2 = 19,

        /// <summary>Serial Line [Jon Postel]</summary>
        SerialLine = 20,

        /// <summary>Asynchronous Transmission Mode (ATM) [Mike Burrows]</summary>
        ATM3 = 21,

        /// <summary>MIL-STD-188-220 [Herb Jensen]</summary>
        MILSTD188220 = 22,

        /// <summary>Metricom [Jonathan Stone]</summary>
        Metricom = 23,

        /// <summary>IEEE 1394.1995 [Myron Hattig]</summary>
        IEEE1394 = 24,

        /// <summary>MAPOS [Mitsuru Maruyama] (RFC2176)</summary>
        MAPOS = 25,

        /// <summary>Twinaxial [Marion Pitts]</summary>
        Twinaxial = 26,

        /// <summary>EUI-64 [Kenji Fujisawa]</summary>
        EUI64 = 27,

        /// <summary>HIPARP [Jean Michel Pittet]</summary>
        HIPARP = 28,

        /// <summary>IP and ARP over ISO 7816-3 [Scott Guthery]</summary>
        IPoverISO78163 = 29,

        /// <summary>ARPSec [Jerome Etienne]</summary>
        ARPSec = 30,

        /// <summary>IPsec tunnel (RFC3456)</summary>
        IPsecTunnel = 31,

        /// <summary>InfiniBand (RFC4391)</summary>
        InfiniBand = 32,

        /// <summary>TIA-102 Project 25 Common Air Interface (CAI) [Jeff Anderson, TIA TR-8.5]</summary>
        TIA102CAI = 33,

        /// <summary>Wiegand Interface [Scott Guthery]</summary>
        WiegandInterface = 34,

        /// <summary>Pure IP [Inaky Perez-Gonzalez]</summary>
        PureIP = 35,

        /// <summary>HW_EXP1 (RFC5494)</summary>
        HW_EXP1 = 36,

        /// <summary>HFI [Tseng-Hui Lin]</summary>
        HFI = 37,

        /// <summary>Unified Bus (UB) [Wei Pan]</summary>
        UnifiedBus = 38,

        /// <summary>HW_EXP2 (RFC5494)</summary>
        HW_EXP2 = 256,

        /// <summary>AEthernet [Geoffroy Gramaize]</summary>
        AEthernet = 257,

        /// <summary>Reserved (RFC5494)</summary>
        Reserved65535 = 65535
    }
}
