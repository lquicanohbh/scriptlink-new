using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestScriptLink2.Entities
{
    public class Staff
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeId { get; set; }
        public string Title { get; set; }
        public string HomeRu { get; set; }
        public string Location { get; set; }
        public double Hours { get; set; }
    }
}