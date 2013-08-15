using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.Odbc;
using TestScriptLink2.Entities;

namespace TestScriptLink2.Repositories
{
    public class ProgressNoteRepository
    {
        public static ProgressNote GetLastMGAF(string Patid, double Episode)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["AvatarDBCWS"].ConnectionString;
            var ProgressNote = new ProgressNote();
            #region commandText
            var commandText = "SELECT TOP(1) all_notes.MGAF " +
                                "FROM " +
                                "( " +
                                "SELECT TOP(1) notes.ss_note_integer_4 as MGAF, " +
                                "notes2.date_of_note as NoteDate, " +
                                "notes2.note_time as NoteTime " +
                                "FROM SYSTEM.cw_patient_notes_supp_1 notes " +
                                "INNER JOIN SYSTEM.cw_patient_notes notes2 " +
                                "ON notes.PATID = notes2.PATID " +
                                "AND notes.NOT_uniqueid = notes2.NOT_uniqueid " +
                                "AND notes.FACILITY = notes2.FACILITY " +
                                "WHERE notes.PATID=? " +
                                "AND notes.EPISODE_NUMBER=? " +
                                "AND notes.ss_note_integer_4 IS NOT NULL " +
                                "ORDER BY CAST(STRING(CONVERT(varchar(10),notes2.date_of_note,101),' ',notes2.note_time) as TIMESTAMP) DESC " +
                                "UNION " +
                                "SELECT TOP(1) notes.MGAF as MGAF, " +
                                "notes.Assessing_Date as NoteDate, " +
                                "notes.Evaluation_Time as NoteTime " +
                                "FROM SYSTEM.Medical_Services_Progress notes " +
                                "WHERE notes.PATID=? " +
                                "AND notes.EPISODE_NUMBER=? " +
                                "AND notes.MGAF IS NOT NULL " +
                                "ORDER BY CAST(STRING(CONVERT(varchar(10),notes.Assessing_Date,101),' ',notes.Evaluation_Time) as TIMESTAMP) DESC " +
                                ")all_notes " +
                                "ORDER BY CAST(STRING(CONVERT(varchar(10),all_notes.NoteDate,101),' ',all_notes.NoteTime) as TIMESTAMP) DESC ";
            #endregion
            try
            {
                using (var connection = new OdbcConnection(connectionString))
                {
                    connection.Open();
                    using (var dbcommand = new OdbcCommand(commandText, connection))
                    {
                        dbcommand.Parameters.Add(new OdbcParameter("PATID", Patid));
                        dbcommand.Parameters.Add(new OdbcParameter("EPISODE_NUMBER", Episode.ToString()));
                        dbcommand.Parameters.Add(new OdbcParameter("PATID", Patid));
                        dbcommand.Parameters.Add(new OdbcParameter("EPISODE_NUMBER", Episode.ToString()));
                        using(var reader = dbcommand.ExecuteReader())
                        {
                            if(reader.Read())
                            {
                                ProgressNote.MGAF = reader["MGAF"].ToString();
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch(Exception ex)
            {
            }
            return ProgressNote;
        }
    }
}