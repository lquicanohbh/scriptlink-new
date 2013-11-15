using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using TestScriptLink2.Entities;

namespace TestScriptLink2.DCI
{
    [XmlRoot(ElementName = "option", Namespace = "")]
    public class Option
    {
        [XmlElement(ElementName = "optionidentifier")]
        public string OptionIdentifier { get; set; }
        [XmlElement(ElementName = "optiondata")]
        public List<OptionData> OptionData { get; set; }
        public Option(string OptionIdentifier)
        {
            this.OptionIdentifier = OptionIdentifier;
            this.OptionData = new List<OptionData>();
        }

        public void Initialize(ReleaseOfInformation ROI)
        {
            //this.OptionData.Add(new OptionData()
            //{
            //    ClientId = ROI.ClientId,
            //    Record = new Record()
            //    {
            //        ContactName = new ContactName()
            //        {
            //            First = ROI
            //        }
            //    }
            //});
        }
    }
}