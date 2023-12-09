using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("Cutscene")]
public class Cutscene_Serializer
{
    [XmlArray("AudioData"),XmlArrayItem("Audio")]
    public Cutscene_Audio[] AudioList;

    [XmlArray("DialogueData"), XmlArrayItem("Dialogue")]
    public Cutscene_Dialogue[] DialogueList;

    // Will load all relevant data
    public static Cutscene_Serializer Load(string path)
    {
        var serializer = new XmlSerializer(typeof(Cutscene_Serializer));

		TextAsset xmlText = Resources.Load<TextAsset>(path);
		StringReader strReader = new StringReader(xmlText.ToString());

		Cutscene_Serializer CS_Serializer  = serializer.Deserialize(strReader) as Cutscene_Serializer;
		strReader.Close();
		return CS_Serializer;
    }

    public int TotalSizeAudio()
    {
        return AudioList.Length;
    }

    public int TotalSizeDialogue()
    {
        return DialogueList.Length;
    }

    public void DebugPrintAllAudio()
    {
        Debug.Log("Start Audio Debugging");
        Debug.Log("Current audio size is " + TotalSizeAudio());

        foreach(Cutscene_Audio i in AudioList)
            Debug.Log("Audio Filename: " + i.Audio_File_Name + " Time: " + i.Time + " Loop: " + i.Loop + " Played: " + i.Played + " Volume: " + i.Volume + " FadeTime: " + i.FadeTime
            );

        Debug.Log("End Audio Debugging");
    }

    public void DebugPrintAllDialogue()
    {
        Debug.Log("Start Dialogue Debugging");
        Debug.Log("Current dialogue size is " + TotalSizeDialogue());

        foreach (Cutscene_Dialogue i in DialogueList)
            Debug.Log("Sequence: " + i.SequenceOrder + " Time: " + i.TimeInSequence + " Text: " + i.DialogueText
            );

        Debug.Log("End Dialogue Debugging");
    }
}
