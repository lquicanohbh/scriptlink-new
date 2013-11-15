using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestScriptLink2.Entities
{
    public class ReleaseOfInformation
    {
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string ReleaseFor { get; set; }
        public string Program { get; set; }
        public string Site { get; set; }
        public string Acknowledgement { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Purpose { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? DateRevoked { get; set; }
        public string ClientId { get; set; }

        public ReleaseOfInformation()
        {
        }

        public void Initialize(OptionObject optionObject)
        {
            try
            {
                var form = optionObject.Forms.FirstOrDefault(x => x.FormId == "962");
                foreach (var field in form.CurrentRow.Fields)
                {
                    if (field.FieldNumber.Equals("131.77"))
                        Date = DateTime.Parse(field.FieldValue);
                    if (field.FieldNumber.Equals("132.02"))
                        Time = field.FieldValue;
                    if (field.FieldNumber.Equals("135.09"))
                        ReleaseFor = field.FieldValue;
                    if (field.FieldNumber.Equals("135.08"))
                        Program = field.FieldValue;
                    if (field.FieldNumber.Equals("131.78"))
                        Site = field.FieldValue;
                    if (field.FieldNumber.Equals("131.88"))
                        Acknowledgement = field.FieldValue;
                    if (field.FieldNumber.Equals("131.82"))
                        Name = field.FieldValue;
                    if (field.FieldNumber.Equals("131.83"))
                        Address = field.FieldValue;
                    if (field.FieldNumber.Equals("131.84"))
                        City = field.FieldValue;
                    if (field.FieldNumber.Equals("131.86"))
                        State = field.FieldValue;
                    if (field.FieldNumber.Equals("131.85"))
                        ZipCode = field.FieldValue;
                    if (field.FieldNumber.Equals("131.87"))
                        PhoneNumber = field.FieldValue;
                    if (field.FieldNumber.Equals("131.89"))
                        Purpose = field.FieldValue;
                    if (field.FieldNumber.Equals("131.90"))
                        FromDate = Helper.ConvertStringToNullableDatetime(field.FieldValue);
                    if (field.FieldNumber.Equals("131.91"))
                        ToDate = Helper.ConvertStringToNullableDatetime(field.FieldValue);
                    if (field.FieldNumber.Equals("131.98"))
                        ExpirationDate = Helper.ConvertStringToNullableDatetime(field.FieldValue);
                    if (field.FieldNumber.Equals("135.16"))
                        DateRevoked = Helper.ConvertStringToNullableDatetime(field.FieldValue);
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}