using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace TestScriptLink2.DCI
{
    public class Record
    {
        [XmlElement(ElementName = "Contact_Name")]
        public ContactName ContactName { get; set; }
        [XmlElement(ElementName = "Contact_Type")]
        public string ContactType { get; set; }
        [XmlElement(ElementName = "Contact_Comment")]
        public string Comments { get; set; }
    }
}