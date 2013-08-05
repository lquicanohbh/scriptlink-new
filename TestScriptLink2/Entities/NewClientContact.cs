using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestScriptLink2
{
    public class NewClientContact
    {
        public string ClientId { get; set; }
        public string Episode { get; set; }
        public DateTime EntryDate { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int Zip { get; set; }
        public int ContactType { get { return 99; } }
        public int EmergencyContact { get { return 2; } }
        public string HomePhone { get; set; }
        public string PurposeOfRelease { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? DateRevoked { get; set; }
        public string Comments
        {
            get
            {
                string comments = String.Empty;

                if (PurposeOfRelease != null)
                {
                    comments += "The purpose for the release of this information is: " + PurposeOfRelease + ".\n";
                }

                if ((FromDate.HasValue) && (ToDate.HasValue))
                {
                    comments += "Information was authorized to be released from " +
                        FromDate.Value.ToString("yyyy-MM-dd") + 
                        " to " +
                        ToDate.Value.ToString("yyyy-MM-dd") + 
                        ".\n";
                }

                if (ExpirationDate.HasValue)
                {
                    comments += "Authorization Expires " + ExpirationDate.Value.ToString("yyyy-MM-dd") + ".\n";
                }

                if (DateRevoked.HasValue)
                {
                    comments += "Authorization was revoked " + DateRevoked.Value.ToString("yyyy-MM-dd") + ".\n";
                }

                return comments;
            }
        }
        public String ToString()
        {
            return "<optiondata>" +
                        "<PATID>" + this.ClientId + "</PATID>" +
                        "<EPISODE_NUMBER>" + this.Episode + "</EPISODE_NUMBER>" +
                        "<SYSTEM.new_client_contact_info>" +
                            "<Contact_Date>" + this.EntryDate.ToString("yyyy-MM-dd") + "</Contact_Date>" +
                            "<Contact_Name>" + this.Name + "</Contact_Name>" +
                            "<Home_Phone>" + this.HomePhone + "</Home_Phone>" +
                            "<Street_Address_1>" + this.Address + "</Street_Address_1>" +
                            "<City>" + this.City + "</City>" +
                            "<State>" + this.State + "</State>" +
                            "<Zip_Code>" + this.Zip + "</Zip_Code>" +
                            "<Contact_Type>" + this.ContactType + "</Contact_Type>" +
                            "<Emergency_Contact>" + this.EmergencyContact + "</Emergency_Contact>" +
                            "<Comments_Special>" + this.Comments + "</Comments_Special>" +
                        "</SYSTEM.new_client_contact_info>" +
                    "</optiondata>";
        }

    }
}