using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestScriptLink2.ClientChargeInput;
using TestScriptLink2.Entities;
using System.Configuration;

namespace TestScriptLink2.Repositories
{
    public class ServiceRepository
    {
        public string SystemCode { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public ServiceRepository()
        {
            this.SystemCode = ConfigurationManager.AppSettings["SystemCode"].ToString();
            this.UserName = ConfigurationManager.AppSettings["Username"].ToString();
            this.Password = ConfigurationManager.AppSettings["Password"].ToString();
        }
        public WebServiceResponse FileService(Service service)
        {
            var clientChargeInput = new ClientChargeInput.ClientChargeInput();
            var response = clientChargeInput.FileClientChargeInput(this.SystemCode, this.UserName, this.Password,
                service.WebSvcObject);
            return response;
        }
    }
}