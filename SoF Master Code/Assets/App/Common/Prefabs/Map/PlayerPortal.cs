using SimpleJSON;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerPortal {

	public long id;
	public string platform;
	public string platformId;
	public string platformToken;
	public string name;
	public string email;
	public int isVerified;
	public DateTime linkTime;

	// instantiate from JSONNode
	public  void init (JSONNode json)
	{
		if (json["Id"] != null) {
			id = json["Id"].AsLong;
		}
		if (json["PlatformType"] != null) {
			platform = json["PlatformType"];
		}
		if (json["PlatformUserId"] != null) {
			platformId = json["PlatformUserId"];
		}

		if (json["PlatformToken"] != null) {
			platformToken = json["PlatformToken"];
		}
		if (json["PlatformName"] != null) {
			name = json["PlatformName"];
		}
		if (json["PlatformEmail"] != null) {
			email = json["PlatformEmail"];
		}

		if (json["PlatformVerified"] != null) {
			isVerified = json["PlatformVerified"].AsInt;
		}

		if (json["LinkTime"] != null) {
			linkTime = DateTime.Parse(json["LinkTime"], Constants.CULTURE, System.Globalization.DateTimeStyles.AssumeLocal);;
		}
	}


}