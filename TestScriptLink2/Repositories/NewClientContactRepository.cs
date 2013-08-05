using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.Odbc;

namespace TestScriptLink2
{
    public class NewClientContactRepository
    {
        public static List<NewClientContact> GetContactInfo(string ClientId, string Episode, DateTime? EntryDate)//, DateTime? FormTime)
        {
            return QueryGetContactInfo(ClientId, Episode, EntryDate);//, FormTime);
        }
        private static List<NewClientContact> QueryGetContactInfo(string ClientId, string Episode, DateTime? EntryDate)//, DateTime? FormTime)
        {
            var ContactInfoList = new List<NewClientContact>();
            var connectionString = ConfigurationManager.ConnectionStrings["AvatarDBPM"].ConnectionString;
            #region commandText
            var commandTxt = "SELECT Auth_Release_Request_1.Date_AFRRI as EntryDate," +
                                     "Auth_Release_Request_1.To_From_Name as Name," +
                                     "Auth_Release_Request_1.To_From_Address as Address," +
                                     "Auth_Release_Request_1.To_From_City as City," +
                                     "Auth_Release_Request_1.To_From_State as State," +
                                     "Auth_Release_Request_1.To_From_Zip as Zip," +
                                     "Auth_Release_Request_1.To_From_Phone_Number as HomePhone," +
                                     "Auth_Release_Request_1.Purpose as PurposeOfRelease," +
                                     "Auth_Release_Request_1.From_Date as FromDate," +
                                     "Auth_Release_Request_1.To_Date as ToDate," +
                                     "Auth_Release_Request_1.Expiration_Date as ExpirationDate," +
                                     "Auth_Release_Request_1.Date_Revoked as DateRevoked " +
                              "FROM SYSTEM.Auth_Release_Request_1 ";
            #endregion
            try
            {
                using (var connection = new OdbcConnection(connectionString))
                {
                    connection.Open();
                    using (var dbcommand = new OdbcCommand(commandTxt, connection))
                    {
                        commandTxt += "WHERE Auth_Release_Request_1.PATID =? ";
                        dbcommand.Parameters.Add(new OdbcParameter("PATID", ClientId));
                        commandTxt += "AND Auth_Release_Request_1.EPISODE_NUMBER =? ";
                        dbcommand.Parameters.Add(new OdbcParameter("EPISODE_NUMBER", Episode));
                        commandTxt += "AND Auth_Release_Request_1.Date_AFRRI =? ";
                        dbcommand.Parameters.Add(new OdbcParameter("Date_AFRRI", EntryDate));
                        //commandTxt += "AND (Auth_Release_Request_1.Data_Entry_Time =? " +
                                     // "OR Auth_Release_Request_1.Data_Entry_Time =?) ";
                        //dbcommand.Parameters.Add(new OdbcParameter("Data_Entry_Time", FormTime.Value.ToString("hh:mm tt")));
                        //dbcommand.Parameters.Add(new OdbcParameter("Data_Entry_Time", FormTime.Value.AddMinutes(-1).ToString("hh:mm tt")));

                        dbcommand.CommandText = commandTxt;

                        using (var reader = dbcommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                #region ReadInValues
                                var contactInfo = new NewClientContact();
                                contactInfo.EntryDate = reader.GetDate(reader.GetOrdinal("EntryDate"));
                                contactInfo.Name = reader["Name"].ToString();
                                contactInfo.Address = reader["Address"].ToString();
                                contactInfo.City = reader["City"].ToString();
                                contactInfo.State = reader["State"].ToString();
                                contactInfo.Zip = Convert.ToInt32(reader["Zip"]);
                                contactInfo.HomePhone = reader["HomePhone"].ToString();
                                contactInfo.PurposeOfRelease = reader["PurposeOfRelease"].ToString();
                                contactInfo.FromDate = reader.GetDate(reader.GetOrdinal("FromDate"));
                                contactInfo.ToDate = reader.GetDate(reader.GetOrdinal("ToDate"));
                                contactInfo.ExpirationDate = reader.GetDate(reader.GetOrdinal("ExpirationDate"));
                                contactInfo.DateRevoked = reader.GetDate(reader.GetOrdinal("DateRevoked"));
                                contactInfo.ClientId = ClientId;
                                contactInfo.Episode = Episode;
                                ContactInfoList.Add(contactInfo);
                                #endregion
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {

            }
            return ContactInfoList;
        }
    }
}