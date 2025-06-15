using RDMSharp.Metadata;
using RDMSharp.Metadata.JSON;
using System;
using System.Collections.Generic;

namespace RDMSharp
{
    [DataTreeObject(ERDM_Parameter.REAL_TIME_CLOCK, Command.ECommandDublicate.GetResponse)]
    [DataTreeObject(ERDM_Parameter.REAL_TIME_CLOCK, Command.ECommandDublicate.SetRequest)]
    public class RDMRealTimeClock : AbstractRDMPayloadObject, IComparable
    {
        [DataTreeObjectConstructor]
        public RDMRealTimeClock(
            [DataTreeObjectParameter("year")] ushort year = 2003,
            [DataTreeObjectParameter("month")] byte month = 1,
            [DataTreeObjectParameter("day")] byte day = 1,
            [DataTreeObjectParameter("hour")] byte hour = 0,
            [DataTreeObjectParameter("minute")] byte minute = 0,
            [DataTreeObjectParameter("second")] byte second = 0)
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

        [DataTreeObjectProperty("year", 0)]
        public ushort Year { get; private set; }
        [DataTreeObjectProperty("month", 1)]
        public byte Month { get; private set; }
        [DataTreeObjectProperty("day", 2)]
        public byte Day { get; private set; }
        [DataTreeObjectProperty("hour", 3)]
        public byte Hour { get; private set; }
        [DataTreeObjectProperty("minute", 4)]
        public byte Minute { get; private set; }
        [DataTreeObjectProperty("second", 5)]
        public byte Second { get; private set; }
        public const int PDL = 7;

        public override string ToString()
        {
            return $"{Date:G}";
        }
        public static RDMRealTimeClock FromMessage(RDMMessage msg)
        {
            RDMMessageInvalidException.ThrowIfInvalidPDL(msg, ERDM_Command.GET_COMMAND_RESPONSE, ERDM_Parameter.REAL_TIME_CLOCK, PDL);

            return FromPayloadData(msg.ParameterData);
        }
        public static RDMRealTimeClock FromPayloadData(byte[] data)
        {
            RDMMessageInvalidPDLException.ThrowIfInvalidPDL(data, PDL);

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

        public int CompareTo(object obj)
        {
            if (obj is RDMRealTimeClock rtc)
            {
                return this.Date.CompareTo(rtc.Date);
            }
            throw new ArgumentException("Object is not a RDMRealTimeClock", nameof(obj));
        }
    }
}