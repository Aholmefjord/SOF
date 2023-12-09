using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace JULESTech
{
	public class AnalyticsPacket
	{
		public string DataType;
		public string DataPacket;
		public int Time;

		public AnalyticsPacket(string _dataType, string _dataPacket, int _time)
		{
			DataType = _dataType;
			DataPacket = _dataPacket;
			Time = _time;
		}
	}
	
	/// <summary>
	/// Analytics System that reports data periodically to server
	/// Note that this uses JulesNet for sending HTTP POST requests
	/// </summary>
	public class AnalyticsSys : MonoBehaviour 
	{
		private static AnalyticsSys instance = null;
		private string targetAddress = "";
		private string senderID = "";
		private float uploadInterval = 1.0f;
		private float timeCounter = 0.0f;
		private int packetLimit = 10;
		private List<AnalyticsPacket> packetList = new List<AnalyticsPacket>();
		private bool mIsDisabled = false;

		public bool IsDisabled
		{
			get { return mIsDisabled; }
			set { mIsDisabled = value; }
		}

		// Game Instance Singleton
		public static AnalyticsSys Instance
		{
			get
			{ 
				return instance; 
			}
		}

		private void Awake()
		{
			// if the singleton hasn't been initialized yet
			if (instance != null && instance != this) 
			{
				Destroy(this.gameObject);
			}

			instance = this;
			DontDestroyOnLoad( this.gameObject );
		}

		public void Update()
		{
			timeCounter += Time.deltaTime;
			if (timeCounter < uploadInterval && packetList.Count < packetLimit)
				return;

			UploadData();
			timeCounter = 0.0f;
		}
			
		public void Init(string _address, string _senderID, float _uploadInterval = 1.0f, int _packetLimit = 10)
		{
			if (packetList.Count > 0)
			{
				UploadData();
				packetList.Clear();
			}
			
			targetAddress = _address;
			senderID = _senderID;
			uploadInterval = _uploadInterval;
			packetLimit = _packetLimit;
		}

		private void UploadData()
		{
			StringHelper.DebugPrint("Analytics System - Uploading Data");

			// Do nothing if there are no packets to send
			if (packetList.Count <= 0)
				return;

			string dataBlob = "";
			lock(packetList)
			{
				// Extract all current data and setup data to add to form
				foreach(AnalyticsPacket currPacket in packetList)
				{
					dataBlob += currPacket.DataType + "\t" + currPacket.DataPacket + "\t" + currPacket.Time + "\t";
				}
				dataBlob.TrimEnd("\t".ToCharArray());

				packetList.Clear();
			}

			dataBlob = StringHelper.Base64Encode(dataBlob);
			StringHelper.DebugPrint("Upload Data Blob : " + dataBlob);
				
			WWWForm dataForm = new WWWForm();
			dataForm.AddField("account_id", senderID);
			dataForm.AddField("dataBlob", dataBlob);
			dataForm.AddField("dataHash", StringHelper.GetMD5Hash(dataBlob));
			JulesNet.Instance.SendPOSTRequest(targetAddress, dataForm,
				(byte[] _msg) => {
					// Received a response
					StringHelper.DebugPrint(_msg);
					string[] data = Encoding.UTF8.GetString (_msg).Split ('\t');

					if (data[0] == "OK")
					{
						// Success, time to setup the game
					}
					else
					{
						// Failed
					}
				},
				(string _error) => {
					// Failed to send
				}
			);
		}

		public void Report(string _dataType, string _data)
		{
			// Don't allow reporting if disabled
			if (IsDisabled)
			{
				StringHelper.DebugPrint("Analytics System Disabled : " + _dataType + " - " + _data);
				return;
			}

			// Lock the packet list in case of concurrent access
			lock(packetList)
			{
				packetList.Add(new AnalyticsPacket(_dataType, _data, TimeHelper.ServerEpochTime));
				StringHelper.DebugPrint("Analytics System Reporting : " + _dataType + " - " + _data);
			}
		}
	}
}