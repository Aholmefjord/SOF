using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;

namespace JULESTech
{
	public delegate void NetworkCallback(byte[] _data);
	public delegate void NetworkFailCallback(string _error);
	public  class JulesNet : MonoBehaviour
	{
		private string mServerBaseAddress = null;

		private static JulesNet instance = null;

		// Game Instance Singleton
		public static JulesNet Instance
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

		void Update()
		{
		}
			
		public void Init(string _serverBaseAddress)
		{
			mServerBaseAddress = _serverBaseAddress;
		}

		private IEnumerator HandleUnityWebRequest(UnityWebRequest _www, NetworkCallback _callbackDelegate = null, NetworkFailCallback _failCallbackDelegate = null, int _timeOut = 10000)
		{
			yield return _www.Send ();

			byte[] results = null;

			if (_www.isError) {
				if (_failCallbackDelegate != null)
					_failCallbackDelegate (_www.error);
			} else {
				// Retrieve results as binary data
				results = _www.downloadHandler.data;

				if (_callbackDelegate != null)
					_callbackDelegate (results);
			}
		}

		public void SendPOSTRequest(string _address, WWWForm _postData, NetworkCallback _callbackDelegate = null, NetworkFailCallback _failCallbackDelegate = null, int _timeOut = 10000)
		{
			UnityWebRequest www = UnityWebRequest.Post (mServerBaseAddress + _address, _postData);
			StartCoroutine(HandleUnityWebRequest(www, _callbackDelegate, _failCallbackDelegate, _timeOut));
        }
        public IEnumerator SendPOSTRequestCoroutine(string _address, WWWForm _postData, NetworkCallback _callbackDelegate = null, NetworkFailCallback _failCallbackDelegate = null, int _timeOut = 10000)
        {
            UnityWebRequest www = UnityWebRequest.Post(mServerBaseAddress + _address, _postData);
            yield return StartCoroutine(HandleUnityWebRequest(www, _callbackDelegate, _failCallbackDelegate, _timeOut));
        }

        public void SendGETRequest(string _address, NetworkCallback _callbackDelegate = null, NetworkFailCallback _failCallbackDelegate = null, int _timeOut = 10000)
		{
			UnityWebRequest www = UnityWebRequest.Get (mServerBaseAddress + _address);
			StartCoroutine(HandleUnityWebRequest(www, _callbackDelegate, _failCallbackDelegate, _timeOut));
		}

		public static string GetNetworkHostAddress()
		{
			IPHostEntry host;
			host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (IPAddress ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					return ip.ToString();
				}
			}
			return "";
		}
	}
}