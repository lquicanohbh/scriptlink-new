using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestScriptLink2.Entities
{
    public class VitalSign
    {
        public string Id { get; set; }
        public DateTime EntryDate { get; set; }
        public string EntryName { get; set; }
        public DateTime EntryTime { get; set; }
        public string ClientId { get; set; }
        public DateTime DateTaken { get; set; }
        public DateTime TimeTaken { get; set; }
        public string MeasuredUnit { get; set; }
        public string Reading { get; set; }
        public string ReadingEntry { get; set; }
        public string ReadingValue { get; set; }
        public string VitalSignDescription { get; set; }
    }
}