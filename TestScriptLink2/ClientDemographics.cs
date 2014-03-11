using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestScriptLink2.Repositories;

namespace TestScriptLink2
{
    public class ClientDemographics
    {
        private OptionObject optionObject;
        public OptionObject ReturnOptionObject { get; set; }

        public ClientDemographics(OptionObject optionObject)
        {
            this.optionObject = optionObject;
            ReturnOptionObject = new OptionObject();
        }

        public bool UpdateDemographics(ClientDemographicsWebSvc.ClientDemographicsObject ClientDemo)
        {
            var clientRepo = new ClientRepository();
            PopulateReturnOptionObject();
            var client = clientRepo.GetClientById(optionObject.EntityID);
            if (!String.IsNullOrEmpty(client.City))
                ClientDemo.ClientAddressCity = client.City;
            return clientRepo.UpdateClientDemographics(ClientDemo, optionObject.EntityID);
        }

        public FieldObject GetField(string fieldNumber)
        {
            return optionObject.Forms.SelectMany(r => r.CurrentRow.Fields).FirstOrDefault(f => f.FieldNumber.Equals(fieldNumber));
        }
        public void PopulateReturnOptionObject()
        {
            ReturnOptionObject.EntityID = optionObject.EntityID;
            ReturnOptionObject.EpisodeNumber = optionObject.EpisodeNumber;
            ReturnOptionObject.Facility = optionObject.Facility;
            ReturnOptionObject.OptionId = optionObject.OptionId;
            ReturnOptionObject.OptionStaffId = optionObject.OptionStaffId;
            ReturnOptionObject.OptionUserId = optionObject.OptionUserId;
            ReturnOptionObject.SystemCode = optionObject.SystemCode;
        }
    }
}
