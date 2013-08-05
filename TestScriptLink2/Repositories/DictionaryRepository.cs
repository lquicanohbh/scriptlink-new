using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestScriptLink2.Entities;
using System.Configuration;
using System.Data.Odbc;

namespace TestScriptLink2.Repositories
{
    public class DictionaryRepository
    {
        public static List<FormDictionary> GetDictionaryValues(string FieldNumber, string CodeList)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["AvatarDBPM"].ConnectionString;
            var dictionary = new List<FormDictionary>();
            #region commandText
            var commandText = "SELECT dict.field_number as FieldNumber, " +
                                "dict.dictionary_code as Code, "+
                                "dict.dictionary_value as Value, "+
                                "dict.field_description as FieldDescription, "+
                                "dict.inactive_code as Inactive, "+
                                "dict.user_defined_row_id as Id "+
                                "FROM RADplusCWS.RADplus_dict_user_def dict " +
                                "WHERE field_number=? " +
                                "AND dictionary_code in ("+CodeList+") ";
            #endregion
            try
            {
                using (var connection = new OdbcConnection(connectionString))
                {
                    connection.Open();
                    using (var dbcommand = new OdbcCommand(commandText, connection))
                    {
                        dbcommand.Parameters.Add(new OdbcParameter("field_number", FieldNumber));
                        using (var reader = dbcommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                #region ReadInValues
                                var dict = new FormDictionary();
                                dict.Id = reader["Id"].ToString();
                                dict.FieldNumber = reader["FieldNumber"].ToString();
                                dict.Code = reader["Code"].ToString();
                                dict.Value = reader["Value"].ToString();
                                dict.FieldDescription = reader["FieldDescription"].ToString();
                                dict.Inactive = reader["Inactive"].ToString();
                                dictionary.Add(dict);
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
            return dictionary;
        }

        public static List<FormDictionary> GetDictionaryValues(List<FormDictionary> list)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["AvatarDBPM"].ConnectionString;
            var dictionaryList = new List<FormDictionary>();
            #region commandText
            var commandText = "SELECT dict.field_number as FieldNumber, " +
                                "dict.dictionary_code as Code, " +
                                "dict.dictionary_value as Value, " +
                                "dict.field_description as FieldDescription, " +
                                "dict.inactive_code as Inactive, " +
                                "dict.user_defined_row_id as Id " +
                                "FROM RADplusCWS.RADplus_dict_user_def dict ";
            if (list.Count > 0)
            {
                var tempText = String.Format("WHERE (dict.field_number = {0} AND dict.dictionary_code = {1}) ",
                    list.First().FieldNumber,
                    list.First().Code);
                foreach (var dictionary in list.Skip(1))
                {
                    tempText += String.Format("OR (dict.field_number = {0} AND dict.dictionary_code = {1}) ",
                        dictionary.FieldNumber,
                        dictionary.Code);
                }
                commandText += tempText;
            }
            #endregion

            try
            {
                using (var connection = new OdbcConnection(connectionString))
                {
                    connection.Open();
                    using (var dbcommand = new OdbcCommand(commandText, connection))
                    {
                        using (var reader = dbcommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                #region ReadInValues
                                var dict = new FormDictionary();
                                dict.Id = reader["Id"].ToString();
                                dict.FieldNumber = reader["FieldNumber"].ToString();
                                dict.Code = reader["Code"].ToString();
                                dict.Value = reader["Value"].ToString();
                                dict.FieldDescription = reader["FieldDescription"].ToString();
                                dict.Inactive = reader["Inactive"].ToString();
                                dictionaryList.Add(dict);
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
            return dictionaryList;
        }
    }
}