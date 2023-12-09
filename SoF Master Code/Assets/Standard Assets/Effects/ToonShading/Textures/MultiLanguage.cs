using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class MultiLanguage {
    public static readonly string CONFIG_FILENAME = "localization_language_list";
    delegate void LoadConfigProcessor(string data, string languageNodeName);

    #region Singleton
    private static MultiLanguage instance = null;
    private MultiLanguage() { }
    public static MultiLanguage getInstance()
    {
        if (instance == null) {
            instance = new MultiLanguage();
            instance.init();
        }

        return instance;
    }
    #endregion

    public delegate void OnLanguageChangedCallback(string previous, string current);
    public event OnLanguageChangedCallback OnLanguageChanged;

    private Dictionary<string,LanguageNode> languageNodeList;
    private LanguageNode currentLanguageNode;

    bool loadByAssetbundle = false;

    private void init()
    {
        //Load xml file containing information of what we have for language files and their related file locations
        String fileContent = Cleanbox.LoadTextFile("Language/language_list");
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(fileContent);

        //make sure we successfully load the XML file
        if (xml.FirstChild != null)
        {
            //new file location list
            languageNodeList = new Dictionary<string, LanguageNode>();

            //the root content is the last child. <Document>
            XmlNode rootNode = xml.LastChild;

            //Go through the list
            for (int i = 0; i < rootNode.ChildNodes.Count; ++i)
            {
                //Each node here contains an item
                XmlNode itemNode = rootNode.ChildNodes[i];

                //Create langauge to store this language content
                currentLanguageNode = new LanguageNode();
                currentLanguageNode.name = itemNode.LocalName;

                //Load this language
                loadLanguage(itemNode.Attributes["text_location"].Value, itemNode.Attributes["style_location"].Value);

                //Load images content in this language
                loadImage(itemNode.Attributes["image_location"].Value);

                currentLanguageNode.cinematicScriptLocation = itemNode.Attributes["cinematic_script_location"].Value;

                //Now add the language to language list
                languageNodeList.Add(itemNode.LocalName, currentLanguageNode);
            }
        }
        else
        {
            Debug.LogError("The specified language does not exist");
        }

        //Default always english
        //other wise load the saved language
        setLanguage(PlayerPrefs.GetString("langauge", Constants.defaultLanguage));
    }

    public static IAsyncRequest Initialize()
    {
        SimpleEventListener el = new SimpleEventListener();
        GenericAsyncState state = new GenericAsyncState(el);
        GenericCoroutineRequest req = new GenericCoroutineRequest(state, InitSystemCoroutine);

        Action<bool, string> stopCall = (success, msg) => { req.Stop(); };
        el.Register("OnFinish", stopCall);

        return req;
    }
    /// <summary>
    /// Tom: load language manifest using json and AssetBundle
    /// </summary>
    static IEnumerator InitSystemCoroutine(IAsyncRequest req)
    {
        if (instance != null) yield break;
        instance = new MultiLanguage();
        GenericAsyncState reqState = (GenericAsyncState)req.CurrentState;
        Action<string> Print = (msg) => {
            if (req.EventListener.Contains("OnPrintMessage"))
                req.EventListener.Call<Action<string>>("OnPrintMessage")(msg);
        };

        bool isError = false;
        int registerCount = 0;
        int loadingCoroutineCounter = 0;
        Action<string> OnProcessorComplete = (msg) => {
            if (loadingCoroutineCounter < registerCount)
                loadingCoroutineCounter++;
            float f = loadingCoroutineCounter / (float)registerCount;
            reqState.SetProgress(0.1f + 0.9f * f);
            Print (msg);
        };

        reqState.SetProgress(0);
        Print ("Localization Start");

        SimpleJSON.JSONNode jsonObj = null;
        // default should always have english
        // this loads a manifest for supported list of languages;
        // Future implementation: may convert/change to support getting manifest from Database instead, according to languages subscribed by user
        yield return JULESTech.Resources.AssetBundleManager.LoadAssetTextAsset(Constants.LOCALIZATION_BUNDLE_CONFIG, CONFIG_FILENAME, (languageManifest) => {
            string data = languageManifest.text;
            jsonObj = SimpleJSON.JSON.Parse(data);
        });

        if (jsonObj != null) {
            reqState.SetProgress(0.1f);
            instance.loadByAssetbundle = true;

            //new file location list
            instance.languageNodeList = new Dictionary<string, LanguageNode>();

            SimpleJSON.JSONArray languageList = jsonObj["languageList"].AsArray;
            int noOfLanguageAvailable = languageList.Count;
            loadingCoroutineCounter = 0;
            registerCount = noOfLanguageAvailable * 3;

            for (int i = 0; i < noOfLanguageAvailable; ++i) {
                instance.currentLanguageNode = new LanguageNode();
                instance.currentLanguageNode.name = languageList[i]["label"];
                instance.languageNodeList.Add(instance.currentLanguageNode.name, instance.currentLanguageNode);
            }

            // load internals
            for (int i = 0; i < noOfLanguageAvailable; ++i) {

                yield return instance.LoadConfigFile(
                    languageList[i]["label"],
                    languageList[i]["text_location"],
                    (string strData, string langNodeName) => {
                        instance.loadTextFromStringData(strData, langNodeName);
                        OnProcessorComplete(languageList[i]["label"]+" text loaded.");
                    }
                );
                
                yield return instance.LoadConfigFile(
                    languageList[i]["label"],
                    languageList[i]["style_location"],
                    (string strData, string langNodeName) => {
                        instance.loadStyleFromStringData(strData, langNodeName);
                        OnProcessorComplete(languageList[i]["label"] + " styles loaded.");
                    }
                );

                yield return instance.LoadConfigFile(
                    languageList[i]["label"],
                    languageList[i]["image_location"],
                    (string strData, string langNodeName) => {
                        instance.loadImageFromStringData(strData, langNodeName);
                        OnProcessorComplete(languageList[i]["label"] + " img loaded.");
                    }
                );

                Print ("Loaded: " + languageList[i]["label"]);
                instance.currentLanguageNode.cinematicScriptLocation = languageList[i]["cinematic_script_location"];
            }
        } else {
            // error, invalid json config file
            // TOM: there are more exceptions that were not caught; 
            //    eg error in loading text_, styles_ and images_ config files
            isError = true;
        }

        if (isError)
            reqState.OnEnd(false, "Failed to load localization data");
        else {
            instance.setLanguage(PlayerPrefs.GetString("langauge", Constants.defaultLanguage));
            reqState.OnEnd(true, "Localization data loaded successfully.");
        }
    }

    public void setLanguage(String language)
    {
        //Safety check
        //Incase we are loading an language that we don't have
        if (!languageNodeList.ContainsKey(language))
        {
            Debug.LogError("Does not contain this language: " + language);
            return;
            //Set it back to English
            //Debug.LogError("Does not contain this language: " + language + " Use english instead");
            //language = "English";
        }

        string old = currentLanguageNode.name;

        //Save the current language node
        currentLanguageNode = (LanguageNode)languageNodeList[language];

        //Save the langauge settings
        PlayerPrefs.SetString("language", language);
        PlayerPrefs.Save();

        if (OnLanguageChanged != null) {
            OnLanguageChanged(old, language);
        }
    }

    public string getCurrentLanauge()
    {
        return PlayerPrefs.GetString("language");
    }

    public string[] GetAvailableLanguages()
    {
        string[] langList = new string[languageNodeList.Count];
        int i = 0;
        foreach(var node in languageNodeList) {
            langList[i++] = node.Value.name;
        }
        return langList;
    }

    private void loadLanguage(String textpath, String stylepath)
    {
        loadText(textpath);
        loadStyle(stylepath);
    }

    private Coroutine LoadConfigFile(string nodeName, string path, LoadConfigProcessor processor)
    {
        return JULESTech.Resources.AssetBundleManager.LoadAssetTextAsset (
            string.Format(Constants.LOCALIZATION_BUNDLE_TEMPLATE, 
            nodeName.ToLower()), 
            path, 
            (loadedTextAsset) => {
                processor(loadedTextAsset.text, nodeName);
            }
        );
    }
    //Load text content
    private void loadText(String path)
    {
        loadTextFromStringData(Cleanbox.LoadTextFile(path), currentLanguageNode.name);
    }
    private void loadTextFromStringData(string data, string targetNodeName)
    {
        if (languageNodeList.ContainsKey(targetNodeName))
            currentLanguageNode = languageNodeList[targetNodeName];

        XmlDocument xml = new XmlDocument();
        xml.LoadXml(data);

        if (xml.FirstChild != null) {
            currentLanguageNode.textList = new Hashtable();

            //the root content is the last child. <Document>
            XmlNode rootNode = xml.LastChild;

            for (int i = 0; i < rootNode.ChildNodes.Count; ++i) {
                XmlNode xmlItem = rootNode.ChildNodes[i];
                //skip comments
                if (xmlItem.LocalName.CompareTo("#comment") == 0)
                    continue;

                //  Debug.Log("Multilangauge Loading Name: " + xmlItem.Attributes["name"].Value.ToString());

                currentLanguageNode.textList.Add(xmlItem.Attributes["name"].Value, xmlItem.InnerText);
            }
        } else {
            Debug.LogError("MultiLanguage load text.The specified language does not exist: " + currentLanguageNode.name);
        }
    }

    //Load style content
    private void loadStyle(String path)
    {
        loadStyleFromStringData(Cleanbox.LoadTextFile(path), currentLanguageNode.name);
    }
    private void loadStyleFromStringData(string data, string targetNodeName)
    {
        if (languageNodeList.ContainsKey(targetNodeName))
            currentLanguageNode = languageNodeList[targetNodeName];

        XmlDocument xml = new XmlDocument();
        xml.LoadXml(data);

        //make sure we successfully load the XML file
        if (xml.FirstChild != null) {
            //new list
            currentLanguageNode.styleList = new Hashtable();

            //the root content is the last child. <Document>
            XmlNode rootNode = xml.LastChild;

            //Go through the list
            for (int i = 0; i < rootNode.ChildNodes.Count; ++i) {
                //Each node here contains an item
                XmlNode itemNode = rootNode.ChildNodes[i];

                //skip comments
                if (itemNode.LocalName.CompareTo("#comment") == 0)
                    continue;

                //Item content is what we wanted for our style
                TextStyle newStyle;

                //First is always default
                if (i == 0)
                    newStyle = new TextStyle();
                else
                    //Alawys start off with default
                    //So that when something is not defined, we assume it is using default values
                    newStyle = new TextStyle(getStyle(currentLanguageNode, "Default"));

                //Now go though each details stored for this item
                for (int k = 0; k < itemNode.ChildNodes.Count; ++k) {
                    XmlNode childNode = itemNode.ChildNodes[k];
                    String localName = childNode.LocalName;

                    if (localName.CompareTo("font") == 0) {
                        //newStyle.Path = "Fonts/" + childNode.InnerText;
                        newStyle.Path = childNode.InnerText;
                    } else if (localName.CompareTo("fontstyle") == 0) {
                        //still trying to figure out what to d with this
                    } else if (localName.CompareTo("fontsize") == 0) {
                        newStyle.Size = int.Parse(childNode.InnerText);
                    } else if (localName.CompareTo("outline") == 0) {
                        newStyle.hasOutline = true;
                        newStyle.outlineX = float.Parse(childNode.Attributes["x"].Value);
                        newStyle.outlineY = float.Parse(childNode.Attributes["y"].Value);


                        float r = float.Parse(childNode.Attributes["r"].Value) / 255;
                        float g = float.Parse(childNode.Attributes["g"].Value) / 255;
                        float b = float.Parse(childNode.Attributes["b"].Value) / 255;
                        float a = float.Parse(childNode.Attributes["a"].Value) / 255;

                        newStyle.outlineColor = new Color(r, g, b, a);
                    } else if (localName.CompareTo("fontcolor") == 0) {
                        //Create color using the values stored.
                        float r = float.Parse(childNode.Attributes["r"].Value) / 255;
                        float g = float.Parse(childNode.Attributes["g"].Value) / 255;
                        float b = float.Parse(childNode.Attributes["b"].Value) / 255;
                        float a = float.Parse(childNode.Attributes["a"].Value) / 255;

                        newStyle.fontColor = new Color(r, g, b, a);
                    } else if (localName.CompareTo("transform_width") == 0) {
                        newStyle.transform_width = int.Parse(childNode.InnerText);
                    } else if (localName.CompareTo("transform_height") == 0) {
                        newStyle.transform_height = int.Parse(childNode.InnerText);
                    }
                }

                //Now add the new style information into our style list
                //Using item name as the key.
                currentLanguageNode.styleList.Add(itemNode.Attributes["name"].Value, newStyle);
            }
        } else {
            Debug.LogError("MultiLanguage Load Style. The specified language does not exist: " + currentLanguageNode.name);
        }
    }

    /// <summary>
    /// load image path
    /// </summary>
    /// <param name="path"></param>
    private void loadImage(String path)
    {
        loadImageFromStringData(Cleanbox.LoadTextFile(path), currentLanguageNode.name);
    }
    private void loadImageFromStringData(string data, string targetNodeName)
    {
        if (languageNodeList.ContainsKey(targetNodeName))
            currentLanguageNode = languageNodeList[targetNodeName];

        XmlDocument xml = new XmlDocument();
        xml.LoadXml(data);

        if (xml.FirstChild != null) {
            currentLanguageNode.imageList = new Hashtable();

            //the root content is the last child. <Document>
            XmlNode rootNode = xml.LastChild;

            for (int i = 0; i < rootNode.ChildNodes.Count; ++i) {
                XmlNode xmlItem = rootNode.ChildNodes[i];
                //skip comments
                if (xmlItem.LocalName.CompareTo("#comment") == 0)
                    continue;

                currentLanguageNode.imageList.Add(xmlItem.Attributes["name"].Value, xmlItem.Attributes["location"].Value);
            }
        } else {
            Debug.LogError("MultiLanguage load Image. The specified language does not exist: " + currentLanguageNode.name);
        }
    }

    public String getString(String name)
    {
		if (!currentLanguageNode.textList.ContainsKey(name)) {
			Debug.LogError("The specified string does not exist: " + name);
			return "";
		}
        
        //Relace \\n because from XML string format all \n is been changed to \\n hence will no longer have new space
        return currentLanguageNode.textList[name].ToString().Replace("\\n", "\n");
	}

    #region Tom: made to display languagelist
    /// <summary>
    /// 
    /// </summary>
    /// <param name="languageName"></param>
    /// <param name="label"></param>
    /// <param name="targetLocation"></param>
    public void ApplyText(string languageName, string label, Text targetLocation)
    {
        targetLocation.text = GetString(languageName, label);
        LanguageNode languageNode = (LanguageNode)languageNodeList[languageName];
        TextStyle style = getStyle(languageNode, label);
        ApplyStyle(targetLocation, style);
    }
    string GetString(string languageName, string label)
    {
        LanguageNode node = (LanguageNode)languageNodeList[languageName];
        return node.textList[label].ToString().Replace("\\n", "\n");
    }
    #endregion

    public int getFontSize(String name)
    {
        //Return the font size element inside the style list
        return getStyle(currentLanguageNode, name).Size;
    }
    
    private TextStyle getStyle(LanguageNode targetNode, String name)
    {
        if (!targetNode.styleList.ContainsKey(name))
        {
            //remove this irritating  
            //Debug.LogError("MultiLanguage getFont Size. The specified style does not exist: " + name + " using default");

            //if there is no style to this name
            //return default instead
            return (TextStyle)targetNode.styleList["Default"];
        }

        return (TextStyle)targetNode.styleList[name];
    }

    public String getImageLocation(String name)
    {
        if (!currentLanguageNode.imageList.ContainsKey(name))
        {
            Debug.LogError("MultiLanguage getImageLocation. The specified image does not exist: " + name);

            return "";
        }

        return currentLanguageNode.imageList[name].ToString();
    }

    public void apply(GameObject go, String name)
    {
        //Saftly check for the Game Object
        if (go == null)
        {
            Debug.LogError("Game Object is null: " + name);
            return;
        }

        //First get the text object from the game object
        Text _textObject = go.GetComponent<Text>();

        //Apply the text
        _textObject.text = getString(name);

        //Get the style out
        TextStyle _style = getStyle(currentLanguageNode, name);
        ApplyStyle(_textObject, _style);

    }
    void ApplyStyle(Text _textObject, TextStyle _style)
    {
        //Transform width and height if the values are defined
        //NOTE, I am assuming we will never set width or height to 0
        if (_style.transform_width != 0)
            _textObject.GetComponent<RectTransform>().sizeDelta = new Vector2(_style.transform_width, _textObject.GetComponent<RectTransform>().sizeDelta.y);

        if (_style.transform_height != 0)
            _textObject.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(_textObject.GetComponent<RectTransform>().sizeDelta.x, _style.transform_height);

        //Set the font
        //_textObject.font = (Font)Resources.Load(_style.Path);
        string fontname = _style.Path;
        JULESTech.Resources.AssetBundleManager.LoadAsset(Constants.FONTS_SHARED_BUNDLE, _style.Path, (loadedAsset) => {
            if (loadedAsset == null) {
                Debug.LogErrorFormat("Font not loaded: ", fontname);

                AssetBundle bundle = JULESTech.Resources.AssetBundleManager.GetBundle(Constants.FONTS_SHARED_BUNDLE);
                System.Text.StringBuilder str = new System.Text.StringBuilder();
                str.AppendLine("Fonts assetbundle");
                string[] assetNames = bundle.GetAllAssetNames();
                foreach (string name in assetNames) {
                    str.AppendLine(name);
                }
                Debug.Log(str.ToString());
                return;
            } else
                _textObject.font = loadedAsset as Font;
        });

        //Set the font color
        if (_style.fontColor != null)
            _textObject.color = _style.fontColor;

        //Set the font size
        if (_style.Size != 0)
            _textObject.fontSize = _style.Size;

        //Check if we should have an outline
        if (_style.hasOutline) {
            //Find the oultine object
            UnityEngine.UI.Outline _outline = _textObject.GetComponent<UnityEngine.UI.Outline>();

            if (_outline != null) {
                //Set the x and y of the outline
                _outline.effectDistance = new Vector2(_style.outlineX, _style.outlineY);

                //Set the color of the outline
                _outline.effectColor = _style.outlineColor;
            } else {
                //Debug.LogError( name + " declared to have outline, but no outline object is found");
            }
        }
    }

    public void applyImage(Image img, String name)
    {
        //safe check if img is invalid
        if(img == null)
        {
            Debug.LogError("GameObject is invalid, name: " + name);

            //exit
            return;
        }

        if (loadByAssetbundle) {
            //* // loading from assetbundle
            string imageFilename = getImageLocation(name);
            StringHelper.TrimFilename(ref imageFilename);
            JULESTech.Resources.AssetBundleManager.LoadAsset(string.Format(Constants.LOCALIZATION_BUNDLE_TEMPLATE, currentLanguageNode.name.ToLower()), imageFilename, (loadedAsset) => {
                Sprite newSprite = JULESTech.Resources.AssetBundleManager.GetSprite(loadedAsset);
                img.sprite = newSprite;
            });
            //*/
        } else {
            Sprite newImages = Resources.Load<Sprite>(getImageLocation(name));
            //reload sprite
            img.sprite = newImages;
        }
    }

    public string getCinematicScriptsLocation()
    {
        return currentLanguageNode.cinematicScriptLocation;
    }

    struct TextStyle
    {
        public String Path;
        public String Style;
        public int Size;
        public Boolean hasOutline;
        public Color outlineColor;
        public float outlineX;
        public float outlineY;
        public Color fontColor;
        public int transform_width;
        public int transform_height;

        public TextStyle(TextStyle defaultStyle)
        {
            Path = defaultStyle.Path;

            if (defaultStyle.Style != null)
                Style = defaultStyle.Style;
            else
                Style = null;

            Size = defaultStyle.Size;

            hasOutline = defaultStyle.hasOutline;
            outlineColor = defaultStyle.outlineColor;
            outlineX = defaultStyle.outlineX;
            outlineY = defaultStyle.outlineY;

            transform_width = defaultStyle.transform_width;
            transform_height = defaultStyle.transform_height;

            fontColor = defaultStyle.fontColor;
        }
    }

    class LanguageNode
    {
        public string name;
        public Hashtable textList;
        public Hashtable styleList;
        public Hashtable imageList;
        public string cinematicScriptLocation;
    }
}
