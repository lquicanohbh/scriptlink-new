using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestScriptLink2.CrisisWalkinContactLog
{
    public class CrisisWalkin
    {
        private OptionObject optionObject;
        private string scriptName;
        public OptionObject ReturnOptionObject { get; set; }

        #region
        private FieldObject RTCMD { get; set; }
        private FieldObject RTCClinician { get; set; }
        private FieldObject Referral { get; set; }
        private FieldObject Henderson { get; set; }
        private FieldObject RTCOther { get; set; }
        private List<FieldObject> Dispositions { get; set; }
        #endregion

        public CrisisWalkin(OptionObject optionObject, string scriptName)
        {
            // TODO: Complete member initialization
            this.optionObject = optionObject;
            this.scriptName = scriptName;
            this.ReturnOptionObject = new OptionObject();
            InitializeFields();
        }

        protected virtual void InitializeFields()
        {
            RTCMD = new FieldObject("153.38");
            RTCClinician = new FieldObject("153.39");
            Referral = new FieldObject("145.59");
            Henderson = new FieldObject("145.78");
            RTCOther = new FieldObject("153.41");

            this.Dispositions = new List<FieldObject>();
            this.Dispositions.Add(new FieldObject("153.72"));
            this.Dispositions.Add(new FieldObject("153.75"));
            this.Dispositions.Add(new FieldObject("153.76"));
            this.Dispositions.Add(new FieldObject("153.77"));
            this.Dispositions.Add(new FieldObject("153.78"));
        }

        public void UpdateFieldState()
        {
            GetDispositionValues();
            RTCMD.Required = this.Dispositions.Any(f => f.FieldValue == "1") ? "1" : "0";
            RTCMD.Enabled = this.Dispositions.Any(f => f.FieldValue == "1") ? "1" : "0";
            RTCClinician.Required = this.Dispositions.Any(f => f.FieldValue == "2") ? "1" : "0";
            RTCClinician.Enabled = this.Dispositions.Any(f => f.FieldValue == "2") ? "1" : "0";
            Referral.Required = this.Dispositions.Any(f => f.FieldValue == "5") ? "1" : "0";
            Referral.Enabled = this.Dispositions.Any(f => f.FieldValue == "5") ? "1" : "0";
            Henderson.Required = this.Dispositions.Any(f => f.FieldValue == "4") ? "1" : "0";
            Henderson.Enabled = this.Dispositions.Any(f => f.FieldValue == "4") ? "1" : "0";
            RTCOther.Required = this.Dispositions.Any(f => f.FieldValue == "3") ? "1" : "0";
            RTCOther.Enabled = this.Dispositions.Any(f => f.FieldValue == "3") ? "1" : "0";
            RTCOther.FieldValue = string.Empty;
            PopulateReturnFormObject();
        }

        protected virtual void PopulateReturnFormObject()
        {
            this.ReturnOptionObject.Forms.Add(new FormObject
            {
                FormId = "189",
                CurrentRow = new RowObject
                {
                    RowId = "189||1",
                    ParentRowId = "0",
                    RowAction = "EDIT",
                    Fields = new List<FieldObject>()
                    {
                        RTCMD, RTCClinician, Referral, Henderson, RTCOther
                    }
                }
            });
        }

        protected virtual void GetDispositionValues()
        {
            foreach (var field in this.Dispositions)
            {
                field.FieldValue = Helper.GetField(optionObject, field.FieldNumber).FieldValue;
            }
        }

        public void PopulateReturnOptionObject()
        {
            ReturnOptionObject.EntityID = optionObject.EntityID;
            ReturnOptionObject.EpisodeNumber = optionObject.EpisodeNumber;
            ReturnOptionObject.Facility = optionObject.Facility;
            ReturnOptionObject.OptionId = optionObject.OptionId;
            ReturnOptionObject.OptionStaffId = optionObject.OptionStaffId;
            ReturnOptionObject.OptionUserId = optionObject.OptionUserId;
            ReturnOptionObject.SystemCode = optionObject.SystemCode;
        }
    }
}