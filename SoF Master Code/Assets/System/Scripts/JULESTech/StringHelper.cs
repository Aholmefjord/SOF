using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;

public sealed class StringHelper
{
	public static void DebugPrint(string _msg)
	{
#if UNITY_EDITOR
		Debug.Log(_msg);
#endif 
	}

	public static void DebugPrint(byte[] _msg)
	{
#if UNITY_EDITOR
		DebugPrint(Encoding.UTF8.GetString(_msg));
#endif
	}

    public static string GetMD5Hash(string _input)
    {
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(_input);
        return GetMD5Hash(inputBytes);
    }

    public static string GetMD5Hash(byte[] rawData)
    {
        // step 1, calculate MD5 hash from input
        MD5 md5 = System.Security.Cryptography.MD5.Create();
        byte[] hash = md5.ComputeHash(rawData);

        // step 2, convert byte array to hex string
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++) {
            sb.Append(hash[i].ToString("x2"));
        }

        return sb.ToString();
    }

    public static string Base64Encode(string _data) 
	{
		byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(_data);
		return System.Convert.ToBase64String(plainTextBytes);
	}

	public static string Base64Decode(string _encodedData) 
	{
		var base64EncodedBytes = System.Convert.FromBase64String(_encodedData);
		return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
	}


    /// <summary>
    /// trims filepath delimited by '/' only, return the last token found
    /// </summary>
    /// <param name="path">path to trim</param>
    /// <returns></returns>
    public static void TrimFilename(ref string path)
    {
        int lastSlashIndex = path.LastIndexOf('/');
        path = path.Substring(lastSlashIndex + 1);
    }
}
