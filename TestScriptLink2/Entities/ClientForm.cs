using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestScriptLink2
{
    public class ClientForm
    {
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string Episode { get; set; }
        public string DraftPAFinalCode{ get; set; }
        public string DraftPAFinalValue { get; set; }
        public DateTime FormDate { get; set; }
        public string FormTime { get; set; }
        public string StaffId { get; set; }
        public string StaffName { get; set; }
        public override string ToString()
        {
            return "Client Id: " + this.ClientId + 
                   "\nClient Name: " + this.ClientName +
                   "\nAssessing Date: " + this.FormDate.ToString("MM/dd/yyyy") +
                   "\nEpisode: " + this.Episode +
                   "\nDraft/Pending Approval/Final: " + this.DraftPAFinalValue +
                   "\nAssessing Clinician ID: " + this.StaffId +
                   "\nClinician Name: " + this.StaffName;
        }
    }
}