using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UDPNetworkCommandGenerator {
    
    /// <summary>
    /// Singleton Instance
    /// </summary>
    private static UDPNetworkCommandGenerator instance = null;

    /// <summary>
    /// Singleton Getter
    /// </summary>
    public static UDPNetworkCommandGenerator Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new UDPNetworkCommandGenerator();
                instance.init();
            }
            return instance;
        }
    }

    const string mHeader = "JSOF";

    Dictionary<string, string> mCommandList;


    private void init()
    {
        mCommandList = new Dictionary<string, string>();
    }

    /// <summary>
    /// Add command into Command List
    /// </summary>
    /// <param name="_key">3 Chatacter key</param>
    /// <param name="_value">Value for this command</param>
    public void AddCommand(string _key, string _value)
    {
        if (_key.Length != 3)
        {
            Debug.Log("UDPNetworkCommandGenerator, add command, invalid command key: " + _key);
            return;
        }

        //add value size into command
        int valueCount = _value.Length;
        string value = formatNumber(valueCount) + _value;

        Debug.Log("UDPNetworkCommandGenerator Comamnd Added: " + _key + " " + _value);
        mCommandList.Add(_key, value);
    }


    /// <summary>
    /// Generate Command based on Command List that was added.
    /// </summary>
    /// <returns>Command String</returns>
    public string GenerateCommand()
    {
        string command = "";

        if (mCommandList.Count == 0)
        {
            Debug.Log("UDPNetworkCommandGenerator, generate command, command list empty");
            return command;
        }

        //calculate message length
        //starts off by 3 because this length also needs to be inside
        int length = 3;

        //Command format
        //JSOF <Total length 3 byte> ( <ID 3 byte> <data length 3 byte> <data> ) and MD5 hash at the end
        //MD5 hash is created in UDPNetwork during send.
        string commandData = "";

        foreach (KeyValuePair<string, string> entry in mCommandList)
        {
            try
            {
                commandData += entry.Key;
                commandData += entry.Value;
            }
            catch (Exception e)
            {
                Debug.Log("UDPNetworkCommandGenerator adding command list: " + e.ToString());
            }
        }

        length += mHeader.Length;
        length += commandData.Length;

        command = mHeader + formatNumber(length) + commandData;

        Debug.Log("UDPNetworkCommandGenerator result: " + command);

        //After generating command, delete command.
        CleanCommand();

        return command;
    }

    /// <summary>
    /// Format number to fit into command list format
    /// </summary>
    /// <param name="_number">Number that needs to be formatted</param>
    /// <returns>return the formatted number as string</returns>
    private string formatNumber(int _number)
    {
        string _format = _number.ToString();

        if (_format.Length == 1)
            _format = "00" + _format;
        else if (_format.Length == 2)
            _format = "0" + _format;
        else if (_format.Length > 3)
        {
            Debug.Log("UDPNetworkComamndGenerator, format number, number is bigger than 3 byte, currently the system only supports 3 byte");
            _format.Substring(0, 3);
        }

        return _format;
    }

    /// <summary>
    /// Clear Command List
    /// </summary>
    public void CleanCommand()
    {
        mCommandList.Clear();
    }

}
