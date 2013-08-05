using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestScriptLink2
{
    public class UMBHTboReq:ClientForm
    {
        public string Initial_Concurrent_Code { get; set; }
        public string Initial_Concurrent_Value { get; set; }
        public string Medicaid { get; set; }
        public string See_Psychiatrist_Code { get; set; }
        public string See_Psychiatrist_Value { get; set; }
        public string PsychiatristName { get; set; }
        public string Past_Psych_Admin { get; set; }
        public string AxisOne { get; set; }
        public string Current_GAF { get; set; }
        public string PCP_Code { get; set; }
        public string PCP_Value { get; set; }
        public string PCP_Name { get; set; }
        public string Chronic_Serious_Code { get; set; }
        public string Chronic_Serious_Value { get; set; }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}