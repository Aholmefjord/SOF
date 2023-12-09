using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System.Xml;

public class TextFilter {
    List<string> filterWords = new List<string>();

    public bool AddFilter(string word)
    {
        bool duplicate = filterWords.Contains(word);
        if (duplicate) {
            Debug.LogErrorFormat("The word [{0}] is already in the list.");
            return false;
        }

        filterWords.Add(word);
        return true;
    }
    public bool RemoveWord(string word)
    {
        return filterWords.Remove(word);
    }
    public bool IsFiltered(string wordToCheckWith)
    {
        return Occurances(wordToCheckWith) > 0;
    }
    public int Occurances(string wordToCheckWith)
    {
        int hits = 0;
        foreach (var filter in filterWords) {
            if (wordToCheckWith.Contains(filter)) {
                hits++;
            }
        }
        return hits;
    }
    public string Serialize()
    {
        return TextFilter.Serialize(this);
    }

    public override string ToString()
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        builder.Append("Filtering words: ");
        foreach (var word in filterWords) {
            builder.Append(word);
            builder.Append(",");
        }
        return builder.ToString();
    }

    public static IEnumerator LoadFrom(string url, System.Action<TextFilter> onSuccess, System.Action<string> onFailure)
    {
        UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.Send();

        if (req.isError) {
            if (onFailure != null) onFailure(req.error);
            yield break;
        }

        // handle AWS XML error
        if (IsAWSXMLError(req.downloadHandler.text)) {
            if (onFailure != null) onFailure("error from aws s3 server");
            yield break;
        }
        
        TextFilter obj = Make(req.downloadHandler.text);
        if (onSuccess != null) onSuccess(obj);
    }

    public static TextFilter Make(string jsonstring)
    {
        JSONNode node = JSON.Parse(jsonstring);
        JSONClass root = node.AsObject;
        JSONArray list = root["filterList"].AsArray;

        if (list == null) {
            return null;
        }

        TextFilter obj = new TextFilter();
        int size = list.Count;
        try {
            for (int i = 0; i < size; ++i) {
                obj.AddFilter(list[i]);
            }
        } catch (System.Exception) {
            // do not return anything on error
            return null;
        }
        return obj;
    }
    public static string Serialize(TextFilter filter)
    {
        System.Text.StringBuilder str = new System.Text.StringBuilder();
        str.Append("{");
        str.Append("filterList:[");
        int size = filter.filterWords.Count;
        for (int i = 0; i < size; ++i) {
            str.Append(filter.filterWords[i]);
            if(i != (size-1))
                str.Append(",");
        }
        str.Append("]}");
        return str.ToString();
    }

    public static bool IsAWSXMLError(string data)
    {
        bool result = true;
        XmlDocument doc = new XmlDocument();
        try {
            doc.LoadXml(data);
        } catch (System.Exception) {
            // not xml, return false
            result = false;
            return result;
        }

        XmlNode root = doc.LastChild;
        if(root.LocalName == "Error") {
            /*
            XmlNode itr = doc.LastChild.FirstChild;
            while (itr != null) {
                Debug.Log(itr.Name);
                itr = itr.NextSibling;
            }
            //*/
            XmlNode code   = root.FirstChild;
            XmlNode msg    = code.NextSibling;
            XmlNode key    = msg.NextSibling;
            XmlNode reqID  = key.NextSibling;
            XmlNode hostID = reqID.NextSibling;

            Debug.LogErrorFormat("{0}: {1}",root.LocalName, msg.InnerText);
        }
        return result;
    }
}