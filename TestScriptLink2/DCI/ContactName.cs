using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TestScriptLink2.DCI
{
    public class ContactName
    {
        [XmlElement(ElementName = "first")]
        public string First { get; set; }
        [XmlElement(ElementName = "last")]
        public string Last { get; set; }
    }
}
