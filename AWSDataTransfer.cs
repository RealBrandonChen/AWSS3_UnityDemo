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

using System;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;

using Amazon.CognitoIdentity;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Amazon.S3.Transfer;
using Amazon.Runtime;


public class AWSDataTransfer : MonoBehaviour
{
    [Header("AWS Settings")]
    public string IdentityPoolId = "";
    public string S3BucketName = "";

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

    private string directoryPath;
    // Specify the document type you want to upload in the directory
    private const string wildCard = "*.csv";

    // Enable the post folder event when upload action like "Space" button pressed
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
        directoryPath = GetFileHelper();
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
    /// Post Object to S3 Bucket in asynchronous method. 
    /// </summary>

#pragma warning disable
// Ignore the await request warning in the console
    void PostFolder()
    {
        UploadDirecoty();
    }
#pragma warning restore

    private async Task UploadDirecoty()
    {
        try
        {
            var directoryTransferUtility = new TransferUtility(Client);
            var request = new TransferUtilityUploadDirectoryRequest
            {
                BucketName = S3BucketName,
                Directory = directoryPath,
                SearchOption = SearchOption.AllDirectories,
                //SearchPattern = wildCard,
                CannedACL = S3CannedACL.Private,
            };

            await directoryTransferUtility.UploadDirectoryAsync(request);
            Debug.Log("All files uploaded!");
        }
        catch (AmazonS3Exception e)
        {
            Debug.LogFormat(
                "Error encountered ***. Message:'{0}' when writing an object", e.Message);
        }
        catch (Exception e)
        {
            Debug.LogFormat(
                 "Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
        }


    }

    private string GetFileHelper()
    {
        string filesDirectory;
        filesDirectory = Directory.GetParent(Application.dataPath).ToString() + Path.DirectorySeparatorChar + "TimeStampFolder";
        return filesDirectory;
    }

    /// <summary>
    /// Post Object to S3 Bucket using synchronous method, this may cause thread blocking if the uploading directory is heavy. 
    /// </summary>
    /*
    void PostFolder()
    {
        //UploadDirAsync().Wait();
        var directoryTransferUtility = new TransferUtility(Client);
        var request = new TransferUtilityUploadDirectoryRequest
        {
            BucketName = S3BucketName,
            Directory = directoryPath,
            SearchOption = SearchOption.AllDirectories,
            //SearchPattern = wildCard,
            CannedACL = S3CannedACL.Private,
        };
        directoryTransferUtility.UploadDirectory(request);
        Debug.Log("All files uploaded!");
    }
    */
}
