using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SimpleJSON;
using System.IO;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace JULESTech.AWS.S3 
{
    public class SimpleS3Bucket 
    {
        public string BucketName { get; private set; }

        //public Amazon.RegionEndpoint Region { get; private set; }
        public RegionEndpoint S3RegionEndpoint { get { return RegionEndpoint.GetBySystemName(S3Region); } }
        [SerializeField]
        string S3Region = RegionEndpoint.APSoutheast1.SystemName;

        IAmazonS3 mClient = null;
        IAmazonS3 Client {
            get {
                //*
                if( mClient==null) {
                    mClient = SimpleS3Controller.Instance.GetClientByRegion(S3RegionEndpoint);
                }
                return mClient;
                //*/
                //return SimpleS3Controller.Instance.Client;
            }
        }
        /// <summary>
        /// load bucketname from a text file located in Unity's Resources folder
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static SimpleS3Bucket LoadFromResources(string path)
        {
            TextAsset config = UnityEngine.Resources.Load<TextAsset>(path);

            if (config == null) {
                Debug.LogError("[SimpleS3Bucket::LoadFromResources] config file not found.");
                return null;
            }

            JSONNode configJSON = JSON.Parse(config.text);
            string bucketName = configJSON["s3_bucket_name"];
            string bucketRegion = configJSON["s3_region_endpoint"];

            //SimpleS3Bucket bucket = new SimpleS3Bucket(config.text);
            SimpleS3Bucket bucket = new SimpleS3Bucket(bucketName, bucketRegion);
            Debug.Log("Bucket name: " + bucket.BucketName);
            return bucket;
        }

        private SimpleS3Bucket() { }
        
        public SimpleS3Bucket(string bucketName, string regionEndpoint)
        {
            BucketName = bucketName;
            S3Region = regionEndpoint;
        }

        /// <summary>
        /// Upload data of byte[] onto this bucket
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        /// <param name="resultCallback"></param>
        public void UploadDataPutMethod(string fileName, byte[] data, AmazonServiceCallback<PutObjectRequest, PutObjectResponse> resultCallback = null)
        {
            Stream dataStream = GenerateStream(data);
            _uploadDataPutMethodInner(fileName, dataStream, resultCallback);
        }
        /// <summary>
        /// Upload string data onto this bucket
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        /// <param name="resultCallback"></param>
        public void UploadDataPutMethod(string fileName, string data, AmazonServiceCallback<PutObjectRequest, PutObjectResponse> resultCallback = null)
        {
            Stream dataStream = GenerateStream(data);
            _uploadDataPutMethodInner(fileName, dataStream, resultCallback);
        }
        void _uploadDataPutMethodInner(string fileName, Stream data, AmazonServiceCallback<PutObjectRequest, PutObjectResponse> resultCallback = null)
        {
            var request = new PutObjectRequest() {
                BucketName = BucketName,//setting.S3BucketName,
                Key = fileName,
                InputStream = data,
                CannedACL = S3CannedACL.Private,
                StreamTransferProgress = (sender, args) => {
                    //Debug.Log("S3 Put Progress: "+ args.PercentDone);
                    Debug.Log("S3 bytes: " + args.TransferredBytes + "/" + args.TotalBytes);
                }
            };

            if (resultCallback != null) {
                // callback handler provided by caller
                Client.PutObjectAsync(request, resultCallback);
            } else {
                // callback handler not provided, provide a default one
                Client.PutObjectAsync(request, (responseObj) => {
                    if (responseObj.Exception == null) {
                        Debug.Log(string.Format("[SimpleS3Bucket::PutObject] object {0} posted to bucket {1}", responseObj.Request.Key, responseObj.Request.BucketName));
                    } else {
                        Debug.LogError("[SimpleS3Bucket::PutObject] Exception while posting the result object" + responseObj.Exception.Message);
                    }
                });
            }
        }

        /// <summary>
        /// Download an object from the given path from this bucket
        /// </summary>
        /// <param name="pathFileName"></param>
        /// <param name="resultCallback"></param>
        public void DownloadObject(string pathFileName, AmazonServiceCallback<GetObjectRequest, GetObjectResponse> resultCallback)
        {
            if (resultCallback != null) {
                Client.GetObjectAsync(BucketName, pathFileName, resultCallback);
                return;
            }

            Client.GetObjectAsync(BucketName, pathFileName, (responseObj) => {
                string data = null;
                var response = responseObj.Response;
                if (response.ResponseStream != null) {
                    using (StreamReader reader = new StreamReader(response.ResponseStream)) {
                        data = reader.ReadToEnd();
                    }
                    Debug.Log("[SimpleS3Bucket::DownloadObject] data: " + data);
                } else {
                    Debug.LogError("[SimpleS3Bucket::DownloadObject] error: " + responseObj.Exception.Message);
                }
            });
        }

        /// <summary>
        /// Delete an object from this bucket
        /// </summary>
        /// <param name="pathFilename"></param>
        /// <param name="on_success"></param>
        /// <param name="on_failure"></param>
        public void DeleteObject(string pathFilename, OnDeleteSuccessCallback on_success, OnDeleteFailureCallback on_failure)
        //public void DeleteObject(string pathFilename, AmazonServiceCallback<DeleteObjectsRequest,DeleteObjectsResponse> resultCallback)
        {
            List<KeyVersion> objects = new List<KeyVersion>();
            objects.Add(new KeyVersion() {
                Key = pathFilename
            });

            var request = new DeleteObjectsRequest() {
                BucketName = BucketName,
                Objects = objects
            };

            System.Text.StringBuilder strb = new System.Text.StringBuilder();
            Client.DeleteObjectsAsync(request, (responseObj) => {
                if (responseObj.Exception == null) {
                    if (on_success != null) {
                        Debug.Log("[SimpleS3Controller::DeleteObject] success");
                        on_success(responseObj.Response.DeletedObjects);
                    }
                    /*
                    else {
                        // sample on how to handle the return/result;
                        strb.AppendLine("[SimpleS3Controller::DeleteObject] success: ");
                        responseObj.Response.DeletedObjects.ForEach((dObj) =>
                        {
                            strb.AppendLine(dObj.Key);
                        });
                        Debug.Log(strb);
                    }
                    //*/
                } else {
                    if (on_failure != null)
                        on_failure(responseObj.Response);
                    //Debug.Log("[SimpleS3Controller::DeleteObject] error: " + responseObj.Exception.Message);
                }
            });
        }

        #region StringHelpers
        // helper; should I move this to StringHelper?
        public static Stream GenerateStream(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        public static Stream GenerateStream(byte[] data)
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(data, 0, data.Length); // TOM: potential problem causing audio sending to fail [7th July, 6pm]
            stream.Position = 0;
            return stream;
        }
        #endregion

        public delegate void OnDeleteSuccessCallback(List<DeletedObject> res);
        public delegate void OnDeleteFailureCallback(DeleteObjectsResponse res);
    }
}