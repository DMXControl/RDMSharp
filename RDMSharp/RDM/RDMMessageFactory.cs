using System;
using System.Linq;

namespace RDMSharp
{
    public static class RDMMessageFactory
    {
        public static RDMMessage BuildDiscUnmute()
        {
            return new RDMMessage()
            {
                Command = ERDM_Command.DISCOVERY_COMMAND,
                Parameter = ERDM_Parameter.DISC_UN_MUTE,
                DestUID = RDMUID.Broadcast,
            };
        }

        public static RDMMessage BuildDiscMute(in RDMUID destUid)
        {
            var m = new RDMMessage()
            {
                Command = ERDM_Command.DISCOVERY_COMMAND,
                Parameter = ERDM_Parameter.DISC_MUTE,
                DestUID = destUid
            };
            return m;
        }

        public static RDMMessage BuildDiscUniqueBranch(in RDMUID startUid,in RDMUID endUid)
        {
            var m = new RDMMessage()
            {
                Command = ERDM_Command.DISCOVERY_COMMAND,
                Parameter = ERDM_Parameter.DISC_UNIQUE_BRANCH,
                DestUID = RDMUID.Broadcast,
                SubDevice = SubDevice.Root,
            };
            m.ParameterData = startUid.ToBytes().Concat(endUid.ToBytes()).ToArray();
            return m;
        }

        /// <summary>
        /// Builds a RDMMessage Response based on a byte[]. Returns null if the data is corrupt
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static RDMMessage BuildResponse(in byte[] data) {
            if (data == null || data.Length < 26) return null;

            //Check startcode and sub-startcode
            if (data[0] != 0xCC || data[1] != 0x01) {
                return null;
            }

            byte length = data[2];

            if (data.Length < length + 2) {
                return null;
            }

            //Calc Checksum
            ushort cs = (ushort)((data[length] << 8) | data[length + 1]);
            ushort cs2 = (ushort)data.Take(length).Sum(c => (int)c);

            if (cs != cs2) //Checksum doesn't match
                return null;

            ushort manIdDest = (ushort)((data[3] << 8) | data[4]);
            uint devIdDest = (uint)((data[5] << 24) | (data[6] << 16) | (data[7] << 8) | data[8]);
            ushort manIdSource = (ushort)((data[9] << 8) | data[10]);
            uint devIdSource = (uint)((data[11] << 24) | (data[12] << 16) | (data[13] << 8) | data[14]);

            byte paramLength = data[23];

            var m = new RDMMessage() {
                SourceUID = new RDMUID(manIdSource, devIdSource),
                DestUID = new RDMUID(manIdDest, devIdDest),
                TransactionCounter = data[15],
                PortID_or_Responsetype = data[16],
                MessageCounter = data[17],
                SubDevice = new SubDevice((ushort)((data[18] << 8) | data[19])),
                Command = (ERDM_Command)data[20],
                Parameter = (ERDM_Parameter)((data[21] << 8) | data[22]),
                ParameterData = data.Skip(24).Take(paramLength).ToArray()
            };
            return m;
        }

        /// <summary>
        /// Builds a RDMMessage Response based on a byte[]. Returns null if the data is corrupt
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static RDMMessage BuildDiscUniqueBranchResponse(in byte[] data)
        {
            if (data == null || data.Length < 17) return null;

            //Parse Byte[] According to Spec
            //data could have a Preample of 0 - 7 bytes, search for Preample seperator
            int dataIndex = Array.IndexOf(data, (byte)0xAA);
            if (dataIndex == -1) //No Preamble seperator found, corrupt
                return null;
            if (data.Length - dataIndex < 17) //Data Missing, corrupt
                return null;

            //Calc Checksum
            ushort cs = (ushort)  (((data[dataIndex + 13] & data[dataIndex + 14]) << 8) |
                                    (data[dataIndex + 15] & data[dataIndex + 16]));

            ushort cs2 = (ushort)data.Skip(dataIndex + 1).Take(12).Sum(c => (int)c);

            if (cs != cs2) //Checksum doesn't match
                return null;

            ushort manId = (ushort)(((data[dataIndex + 1] & data[dataIndex + 2]) << 8) |
                                     (data[dataIndex + 3] & data[dataIndex + 4]));

            uint devId =     (uint)(((data[dataIndex + 5] & data[dataIndex + 6]) << 24) |
                                    ((data[dataIndex + 7] & data[dataIndex + 8]) << 16) |
                                    ((data[dataIndex + 9] & data[dataIndex + 10]) << 8) |
                                     (data[dataIndex + 11] & data[dataIndex + 12]));

            var uid = new RDMUID(manId, devId);
            return BuildDiscUniqueBranchResponse(uid);
        }

        public static RDMMessage BuildDiscUniqueBranchResponse(in RDMUID sourceUid)
        {
            var m = new RDMMessage()
            {
                Command = ERDM_Command.DISCOVERY_COMMAND_RESPONSE,
                Parameter = ERDM_Parameter.DISC_UNIQUE_BRANCH,
                SourceUID = sourceUid
            };
            return m;
        }
    }
}
