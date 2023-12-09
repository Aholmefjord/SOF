using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using JULESTech;

public class UDPNetwork
{
    #region Command Class
    private class CMDCommand : JULESTech.ICommand
    {
        public void Execute()
        {
            CommandMaster.getInstance().ExecuteCommand(UDPNetwork.networkCommandValue);
        }
    }

    //Set Lesson Command
    private class SLCCOmmand : JULESTech.ICommand
    {
        public void Execute()
        {
            GameSceneManager.getInstance().SetCurrentCommand(UDPNetwork.networkCommandValue);
        }
    }

    //Set LessonData Map
    private class SLMCommand : JULESTech.ICommand
    {
        public void Execute()
        {
            string commandString = UDPNetwork.networkCommandValue;

            //Clear previous lessonData
            //JULESTech.DataStore.Instance.lessonData.CleanDataMap();

            while (commandString.Length != 0)
            {
                int keyNumber = int.Parse(commandString.Substring(0, 3));
                commandString = commandString.Substring(3);
                string key = commandString.Substring(0, keyNumber);
                commandString = commandString.Substring(keyNumber);

                int valueNumber = int.Parse(commandString.Substring(0, 3));
                commandString = commandString.Substring(3);
                string value = commandString.Substring(0, valueNumber);
                commandString = commandString.Substring(valueNumber);

                Debug.Log("SLM Command: key: " + key + " value: " + value);
                JULESTech.DataStore.Instance.lessonData.SetString(key, value);
            }
        }
    }

    //Reet LessonData Map
    private class RLMCommand : JULESTech.ICommand
    {
        public void Execute()
        {
            string commandString = UDPNetwork.networkCommandValue;

            //Clear previous lessonData
            JULESTech.DataStore.Instance.lessonData.CleanDataMap();
        }
    }

    private class FRZCommand : JULESTech.ICommand
    {
        public void Execute()
        {
            if (UDPNetwork.networkCommandValue.Equals("all") || UDPNetwork.networkCommandValue.Equals(GameState.me.username))
                MainNavigationController.GoToFreeze();
        }
    }

    //Unfreeze command
    private class UFZCommand : JULESTech.ICommand
    {
        public void Execute()
        {
            if (UDPNetwork.networkCommandValue.Equals("all") || UDPNetwork.networkCommandValue.Equals(GameState.me.username))
                MainNavigationController.GotoMainMenu();
        }
    }

    private class HOMCommand : JULESTech.ICommand
    {
       public void Execute()
        {
            MainNavigationController.GoToHome();
        }
    }
    // go campaign mode
    private class CAMCommand : JULESTech.ICommand
    {
        public void Execute()
        {
            MainNavigationController.GoToScene("CampaignLevel");
        }
    }

    //Group command
    private class GRPCommand : JULESTech.ICommand
    {
        //This means we only want certain group to receive and react to this command.
        public void Execute()
        {
            //if not set for some reason
            //ignore so at least we will still receive message
            if (UDPNetwork.getInstance().mGroupName.Equals(""))
                return;

            //How to not receive the rest?
            if (!UDPNetwork.networkCommandValue.ToLower().Equals(UDPNetwork.getInstance().mGroupName.ToLower()))
            {
                //Target is not me
                Debug.Log("UDP Message Target: " + UDPNetwork.networkCommandValue.ToLower() + " my group name: " + UDPNetwork.getInstance().mGroupName);
                UDPNetwork.getInstance().stopExecution = true;
            }
        }
    }

    private class GTSCommand : JULESTech.ICommand
    {
        public void Execute()
        {
            //use the scene name inside the lesson plan data
            //this is to make lesson plan data easier to read, because the actual scene name will be stored inside game command
            //Which is loaded into lesson plan data when init.
            MainNavigationController.GoToScene(JULESTech.DataStore.Instance.gameCommandData.GetString(UDPNetwork.networkCommandValue));
        }
    }
    #endregion
    private static UDPNetwork _instance;
    private static object instanceLock = new object();
    private static bool destroyed = false;
    protected static string networkCommandValue;
    private static UDPGameobjectSlave gameObjectSlave;
    
    private CommandMap myCmdMap = new CommandMap();

    UdpClient recvClient = null;
    Socket senderSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    IPEndPoint sendEndPoint = null;
    IPEndPoint listenEndPoint = null;
    Queue<byte[]> messageQueue = new Queue<byte[]>();
    string msgIdentifier = "JSOF";
    int defaultPort = 11000;

    List<Dictionary<string, string>> dictReference = new List<Dictionary<string, string>>();

    protected bool stopExecution = false;  //To know if we are the correct target audiance.

    public string mGroupName = "";

    private List<Dictionary<string, string>> messageLog = new List<Dictionary<string, string>>();

