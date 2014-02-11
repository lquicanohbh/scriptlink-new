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
using TestScriptLink2.CopyFieldValue;
using TestScriptLink2.Required;
using TestScriptLink2.CrisisWalkinContactLog;
using TestScriptLink2.AuthorizationReleaseOfInformation;

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
                case "CopyProblem":
                    medicalNote.CopyProblem();
                    returnOptionObject = medicalNote.ReturnOptionObject;
                    break;
                case "CalculateServiceCode":
                    medicalNote.GetServiceCode();
                    returnOptionObject = medicalNote.ReturnOptionObject;
                    break;
                case "CreateClientContact":
                    var ROI = new ReleaseOfInformation();
                    ROI.Initialize(optionObject);
                    var ClientContactRepository = new ClientContactRepository();
                    break;
                case "UpdateDemographics":
                    var Demo = new ClientDemographics(optionObject);
                    string fieldNumber = null;
                    if (Helper.SplitAndGetValueAt(scriptName, ',', 1, out fieldNumber))
                    {
                        var LGBTField = Demo.GetField(fieldNumber);
                        if (LGBTField != null && !String.IsNullOrEmpty(LGBTField.FieldValue))
                        {
                            var client = new ClientDemographicsWebSvc.ClientDemographicsObject()
                            {
                                SSDemographicsDict8 = LGBTField.FieldValue
                            };
                            Demo.UpdateDemographics(client);
                        }
                        returnOptionObject = Demo.ReturnOptionObject;
                    }
                    break;
                case "CopyFieldValue":
                    var copyField = new FieldCopy(optionObject, scriptName);
                    copyField.PerformCopy();
                    copyField.PopulateReturnOptionObject();
                    returnOptionObject = copyField.ReturnOptionObject;
                    break;
                case "MakeRequiredCond":
                    var fieldsRequired = new FieldsRequiredConditionally(optionObject, scriptName);
                    fieldsRequired.MakeFieldsRequired();
                    fieldsRequired.PopulateReturnOptionObject();
                    returnOptionObject = fieldsRequired.ReturnOptionObject;
                    break;
                case "CrisisWalkinDisposition":
                    var crisisWalkin = new CrisisWalkin(optionObject, scriptName);
                    crisisWalkin.UpdateFieldState();
                    crisisWalkin.PopulateReturnOptionObject();
                    returnOptionObject = crisisWalkin.ReturnOptionObject;
                    break;
                case "AuthorizationROI":
                    var authROI = new AuthorizationROI(optionObject, scriptName);
                    authROI.UpdateFieldState();
                    authROI.PopulateReturnOptionObject();
                    returnOptionObject = authROI.ReturnOptionObject;
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

    }
}
