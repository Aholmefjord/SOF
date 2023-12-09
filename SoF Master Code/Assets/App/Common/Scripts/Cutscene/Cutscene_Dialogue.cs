using System.Xml;
using System.Xml.Serialization;


public class Cutscene_Dialogue{
    [XmlAttribute("sequence")]
    public int SequenceOrder;
    public string DialogueText;
    public float TimeInSequence;
}