    // Use this for initialization
    void Init()
    {
        Debug.Log("[UDPNetwork initializing...]");
        Connect(IPAddress.Parse("255.255.255.255"), defaultPort, defaultPort);

        //Set Lesson Command
        myCmdMap.AddCommand("SLC", new SLCCOmmand());
        //Set Lesson Map
        myCmdMap.AddCommand("SLM", new SLMCommand());
        //Reset Lesson Map
        myCmdMap.AddCommand("RLM", new RLMCommand());
        //Commmand
        myCmdMap.AddCommand("CMD", new CMDCommand());
        //go to the freeze screen
        myCmdMap.AddCommand("FRZ", new FRZCommand());
        //Unfreeze Command
        myCmdMap.AddCommand("UFZ", new UFZCommand());
        //go home screen
        myCmdMap.AddCommand("HOM", new HOMCommand());
        //go campagin mode
        myCmdMap.AddCommand("CAM", new CAMCommand());
        //Set target group
        myCmdMap.AddCommand("GRP", new GRPCommand());
        //Go To Scene
        myCmdMap.AddCommand("GTS", new GTSCommand());

        // TOM: creating a monobehaviour slave
        GameObject obj = new GameObject("UDPNetwork.GameobjectSlave");
        obj.hideFlags = HideFlags.NotEditable;
        gameObjectSlave = obj.AddComponent<UDPGameobjectSlave>();
    }

