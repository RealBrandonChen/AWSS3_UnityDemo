// Copyright 2014-2015 Amazon.com, 
// Inc. or its affiliates. All Rights Reserved.
// 
// Licensed under the AWS Mobile SDK For Unity 
// Sample Application License Agreement (the "License"). 
// You may not use this file except in compliance with the 
// License. A copy of the License is located 
// in the "license" file accompanying this file. This file is 
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, express or implied. See the License 
// for the specific language governing permissions and 
// limitations under the License.
//
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

using Amazon.CognitoIdentity;
using Amazon;
using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using Amazon.S3.Util;

namespace AWSSDK.Examples
{
    public class S3Example : MonoBehaviour
    {
        public string IdentityPoolId = "us-west-1:8bc8e112-795c-4fef-8036-e0aa1d153842";
        public string CognitoIdentityRegion = RegionEndpoint.USWest1.SystemName;
        private RegionEndpoint _CognitoIdentityRegion
        {
            get { return RegionEndpoint.GetBySystemName(CognitoIdentityRegion); }
        }
        public string S3Region = RegionEndpoint.APEast1.SystemName;
        private RegionEndpoint _S3Region
        {
            get { return RegionEndpoint.GetBySystemName(S3Region); }
        }
        public string S3BucketName = "bucketforunityvr";
        private const string directoryPath = @"*** directory path ***";
        // The example uploads only .txt files.
        private const string wildCard = "*.csv";

        private void OnEnable()
        {
            DataRecording.OnUpload += PostFolder;
        }
        private void OnDisable()
        {
            DataRecording.OnUpload -= PostFolder;
        }
        void Start()
        {
            
        }

        private void Update()
        {
            
        }

        #region private members

        private IAmazonS3 _s3Client;
        private AWSCredentials _credentials;

        private AWSCredentials Credentials
        {
            get
            {
                if (_credentials == null)
                    _credentials = new CognitoAWSCredentials(IdentityPoolId, _CognitoIdentityRegion);
                return _credentials;
            }
        }

        private IAmazonS3 Client
        {
            get
            {
                if (_s3Client == null)
                {
                    _s3Client = new AmazonS3Client(Credentials, _S3Region);
                }
                //test comment
                return _s3Client;
            }
        }


        #endregion

        /// <summary>
        /// Post Object to S3 Bucket. 
        /// </summary>
        
        void PostFolder()
        {
            UploadDirAsync().Wait();
        }
           
       

