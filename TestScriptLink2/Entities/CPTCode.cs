using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestScriptLink2.Entities
{
    public class CPTCode
    {
        public string ServiceCode { get; set; }
        public bool InteractiveComplexity { get; set; }
        public CPTCode()
        {
            this.ServiceCode = "99211";
            this.InteractiveComplexity = false;
        }
        public CPTCode(string ServiceCode, bool InteractiveComplexity)
        {
            this.ServiceCode = ServiceCode;
            this.InteractiveComplexity = InteractiveComplexity;
        }
        public override string ToString()
        {
            return String.Format("Service Code: {0} \nInteractive Complexity: {1}",
                this.ServiceCode, this.InteractiveComplexity);
        }
    }
}