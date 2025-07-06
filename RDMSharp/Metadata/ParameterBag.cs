using System;

namespace RDMSharp.Metadata
{
    public readonly struct ParameterBag : IEquatable<ParameterBag>
    {
        public readonly ERDM_Parameter PID { get; }
        public readonly ushort ManufacturerID { get; }
        public readonly ushort? DeviceModelID { get; }
        public readonly uint? SoftwareVersionID { get; }
        public ParameterBag()
        {
            throw new NotSupportedException("Its not allowed to have an default of this Type");
        }
        public ParameterBag(ERDM_Parameter pid, ushort manufacturerID = 0, ushort? deviceModelID = null, uint? softwareVersionID = null)
        {
            if ((ushort)pid >= 0x8000)
            {
                if (manufacturerID == 0)
                    throw new ArgumentNullException($"{nameof(pid)} in range 0x8000 -0xFFFF requires {nameof(manufacturerID)} != 0");
            }
            else if (pid != ERDM_Parameter.QUEUED_MESSAGE)
            {
                manufacturerID = 0;
                deviceModelID = null;
                softwareVersionID = null;
            }

            PID = pid;
            ManufacturerID = manufacturerID;
            DeviceModelID = deviceModelID;
            SoftwareVersionID = softwareVersionID;
        }
        public override string ToString()
        {
            if (ManufacturerID != 0)
            {
                if (DeviceModelID != null)
                {
                    if (SoftwareVersionID != null)
                        return $"{PID} ManufacturerID: {ManufacturerID}, DeviceModelID: {DeviceModelID}, SoftwareVersionID: {SoftwareVersionID}";
                    return $"{PID} ManufacturerID: {ManufacturerID}, DeviceModelID: {DeviceModelID}";
                }
                return $"{PID} ManufacturerID: {ManufacturerID}";
            }
            return $"{PID}";
        }

        public override bool Equals(object obj)
        {
            return obj is ParameterBag bag && Equals(bag);
        }

        public bool Equals(ParameterBag other)
        {
            return PID == other.PID &&
                   ManufacturerID == other.ManufacturerID &&
                   DeviceModelID == other.DeviceModelID &&
                   SoftwareVersionID == other.SoftwareVersionID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PID, ManufacturerID, DeviceModelID, SoftwareVersionID);
        }

        public static bool operator ==(ParameterBag left, ParameterBag right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ParameterBag left, ParameterBag right)
        {
            return !(left == right);
        }
    }
}
