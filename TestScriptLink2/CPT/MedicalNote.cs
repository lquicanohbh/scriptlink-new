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
        public string MGAFFieldNumber { get; set; }
        public string CurrentProblems { get; set; }
        public List<string> NoteProblems { get; set; }
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
            this.ChiefComplaintFieldNumber = "151.97";
            this.MedicalConcernsFieldNumber = "152.8";
            this.TreatmentPlanFieldNumber = "152.35";
            this.PlanFieldNumber = "151.99";
            this.ParticipantsFieldNumber = "152.71";
            this.NoteSummaryFieldNumber = "152.88";
            this.DateFieldNumber = "151.95";    
            this.VitalSignsFieldNumber = "153.05";
            this.ProgramFieldNumber = "152.33";
            this.ServiceCodeFieldNumber = "152.3";
            this.LocationFieldNumber = "152.51";
            this.ICFieldNumber = "152.37";
            this.MGAFFieldNumber = "153.23";
            this.ReturnFormObject = new FormObject("188");
            this.ReturnCurrentRowObject = new RowObject("0", "188||1", "EDIT");
            this.ParticipantsWording = "Present in this session";
            this.Problem1FieldNumber = "153.08";
            this.CurrentProblems = "153.51";
            this.NoteProblems = new List<string>() { "153.08", "153.09", "153.1", "153.11" };
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
        public void CopyProblem()
        {
            var currentProblems = this.OriginalOptionObject.Forms.First().CurrentRow.Fields.FirstOrDefault(f => f.FieldNumber.Equals(this.CurrentProblems)).FieldValue.Split('&').ToList();
            var noteProblems = new List<FieldObject>();
            foreach (var field in this.OriginalOptionObject.Forms.First().CurrentRow.Fields)
            {
                if (this.NoteProblems.Contains(field.FieldNumber))
                    noteProblems.Add(field);
            }
            foreach (var fieldValue in currentProblems)
            {
                if (!noteProblems.Select(f => f.FieldValue).Contains(fieldValue))
                {
                    foreach (var tempField in noteProblems)
                    {
                        if (String.IsNullOrEmpty(tempField.FieldValue))
                        {
                            UpdateReturnOptionObject(tempField.FieldNumber, fieldValue);
                            tempField.FieldValue = "X";
                            break;
                        }
                    }
                }
            }
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
        public void DefaultMGAF()
        {
            var currentMGAF = this.OriginalOptionObject.Forms.First().CurrentRow.Fields.FirstOrDefault(f => f.FieldNumber.Equals(this.MGAFFieldNumber));
            if (currentMGAF != null & String.IsNullOrEmpty(currentMGAF.FieldValue))
            {
                var note = ProgressNoteRepository.GetLastMGAF(
                    this.OriginalOptionObject.EntityID,
                    this.OriginalOptionObject.EpisodeNumber);
                UpdateReturnOptionObject(this.MGAFFieldNumber, note.MGAF);
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
            var mspsych = new StringBuilder();
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
                    else if (GetSectionList("Mental Status Psychotic Thoughts").Contains(dictionary.FieldNumber))
                    {
                        if (dictionary.Value.Equals("Denies"))
                            mspsych.AppendFormat("{0} {1}. ", dictionary.Value, dictionary.FieldDescription);
                        else
                            mspsych.AppendFormat("Admits {0}, {1}. ", dictionary.FieldDescription, GetPsychComments(dictionary.FieldNumber));
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
            if (mentalStatus.Any() || mspsych.Length != 0)
            {
                ms.AppendFormat("Mental Status Examination:\n");
                foreach (var temp in mentalStatus)
                {
                    ms.AppendFormat("{0}. ", temp.Value.ToString());
                }
                if (mspsych.Length != 0)
                    ms.Append(mspsych.ToString());
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
            if (dictionary.FieldNumber == "152.86")//severity
            {
                return String.Format("{0} {1}. ", dictionary.Value, dictionary.FieldDescription);
            }
            else if (dictionary.FieldNumber == "152.87")//timing
            {
                return String.Format("{0} {1}. ", dictionary.Value, dictionary.FieldDescription);
            }
            else if (dictionary.FieldNumber == "152.84")//context
            {
                return GetHPIComments("152.84").Replace("\n", " ");
            }
            else if (dictionary.FieldNumber == "152.85")//modifying factors
            {
                return GetHPIComments("152.85").Replace("\n", " ");
            }
            else if (dictionary.FieldNumber == "152.4")//associated signs and symptoms
            {
                return GetHPIComments("152.83").Replace("\n", " ");
            }
            else if (dictionary.FieldNumber == "152.43")//duration
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
            if (fieldNumber == "152.51")//location
            {
                return GetFieldValue("152.52");
            }
            else if (fieldNumber == "152.84")//context
            {
                return GetFieldValue("151.49");
            }
            else if (fieldNumber == "152.85")//modifying factors
            {
                return GetFieldValue("152.54");
            }
            else if (fieldNumber == "152.83")//associated signs and symptoms
            {
                return GetFieldValue("152.55");
            }
            else if (fieldNumber == "151.43")//duration
            {
                return GetFieldValue("151.42");
            }
            else
            {
                return String.Empty;
            }
        }
        private string GetPsychComments(string fieldNumber)
        {
            if (fieldNumber == "153.13")
            {
                return GetFieldValue("153.18").Replace("\n", "");
            }
            else if (fieldNumber == "153.14")
            {
                return GetFieldValue("153.19").Replace("\n", "");
            }
            else if (fieldNumber == "153.15")
            {
                return GetFieldValue("153.2").Replace("\n", "");
            }
            else if (fieldNumber == "153.16")
            {
                return GetFieldValue("153.21").Replace("\n", "");
            }
            else if (fieldNumber == "153.17")
            {
                return GetFieldValue("153.22").Replace("\n", "");
            }
            else
            {
                return String.Empty;
            }
        }
        private string GetProblemTypeForProblem(string fieldNumber)
        {
            if (fieldNumber == "153.01")
                return GetFieldValue("153.08").Replace("\n", String.Empty);
            else if (fieldNumber == "153.02")
                return GetFieldValue("153.09").Replace("\n", String.Empty);
            else if (fieldNumber == "153.03")
                return GetFieldValue("153.1").Replace("\n", String.Empty);
            else if (fieldNumber == "153.04")
                return GetFieldValue("153.11").Replace("\n", String.Empty);
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
                    var temp1 = new List<string> { "152.71" };
                    temp1.ForEach(x => list.Add(x));
                    break;
                case "Presenting Problems":
                    var temp2 = new List<string> { "153.01", "153.02", "153.03", "153.04" };
                    temp2.ForEach(x => list.Add(x));
                    break;
                case "History of Present Illness":
                    var temp3 = new List<string> { "152.86", "152.87", "152.51", "152.84", "152.85", "152.83", "152.43" };
                    temp3.ForEach(x => list.Add(x));
                    break;
                case "Review of Systems":
                    var temp4 = new List<string> { "152.72", "152.66", "152.68", "152.67", "152.69", "152.7", "152.73", "152.74", "152.75", "152.76", "152.77", "152.78", "152.79" };
                    temp4.ForEach(x => list.Add(x));
                    break;
                case "Mental Status Examination":
                    var temp5 = new List<string> { "153.06", "152", "152.02", "152.03", "152.12", "152.06", "152.07", "152.08", "152.09", "152.1", "152.11" };
                    temp5.ForEach(x => list.Add(x));
                    break;
                case "Vital Signs":
                    var temp6 = new List<string> { "153.07" };
                    temp6.ForEach(x => list.Add(x));
                    break;
                case "Interactive Complexity":
                    var temp7 = new List<string> { "152.25", "152.26", "152.29", "152.28", "152.27" };
                    temp7.ForEach(x => list.Add(x));
                    break;
                case "Data Points":
                    var temp8 = new List<string> { "152.91", "152.92", "152.93", "152.94", "152.97", "152.95", "152.96" };
                    temp8.ForEach(x => list.Add(x));
                    break;
                case "Mental Status Psychotic Thoughts":
                    var temp9 = new List<string> { "153.13", "153.14", "153.15", "153.16", "153.17" };
                    temp9.ForEach(x => list.Add(x));
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
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.71" });//participants
            #endregion

            #region Presenting Problems
            dictionaryList.Add(new FormDictionary { FieldNumber = "153.01" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "153.02" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "153.03" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "153.04" });
            #endregion

            #region History of Present Illness
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.86" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.87" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.51" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.84" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.85" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.83" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.43" });
            #endregion

            #region Review of Systems
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.72" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.66" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.68" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.67" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.69" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.7" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.73" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.74" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.75" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.76" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.77" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.78" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.79" });
            #endregion

            #region Mental Status Examination
            dictionaryList.Add(new FormDictionary { FieldNumber = "153.06" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.02" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.03" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.12" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.06" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.07" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.08" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.09" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.1" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.11" });
            #endregion

            #region Mental Status Examination Psychotic Thoughts
            dictionaryList.Add(new FormDictionary { FieldNumber = "153.13" });//auditory
            dictionaryList.Add(new FormDictionary { FieldNumber = "153.14" });//visual
            dictionaryList.Add(new FormDictionary { FieldNumber = "153.15" });//suicidal
            dictionaryList.Add(new FormDictionary { FieldNumber = "153.16" });//homicidal
            dictionaryList.Add(new FormDictionary { FieldNumber = "153.17" });//delusions
            #endregion

            #region Vital Signs
            dictionaryList.Add(new FormDictionary { FieldNumber = "153.07" });
            #endregion

            #region Interactive Complexity
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.25" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.26" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.27" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.28" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.29" });
            #endregion

            #region Data Points
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.91" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.92" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.93" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.94" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.95" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.96" });
            dictionaryList.Add(new FormDictionary { FieldNumber = "152.97" });
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
            var HPIFields = new List<string> { "152.86", "152.87", "152.51", "152.84", "152.85", "152.83", "152.43" };
            var PFSHFields = new List<string> { "152.4", "152.89", "152.9" };
            var ROSFields = new List<string> {"152.72","152.66","152.68","152.67","152.69",
                "152.7","152.73","152.74","152.75","152.76",
                "152.77","152.78","152.79",};
            #endregion

            #region Examination
            var PsyExamFields = new List<string> { "153.06", "152", "152.02", "152.03", "152.12", "152.06", "152.07", "152.08", "152.09", "152.1", "152.11" };
            var MuscoloskeletalFields = new List<string> { "152.74" };
            var ConstitutionalFields = new List<string> { "153.07", "153.06" };
            #endregion

            #region Examination Psychotic
            var ExamPsychFields = new List<string> { "153.13", "153.14", "153.15", "153.16", "153.17" };
            #endregion

            #region MDM
            var ProblemPointsFields = new List<string> { "153.01", "153.02", "153.03", "153.04" };
            var DataPointsFields = new List<string> { "152.91", "152.92", "152.93", "152.94", "152.97", "152.95", "152.96" };
            //var TableOfRiskFields = new List<string> { "" };
            #endregion

            #region Interactive Complexity
            var ICFields = new List<string> { "152.25", "152.26", "152.29", "152.28", "152.27" };
            #endregion


            var HPIValue = CountFields(list, HPIFields);
            var PFSHValue = CountFields(fields, PFSHFields);
            var ROSValue = CountFields(list, ROSFields);

            var PsyExamValue = CountFields(list, PsyExamFields);
            var ExamPsychValue = CountFields(list, ExamPsychFields);
            var MusculoValue = CountFields(list, MuscoloskeletalFields);
            var ConstitutionalValue = CountFields(list, ConstitutionalFields);

            var ProblemPointsValue = SumFieldValue(fields, ProblemPointsFields);
            var DataPointsValue = SumDictionaryCode(list, DataPointsFields);

            var ICValue = CountFields(fields, ICFields);

            HistoryType = CalculateHistoryType(HPIValue, PFSHValue, ROSValue);
            ExaminationType = ExamPsychValue > 0 ? CalculateExaminationType((PsyExamValue + 1), MusculoValue, ConstitutionalValue)
                : CalculateExaminationType(PsyExamValue, MusculoValue, ConstitutionalValue);
            MDMType = CalculateMDMType(ProblemPointsValue, DataPointsValue, 0);

            int min = Helper.Min(HistoryType, ExaminationType, MDMType);
            var pointsList = new List<int> { HistoryType, ExaminationType, MDMType };
            pointsList.Remove(min);
            var max1 = pointsList.ElementAt(0);
            var max2 = pointsList.ElementAt(1);


            string ServiceCode = "1103";//99211

            if (max1 >= 4 && max2 >= 4)
            {
                ServiceCode = "1107";//99215
            }
            else if (max1 >= 3 && max2 >= 3)
            {
                ServiceCode = "1106";//99214
            }
            else if (max1 >= 2 && max2 >= 2)
            {
                ServiceCode = "1105";//99213
            }
            else if (max1 >= 1 && max2 >= 1)
            {
                ServiceCode = "1104";//99212
            }
            else
            {
                ServiceCode = "1103";//99211
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
                if (!dictionary.Value.Equals("Not Reviewed") && !dictionary.Value.Equals("N/A"))
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

