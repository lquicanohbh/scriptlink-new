using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestScriptLink2.Required
{
    public class FieldsRequiredConditionally
    {
        private OptionObject optionObject;
        private string scriptName;
        public OptionObject ReturnOptionObject { get; set; }
        private FieldObject ProgramField { get; set; }
        private FormObject SiteSpecificForm { get; set; }
        private RowObject SiteSpecificCurrentRow { get; set; }
        private List<FieldObject> FieldsToMakeRequired { get; set; }

        public FieldsRequiredConditionally(OptionObject optionObject, string scriptName)
        {
            this.optionObject = optionObject;
            this.scriptName = scriptName;
            this.ReturnOptionObject = new OptionObject();
            InitializeProgramField();
            InitializeSiteSpecificForm();
            InitializeSiteSpecificCurrentRow();
            InitializeFieldToMakeRequired();
        }

        private void InitializeFieldToMakeRequired()
        {
            FieldsToMakeRequired = new List<FieldObject>();
            FieldsToMakeRequired.Add(new FieldObject()
            {
                FieldNumber = "21032.1"
            });
            FieldsToMakeRequired.Add(new FieldObject()
            {
                FieldNumber = "21032.2"
            });
            FieldsToMakeRequired.Add(new FieldObject()
            {
                FieldNumber = "21001.1"
            });
        }

        private void InitializeSiteSpecificCurrentRow()
        {
            SiteSpecificCurrentRow = new RowObject();
            SiteSpecificCurrentRow.ParentRowId = optionObject.Forms[2].CurrentRow.ParentRowId;
            SiteSpecificCurrentRow.RowAction = "EDIT";
            SiteSpecificCurrentRow.RowId = optionObject.Forms[2].CurrentRow.RowId;
        }

        private void InitializeSiteSpecificForm()
        {
            SiteSpecificForm = new FormObject();
            SiteSpecificForm.FormId = optionObject.Forms[2].FormId;
        }

        private void InitializeProgramField()
        {
            ProgramField = new FieldObject();
            ProgramField.FieldNumber = "5";
            ProgramField.FieldValue = optionObject.Forms.First().CurrentRow.Fields.FirstOrDefault(f => f.FieldNumber == ProgramField.FieldNumber).FieldValue;
        }

        public void MakeFieldsRequired()
        {
            FieldsToMakeRequired.ForEach(f => f.Required = "1");
        }

        public void PopulateReturnOptionObject()
        {
            if (ProgramField.FieldValue == "801")
            {
                SiteSpecificCurrentRow.Fields = FieldsToMakeRequired;
                SiteSpecificForm.CurrentRow = SiteSpecificCurrentRow;
                ReturnOptionObject.Forms.Add(SiteSpecificForm);
            }

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