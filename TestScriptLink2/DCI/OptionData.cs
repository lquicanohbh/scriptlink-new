using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TestScriptLink2.DCI
{
    public class OptionData
    {
        [XmlElement(ElementName="PATID")]
        public string ClientId { get; set; }
        [XmlElement(ElementName="EPISODE_NUMBER")]
        public string EpisodeNumber { get; set; }
        [XmlElement(ElementName="SYSTEM.Client_Contact_Information_1")]
        public Record Record { get; set; }
    }
}
