using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestScriptLink2
{
    public class PsychcareAuthForm : ClientForm
    {
        public string MedicaidId { get; set; }
        public string InsuranceCode {get; set;}
        public string InsuranceValue { get; set; }
        public string RequestTypeCode { get; set; }
        public string RequestTypeValue { get; set; }
        public string HistoryCode {get; set;}
        public string HistoryValue { get; set; }
        public string Comments { get; set; }
        public string ServicesReqCode { get; set; }
        public string ServicesReqValue { get; set; }
        public override string ToString()
        {
            return base.ToString() +
                    "\nInsurance: " + this.InsuranceValue +
                    "\nServices Requested: " + this.ServicesReqValue;
        }
                    
    }
}