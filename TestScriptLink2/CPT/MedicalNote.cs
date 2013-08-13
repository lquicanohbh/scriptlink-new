using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using TestScriptLink2.Entities;
using TestScriptLink2.Repositories;
using System.Text;

namespace TestScriptLink2.CPT
{
    public class MedicalNote
    {
        public Client Client { get; set; }
        public OptionObject ReturnOptionObject { get; set; }
        public OptionObject OriginalOptionObject { get; set; }

        #region FieldNumbers
        public string ChiefComplaintFieldNumber { get; set; }
        public string MedicalConcernsFieldNumber { get; set; }
        public string TreatmentPlanFieldNumber { get; set; }
        public string AbnormalPsychoticThoughtsCommentsFieldNumber { get; set; }
        public string AbnormalPsychoticThoughtsFieldNumber { get; set; }
        public string PlanFieldNumber { get; set; }
        public string ParticipantsFieldNumber { get; set; }
        public string LocationFieldNumber { get; set; }
        public string NoteSummaryFieldNumber { get; set; }
        public string DateFieldNumber { get; set; }
        public string VitalSignsFieldNumber { get; set; }
        public string ProgramFieldNumber { get; set; }
        public string ServiceCodeFieldNumber { get; set; }
        public string ICFieldNumber { get; set; }
        public string Problem1FieldNumber { get; set; }
        #endregion

        public FormObject ReturnFormObject { get; set; }
        public RowObject ReturnCurrentRowObject { get; set; }
        public string ParticipantsWording { get; set; }

