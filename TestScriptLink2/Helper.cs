using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Configuration;
using System.Data.Odbc;
using System.Xml.Serialization;
using System.IO;

namespace TestScriptLink2
{
    public static class Helper
    {
        public static string GetFieldValue(OptionObject optionObject, string FieldNumber)
        {
            return optionObject.Forms.First().CurrentRow.Fields.FirstOrDefault(f => f.FieldNumber.Equals(FieldNumber)).FieldValue;
        }
        public static string SerializeToString(object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                return writer.ToString();
            }
        }
        public static bool SplitAndGetValueAt(string Scriptname, char delimiter, int index, out string value)
        {
            try
            {
                value = Scriptname.Split(delimiter)[index];
                return true;
            }
            catch (IndexOutOfRangeException ex)
            {
                value = null;
                return false;
            }
        }
        public static DateTime? GetNullableDatetime(OdbcDataReader reader, string columnName)
        {
            int x = reader.GetOrdinal(columnName);
            return reader.IsDBNull(x) ? (DateTime?)null : reader.GetDateTime(x);
        }
        public static DateTime? ConvertStringToNullableDatetime(string DateValue)
        {
            return String.IsNullOrEmpty(DateValue) ? (DateTime?)null : DateTime.Parse(DateValue);
        }
        public static string FormatMultipleValueToQueryParameter(string MultipleValue)
        {
            var temp = MultipleValue;
            temp = temp.Replace("&", "','");
            temp = temp.Insert(0, "'");
            temp = temp.Insert(temp.Length, "'");
            return temp;
        }
        public static OptionObject ReturnOptionObjectWithErrorMessage(double errorCode, string errorMessage, OptionObject originalOptionObject)
        {
            var returnOptionObject = new OptionObject();
            returnOptionObject.EntityID = originalOptionObject.EntityID;
            returnOptionObject.EpisodeNumber = originalOptionObject.EpisodeNumber;
            returnOptionObject.ErrorCode = errorCode;
            returnOptionObject.ErrorMesg = errorMessage;
            returnOptionObject.Facility = originalOptionObject.Facility;
            returnOptionObject.OptionId = originalOptionObject.OptionId;
            returnOptionObject.OptionStaffId = originalOptionObject.OptionStaffId;
            returnOptionObject.OptionUserId = originalOptionObject.OptionUserId;
            returnOptionObject.SystemCode = originalOptionObject.SystemCode;
            return returnOptionObject;
        }
        public static OptionObject ReturnOptionObject(OptionObject originalOptionObject)
        {
            var returnOptionObject = new OptionObject();
            returnOptionObject.EntityID = originalOptionObject.EntityID;
            returnOptionObject.EpisodeNumber = originalOptionObject.EpisodeNumber;
            returnOptionObject.Facility = originalOptionObject.Facility;
            returnOptionObject.OptionId = originalOptionObject.OptionId;
            returnOptionObject.OptionStaffId = originalOptionObject.OptionStaffId;
            returnOptionObject.OptionUserId = originalOptionObject.OptionUserId;
            returnOptionObject.SystemCode = originalOptionObject.SystemCode;
            return returnOptionObject;
        }
        public static void sendEmail(string Sender, string Subject, string Body, List<string> Recipients, List<string> CCRecipients, List<string> SpiceworksCommands)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = ConfigurationManager.AppSettings["SMTPServer"].ToString();
                smtpClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"].ToString());
                mailMessage.From = new MailAddress(Sender);
                mailMessage.To.Clear();
                Recipients.ForEach(x => mailMessage.To.Add(x));
                if (CCRecipients.Count > 0)
                    CCRecipients.ForEach(x => mailMessage.CC.Add(x));
                mailMessage.Subject = Subject;
                mailMessage.Body = Body;
                if (SpiceworksCommands.Count > 0)
                {
                    mailMessage.Body += "\n";
                    foreach (var x in SpiceworksCommands)
                    {
                        mailMessage.Body += "\n" + x;
                    }
                }
                smtpClient.Send(mailMessage);
                mailMessage.Dispose();
            }
            catch (Exception e)
            {
            }
        }
        public static void sendEmail(string Sender, string Subject, string Body, List<string> Recipients, List<string> CCRecipients)
        {
            sendEmail(Sender, Subject, Body, Recipients, CCRecipients, new List<string>());
        }
        public static void sendEmail(string Sender, string Subject, string Body, List<string> Recipients)
        {
            sendEmail(Sender, Subject, Body, Recipients, new List<string>(), new List<string>());
        }
        public static int Min(int x, int y, int z)
        {
            return Math.Min(x, Math.Min(y, z));
        }
    }
}