using System.Xml;
using System.Xml.Serialization;

public class Cutscene_Audio
{
    [XmlAttribute("filename")]
    public string Audio_File_Name;
    public float Time;
    public float Volume;
    public float FadeTime;
    public bool Loop;
    public bool Played = false;
}