        private async Task UploadDirAsync()
        {   
            try
            {
                var directoryTransferUtility = new TransferUtility(Client);

                // 1. Upload a directory.
                await directoryTransferUtility.UploadDirectoryAsync(directoryPath, S3BucketName);
                Console.WriteLine("Upload statement 1 completed");

                // 2. Upload only the .txt files from a directory 
                //    and search recursively. 
                await directoryTransferUtility.UploadDirectoryAsync(
                                               directoryPath,
                                               S3BucketName,
                                               wildCard,
                                               SearchOption.AllDirectories);
                Console.WriteLine("Upload statement 2 completed");

                // 3. The same as Step 2 and some optional configuration. 
                //    Search recursively for .txt files to upload.
                var request = new TransferUtilityUploadDirectoryRequest
                {
                    BucketName = S3BucketName,
                    Directory = directoryPath,
                    SearchOption = SearchOption.AllDirectories,
                    SearchPattern = wildCard
                };

                await directoryTransferUtility.UploadDirectoryAsync(request);
                Console.WriteLine("Upload statement 3 completed");
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine(
                        "Error encountered ***. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
        }
    }
}
        /*
        #region helper methods
        
        private string GetFileHelper()
        {
            var fileName = SampleFileName;

            if (!File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + fileName))
            {
                var streamReader = File.CreateText(Application.persistentDataPath + Path.DirectorySeparatorChar + fileName);
                streamReader.WriteLine("This is a sample s3 file uploaded from unity s3 sample");
                streamReader.Close();
            }
            return fileName;
        }

        private string GetPostPolicy(string bucketName, string key, string contentType)
        {
            bucketName = bucketName.Trim();

            key = key.Trim();
            // uploadFileName cannot start with /
            if (!string.IsNullOrEmpty(key) && key[0] == '/')
            {
                throw new ArgumentException("uploadFileName cannot start with / ");
            }

            contentType = contentType.Trim();

            if (string.IsNullOrEmpty(bucketName))
            {
                throw new ArgumentException("bucketName cannot be null or empty. It's required to build post policy");
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("uploadFileName cannot be null or empty. It's required to build post policy");
            }
            if (string.IsNullOrEmpty(contentType))
            {
                throw new ArgumentException("contentType cannot be null or empty. It's required to build post policy");
            }

            string policyString = null;
            int position = key.LastIndexOf('/');
            if (position == -1)
            {
                policyString = "{\"expiration\": \"" + DateTime.UtcNow.AddHours(24).ToString("yyyy-MM-ddTHH:mm:ssZ") + "\",\"conditions\": [{\"bucket\": \"" +
                    bucketName + "\"},[\"starts-with\", \"$key\", \"" + "\"],{\"acl\": \"private\"},[\"eq\", \"$Content-Type\", " + "\"" + contentType + "\"" + "]]}";
            }
            else
            {
                policyString = "{\"expiration\": \"" + DateTime.UtcNow.AddHours(24).ToString("yyyy-MM-ddTHH:mm:ssZ") + "\",\"conditions\": [{\"bucket\": \"" +
                    bucketName + "\"},[\"starts-with\", \"$key\", \"" + key.Substring(0, position) + "/\"],{\"acl\": \"private\"},[\"eq\", \"$Content-Type\", " + "\"" + contentType + "\"" + "]]}";
            }

            return policyString;
        }

    }
    
#endregion
        */


/*
namespace S3CreateAndList
{
    class Program
    {
        // Main method
        static async Task Main(string[] args)
        {
            // Before running this app:
            // - Credentials must be specified in an AWS profile. If you use a profile other than
            //   the [default] profile, also set the AWS_PROFILE environment variable.
            // - An AWS Region must be specified either in the [default] profile
            //   or by setting the AWS_REGION environment variable.

            // Create an S3 client object.
            var s3Client = new AmazonS3Client();

            // Parse the command line arguments for the bucket name.
            if (GetBucketName(args, out String bucketName))
            {
                // If a bucket name was supplied, create the bucket.
                // Call the API method directly
                try
                {
                    Console.WriteLine($"\nCreating bucket {bucketName}...");
                    var createResponse = await s3Client.PutBucketAsync(bucketName);
                    Console.WriteLine($"Result: {createResponse.HttpStatusCode.ToString()}");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Caught exception when creating a bucket:");
                    Console.WriteLine(e.Message);
                }
            }

            // List the buckets owned by the user.
            // Call a class method that calls the API method.
            Console.WriteLine("\nGetting a list of your buckets...");
            var listResponse = await MyListBucketsAsync(s3Client);
            Console.WriteLine($"Number of buckets: {listResponse.Buckets.Count}");
            foreach (S3Bucket b in listResponse.Buckets)
            {
                Console.WriteLine(b.BucketName);
            }
        }


        // 
        // Method to parse the command line.
        private static Boolean GetBucketName(string[] args, out String bucketName)
        {
            Boolean retval = false;
            bucketName = String.Empty;
            if (args.Length == 0)
            {
                Console.WriteLine("\nNo arguments specified. Will simply list your Amazon S3 buckets." +
                  "\nIf you wish to create a bucket, supply a valid, globally unique bucket name.");
                bucketName = String.Empty;
                retval = false;
            }
            else if (args.Length == 1)
            {
                bucketName = args[0];
                retval = true;
            }
            else
            {
                Console.WriteLine("\nToo many arguments specified." +
                  "\n\ndotnet_tutorials - A utility to list your Amazon S3 buckets and optionally create a new one." +
                  "\n\nUsage: S3CreateAndList [bucket_name]" +
                  "\n - bucket_name: A valid, globally unique bucket name." +
                  "\n - If bucket_name isn't supplied, this utility simply lists your buckets.");
                Environment.Exit(1);
            }
            return retval;
        }


        //
        // Async method to get a list of Amazon S3 buckets.
        private static async Task<ListBucketsResponse> MyListBucketsAsync(IAmazonS3 s3Client)
        {
            return await s3Client.ListBucketsAsync();
        }

    }
}
*/

