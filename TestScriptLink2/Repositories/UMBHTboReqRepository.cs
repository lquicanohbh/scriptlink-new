using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.Odbc;

namespace TestScriptLink2
{
    public class UMBHTboReqRepository
    {
        public static List<UMBHTboReq> GetForm(string ClientId)
        {
            return QueryUMBHTboReqTable(ClientId, string.Empty, null, string.Empty, null,"Client");
        }
        public static List<UMBHTboReq> GetForm(string ClientId, string Episode)
        {
            return QueryUMBHTboReqTable(ClientId, Episode, null, string.Empty, null, "Episode");
        }
        public static List<UMBHTboReq> GetForm(string ClientId, string Episode, DateTime? FormDate)
        {
            return QueryUMBHTboReqTable(ClientId, Episode, FormDate, string.Empty, null, "Date");
        }
        public static List<UMBHTboReq> GetForm(string ClientId, string Episode, DateTime? FormDate, string StaffId)
        {
            return QueryUMBHTboReqTable(ClientId, Episode, FormDate, StaffId, null, "Staff");
        }
        public static List<UMBHTboReq> GetForm(string ClientId, string Episode, DateTime? FormDate, string StaffId, DateTime? FormTime)
        {
            return QueryUMBHTboReqTable(ClientId, Episode, FormDate, StaffId, FormTime, "Time");
        }
        private static List<UMBHTboReq> QueryUMBHTboReqTable(string ClientId, string Episode, DateTime? FormDate, string StaffId, DateTime? FormTime, string QueryType)
        {
            var UMBHTboReqList = new List<UMBHTboReq>();
            var connectionString = ConfigurationManager.ConnectionStrings["AvatarDBPM"].ConnectionString;
            #region commandText
            var commandText = "SELECT patient_current_demographics.PATID as ClientId," +
                                    "patient_current_demographics.patient_name as ClientName," +
                                    "staff_current_demographics.STAFFID as StaffId," +
                                    "staff_current_demographics.name as StaffName," +
                                    "UMBH_TBOS_Authorization.Assessing_Date as FormDate," +
                                    "UMBH_TBOS_Authorization.Data_Entry_Time as FormTime," +
                                    "UMBH_TBOS_Authorization.EPISODE_NUMBER as Episode," +
                                    "UMBH_TBOS_Authorization.Initial_or_Concurrent as Initial_Concurrent_Code," +
                                    "UMBH_TBOS_Authorization.Initial_or_Concurrent_Value as Initial_Concurrent_Value," +
                                    "UMBH_TBOS_Authorization.Member_Medicaid as Medicaid," +
                                    "UMBH_TBOS_Authorization.Does_Cl_Psychiatrist as See_Psychiatrist_Code," +
                                    "UMBH_TBOS_Authorization.Does_Cl_Psychiatrist_Value as See_Psychiatrist_Value," +
                                    "UMBH_TBOS_Authorization.If_Name_Psychiatrist as PsychiatristName," +
                                    "UMBH_TBOS_Authorization.Num_Past_Psy_Adms as Past_Psych_Admin," +
                                    "UMBH_TBOS_Authorization.Axis_I as AxisOne," +
                                    "UMBH_TBOS_Authorization.Current_GAF as Current_GAF," +
                                    "UMBH_TBOS_Authorization.Does_Client_PCP as PCP_Code," +
                                    "UMBH_TBOS_Authorization.Does_Client_PCP_Value as PCP_Value," +
                                    "UMBH_TBOS_Authorization.If_Name_PCP as PCP_Name," +
                                    "UMBH_TBOS_Authorization.Does_Cl_Chronic_Med_Cond as Chronic_Serious_Code," +
                                    "UMBH_TBOS_Authorization.Does_Cl_Chronic_Med_Cond_Value as Chronic_Serious_Value," +
                                    "UMBH_TBOS_Authorization.Draft_PA_Final as DraftPAFinalCode," +
                                    "UMBH_TBOS_Authorization.Draft_PA_Final_Value as DraftPAFinalValue " +
                                "FROM CWSSYSTEM.UMBH_TBOS_Authorization " +
                                "INNER JOIN SYSTEM.patient_current_demographics " +
                                "ON UMBH_TBOS_Authorization.PATID = patient_current_demographics.PATID " +
                                "AND UMBH_TBOS_Authorization.FACILITY = patient_current_demographics.FACILITY " +
                                "INNER JOIN SYSTEM.staff_current_demographics " +
                                "ON UMBH_TBOS_Authorization.Assessing_Clinician = staff_current_demographics.STAFFID " +
                                "AND UMBH_TBOS_Authorization.FACILITY = staff_current_demographics.FACILITY ";
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
                            #region Switch QueryType
                            case "Client":
                                commandText += "WHERE UMBH_TBOS_Authorization.PATID = ? ";
                                dbcommand.Parameters.Add(new OdbcParameter("PATID", ClientId));
                                break;
                            case "Episode":
                                commandText += "WHERE UMBH_TBOS_Authorization.PATID = ? ";
                                dbcommand.Parameters.Add(new OdbcParameter("PATID", ClientId));
                                commandText += "AND UMBH_TBOS_Authorization.EPISODE_NUMBER = ? ";
                                dbcommand.Parameters.Add(new OdbcParameter("EPISODE_NUMBER", Episode));
                                break;
                            case "Date":
                                commandText += "WHERE UMBH_TBOS_Authorization.PATID = ? ";
                                dbcommand.Parameters.Add(new OdbcParameter("PATID", ClientId));
                                commandText += "AND UMBH_TBOS_Authorization.EPISODE_NUMBER = ? ";
                                dbcommand.Parameters.Add(new OdbcParameter("EPISODE_NUMBER", Episode));
                                commandText += "AND UMBH_TBOS_Authorization.Assessing_Date = ? ";
                                dbcommand.Parameters.Add(new OdbcParameter("Assessing_Date", FormDate));
                                break;
                            case "Staff":
                                commandText += "WHERE UMBH_TBOS_Authorization.PATID = ? ";
                                dbcommand.Parameters.Add(new OdbcParameter("PATID", ClientId));
                                commandText += "AND UMBH_TBOS_Authorization.EPISODE_NUMBER = ? ";
                                dbcommand.Parameters.Add(new OdbcParameter("EPISODE_NUMBER", Episode));
                                commandText += "AND UMBH_TBOS_Authorization.Assessing_Date = ? ";
                                dbcommand.Parameters.Add(new OdbcParameter("Assessing_Date", FormDate));
                                commandText += "AND UMBH_TBOS_Authorization.Assessing_Clinician= ? ";
                                dbcommand.Parameters.Add(new OdbcParameter("Assessing_Clinician", StaffId));
                                break;
                            case "Time":
                                commandText += "WHERE UMBH_TBOS_Authorization.PATID = ? ";
                                dbcommand.Parameters.Add(new OdbcParameter("PATID", ClientId));
                                commandText += "AND UMBH_TBOS_Authorization.EPISODE_NUMBER = ? ";
                                dbcommand.Parameters.Add(new OdbcParameter("EPISODE_NUMBER", Episode));
                                commandText += "AND UMBH_TBOS_Authorization.Assessing_Date = ? ";
                                dbcommand.Parameters.Add(new OdbcParameter("Assessing_Date", FormDate));
                                commandText += "AND UMBH_TBOS_Authorization.Assessing_Clinician = ? ";
                                dbcommand.Parameters.Add(new OdbcParameter("Assessing_Clinician", StaffId));
                                commandText += "AND (UMBH_TBOS_Authorization.Data_Entry_Time = ? " +
                                               "OR UMBH_TBOS_Authorization.Data_Entry_Time = ?) ";
                                dbcommand.Parameters.Add(new OdbcParameter("Data_Entry_Time", FormTime.Value.ToString("hh:mm tt")));
                                dbcommand.Parameters.Add(new OdbcParameter("Data_Entry_Time", FormTime.Value.AddMinutes(-1).ToString("hh:mm tt")));
                                break;
                            default:
                                break;
                            #endregion
                        }
                        dbcommand.CommandText = commandText;
                        using (var reader = dbcommand.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                #region Read In Values
                                var umbhreq = new UMBHTboReq();
                                umbhreq.ClientId = reader["ClientId"].ToString();
                                umbhreq.ClientName = reader["ClientName"].ToString();
                                umbhreq.Episode = reader["Episode"].ToString();
                                umbhreq.DraftPAFinalCode = reader["DraftPAFinalCode"].ToString();
                                umbhreq.DraftPAFinalValue = reader["DraftPAFinalValue"].ToString();
                                umbhreq.FormDate = reader.GetDate(reader.GetOrdinal("FormDate"));
                                umbhreq.FormTime = reader["FormTime"].ToString();
                                umbhreq.StaffId = reader["StaffId"].ToString();
                                umbhreq.StaffName = reader["StaffName"].ToString();
                                umbhreq.Initial_Concurrent_Code = reader["Initial_Concurrent_Code"].ToString();
                                umbhreq.Initial_Concurrent_Value = reader["Initial_Concurrent_Value"].ToString();
                                umbhreq.Medicaid = reader["Medicaid"].ToString();
                                umbhreq.See_Psychiatrist_Code = reader["See_Psychiatrist_Code"].ToString();
                                umbhreq.See_Psychiatrist_Value = reader["See_Psychiatrist_Value"].ToString();
                                umbhreq.PsychiatristName = reader["PsychiatristName"].ToString();
                                umbhreq.Past_Psych_Admin = reader["Past_Psych_Admin"].ToString();
                                umbhreq.AxisOne = reader["AxisOne"].ToString();
                                umbhreq.Current_GAF = reader["Current_GAF"].ToString();
                                umbhreq.PCP_Code = reader["PCP_Code"].ToString();
                                umbhreq.PCP_Value = reader["PCP_Value"].ToString();
                                umbhreq.PCP_Name = reader["PCP_Name"].ToString();
                                umbhreq.Chronic_Serious_Code = reader["Chronic_Serious_Code"].ToString();
                                umbhreq.Chronic_Serious_Value = reader["Chronic_Serious_Value"].ToString();
                                UMBHTboReqList.Add(umbhreq);
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
            return UMBHTboReqList;
        }
           
    }
}