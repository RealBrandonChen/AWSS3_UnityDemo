# AWSS3_UnityDemo
## Upload your local directory/mutiple files created from Unity to Amazon S3 bucket via its high-level API UtilityTransfer.

> Pardon my coding style and let's get our hands dirty.

- ### AWS S3 Set Up
  - #### Four parametes are necessary for connection between Amazon and Unity, which are `CognitoIdentity`, `S3 Bucket Name`, `Cognito Region` and `S3 Region`. If you have already set up these, skipping this part is fine.
  - #### Obtain the Identity Pool ID using Amazon Cognito as the official doc describes:
  1. Log in to the Amazon Cognito Console and click Create new identity pool.
  2. Enter a name for your Identity Pool and check the checkbox to enable access to unauthenticated identities. Click Create Pool to create your identity pool.
  3. Click Allow to create the two default roles associated with your identity pool–one for unauthenticated users and one for authenticated users. These default roles provide your identity pool access to Cognito Sync and Mobile Analytics. Then you shall get the `CognitoIdentity` and the `Cognito Region`:

```C#
    CognitoAWSCredentials credentials = new CognitoAWSCredentials (
      "IDENTITY_POOL_ID", // Identity Pool ID
      RegionEndpoint.USEast1 // Region
    );
```
  - #### For Amazon S3(simple storage service) set up, please refer to [Getting started with Amazon S3].(https://docs.aws.amazon.com/AmazonS3/latest/userguide/GetStartedWithS3.html)
  - #### Last step you should set Cognito identity pool to access Amazon S3: `Identity and Access Management Console` --> click `Roles` in the left-hand pane --> Type your identity pool name into the search box --> Two roles will be listed: one for `unauthenticated users` and one for `authenticated users` --> Click the role for unauthenticated users --> Select `AmazonS3FullAcess` --> Click `Attach Policy`. The settings shown below will give your identity pool full to access to all actions for the specified bucket.
 ![Attach S3FullAcess Policy](https://user-images.githubusercontent.com/46734495/125425143-1b9e70ac-d415-40c9-b20a-0eca960ece1f.PNG)

- ### Getting Started
  - #### The release in this repo contains all the necessary AWS.SDK dependencies, including AWS SDK for .NET.45 and additional DLLs specifed in [AWS Unity Support](https://docs.aws.amazon.com/sdk-for-net/latest/developer-guide/unity-special.html), so just download the release and install it in your Unity Assets.
  - #### Input four parametes of yours in the Unity inspector window. Click `Run` and press `Space` button should upload the time stamp files generated in the application directory to the S3 bucket, and it's done.
    ![AWS S3 Settings](https://user-images.githubusercontent.com/46734495/125426929-81618e3b-e3cd-47cb-ae9b-81f26aa4f4d7.PNG)
    
- ### About environment dependencies: two issues you may consider when you set the environment yourself.
  - #### Here I use the aws-sdk-net45 as the AWSSDK core rather than the Standard 2.0 as the [official document](https://docs.aws.amazon.com/sdk-for-net/latest/developer-guide/unity-special.html) recommends, because I found the Standard 2.0 would not work in both Unity standard 2.0 and .Net 4.X envs, while the aws-sdk-net45 works well on both envs.
   - #### The original script uploading object to S3 is written by @John Pignata in his repo [aws-samples/aws-sdk-unity-samples](https://github.com/aws-samples/aws-sdk-unity-samples/tree/master/S3), but this example only contains low-level API that you must specify the file's name when you upload, besides, the compatility is not ideal for some API is not supported like UtilityTransfer.