    // Update is called once per frame
    public void Update()
    {
        //if the user is not logged in, we should receive any message
        if (GameState.me == null)
            return;

        if (MessageCount > 0)
        {
            Debug.Log("Received messages " + MessageCount);
            for (int i = 0; i < MessageCount; i++)
            {
                dictReference.Add(UnpackFormattedMessage(GetMessage()));
            }
        }

        #region Clear messageLog
        if (messageLog.Count >= 1)
        {   
            for (int i = 0; i < messageLog.Count; i++)
            {
                string temp = messageLog[i]["MessageLifespan"];
                float lifespan = float.Parse(temp);

                if (lifespan <= 0)
                {
                    //do not delete directly here because it breaks the messageLog Count
                    //add to the temp list instead.
                    messageLog.RemoveAt(i);
                    Debug.Log("Deleted logEntry from messageLog at: " + System.DateTime.Now);
                }
                else
                {
                    lifespan -= Time.deltaTime;
                    messageLog[i].Remove("MessageLifespan");
                    messageLog[i].Add("MessageLifespan", lifespan.ToString());
                }
            }
        }
        #endregion

        //reset stop execution so we are ready for next list of commands
        stopExecution = false;

        if (dictReference.Count > 0)
        {
            for (int i = 0; i < dictReference.Count; i++)
            {
                #region add messages to messageLog
                if (messageLog.Count >= 1)
                {
                    bool equal = false;
                    #region check for duplicate commands
                    try
                    {
                        for (int j = 0; j < messageLog.Count; j++)
                        {
                            string existingTimeStamp, receivedTimeStamp;
                            messageLog[j].TryGetValue("TMS", out existingTimeStamp);
                            dictReference[i].TryGetValue("TMS", out receivedTimeStamp);

                            // Check if timeStamps are equal, skip key & value checking if they are not equal
                            if (existingTimeStamp == receivedTimeStamp)
                            {
                                equal = true;
                                foreach (KeyValuePair<string, string> pair in dictReference[i])
                                {
                                    string value;
                                    if (messageLog[j].TryGetValue(pair.Key, out value))
                                    {
                                        // Exclude messageLog lifespan from checking
                                        if (pair.Key != "MessageLifespan")
                                        {
                                            // Stop comparing values if an unequal pair is found
                                            if (value != pair.Value)
                                            {
                                                equal = false;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // Stop comparing if unable to find a matching key
                                        equal = false;
                                        break;
                                    }
                                }
                                // Stop comparing with messageLog if duplicate message is found
                                if (equal)
                                    break;
                            }
                            else
                                equal = false; // timeStamps are not equal, no need to check further
                        }
                    }
                    catch (Exception e) { Debug.LogError(e); }
                    #endregion

                    // if message is duplicate, stop it from executing
                    if (equal)
                    {
                        stopExecution = true;
                        break;
                    }
                    else // if message is new, add it to messageLog
                    {
                        dictReference[i].Add("MessageLifespan", "2.0");
                        messageLog.Add(dictReference[i]);
                    }
                }
                else
                {
                    dictReference[i].Add("MessageLifespan", "2.0");
                    messageLog.Add(dictReference[i]);
                }
                #endregion

                foreach (KeyValuePair<string, string> entry in dictReference[i])
                {
                    UDPNetwork.networkCommandValue = entry.Value;

                    try
                    {
                        if (stopExecution)
                        {
                            Debug.Log("Stopped execution of entry: " + dictReference[i].ToStringFull());
                            //reset stop execution so we are ready for next list of commands
                            stopExecution = false;

                            //Clean up after running the message
                            dictReference.Clear();
                            return;
                        }
                        myCmdMap.ExecuteCommand(entry.Key);
                        Debug.Log("Message executing key:" + entry.Key + " value:" + entry.Value);
                    }
                    catch (Exception e) { Debug.Log(e.ToString()); }
                }
            }

            //Clean up after running the message
            dictReference.Clear();
        }
    }

    [RuntimeInitializeOnLoadMethod]
    static public void RuntimeInitializeOnLoad()
    {
        getInstance();
    }

    internal static UDPNetwork getInstance()
    {
        lock (instanceLock)
        {
            if (_instance == null)
            {
                _instance = new UDPNetwork();
                _instance.Init();
            }
            return _instance;
        }
    }

    public bool IsConnected
    {
        get
        {
            if (sendEndPoint != null)
                return true;
            return false;
        }
    }

    public void Connect(IPAddress _address, int _sendPort, int _recvPort)
    {
        sendEndPoint = new IPEndPoint(_address, _sendPort);
        listenEndPoint = new IPEndPoint(IPAddress.Any, _recvPort);

        senderSocket.EnableBroadcast = true;

        recvClient = new UdpClient(_recvPort);
        Thread recvThread = new Thread(this.ReceiveMessages);
        recvThread.Start();
    }

    public void Disconnect()
    {
        recvClient.Close();
        sendEndPoint = null;
        Debug.Log("[UDPNetwork cleaning up...]");
    }

    private void ReceiveMessages()
    {
        while (IsConnected)
        {
            byte[] receivedBytes = null;
            try
            {
                receivedBytes = recvClient.Receive(ref listenEndPoint);
            }
            catch
            {
                return;
            }

            if (receivedBytes == null || receivedBytes.Length == 0)
                continue;

            lock (messageQueue)
            {
                messageQueue.Enqueue(receivedBytes);
            }
        }
    }

    public void Send(byte[] _dataToSend)
    {
        if (sendEndPoint == null)
            return;

        try
        {
            senderSocket.SendTo(_dataToSend, sendEndPoint);
        }
        catch (Exception sendException)
        {
            sendEndPoint = null;
            throw sendException;
        }
    }

    public void Send(string _message)
    {
        Send(Encoding.ASCII.GetBytes(_message + CreateMD5(_message)));
    }

    public int MessageCount
    {
        get { return messageQueue.Count; }
    }

    public byte[] GetMessage()
    {
        if (MessageCount == 0)
            throw new InvalidOperationException("UDP Channel message queue is empty");

        byte[] result = null;
        lock (messageQueue)
        {
            result = messageQueue.Dequeue();
        }
        return result;
    }

    static public Dictionary<string, string> UnpackFormattedMessage(byte[] _message)
    {
        #region Check message identifier, discard message if identifier is invalid
        if (Encoding.ASCII.GetString(_message, 0, 4) != "JSOF")
        {
            Debug.Log("UDPNetwork unpacking, message does not start with JSOF");
            return new Dictionary<string, string>();
        }
        else
            Console.WriteLine(Encoding.ASCII.GetString(_message, 0, _message.Length));
        #endregion

        #region Get length of message, excluding the md5 hash
        int msgLength = int.Parse(Encoding.ASCII.GetString(_message, 4, 3));
        #endregion

        #region Generate md5 hash from received message and compare with received hash
        string generatedHash = UDPNetwork.getInstance().CreateMD5(Encoding.ASCII.GetString(_message, 0, msgLength));
        string receivedHash = Encoding.ASCII.GetString(_message, _message.Length - 32, 32);

        // Discard message if hashes do not match
        if (generatedHash != receivedHash)
        {
            Debug.Log("UDPNetwork unpacking, message hash does not match");
            return new Dictionary<string, string>();
        }
        else
            Console.WriteLine("Received and Generated Hash Codes match");
        #endregion

        #region Read data and add to dictionary
        Dictionary<string, string> newDict = new Dictionary<string, string>();
        for (int i = 7; i < msgLength;)
        {
            string dataName = Encoding.ASCII.GetString(_message, i, 3);
            int dataLength = int.Parse(Encoding.ASCII.GetString(_message, i + 3, 3));
            string data = Encoding.ASCII.GetString(_message, i + 6, dataLength);

            newDict.Add(dataName, data);
            Console.WriteLine(dataName + " added to dictionary - " + data);
            i += 6 + dataLength;
        }
        #endregion

        return newDict;
    }

    public string CreateMD5(string input)
    {
        // Use input string to calculate MD5 hash
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }

    // Slave monobehaviour to run Update() and to catch OnApplicationQuit() callback event, to close thread;
    internal class UDPGameobjectSlave : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            UDPNetwork.getInstance().Update();
        }
        private void OnApplicationQuit()
        {
            UDPNetwork.getInstance().Disconnect();
        }
    }
}