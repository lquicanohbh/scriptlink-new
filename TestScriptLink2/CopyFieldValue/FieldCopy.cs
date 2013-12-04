using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestScriptLink2.CopyFieldValue
{
    public class FieldCopy
    {
        private OptionObject optionObject;
        public OptionObject ReturnOptionObject { get; set; }
        public FieldObject FieldCopyFrom { get; private set; }
        public FieldObject FieldCopyTo { get; private set; }

        public FieldCopy(OptionObject optionObject, string ScriptName)
        {
            this.optionObject = optionObject;
            ReturnOptionObject = new OptionObject();
            this.FieldCopyFrom = GetField(GetFieldNumber(ScriptName, 1), optionObject);
            this.FieldCopyTo = GetField(GetFieldNumber(ScriptName, 2), optionObject);
        }
        public string GetFieldNumber(string ScriptName, int index)
        {
            string temp;
            if (Helper.SplitAndGetValueAt(ScriptName, ',', index, out temp))
                return temp;
            return String.Empty;
        }
        public FieldObject GetField(string fieldNumber, OptionObject optionObject)
        {
            return optionObject.Forms.SelectMany(r => r.CurrentRow.Fields).FirstOrDefault(f => f.FieldNumber.Equals(fieldNumber));
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

        public void PerformCopy()
        {
            if (ReturnOptionObject.Forms.Any())
            {
                UpdateField(FieldCopyFrom, FieldCopyTo);
            }
            else
            {
                AddField(FieldCopyFrom, FieldCopyTo);
            }
        }

        private void AddField(FieldObject FieldCopyFrom, FieldObject FieldCopyTo)
        {
            var fields = new List<FieldObject>();
            var row = GetRowContainingField(FieldCopyTo);
            var form = GetFormContainingRow(row);
            fields.Add(new FieldObject
            {
                FieldNumber = FieldCopyTo.FieldNumber,
                FieldValue = FieldCopyFrom.FieldValue
            });
            ReturnOptionObject.Forms.Add(new FormObject
            {
                CurrentRow = new RowObject()
                {
                    Fields = fields,
                    ParentRowId = row.ParentRowId,
                    RowAction = "EDIT",
                    RowId = row.RowId
                },
                FormId = form.FormId,
                MultipleIteration = form.MultipleIteration
            });
        }

        private FormObject GetFormContainingRow(RowObject row)
        {
            FormObject form = optionObject.Forms.FirstOrDefault(f => f.CurrentRow.Equals(row));
            return form;
        }
        private RowObject GetRowContainingField(FieldObject SearchField)
        {
            RowObject row = optionObject.Forms.Select(f => f.CurrentRow).FirstOrDefault(r => r.Fields.Contains(SearchField));
            return row;
        }

        private void UpdateField(FieldObject FieldCopyFrom, FieldObject FieldCopyTo)
        {
            var field = GetField(FieldCopyTo.FieldNumber, ReturnOptionObject);
            if (field != null)
                field.FieldValue = FieldCopyFrom.FieldValue;
        }
    }
}