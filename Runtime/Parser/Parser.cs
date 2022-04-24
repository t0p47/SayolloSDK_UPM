using System.Xml;
using System.Xml.Serialization;

namespace Sayollo {
    [XmlRoot(ElementName = "MediaFiles")]
    public class MediaFiles
    {
        [XmlElement(ElementName = "MediaFile")]
        public string MediaFile { get; set; }
    }

    [XmlRoot(ElementName = "Linear")]
    public class Linear
    {
        [XmlElement(ElementName = "Duration")]
        public string Duration { get; set; }
        [XmlElement(ElementName = "MediaFiles")]
        public MediaFiles MediaFiles { get; set; }
        [XmlElement(ElementName = "TrackingEvents")]
        public string TrackingEvents { get; set; }
    }

    [XmlRoot(ElementName = "Creative")]
    public class Creative
    {
        [XmlElement(ElementName = "Linear")]
        public Linear Linear { get; set; }
    }

    [XmlRoot(ElementName = "Creatives")]
    public class Creatives
    {
        [XmlElement(ElementName = "Creative")]
        public Creative Creative { get; set; }
    }

    [XmlRoot(ElementName = "InLine")]
    public class InLine
    {
        [XmlElement(ElementName = "Error")]
        public string Error { get; set; }
        [XmlElement(ElementName = "Creatives")]
        public Creatives Creatives { get; set; }
    }

    [XmlRoot(ElementName = "Ad")]
    public class Ad
    {
        [XmlElement(ElementName = "InLine")]
        public InLine InLine { get; set; }
    }

    [XmlRoot(ElementName = "VAST")]
    public class VAST
    {
        [XmlElement(ElementName = "Ad")]
        public Ad Ad { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
        [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsd { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
    }
}