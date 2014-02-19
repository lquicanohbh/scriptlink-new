using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestScriptLink2.AuthorizationReleaseOfInformation
{
    public class AuthorizationROI
    {
        private OptionObject optionObject;
        private string scriptName;
        public OptionObject ReturnOptionObject { get; set; }

        #region
        private FieldObject AutoPopulateChoice { get; set; }
        private FieldObject HBHLocation { get; set; }
        private FieldObject RequestedInfo { get; set; }
        private List<string> NovaCodes { get; set; }
        private List<string> BCCodes { get; set; }
        #endregion

        public AuthorizationROI(OptionObject optionObject, string scriptName)
        {
            // TODO: Complete member initialization
            this.optionObject = optionObject;
            this.scriptName = scriptName;
            this.ReturnOptionObject = new OptionObject();
            InitializeFields();
        }

        protected virtual void InitializeFields()
        {
            AutoPopulateChoice = new FieldObject("136.1");
            HBHLocation = new FieldObject("135.08");
            NovaCodes = new List<string>() { "901", "902", "910" };
            BCCodes = new List<string>() { "951", "952", "960" };
        }
        protected virtual List<FieldObject> PopulateFields(string choice)
        {
            var fields = new List<FieldObject>();
            switch (choice)
            {
                case "3rdParty":
                    fields.Add(new FieldObject() { FieldNumber = "131.93", FieldValue = "3&9" });
                    break;
                case "BC":
                    fields.Add(new FieldObject() { FieldNumber = "131.89", FieldValue = "Emergency Communication for Situations Involving Perceived Threat" });
                    fields.Add(new FieldObject() { FieldNumber = "131.82", FieldValue = "Broward College" });
                    break;
                case "NOVA":
                    fields.Add(new FieldObject() { FieldNumber = "131.89", FieldValue = "Emergency Communication for Situations Involving Perceived Threat" });
                    fields.Add(new FieldObject() { FieldNumber = "131.82", FieldValue = "Nova Southeastern University" });
                    break;
                default:
                    break;
            }
            return fields;
        }

        public void UpdateFieldState()
        {
            GetAutopopulateChoice();
            GetHBHLocation();
            if (AutoPopulateChoice.FieldValue == "1")
            {
                PopulateReturnFormObject("3rdParty");
            }
            else if (AutoPopulateChoice.FieldValue == "2" && IsNova())
            {
                PopulateReturnFormObject("NOVA");
            }
            else if (AutoPopulateChoice.FieldValue == "2" && IsBC())
            {
                PopulateReturnFormObject("BC");
            }
        }

        private bool IsBC()
        {
            return BCCodes.Contains(HBHLocation.FieldValue);
        }

        private bool IsNova()
        {
            return NovaCodes.Contains(HBHLocation.FieldValue);
        }

        protected virtual void PopulateReturnFormObject(string choice)
        {
            this.ReturnOptionObject.Forms.Add(new FormObject
            {
                FormId = "962",
                CurrentRow = new RowObject
                {
                    RowId = "962||1",
                    ParentRowId = "0",
                    RowAction = "EDIT",
                    Fields = PopulateFields(choice)
                }
            });
        }

        protected virtual void GetAutopopulateChoice()
        {
            AutoPopulateChoice.FieldValue = Helper.GetField(optionObject, AutoPopulateChoice.FieldNumber).FieldValue;
        }
        protected virtual void GetHBHLocation()
        {
            HBHLocation.FieldValue = Helper.GetField(optionObject, HBHLocation.FieldNumber).FieldValue;
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
