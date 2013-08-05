using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestScriptLink2.ClientChargeInput;
using TestScriptLink2.Repositories;

namespace TestScriptLink2.Entities
{
    public class Service
    {
        public FieldObject ServiceDateField { get; set; }
        public FieldObject StartTimeField { get; set; }
        public FieldObject DurationField { get; set; }
        public FieldObject ProgramCodeField { get; set; }
        public FieldObject ServiceCodeField { get; set; }
        public FieldObject NoteStatus { get; set; }
        public ClientChargeInputObject WebSvcObject { get; set; }
        public Service()
        {
            this.ServiceDateField = new FieldObject("1.34");
            this.StartTimeField = new FieldObject("1.35");
            this.DurationField = new FieldObject("1.36");
            this.ProgramCodeField = new FieldObject("1.4");
            this.ServiceCodeField = new FieldObject("1.38");
            this.NoteStatus = new FieldObject("1.42");
            this.WebSvcObject = new ClientChargeInputObject();

        }
        public bool PopulateService(OptionObject optionObject)
        {
            var status = false;
            try
            {
                foreach (var field in optionObject.Forms.First().CurrentRow.Fields)
                {
                    if (field.FieldNumber.Equals(this.NoteStatus.FieldNumber))
                        NoteStatus.FieldValue = field.FieldValue;
                    if (field.FieldNumber.Equals(this.ServiceDateField.FieldNumber))
                        ServiceDateField.FieldValue = field.FieldValue;
                    if (field.FieldNumber.Equals(this.StartTimeField.FieldNumber))
                        StartTimeField.FieldValue = field.FieldValue;
                    if (field.FieldNumber.Equals(this.DurationField.FieldNumber))
                        DurationField.FieldValue = field.FieldValue;
                    if (field.FieldNumber.Equals(this.ProgramCodeField.FieldNumber))
                        ProgramCodeField.FieldValue = field.FieldValue.Substring(field.FieldValue.IndexOf('(') + 1, field.FieldValue.IndexOf(')') - 1);
                    if (field.FieldNumber.Equals(this.ServiceCodeField.FieldNumber))
                        ServiceCodeField.FieldValue = field.FieldValue.Substring(field.FieldValue.IndexOf('(') + 1, field.FieldValue.IndexOf(')') - 1);
                }
                if (this.NoteStatus.FieldValue.Equals("F"))
                {
                    this.WebSvcObject.DateOfService = DateTime.Parse(ServiceDateField.FieldValue);
                    this.WebSvcObject.DateOfServiceSpecified = (this.WebSvcObject.DateOfService != null);
                    this.WebSvcObject.ServiceStartTime = StartTimeField.FieldValue;
                    this.WebSvcObject.Duration = Convert.ToInt64(DurationField.FieldValue);
                    this.WebSvcObject.DurationSpecified = true;
                    this.WebSvcObject.Program = ProgramCodeField.FieldValue;
                    this.WebSvcObject.ServiceCode = ServiceCodeField.FieldValue;
                    this.WebSvcObject.Practitioner = optionObject.EntityID;
                    status = true;
                }
                else
                {
                    status = false;
                }
            }
            catch (Exception e)
            {
            }
            return status;
        }
    }
}