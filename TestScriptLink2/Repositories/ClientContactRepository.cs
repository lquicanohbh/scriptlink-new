using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestScriptLink2.DCI;
using System.Configuration;

namespace TestScriptLink2.Repositories
{
    public class ClientContactRepository
    {
        public string SystemCode { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public long FileWarnings { get; set; }
        public bool FileWarningsSpecified { get; set; }
        public string ResultStream { get; set; }
        public string RecordStream { get; set; }
        public string Record { get; set; }

        public DCIImport.DCIImport DCIImport { get; set; }

        public ClientContactRepository()
        {
            this.SystemCode = ConfigurationManager.AppSettings["SystemCode"].ToString();
            this.Username = ConfigurationManager.AppSettings["Username"].ToString();
            this.Password = ConfigurationManager.AppSettings["Password"].ToString();

            this.FileWarnings = 1;
            this.FileWarningsSpecified = true;
            this.ResultStream = String.Empty;
            this.RecordStream = String.Empty;
            this.Record = String.Empty;
            
            this.DCIImport = new DCIImport.DCIImport();
        }

        public string AddClientContact(Option ClientContact)
        {
            this.RecordStream = Helper.SerializeToString(ClientContact);
            return DCIImport.ImportRecord(SystemCode,
                Username,
                Password,
                Record,
                FileWarnings,
                FileWarningsSpecified,
                RecordStream,
                ResultStream);

        }
    }
}