        public MedicalNote(OptionObject optionObject)
        {
            this.ReturnOptionObject = new OptionObject(optionObject.EntityID,
                optionObject.EpisodeNumber,
                optionObject.Facility,
                optionObject.OptionId,
                optionObject.OptionStaffId,
                optionObject.OptionUserId,
                optionObject.SystemCode);
            this.OriginalOptionObject = optionObject;
            this.ChiefComplaintFieldNumber = "151.3";
            this.MedicalConcernsFieldNumber = "152.37";
            this.TreatmentPlanFieldNumber = "151.8";
            this.AbnormalPsychoticThoughtsFieldNumber = "151.38";
            this.AbnormalPsychoticThoughtsCommentsFieldNumber = "152.74";
            this.PlanFieldNumber = "151.32";
            this.ParticipantsFieldNumber = "152.28";
            this.NoteSummaryFieldNumber = "152.45";
            this.DateFieldNumber = "151.28";
            this.VitalSignsFieldNumber = "152.67";
            this.ProgramFieldNumber = "151.78";
            this.ServiceCodeFieldNumber = "151.75";
            this.LocationFieldNumber = "151.99";
            this.ICFieldNumber = "151.82";
            this.ReturnFormObject = new FormObject("188");
            this.ReturnCurrentRowObject = new RowObject("0", "188||1", "EDIT");
            this.ParticipantsWording = "Present in this session";
            this.Problem1FieldNumber = "152.7";
        }
        private string FormatVitalSigns(List<VitalSign> VitalSigns)
        {
            var firstVital = VitalSigns.First();
            var temp = String.Format("Vital signs taken by {0} on {1} {2}:\n",
                                    firstVital.EntryName,
                                    firstVital.DateTaken.ToString("MM/dd/yyyy"),
                                    firstVital.TimeTaken.ToString("hh:mm tt"));
            foreach (var vitalSign in VitalSigns)
            {
                temp += String.Format("{0} : {1}\n", vitalSign.VitalSignDescription, vitalSign.ReadingValue);
            }
            return temp;
        }
        public void DefaultVitalSigns()
        {
            var dateField = this.OriginalOptionObject.Forms.First().CurrentRow.Fields.FirstOrDefault(f => f.FieldNumber.Equals(this.DateFieldNumber));
            var vitalSigns = VitalSignRepository.GetByDateClient(this.OriginalOptionObject.EntityID, DateTime.Parse(dateField.FieldValue));
            if (vitalSigns != null)
            {
                UpdateReturnOptionObject(this.VitalSignsFieldNumber, FormatVitalSigns(vitalSigns));
            }
        }
        private void CreateReturnOptionObject(string FieldNumber, string FieldValue)
        {
            this.ReturnOptionObject.Forms.Add(new FormObject
            {
                FormId = this.ReturnFormObject.FormId,
                CurrentRow = new RowObject
                {
                    ParentRowId = this.ReturnCurrentRowObject.ParentRowId,
                    RowId = this.ReturnCurrentRowObject.RowId,
                    RowAction = this.ReturnCurrentRowObject.RowAction,
                    Fields = new List<FieldObject>
                        {
                            new FieldObject{
                                FieldNumber = FieldNumber,
                                FieldValue = FieldValue
                            }
                        }
                }
            });
        }
        private void AddFieldToReturnOptionObject(string FieldNumber, string FieldValue)
        {
            this.ReturnOptionObject.Forms.First().CurrentRow.Fields.Add(
                    new FieldObject
                    {
                        FieldNumber = FieldNumber,
                        FieldValue = FieldValue
                    });
        }
        public void CreateChiefComplaint()
        {
            var currentChiefComplaintValue = this.OriginalOptionObject.Forms.First().CurrentRow.Fields.FirstOrDefault(f => f.FieldNumber.Equals(this.ChiefComplaintFieldNumber));
            if (currentChiefComplaintValue != null && String.IsNullOrEmpty(currentChiefComplaintValue.FieldValue))
            {
                var Client = ClientRepository.GetClientById(this.OriginalOptionObject.EntityID);
                var DefaultText = String.Format("Patient, {0}, a {1} year old {2} {3} came in for follow-up on {4}.",
                    Client.Name,
                    Client.Age,
                    Client.GenderValue,
                    Client.RaceValue,
                    DateTime.Now.ToShortDateString());
                UpdateReturnOptionObject(this.ChiefComplaintFieldNumber, DefaultText);
            }
        }
        public void UpdateChiefComplaint()
        {
            var ChiefComplaintField = new FieldObject();
            ChiefComplaintField = this.OriginalOptionObject.Forms.First().CurrentRow.Fields.First(f => f.FieldNumber == this.ChiefComplaintFieldNumber);
            var ParticipantsField = this.OriginalOptionObject.Forms.First().CurrentRow.Fields.First(f => f.FieldNumber == this.ParticipantsFieldNumber);
            var ParticipantsParameter = Helper.FormatMultipleValueToQueryParameter(ParticipantsField.FieldValue);
            var DictionaryValues = DictionaryRepository.GetDictionaryValues(ParticipantsField.FieldNumber, ParticipantsParameter);

            if (DictionaryValues.Count > 0)
                UpdateReturnOptionObject(this.ChiefComplaintFieldNumber, AppendParticipantsToCurrentChiefComplaint(ChiefComplaintField.FieldValue, DictionaryValues));
            else
                UpdateReturnOptionObject(this.ChiefComplaintFieldNumber, GetOriginalTextInChiefComplaint(ChiefComplaintField.FieldValue));
        }
        public void DefaultMostRecentProblem()
        {
            var currentProblemValue = this.OriginalOptionObject.Forms.First().CurrentRow.Fields.FirstOrDefault(f => f.FieldNumber.Equals(this.Problem1FieldNumber));
            if (currentProblemValue != null && String.IsNullOrEmpty(currentProblemValue.FieldValue))
            {
                var problems = ProblemRepository.GetMostRecentProblems(
                    this.OriginalOptionObject.EntityID,
                    1);
                var problem = problems.FirstOrDefault();
                var problemDescription = problem == null ? String.Empty : String.Format("({0}) {1}", problem.ProblemCode, problem.ProblemDescription);
                UpdateReturnOptionObject(this.Problem1FieldNumber, problemDescription);
            }
        }
        public void DefaultCurrentProgram()
        {
            var currentProgramValue = this.OriginalOptionObject.Forms.First().CurrentRow.Fields.FirstOrDefault(f => f.FieldNumber.Equals(this.ProgramFieldNumber));
            if (currentProgramValue != null && String.IsNullOrEmpty(currentProgramValue.FieldValue))
            {
                var client = ClientRepository.GetClientByIdWithEpisode(
                    this.OriginalOptionObject.EntityID,
                    this.OriginalOptionObject.EpisodeNumber);
                UpdateReturnOptionObject(this.ProgramFieldNumber, client.EpisodeInformation.ProgramCode);
            }
        }
        private void UpdateReturnOptionObject(string FieldNumber, string FieldValue)
        {
            if (this.ReturnOptionObject.Forms.Any() && this.ReturnOptionObject.Forms.First().CurrentRow.Fields.Any())
            {
                AddFieldToReturnOptionObject(FieldNumber, FieldValue);
            }
            else
            {
                CreateReturnOptionObject(FieldNumber, FieldValue);
            }
        }
        private string AppendParticipantsToCurrentChiefComplaint(string CurrentText, List<FormDictionary> Dictionaries)
        {
            CurrentText = GetOriginalTextInChiefComplaint(CurrentText);
            var lastChar = String.IsNullOrEmpty(CurrentText) ? String.Empty : CurrentText.Last().ToString();
            if (lastChar == "\n" || String.IsNullOrEmpty(lastChar))
            {
                CurrentText += Dictionaries.Count == 1 ? this.ParticipantsWording + " is " : this.ParticipantsWording + " are ";
            }
            else
            {
                CurrentText += Dictionaries.Count == 1 ? "\n" + this.ParticipantsWording + " is " : "\n" + this.ParticipantsWording + " are ";
            }
            foreach (var code in Dictionaries.Take(Dictionaries.Count - 1))
            {
                CurrentText += code.Value + ", ";
            }
            CurrentText += Dictionaries.Last().Value + ".";
            return CurrentText;
        }
        private string GetOriginalTextInChiefComplaint(string ChiefComplaintText)
        {
            var indexOfParticipants = ChiefComplaintText.IndexOf("\n" + this.ParticipantsWording);
            var indexOfParticipants2 = ChiefComplaintText.IndexOf(this.ParticipantsWording);
            if (indexOfParticipants >= 0)
                return ChiefComplaintText.Substring(0, indexOfParticipants);
            else if (indexOfParticipants < 0 && indexOfParticipants2 >= 0)
                return ChiefComplaintText.Substring(0, indexOfParticipants2);
            else
                return ChiefComplaintText;
        }
        public void CreateNoteSummary()
        {
            var dictionaryList = InitializeDictionaryList();
            foreach (var field in OriginalOptionObject.Forms.First().CurrentRow.Fields)
            {
                GetAndSetField(dictionaryList, field.FieldNumber, field.FieldValue);
            }
            var populatedList = PopulateDictionary(dictionaryList);

            var summaryText = GetSummaryText(populatedList);
            UpdateReturnOptionObject(this.NoteSummaryFieldNumber, summaryText);
        }
        public void GetServiceCode()
        {
            var dictionaryList = InitializeDictionaryList();
            foreach (var field in OriginalOptionObject.Forms.First().CurrentRow.Fields)
            {
                GetAndSetField(dictionaryList, field.FieldNumber, field.FieldValue);
            }
            var populatedList = PopulateDictionary(dictionaryList);
            var serviceCode = CalculateServiceCode(populatedList, OriginalOptionObject.Forms.First().CurrentRow.Fields);
            if (serviceCode.InteractiveComplexity)
                UpdateReturnOptionObject(this.ICFieldNumber, "1");
            UpdateReturnOptionObject(this.ServiceCodeFieldNumber, serviceCode.ServiceCode);
        }
        private string GetSummaryText(List<FormDictionary> list)
        {
            var sb = new StringBuilder();
            var hpi = new StringBuilder();
            var pp = new StringBuilder();
            var ros = new StringBuilder();
            var ms = new StringBuilder();
            var vs = new StringBuilder();
            var ic = new StringBuilder();
            var dp = new StringBuilder();
            var hpiLocation = new StringBuilder();
            var mentalStatus = new Dictionary<string, StringBuilder>();
            sb.AppendFormat("Chief Complaint: {0}\n", GetFieldValue(this.ChiefComplaintFieldNumber));//chiefcomplaint
            foreach (var dictionary in list)
            {
                if (!dictionary.Value.Equals("Not Reviewed") && !dictionary.Value.Equals("N/A"))
                {
                    if (GetSectionList("Presenting Problems").Contains(dictionary.FieldNumber))
                    {
                        if (pp.Length == 0)
                            pp.AppendFormat("This note addresses the following problem(s):\n");
                        pp.AppendFormat("-{0} as {1}.\n", GetProblemTypeForProblem(dictionary.FieldNumber), dictionary.Value);
                    }
                    else if (GetSectionList("History of Present Illness").Contains(dictionary.FieldNumber))
                    {
                        if (hpi.Length == 0)
                            hpi.AppendFormat("History of Present Illness:\n");
                        if (dictionary.FieldNumber == this.LocationFieldNumber)
                        {
                            if (hpiLocation.Length == 0)
                                hpiLocation.AppendFormat("Located on the");
                            hpiLocation.AppendFormat(" {0},", dictionary.Value);
                        }
                        else
                        {
                            hpi.Append(GetHPIString(dictionary));
                        }
                    }
                    else if (GetSectionList("Review of Systems").Contains(dictionary.FieldNumber))
                    {
                        if (ros.Length == 0)
                            ros.AppendFormat("Review of Systems:\n");
                        ros.AppendFormat("{0} {1} system. ", dictionary.Value, dictionary.FieldDescription);
                    }
                    else if (GetSectionList("Vital Signs").Contains(dictionary.FieldNumber))
                    {
                        vs.AppendFormat("{0} were {1}.\n", dictionary.FieldDescription, dictionary.Value);
                    }
                    else if (GetSectionList("Mental Status Examination").Contains(dictionary.FieldNumber))
                    {
                        UpdateMentalStatusSummary(mentalStatus, dictionary);
                    }
                    else if (GetSectionList("Interactive Complexity").Contains(dictionary.FieldNumber))
                    {
                        if (ic.Length == 0)
                            ic.AppendFormat("Interactive Complexity in this session:\n");
                        ic.AppendFormat("-{0}.\n", dictionary.FieldDescription);
                    }
                    else if (GetSectionList("Data Points").Contains(dictionary.FieldNumber))
                    {
                        if (dp.Length == 0)
                            dp.AppendFormat("Other procedures addressed:\n");
                        dp.AppendFormat("-{0}.\n", dictionary.FieldDescription);
                    }
                }
            }
            if (pp.Length != 0)
                sb.AppendFormat("{0}\n", pp.ToString());
            if (hpiLocation.Length != 0)
                hpi.Append(hpiLocation.Remove(hpiLocation.Length - 1, 1).Append(". ").Append(GetHPIComments(this.LocationFieldNumber)));
            if (hpi.Length != 0)
                sb.AppendFormat("{0}\n", hpi.ToString());
            if (ros.Length != 0)
                sb.AppendFormat("{0}\n", ros.Append(GetFieldValue(this.MedicalConcernsFieldNumber)));
            if (vs.Length != 0)
                sb.AppendFormat("{0}\n", vs.ToString());
            var medicalConcern = GetFieldValue(this.MedicalConcernsFieldNumber);
            if (!String.IsNullOrEmpty(medicalConcern))
                sb.AppendFormat("Medical Concerns: {0}\n", medicalConcern);
            var treatmentPlan = GetFieldValue(this.TreatmentPlanFieldNumber);
            if (!String.IsNullOrEmpty(treatmentPlan))
                sb.AppendFormat("Treatment Plan: {0}\n", treatmentPlan);
            if (mentalStatus.Any())
            {
                ms.AppendFormat("Mental Status Examination:\n");
                foreach (var temp in mentalStatus)
                {
                    ms.AppendFormat("{0}. ", temp.Value.ToString());
                    if (temp.Key.Equals(this.AbnormalPsychoticThoughtsFieldNumber))
                        ms.AppendFormat("Comments: {0}", GetFieldValue(this.AbnormalPsychoticThoughtsCommentsFieldNumber));
                }
            }
            if (ms.Length != 0)
                sb.AppendFormat("{0}\n\n", ms.ToString());
            sb.AppendFormat("Plan: {0}\n", GetFieldValue(this.PlanFieldNumber));
            if (dp.Length != 0)
                sb.AppendFormat("{0}\n", dp.ToString());
            if (ic.Length != 0)
                sb.AppendFormat("{0}\n", ic.ToString());

            return sb.ToString();
        }
        private void UpdateMentalStatusSummary(Dictionary<string, StringBuilder> mentalStatus, FormDictionary dictionary)
        {
            if (mentalStatus.ContainsKey(dictionary.FieldNumber))
            {
                mentalStatus[dictionary.FieldNumber] = mentalStatus[dictionary.FieldNumber].Insert(0, String.Format("{0}, ", dictionary.Value));
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendFormat("{0} {1}", dictionary.Value, dictionary.FieldDescription);
                mentalStatus.Add(dictionary.FieldNumber, sb);
            }
        }
        private string GetHPIString(FormDictionary dictionary)
        {
            if (dictionary.FieldNumber == "152.43")//severity
            {
                return String.Format("{0} {1}. ", dictionary.Value, dictionary.FieldDescription);
            }
            else if (dictionary.FieldNumber == "152.44")//timing
            {
                return String.Format("{0} {1}. ", dictionary.Value, dictionary.FieldDescription);
            }
            else if (dictionary.FieldNumber == "152.41")//context
            {
                return GetHPIComments("152.41").Replace("\n", " ");
            }
            else if (dictionary.FieldNumber == "152.42")//modifying factors
            {
                return GetHPIComments("152.42").Replace("\n", " ");
            }
            else if (dictionary.FieldNumber == "152.4")//associated signs and symptoms
            {
                return GetHPIComments("152.4").Replace("\n", " ");
            }
            else if (dictionary.FieldNumber == "151.89")//duration
            {
                return String.Format("Lasting approximately {0} {1}. ", GetHPIComments("151.89"), dictionary.Value);
            }
            else
            {
                return String.Empty;
            }
        }
        private string GetHPIComments(string fieldNumber)
        {
            if (fieldNumber == "151.99")//location
            {
                return GetFieldValue("152");
            }
            else if (fieldNumber == "152.41")//context
            {
                return GetFieldValue("151.97");
            }
            else if (fieldNumber == "152.42")//modifying factors
            {
                return GetFieldValue("152.02");
            }
            else if (fieldNumber == "152.4")//associated signs and symptoms
            {
                return GetFieldValue("152.04");
            }
            else if (fieldNumber == "151.89")//duration
            {
                return GetFieldValue("151.91");
            }
            else
            {
                return String.Empty;
            }
        }
        private string GetProblemTypeForProblem(string fieldNumber)
        {
            if (fieldNumber == "152.63")
                return GetFieldValue("152.7").Replace("\n", String.Empty);
            else if (fieldNumber == "152.64")
                return GetFieldValue("152.71").Replace("\n", String.Empty);
            else if (fieldNumber == "152.65")
                return GetFieldValue("152.72").Replace("\n", String.Empty);
            else if (fieldNumber == "152.66")
                return GetFieldValue("152.73").Replace("\n", String.Empty);
            else
                return String.Empty;
        }
        private string GetFieldValue(string fieldNumber)
        {
            return this.OriginalOptionObject.Forms.First().CurrentRow.Fields.FirstOrDefault(x => x.FieldNumber.Equals(fieldNumber)).FieldValue;
        }
        private List<string> GetSectionList(string section)
        {
            var list = new List<string>();
            switch (section)
            {
                case "Participants":
                    var temp1 = new List<string> { "152.28" };
                    temp1.ForEach(x => list.Add(x));
                    break;
                case "Presenting Problems":
                    var temp2 = new List<string> { "152.63", "152.64", "152.65", "152.66" };
                    temp2.ForEach(x => list.Add(x));
                    break;
                case "History of Present Illness":
                    var temp3 = new List<string> { "151.89", "151.99", "152.4", "152.41", "152.42", "152.43", "152.44" };
                    temp3.ForEach(x => list.Add(x));
                    break;
                case "Review of Systems":
                    var temp4 = new List<string> { "152.2", "152.21", "152.22", "152.23", "152.24", "152.29", "152.3", "152.31", "152.32", "152.33", "152.34", "152.35", "152.36" };
                    temp4.ForEach(x => list.Add(x));
                    break;
                case "Mental Status Examination":
                    var temp5 = new List<string> { "151.33", "151.36", "151.37", "151.38", "151.4", "151.41", "151.42", "151.43", "151.44", "151.45", "151.56", "152.68" };
                    temp5.ForEach(x => list.Add(x));
                    break;
                case "Vital Signs":
                    var temp6 = new List<string> { "152.69" };
                    temp6.ForEach(x => list.Add(x));
                    break;
                case "Interactive Complexity":
                    var temp7 = new List<string> { "151.7", "151.71", "151.72", "151.73", "151.74" };
                    temp7.ForEach(x => list.Add(x));
                    break;
                case "Data Points":
                    var temp8 = new List<string> { "152.53", "152.54", "152.55", "152.56", "152.57", "152.58", "152.59" };
                    temp8.ForEach(x => list.Add(x));
                    break;
                default:
                    list = null;
                    break;
            }
            return list;
        }
        private List<FormDictionary> InitializeDictionaryList()
        {
            var dictionaryList = new List<FormDictionary>();
            #region Participants
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.28" });//participants
            #endregion

            #region Presenting Problems
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.63" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.64" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.65" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.66" });
            #endregion

