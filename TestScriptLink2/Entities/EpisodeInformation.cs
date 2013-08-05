using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestScriptLink2.Entities
{
    public class EpisodeInformation
    {
        public string Episode { get; set; }
        public string ProgramCode { get; set; }
        public string ProgramValue { get; set; }
        public DateTime AdmitDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public DateTime? LastServiceDate { get; set; }
    }
}