using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.Odbc;

namespace TestScriptLink2
{
    public class PsychcareAuthFormRepository
    {
        public static List<PsychcareAuthForm> GetForm(string ClientId)
        {
            return QueryPsychcareAuthTable(ClientId, String.Empty, null, string.Empty, null, "Client");
        }
        public static List<PsychcareAuthForm> GetForm(string ClientId, string Episode)
        {
            return QueryPsychcareAuthTable(ClientId, Episode, null, string.Empty, null, "Episode");
        }
        public static List<PsychcareAuthForm> GetForm(string ClientId, string Episode, DateTime? FormDate)
        {
            return QueryPsychcareAuthTable(ClientId, Episode, FormDate, String.Empty, null, "Date");
        }
        public static List<PsychcareAuthForm> GetForm(string ClientId, string Episode, DateTime? FormDate, string StaffId)
        {
            return QueryPsychcareAuthTable(ClientId, Episode, FormDate, StaffId, null, "Staff");
        }
        public static List<PsychcareAuthForm> GetForm(string ClientId, string Episode, DateTime? FormDate, string StaffId, DateTime? FormTime)
        {
            return QueryPsychcareAuthTable(ClientId, Episode, FormDate, StaffId, FormTime, "Time");
        }
        private static List<PsychcareAuthForm> QueryPsychcareAuthTable(string ClientId, string Episode, DateTime? FormDate, string StaffId, DateTime? FormTime, string QueryType)
        {
            var PsychcareAuthFormList = new List<PsychcareAuthForm>();
            var connectionString = ConfigurationManager.ConnectionStrings["AvatarDBPM"].ConnectionString;
            #region commandText
            var commandText = "SELECT patient_current_demographics.PATID as ClientId," +
                                    "patient_current_demographics.patient_name as ClientName," +
                                    "Psychcare_Authorization.Draft_PA_Final as DraftPAFinalCode," +
                                    "Psychcare_Authorization.Draft_PA_Final_Value as DraftPAFinalValue," +
                                    "Psychcare_Authorization.EPISODE_NUMBER as Episode," +
                                    "Psychcare_Authorization.Assessing_Date as FormDate," +
                                    "Psychcare_Authorization.data_entry_time as FormTime," + 
                                    "staff_current_demographics.STAFFID as StaffId," +
                                    "staff_current_demographics.name as StaffName," +
                                    "Psychcare_Authorization.Medicaid_ID as MedicaidId," +
                                    "Psychcare_Authorization.Insurance as InsuranceCode," +
                                    "Psychcare_Authorization.Insurance_Value as InsuranceValue," +
                                    "Psychcare_Authorization.Initial_Concurrent as RequestTypeCode," +
                                    "Psychcare_Authorization.Initial_Concurrent_Value as RequestTypeValue," +
                                    "Psychcare_Authorization.History_Cognitive as HistoryCode," +
                                    "Psychcare_Authorization.History_Cognitive_Value as HistoryValue," +
                                    "Psychcare_Authorization.Services_Requested as ServicesReqCode," +
                                    "Psychcare_Authorization.Services_Requested_Value as ServicesReqValue," +
                                    "Psychcare_Authorization.Comments_Cognitive as Comments " +
                                "FROM CWSSYSTEM.Psychcare_Authorization " +
                                "INNER JOIN SYSTEM.patient_current_demographics " +
                                "ON Psychcare_Authorization.PATID = patient_current_demographics.PATID " +
                                "AND Psychcare_Authorization.FACILITY = patient_current_demographics.FACILITY " +
                                "INNER JOIN SYSTEM.staff_current_demographics " +
                                "ON Psychcare_Authorization.Assessing_Clinician = staff_current_demographics.STAFFID " +
                                "AND Psychcare_Authorization.FACILITY = staff_current_demographics.FACILITY ";
            #endregion
            try
            {
                using (var connection = new OdbcConnection(connectionString))
                {
                    connection.Open();
                    using (var dbcommand = new OdbcCommand(commandText, connection))
                    {
                        switch (QueryType)
                        {
                            case "Client":
                                commandText += "WHERE Psychcare_Authorization.PATID=? ";
                                dbcommand.Parameters.Add(new OdbcParameter("PATID", ClientId));
                                break;
                            case "Episode":
                                commandText += "WHERE Psychcare_Authorization.PATID=? ";
                                dbcommand.Parameters.Add(new OdbcParameter("PATID", ClientId));
                                commandText += "AND Psychcare_Authorization.EPISODE_NUMBER=? ";
                                dbcommand.Parameters.Add(new OdbcParameter("EPISODE_NUMBER", Episode));
                                break;
                            case "Date":
                                commandText += "WHERE Psychcare_Authorization.PATID=? ";
                                dbcommand.Parameters.Add(new OdbcParameter("PATID", ClientId));
                                commandText += "AND Psychcare_Authorization.EPISODE_NUMBER=? ";
                                dbcommand.Parameters.Add(new OdbcParameter("EPISODE_NUMBER", Episode));
                                commandText += "AND Psychcare_Authorization.Assessing_Date=? ";
                                dbcommand.Parameters.Add(new OdbcParameter("Assessing_Date", FormDate.Value.ToString("yyyy-MM-dd")));
                                break;
                            case "Staff":
                                commandText += "WHERE Psychcare_Authorization.PATID=? ";
                                dbcommand.Parameters.Add(new OdbcParameter("PATID", ClientId));
                                commandText += "AND Psychcare_Authorization.EPISODE_NUMBER=? ";
                                dbcommand.Parameters.Add(new OdbcParameter("EPISODE_NUMBER", Episode));
                                commandText += "AND Psychcare_Authorization.Assessing_Date=? ";
                                dbcommand.Parameters.Add(new OdbcParameter("Assessing_Date", FormDate.Value.ToString("yyyy-MM-dd")));
                                commandText += "AND Psychcare_Authorization.Assessing_Clinician=? ";
                                dbcommand.Parameters.Add(new OdbcParameter("Assessing_Clinician", StaffId));
                                break;
                            case "Time":
                                commandText += "WHERE Psychcare_Authorization.PATID=? ";
                                dbcommand.Parameters.Add(new OdbcParameter("PATID", ClientId));
                                commandText += "AND Psychcare_Authorization.EPISODE_NUMBER=? ";
                                dbcommand.Parameters.Add(new OdbcParameter("EPISODE_NUMBER", Episode));
                                commandText += "AND Psychcare_Authorization.Assessing_Date=? ";
                                dbcommand.Parameters.Add(new OdbcParameter("Assessing_Date", FormDate.Value.ToString("yyyy-MM-dd")));
                                commandText += "AND Psychcare_Authorization.Assessing_Clinician=? ";
                                dbcommand.Parameters.Add(new OdbcParameter("Assessing_Clinician", StaffId));
                                commandText += "AND (Psychcare_Authorization.data_entry_time=? "+
                                                "OR Psychcare_Authorization.data_entry_time=?) ";
                                dbcommand.Parameters.Add(new OdbcParameter("data_entry_time", FormTime.Value.ToString("hh:mm tt")));
                                dbcommand.Parameters.Add(new OdbcParameter("data_entry_time", FormTime.Value.AddMinutes(-1).ToString("hh:mm tt")));
                                break;
                            default:
                                break;
                        }
                        dbcommand.CommandText = commandText;
                        using (var reader = dbcommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                #region ReadInValues
                                var psychcareAuthForm = new PsychcareAuthForm();
                                psychcareAuthForm.FormDate = reader.GetDate(reader.GetOrdinal("FormDate"));
                                psychcareAuthForm.FormTime = reader["FormTime"].ToString();
                                psychcareAuthForm.Episode = reader["Episode"].ToString();
                                psychcareAuthForm.ClientId = reader["ClientId"].ToString();
                                psychcareAuthForm.ClientName = reader["ClientName"].ToString();
                                psychcareAuthForm.Comments = reader["Comments"].ToString();
                                psychcareAuthForm.HistoryCode = reader["HistoryCode"].ToString();
                                psychcareAuthForm.HistoryValue = reader["HistoryValue"].ToString();
                                psychcareAuthForm.InsuranceCode = reader["InsuranceCode"].ToString();
                                psychcareAuthForm.InsuranceValue = reader["InsuranceValue"].ToString();
                                psychcareAuthForm.MedicaidId = reader["MedicaidId"].ToString();
                                psychcareAuthForm.RequestTypeCode = reader["RequestTypeCode"].ToString();
                                psychcareAuthForm.RequestTypeValue = reader["RequestTypeValue"].ToString();
                                psychcareAuthForm.StaffId = reader["StaffId"].ToString();
                                psychcareAuthForm.StaffName = reader["StaffName"].ToString();
                                psychcareAuthForm.DraftPAFinalCode = reader["DraftPAFinalCode"].ToString();
                                psychcareAuthForm.DraftPAFinalValue = reader["DraftPAFinalValue"].ToString();
                                psychcareAuthForm.ServicesReqCode = reader["ServicesReqCode"].ToString();
                                psychcareAuthForm.ServicesReqValue = reader["ServicesReqValue"].ToString();
                                PsychcareAuthFormList.Add(psychcareAuthForm);
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
            return PsychcareAuthFormList;


        }
    }
}