            #region History of Present Illness
            dictionaryList.Add(new FormDictionary { FieldNumber = "151.89" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "151.99" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.4" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.41" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.42" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.43" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.44" });
            #endregion

            #region Review of Systems
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.2" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.21" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.22" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.23" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.24" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.29" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.3" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.31" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.32" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.33" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.34" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.35" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.36" });
            #endregion

            #region Mental Status Examination
            dictionaryList.Add(new FormDictionary { FieldNumber = "151.33" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "151.36" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "151.37" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "151.38" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "151.4" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "151.41" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "151.42" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "151.43" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "151.44" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "151.45" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "151.56" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.68" });
            #endregion

            #region Vital Signs
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.69" });
            #endregion

            #region Interactive Complexity
            dictionaryList.Add(new FormDictionary { FieldNumber = "151.7" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "151.71" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "151.72" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "151.73" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "151.74" });
            #endregion

            #region Data Points
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.53" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.54" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.55" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.56" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.57" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.58" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.59" });
            #endregion
            return dictionaryList;
        }
        private void GetAndSetField(List<FormDictionary> list, string fieldNumber, string fieldValue)
        {
            var tempField = list.FirstOrDefault(x => x.FieldNumber.Equals(fieldNumber));
            if (tempField != null)
            {
                if (fieldValue.IndexOf('&') > -1)
                {
                    list.Remove(tempField);
                    foreach (var field in fieldValue.Split('&'))
                    {
                        list.Add(new FormDictionary { FieldNumber = fieldNumber, Code = field });
                    }
                }
                else
                {
                    tempField.Code = fieldValue;
                }
            }
        }
        private List<FormDictionary> PopulateDictionary(List<FormDictionary> list)
        {
            var tempList = new List<FormDictionary>();
            foreach (var dictionary in list)
            {
                if (!String.IsNullOrEmpty(dictionary.Code))
                    tempList.Add(dictionary);
            }
            var dictionaries = DictionaryRepository.GetDictionaryValues(tempList);
            return dictionaries;
        }
        private CPTCode CalculateServiceCode(List<FormDictionary> list, List<FieldObject> fields)
        {
            int HistoryType = 0;
            int ExaminationType = 0;
            int MDMType = 0;

            #region History
            var HPIFields = new List<string> { "151.91", "151.99", "152.4", "152.41", "152.42", "152.43", "152.44" };
            var PFSHFields = new List<string> { "151.85", "152.46", "152.47" };
            var ROSFields = new List<string> {"152.2","152.21","152.22","152.23","152.24",
                "152.29","152.3","152.31","152.32","152.33",
                "152.34","152.35","152.36",};
            #endregion

            #region Examination
            var PsyExamFields = new List<string> {"151.33","151.36","151.37","151.38","151.4",
                "151.41","151.42","151.43","151.44","151.45",
                "151.56"};
            #endregion

            #region MDM
            var ProblemPointsFields = new List<string> { "152.63", "152.64", "152.65", "152.66" };
            var DataPointsFields = new List<string> { "152.53", "152.54", "152.55", "152.56", "152.57", "152.58", "152.59" };
            //var TableOfRiskFields = new List<string> { "" };
            #endregion

            #region Interactive Complexity
            var ICFields = new List<string> { "151.7", "151.71", "151.72", "151.73", "151.74" };
            #endregion


            var HPIValue = CountFields(list, HPIFields);
            var PFSHValue = CountFields(fields, PFSHFields);
            var ROSValue = CountFields(list, ROSFields);

            var PsyExamValue = CountFields(list, PsyExamFields);

            var ProblemPointsValue = SumFieldValue(fields, ProblemPointsFields);
            var DataPointsValue = SumDictionaryCode(list, DataPointsFields);

            var ICValue = CountFields(fields, ICFields);

            HistoryType = CalculateHistoryType(HPIValue, PFSHValue, ROSValue);
            ExaminationType = CalculateExaminationType(PsyExamValue, 10, 10);
            MDMType = CalculateMDMType(ProblemPointsValue, DataPointsValue, 0);

            int min = Helper.Min(HistoryType, ExaminationType, MDMType);
            var pointsList = new List<int> { HistoryType, ExaminationType, MDMType };
            pointsList.Remove(min);
            var max1 = pointsList.ElementAt(0);
            var max2 = pointsList.ElementAt(1);


            string ServiceCode = "802";//99211

            if (max1 >= 4 && max2 >= 4)
            {
                ServiceCode = "806";//99215
            }
            else if (max1 >= 3 && max2 >= 3)
            {
                ServiceCode = "805";//99214
            }
            else if (max1 >= 2 && max2 >= 2)
            {
                ServiceCode = "804";//99213
            }
            else if (max1 >= 1 && max2 >= 1)
            {
                ServiceCode = "803";//99212
            }
            else
            {
                ServiceCode = "802";//99211
            }

            return new CPTCode(ServiceCode, ICValue > 0);
        }
        private int CalculateHistoryType(int HPIValue, int PFSHValue, int ROSValue)
        {
            int temp = 0;
            if (HPIValue >= 4 && PFSHValue >= 2 && ROSValue >= 10)
            {
                temp = 4;
            }
            else if (HPIValue >= 4 && PFSHValue >= 1 && ROSValue >= 2)
            {
                temp = 3;
            }
            else if (HPIValue >= 1 && ROSValue >= 1)
            {
                temp = 2;
            }
            else if (HPIValue >= 1)
            {
                temp = 1;
            }
            else
            {
                temp = 0;
            }
            return temp;
        }
        private int CalculateExaminationType(int Psychiatric, int Musco, int Const)
        {
            int temp = 0;
            if (Psychiatric >= 11 && Const >= 2 && Musco >= 1)
            {
                temp = 4;
            }
            else if ((Psychiatric + Musco + Const) >= 9)
            {
                temp = 3;
            }
            else if ((Psychiatric + Musco + Const) >= 6)
            {
                temp = 2;
            }
            else if ((Psychiatric + Musco + Const) >= 1)
            {
                temp = 1;
            }
            else
            {
                temp = 0;
            }
            return temp;
        }
        private int CalculateMDMType(int ProblemPoints, int DataPoints, int Risk)
        {
            int temp = 0;
            int min = Helper.Min(ProblemPoints, DataPoints, Risk);
            var pointsList = new List<int> { ProblemPoints, DataPoints, Risk };
            pointsList.Remove(min);
            var max1 = pointsList.ElementAt(0);
            var max2 = pointsList.ElementAt(1);

            if (max1 >= 4 && max2 >= 4)
            {
                temp = 4; //high
            }
            else if (max1 >= 3 && max2 >= 3)
            {
                temp = 3; //moderate
            }
            else if (max1 >= 2 && max2 >= 2)
            {
                temp = 2; //low
            }
            else if (max1 >= 1 && max2 >= 1)
            {
                temp = 1; //straightforward
            }
            else
            {
                temp = 0; // N/A
            }
            return temp;
        }
        private int CountFields(List<FormDictionary> list, List<string> fieldNumberList)
        {
            var tempList = list.Where(x => fieldNumberList.Contains(x.FieldNumber)).ToList();
            int count = 0;
            foreach (var dictionary in tempList)
            {
                if (!dictionary.Value.Equals("Not Reviewed"))
                {
                    count++;
                }
            }
            return count;
        }
        private int CountFields(List<FieldObject> list, List<string> fieldNumberList)
        {
            var tempList = list.Where(x => fieldNumberList.Contains(x.FieldNumber)).ToList();
            int count = 0;
            foreach (var field in tempList)
            {
                if (!String.IsNullOrEmpty(field.FieldValue))
                {
                    count++;
                }
            }
            return count;
        }
        private int SumFieldValue(List<FieldObject> list, List<string> fieldNumberList)
        {
            var tempList = list.Where(x => fieldNumberList.Contains(x.FieldNumber)).ToList();
            int sum = 0;
            foreach (var field in tempList)
            {
                if (!String.IsNullOrEmpty(field.FieldValue))
                {
                    sum += Convert.ToInt32(field.FieldValue);
                }
            }
            return sum;
        }
        private int SumDictionaryCode(List<FormDictionary> list, List<string> fieldNumberList)
        {
            var tempList = list.Where(x => fieldNumberList.Contains(x.FieldNumber)).ToList();
            int sum = 0;
            foreach (var dictionary in tempList)
            {
                if (!String.IsNullOrEmpty(dictionary.Code))
                {
                    sum += Convert.ToInt32(dictionary.Code);
                }
            }
            return sum;
        }

        public void UpdateCount(string scriptName)
        {
            try
            {
                var fieldNumberToUpdate = scriptName.Split(',')[1];
                var pointLimit = Convert.ToInt32(scriptName.Split(',')[2]);
                var changeValue = Convert.ToInt32(scriptName.Split(',')[3]);
                var action = scriptName.Split(',')[4];
                var CurrentValue = Convert.ToInt32(
                    this.OriginalOptionObject.Forms.First().CurrentRow.Fields
                    .FirstOrDefault(f => f.FieldNumber.Equals(fieldNumberToUpdate)).FieldValue);

                if (action.Equals("A"))
                {
                    if (CurrentValue < pointLimit)
                        CurrentValue += changeValue;
                }
                else if (action.Equals("D"))
                {
                    if (CurrentValue > 0)
                        CurrentValue -= changeValue;
                }
                else
                {
                    CurrentValue = 0;
                }
                MDMProblemPointsOptionObject(new FieldObject
                {
                    FieldNumber = fieldNumberToUpdate,
                    FieldValue = CurrentValue.ToString()
                });
            }
            catch (Exception ex)
            {

            }
        }
        private void MDMProblemPointsOptionObject(FieldObject FieldToUpdate)
        {
            this.ReturnOptionObject.Forms.Add(new FormObject
            {
                FormId = this.ReturnFormObject.FormId,
                CurrentRow = new RowObject
                {
                    ParentRowId = this.ReturnCurrentRowObject.ParentRowId,
                    RowId = this.ReturnCurrentRowObject.RowId,
                    RowAction = this.ReturnCurrentRowObject.RowAction,
                    Fields = new List<FieldObject>
                    {
                        FieldToUpdate
                    }
                }
            });
        }


    }
}
