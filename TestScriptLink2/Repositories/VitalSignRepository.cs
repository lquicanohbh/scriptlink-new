using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestScriptLink2.Entities;
using System.Configuration;
using System.Data.Odbc;

namespace TestScriptLink2.Repositories
{
    public class VitalSignRepository
    {
        public static List<VitalSign> GetByDateClient(string ClientId, DateTime Date)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["AvatarDBCWS"].ConnectionString;
            var vitalSigns = new List<VitalSign>();
            #region commandText
            var commandText = "SELECT vs.unique_row_id as Id, " +
                            "vs.data_entry_date as EntryDate, " +
                            "vs.data_entry_time as EntryTime, " +
                            "vs.data_entry_by as EntryName, " +
                            "vs.PATID as ClientId, " +
                            "vs.admin_date_actual as DateTaken, " +
                            "vs.admin_time_actual_h as TimeTaken, " +
                            "vs.measured_unit as MeasuredUnit, " +
                            "vs.reading as Reading, " +
                            "vs.reading_entry as ReadingEntry, " +
                            "vs.reading_value as ReadingValue, " +
                            "vs.vital_sign as VitalSign " +
                            "FROM SYSTEM.cw_vital_signs vs " +
                            "WHERE vs.PATID=? " +
                            "AND vs.admin_date_actual=? " +
                            "HAVING max(CAST(STRING(convert(varchar(10),vs.data_entry_date,101),' ',vs.data_entry_time) as DATETIME)) " +
                            "= CAST(STRING(convert(varchar(10),vs.data_entry_date,101),' ',vs.data_entry_time) as DATETIME)";
            #endregion
            try
            {
                using (var connection = new OdbcConnection(connectionString))
                {
                    connection.Open();
                    using (var dbcommand = new OdbcCommand(commandText, connection))
                    {
                        dbcommand.Parameters.Add(new OdbcParameter("PATID", ClientId));
                        dbcommand.Parameters.Add(new OdbcParameter("admin_date_actual", Date.ToString("yyyy-MM-dd")));
                        using (var reader = dbcommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var vitalSign = new VitalSign();
                                #region ReadInValues
                                vitalSign.Id = reader["Id"].ToString();
                                vitalSign.EntryDate = DateTime.Parse(reader["EntryDate"].ToString());
                                vitalSign.EntryTime = DateTime.Parse(reader["EntryTime"].ToString());
                                vitalSign.EntryName = reader["EntryName"].ToString();
                                vitalSign.ClientId = reader["ClientId"].ToString();
                                vitalSign.DateTaken = DateTime.Parse(reader["DateTaken"].ToString());
                                vitalSign.TimeTaken = DateTime.Parse(reader["TimeTaken"].ToString());
                                vitalSign.MeasuredUnit = reader["MeasuredUnit"].ToString();
                                vitalSign.Reading = reader["Reading"].ToString();
                                vitalSign.ReadingEntry = reader["ReadingEntry"].ToString();
                                vitalSign.ReadingValue = reader["ReadingValue"].ToString();
                                vitalSign.VitalSignDescription = reader["VitalSign"].ToString();
                                #endregion
                                vitalSigns.Add(vitalSign);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
            }
            if (vitalSigns.Any())
                return vitalSigns;
            else
                return null;
        }
    }
}