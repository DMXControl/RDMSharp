using System.Collections.Generic;
using System.Text;
using System;

namespace RDMSharp
{
    public class RDMRealTimeClock : AbstractRDMPayloadObject
    {
        public RDMRealTimeClock(
            ushort year = 2003,
            byte month = 1,
            byte day = 1,
            byte hour = 0,
            byte minute = 0,
            byte second = 0)
        {
            if (year < 2003)
                throw new ArgumentOutOfRangeException($"{nameof(year)} shold be a value between 2003 and 65535 but is {year}");
            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException($"{nameof(month)} shold be a value between 1 and 12 but is {month}");
            if (day < 1 || day > 31)
                throw new ArgumentOutOfRangeException($"{nameof(day)} shold be a value between 1 and 31 but is {day}");
            if (hour > 23)
                throw new ArgumentOutOfRangeException($"{nameof(hour)} shold be a value between 0 and 23 but is {hour}");
            if (minute > 59)
                throw new ArgumentOutOfRangeException($"{nameof(minute)} shold be a value between 0 and 59 but is {minute}");
            if (second > 59)
                throw new ArgumentOutOfRangeException($"{nameof(second)} shold be a value between 0 and 59 but is {second}");

            this.Year = year;
            this.Month = month;
            this.Day = day;
            this.Hour = hour;
            this.Minute = minute;
            this.Second = second;

            this.Date = new DateTime(year, month, day, hour, minute, second);
        }
        public RDMRealTimeClock(DateTime dateTime)
            : this((ushort)dateTime.Year,
                  (byte)dateTime.Month,
                  (byte)dateTime.Day,
                  (byte)dateTime.Hour,
                  (byte)dateTime.Minute,
                  (byte)dateTime.Second)
        {

        }

        public DateTime Date { get; private set; }

        public ushort Year { get; private set; }
        public byte Month { get; private set; }
        public byte Day { get; private set; }
        public byte Hour { get; private set; }
        public byte Minute { get; private set; }
        public byte Second { get; private set; }
        public const int PDL = 7;

        public override string ToString()
        {
            return $"RDMRealTimeClock {Date.ToString("G")}";
        }
        public static RDMRealTimeClock FromMessage(RDMMessage msg)
        {
            if (msg == null) throw new ArgumentNullException($"Argument {nameof(msg)} can't be null");
            if (!msg.IsAck) throw new Exception($"NACK Reason: {(ERDM_NackReason)msg.ParameterData[0]}");
            if (msg.Command != ERDM_Command.GET_COMMAND_RESPONSE) throw new Exception($"Command is not a {ERDM_Command.GET_COMMAND_RESPONSE}");
            if (msg.Parameter != ERDM_Parameter.REAL_TIME_CLOCK) return null;
            if (msg.PDL != PDL) return null;

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMRealTimeClock FromPayloadData(byte[] data)
        {
            if (data.Length != PDL) throw new Exception($"PDL {data.Length} != {PDL}");

            var i = new RDMRealTimeClock(
                year: Tools.DataToUShort(ref data),
                month: Tools.DataToByte(ref data),
                day: Tools.DataToByte(ref data),
                hour: Tools.DataToByte(ref data),
                minute: Tools.DataToByte(ref data),
                second: Tools.DataToByte(ref data));
            return i;
        }
        public override byte[] ToPayloadData()
        {
            List<byte> data = new List<byte>();
            data.AddRange(Tools.ValueToData(this.Year));
            data.AddRange(Tools.ValueToData(this.Month));
            data.AddRange(Tools.ValueToData(this.Day));
            data.AddRange(Tools.ValueToData(this.Hour));
            data.AddRange(Tools.ValueToData(this.Minute));
            data.AddRange(Tools.ValueToData(this.Second));
            return data.ToArray();
        }
    }
}