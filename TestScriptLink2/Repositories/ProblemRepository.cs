using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestScriptLink2.Entities;
using System.Configuration;
using System.Data.Odbc;

namespace TestScriptLink2.Repositories
{
    public class ProblemRepository
    {
        public static List<Problem> GetMostRecentProblems(string ClientId, int NumberOfProblems)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["AvatarDBPM"].ConnectionString;
            var problemList = new List<Problem>();
            #region commandText
            var commandText = "SELECT TOP(" + NumberOfProblems + ") pr.problem_code as ProblemCode, " +
                                "pr.problem_description as ProblemDescription " +
                                "FROM CWSSYSTEM.cw_problem_list pr " +
                                "WHERE pr.PATID=? " +
                                "ORDER BY date_identified DESC, PLuniqueid DESC ";
            #endregion
            try
            {
                using (var connection = new OdbcConnection(connectionString))
                {
                    connection.Open();
                    using (var dbcommand = new OdbcCommand(commandText, connection))
                    {
                        dbcommand.Parameters.Add(new OdbcParameter("PATID", ClientId));
                        using (var reader = dbcommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                #region ReadInValues
                                problemList.Add(new Problem
                                {
                                    ProblemCode = reader["ProblemCode"].ToString(),
                                    ProblemDescription = reader["ProblemDescription"].ToString()
                                });
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
            return problemList;
        }
    }
}