using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace JULESTech
{
	public class DataMap
	{
		private Dictionary<string, string> dataMap = new Dictionary<string, string>();

		public void LoadTSVData(string _data)
		{
            // Parse the whole data into lines
            string[] lineArray = _data.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            // Read each line
            foreach (string currLine in lineArray)
            {
                // Parse it by tabs
                string[] parts = currLine.Split('\t');

                // Add the data to the data map pair-wise
                for (int i = 0; i < parts.Length - 1; i += 2)
                {
                    if (dataMap.ContainsKey(parts[i]))
                    {
                        Debug.LogError("Load Data Map, key dulplicated: " + parts[i]);
                        continue;
                    }

                    dataMap.Add(parts[i], parts[i + 1]);
                }
            }
        }

		/// <summary>
		/// Loads a TSV file into the data store.
		/// Data must be tab-seperated and come in pairs per line
		/// </summary>
		/// <param name="_filePath">The filepath</param>
		/// <returns>True if success, false if failed</returns>
		public bool LoadTSVFile(string _filePath)
		{
			// Check if file exist
			if (!File.Exists(_filePath))
				return false;

			// Read the file
			using (StreamReader reader = new StreamReader(_filePath))
			{
				// For each line
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					// Parse the line
					string[] parts = line.Split('\t');

					// Add the data to the data map pair-wise
					for (int i = 0; i < parts.Length - 1; i += 2)
					{
						dataMap.Add(parts[i], parts[i + 1]);
					}
				}
			}
			return true;
		}

#if UNITY_5
		public bool LoadTSVFileFromAsset(TextAsset t)
		{
			Debug.Log("Reading TextAsset t: " + t.name);
			String s = t.text;
			using (StringReader reader = new StringReader(t.text))
			{
				string line = string.Empty;
				do
				{
					line = reader.ReadLine();
					if (line != null)
					{
						string[] parts = line.Split('\t');

						// Add the data to the data map pair-wise
						for (int i = 0; i < parts.Length - 1; i += 2)
						{
							dataMap.Add(parts[i], parts[i + 1]);
						}
					}

				} while (line != null);
			}
			return true;
		}
#endif

		public bool HasKey(string _key)
		{
			if (!dataMap.ContainsKey(_key))
				return false;
			return true;
		}

		public int GetInt(string _key)
		{
			if (!dataMap.ContainsKey(_key))
				return 0;

			return int.Parse(dataMap[_key]);
		}

		public float GetFloat(string _key)
		{
			if (!dataMap.ContainsKey(_key))
				return 0;

			return float.Parse(dataMap[_key]);
		}

		public string GetString(string _key)
		{
			if (!dataMap.ContainsKey(_key))
				return "";

			return dataMap[_key];
		}

		public void SetInt(string _key, int _value)
		{
			dataMap[_key] = _value.ToString();
		}

		public void SetFloat(string _key, float _value)
		{
			dataMap[_key] = _value.ToString();
		}

		public void SetString(string _key, string _value)
		{
			dataMap[_key] = _value;
		}

		public void RemoveValue(string _key)
		{
			dataMap.Remove(_key);
		}

		public void CleanDataMap()
		{
			dataMap.Clear();
		}
	}
}