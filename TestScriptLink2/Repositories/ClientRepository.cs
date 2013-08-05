﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestScriptLink2.Entities;
using System.Configuration;
using System.Data.Odbc;

namespace TestScriptLink2.Repositories
{
    public class ClientRepository
    {
        public static Client GetClientById(string id)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["AvatarDBPM"].ConnectionString;
            var client = new Client();
            #region commandText
            var commandText = "SELECT demo.PATID as ClientId," +
                                "demo.patient_name as ClientName, " +
                                "demo.patient_sex_code as GenderCode, "+
                                "demo.patient_sex_value as GenderValue, "+
                                "demo.date_of_birth as DateOfBirth, "+
                                "demo.race_code as RaceCode, "+
                                "demo.race_value as RaceValue "+
                                "FROM SYSTEM.patient_current_demographics demo " +
                                "WHERE demo.PATID=? ";
            #endregion
            try
            {
                using (var connection = new OdbcConnection(connectionString))
                {
                    connection.Open();
                    using (var dbcommand = new OdbcCommand(commandText, connection))
                    {
                        dbcommand.Parameters.Add(new OdbcParameter("PATID", id));
                        using (var reader = dbcommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                #region ReadInValues
                                client.Id = reader["ClientId"].ToString();
                                client.Name = reader["ClientName"].ToString();
                                client.GenderCode = reader["GenderCode"].ToString();
                                client.GenderValue = reader["GenderValue"].ToString();
                                client.DateOfBirth = reader.GetDate(reader.GetOrdinal("DateOfBirth"));
                                client.RaceCode = reader["RaceCode"].ToString();
                                client.RaceValue = reader["RaceValue"].ToString();
                                #endregion
                            }
                            else
                            {
                                client = null;
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {

            }
            return client;
        }
        public static Client GetClientByIdWithEpisode(string ClientId, double Episode)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["AvatarDBPM"].ConnectionString;
            var client = new Client();
            #region commandText
            var commandText = "SELECT demo.PATID as ClientId," +
                                "demo.patient_name as ClientName, " +
                                "demo.patient_sex_code as GenderCode, " +
                                "demo.patient_sex_value as GenderValue, " +
                                "demo.date_of_birth as DateOfBirth, " +
                                "demo.race_code as RaceCode, " +
                                "demo.race_value as RaceValue, " +
                                "ep.program_code as ProgramCode, " +
                                "ep.program_value as ProgramValue, "+
                                "ep.EPISODE_NUMBER as Episode, "+
                                "ep.date_of_discharge as DischargeDate, "+
                                "ep.preadmit_admission_date as AdmissionDate, "+
                                "ep.last_date_of_service as LastServiceDate "+
                                "FROM SYSTEM.patient_current_demographics demo " +
                                "INNER JOIN SYSTEM.episode_history ep " +
                                "ON demo.PATID = ep.PATID " +
                                "AND demo.FACILITY = ep.FACILITY " +
                                "WHERE demo.PATID=? " +
                                "AND ep.EPISODE_NUMBER=? ";
            #endregion
            try
            {
                using (var connection = new OdbcConnection(connectionString))
                {
                    connection.Open();
                    using (var dbcommand = new OdbcCommand(commandText, connection))
                    {
                        dbcommand.Parameters.Add(new OdbcParameter("PATID", ClientId));
                        dbcommand.Parameters.Add(new OdbcParameter("EPISODE_NUMBER", Episode.ToString()));
                        using (var reader = dbcommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                #region ReadInValues
                                client.EpisodeInformation.Episode = reader["Episode"].ToString();
                                client.EpisodeInformation.ProgramCode = reader["ProgramCode"].ToString();
                                client.EpisodeInformation.ProgramValue = reader["ProgramValue"].ToString();
                                client.EpisodeInformation.AdmitDate = reader.GetDate(reader.GetOrdinal("AdmitDate"));
                                client.EpisodeInformation.DischargeDate = Helper.getNullableDatetime(reader, "DischargeDate");
                                client.EpisodeInformation.LastServiceDate = Helper.getNullableDatetime(reader, "LastServiceDate");
                                client.Id = reader["ClientId"].ToString();
                                client.Name = reader["ClientName"].ToString();
                                client.GenderCode = reader["GenderCode"].ToString();
                                client.GenderValue = reader["GenderValue"].ToString();
                                client.DateOfBirth = reader.GetDate(reader.GetOrdinal("DateOfBirth"));
                                client.RaceCode = reader["RaceCode"].ToString();
                                client.RaceValue = reader["RaceValue"].ToString();
                                #endregion
                            }
                            else
                            {
                                client = null;
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {

            }
            return client;
        }
    }
}