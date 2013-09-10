using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Net.Mail;
using System.Configuration;
using TestScriptLink2.CPT;
using TestScriptLink2.Entities;
using TestScriptLink2.Repositories;

namespace TestScriptLink2
{
    /// <summary>
    /// Summary description for ScriptLinkService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class TestScriptlink : System.Web.Services.WebService
    {
        [WebMethod]
        public string GetVersion()
        {
            return "Version 1.0";
        }
        [WebMethod]
        public OptionObject RunScript(OptionObject optionObject, string scriptName)
        {
            var currentTime = DateTime.Now.ToString("hh:mm tt");
            OptionObject returnOptionObject = new OptionObject();
            //initialize the option object
            returnOptionObject.EntityID = optionObject.EntityID;
            returnOptionObject.OptionId = optionObject.OptionId;
            returnOptionObject.Facility = optionObject.Facility;
            returnOptionObject.SystemCode = optionObject.SystemCode;
            var medicalNote = new MedicalNote(optionObject);
            switch (scriptName.Split(',')[0])
            {
                case "EmailPsychcareAuthorization":
                    returnOptionObject = EmailPsychcareAuthorization(optionObject);
                    break;
                case "EmailUMBHRequest":
                    returnOptionObject = EmailUMBHRequest(optionObject);
                    break;
                //case "AddNewClientContact":
                //    returnOptionObject = AddNewClientContact(optionObject);
                //    break;
                case "CreateNonClientCharge":
                    var service = new Service();
                    if (service.PopulateService(optionObject))
                    {
                        var serviceRepository = new ServiceRepository();
                        var response = serviceRepository.FileService(service);
                        if (response.Status != 1)
                            Helper.sendEmail(ConfigurationManager.AppSettings["ErrorSender"].ToString(),
                                "Error while filing non client charge",
                                String.Format("Staff: {0}\nService Code: {1}\nProgram: {2}\nNote Status:{3}\nEntry: {4}",
                                optionObject.EntityID, service.ServiceCodeField.FieldValue,
                                service.ProgramCodeField.FieldValue, service.NoteStatus.FieldValue,
                                DateTime.Now.ToShortDateString()),
                                new List<string> { ConfigurationManager.AppSettings["ErrorRecipient"].ToString() });

                    }
                    else
                    {
                        returnOptionObject = Helper.ReturnOptionObject(optionObject);
                    }
                    break;
                case "MedicalServicesProgressNoteLoadCC":
                    medicalNote.CreateChiefComplaint();
                    medicalNote.DefaultCurrentProgram();
                    medicalNote.DefaultMostRecentProblem();
                    medicalNote.DefaultMGAF();
                    returnOptionObject = medicalNote.ReturnOptionObject;
                    break;
                case "UpdateMedicalServicesProgressNoteCC":
                    medicalNote.UpdateChiefComplaint();
                    returnOptionObject = medicalNote.ReturnOptionObject;
                    break;
                case "CreateNoteSummary":
                    medicalNote.CreateNoteSummary();
                    returnOptionObject = medicalNote.ReturnOptionObject;
                    break;
                case "DefaultVitalSigns":
                    medicalNote.DefaultVitalSigns();
                    returnOptionObject = medicalNote.ReturnOptionObject;
                    break;
                case "CalculateServiceCode":
                    medicalNote.GetServiceCode();
                    returnOptionObject = medicalNote.ReturnOptionObject;
                    break;
                default:
                    break;
            }
            return returnOptionObject;
        }

        private OptionObject EmailPsychcareAuthorization(OptionObject optionObject)
        {
            var returnOptionObject = new OptionObject();
            var assessDate = new FieldObject { FieldNumber = "146.43" };
            var assessClin = new FieldObject { FieldNumber = "146.45" };

            foreach (var form in optionObject.Forms)
            {
                foreach (var field in form.CurrentRow.Fields)
                {
                    if (field.FieldNumber.Equals(assessDate.FieldNumber))
                        assessDate.FieldValue = field.FieldValue;
                    if (field.FieldNumber.Equals(assessClin.FieldNumber))
                        assessClin.FieldValue = field.FieldValue;
                }
            }

            var formInformation = PsychcareAuthFormRepository.GetForm(optionObject.EntityID, optionObject.EpisodeNumber.ToString(), DateTime.Parse(assessDate.FieldValue), assessClin.FieldValue.ToString(), DateTime.Now);

            var emailBody = String.Empty;
            foreach (var f in formInformation)
            {
                emailBody += "\n\n" + f.ToString();
            }
            var firstClient = formInformation[0];
            Helper.sendEmail(ConfigurationManager.AppSettings["FromEmailAuth"].ToString(),
                "Auth Request form for (" + firstClient.ClientId + ") " + firstClient.ClientName,
                "Psychcare Authorization\n\n" + emailBody,
                ConfigurationManager.AppSettings["PsychcareEmaiList"].ToString().Split(',').ToList());

            returnOptionObject.EntityID = optionObject.EntityID;
            returnOptionObject.OptionId = optionObject.OptionId;
            returnOptionObject.Facility = optionObject.Facility;
            returnOptionObject.SystemCode = optionObject.SystemCode;

            return returnOptionObject;
        }
        private OptionObject EmailUMBHRequest(OptionObject optionObject)
        {
            var returnOptionObject = new OptionObject();
            var assessDate = new FieldObject { FieldNumber = "146.89" };
            var assessClin = new FieldObject { FieldNumber = "146.9" };

            foreach (var form in optionObject.Forms)
            {
                foreach (var field in form.CurrentRow.Fields)
                {
                    if (field.FieldNumber.Equals(assessDate.FieldNumber))
                        assessDate.FieldValue = field.FieldValue;
                    if (field.FieldNumber.Equals(assessClin.FieldNumber))
                        assessClin.FieldValue = field.FieldValue;
                }
            }

            //var formInformation = UMBHTboReqRepository.GetForm(optionObject.EntityID, optionObject.EpisodeNumber.ToString(), DateTime.Parse(assessDate.FieldValue));
            var formInformation = UMBHTboReqRepository.GetForm(optionObject.EntityID, optionObject.EpisodeNumber.ToString(), DateTime.Parse(assessDate.FieldValue), assessClin.FieldValue.ToString(), DateTime.Now);

            var emailBody = string.Empty;

            foreach (var f in formInformation)
            {
                emailBody += "\n\n" + f.ToString();
            }
            var firstClient = formInformation[0];
            Helper.sendEmail(ConfigurationManager.AppSettings["FromEmailAuth"].ToString(),
                "Auth Request form for (" + firstClient.ClientId + ") " + firstClient.ClientName,
                "UMBH TBOS Request \n\n" + emailBody,
                ConfigurationManager.AppSettings["UMBHEmailList"].ToString().Split(',').ToList());

            returnOptionObject.EntityID = optionObject.EntityID;
            returnOptionObject.OptionId = optionObject.OptionId;
            returnOptionObject.Facility = optionObject.Facility;
            returnOptionObject.SystemCode = optionObject.SystemCode;

            return returnOptionObject;


        }
        //private OptionObject AddNewClientContact(OptionObject optionObject)
        //{
        //    var returnOptionObject = new OptionObject();
        //    var Acknowledgment = new FieldObject("131.88");
        //    var EntryDate = new FieldObject("131.77");

        //    foreach (var form in optionObject.Forms)
        //    {
        //        foreach (var field in form.CurrentRow.Fields)
        //        {
        //            if (field.FieldNumber.Equals(Acknowledgment.FieldNumber))
        //                Acknowledgment.FieldValue = field.FieldValue;
        //            if (field.FieldNumber.Equals(EntryDate.FieldNumber))
        //                EntryDate.FieldValue = field.FieldValue;
        //        }
        //    }
        //    if (Acknowledgment.FieldValue.Equals("1"))
        //    {
        //        List<NewClientContact> contactInformationList = NewClientContactRepository.GetContactInfo(optionObject.EntityID, optionObject.EpisodeNumber.ToString(), DateTime.Parse(EntryDate.FieldValue));//, DateTime.Now);
        //        AddContact(contactInformationList);
        //    }
        //    returnOptionObject.EntityID = optionObject.EntityID;
        //    returnOptionObject.OptionId = optionObject.OptionId;
        //    returnOptionObject.Facility = optionObject.Facility;
        //    returnOptionObject.SystemCode = optionObject.SystemCode;

        //    return returnOptionObject;
        //}
        //private void AddContact(List<NewClientContact> contactInformationList)
        //{
        //    string xmlHeader = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
        //                                "<option>" +
        //                                "<optionidentifier>USER34</optionidentifier>";
        //    string xmlFooter = "</option>";
        //    var SystemCode = ConfigurationManager.AppSettings["SystemCode"].ToString();
        //    var Username = ConfigurationManager.AppSettings["Username"].ToString();
        //    var Password = ConfigurationManager.AppSettings["Password"].ToString();
        //    var record = "";
        //    long filewarnings = 1;
        //    bool filewarningsSpecified = true;
        //    string resultStream = "";
        //    string recordStream = "";
        //    recordStream = xmlHeader;
        //    foreach (var contact in contactInformationList)
        //    {
        //        recordStream += contact.ToString();
        //    }
        //    recordStream += xmlFooter;
        //    recordStream = "<?xml version=1.0 encoding=UTF-8 standalone=yes?><option><optionidentifier>USER40</optionidentifier><option_data><PATID>1000092</PATID><EPISODE_NUMBER>1</EPISODE_NUMBER><SYSTEM.new_client_contact_info><Contact_Date>2013-05-07</Contact_Date><Contact_Name>TEST,TEST</Contact_Name><Contact_Type>99</Contact_Type></SYSTEM.new_client_contact_info></option_data></option>";
        //    var test = new DCIImport.DCIImport();
        //    var result = test.ImportRecord(SystemCode, Username, Password, record, filewarnings, filewarningsSpecified, recordStream, resultStream);
        //}
    }
}
