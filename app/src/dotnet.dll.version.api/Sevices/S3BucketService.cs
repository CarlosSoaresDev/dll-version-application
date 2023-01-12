using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System.ComponentModel;
using System.IO;

namespace dotnet.dll.version.api.Sevices;

public class S3BucketService
{
    private readonly AmazonS3Client _amazonS3Client;
    public S3BucketService()
    {
        _amazonS3Client = new AmazonS3Client(
            Environment.GetEnvironmentVariable("ACCESS_KEY"),
            Environment.GetEnvironmentVariable("SECRET_KEY"),
            Amazon.RegionEndpoint.USEast1);
    }

    public async Task UploadAsync(TransferUtilityUploadRequest transferUtilityUploadRequest)
    {
        var fileTransferUtility = new TransferUtility(_amazonS3Client);
        await fileTransferUtility.UploadAsync(transferUtilityUploadRequest);
    }

    public async Task<PutObjectResponse> PutObjectAsync(PutObjectRequest putObjectRequest)
    {
        return await _amazonS3Client.PutObjectAsync(putObjectRequest);
    }

    public async Task<GetObjectResponse> GetFileByKeyAsync(string bucketName, string key)
    {
        return await _amazonS3Client.GetObjectAsync(bucketName, key);
    }

    public async Task<ListObjectsResponse> ListObjectsAsync(string bucketName, string version)
    {
        return await _amazonS3Client.ListObjectsAsync(bucketName, $"{version}/dotnet.dll.version");
    }
}

