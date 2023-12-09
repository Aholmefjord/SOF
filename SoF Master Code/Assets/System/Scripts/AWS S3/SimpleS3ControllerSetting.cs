using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon;

namespace JULESTech.AWS.S3
{
    [CreateAssetMenu(fileName = "aws_s3_setting", menuName = "JULESTech/AWS/S3/Settings")]
    public class SimpleS3ControllerSetting : ScriptableObject
    {
        [SerializeField]
        string accessKey;
        [SerializeField]
        string secretAccessKey;
        [SerializeField]
        string S3Region = RegionEndpoint.APSoutheast1.SystemName;

        public string AccessKey { get { return accessKey; } }
        public string SecretAccessKey { get { return secretAccessKey; } }
        public RegionEndpoint S3RegionEndpoint { get { return RegionEndpoint.GetBySystemName(S3Region); } }
    }
}