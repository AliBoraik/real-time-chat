using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Chat.Domain.Dto;
using Chat.Interfaces;
using Microsoft.AspNetCore.Http;
using S3Object = Amazon.S3.Model.S3Object;

namespace Chat.Application;

public class StorageService : IStorageService
{
    private readonly IAmazonS3 _amazonS3;

    public StorageService(IAmazonS3 amazonS3)
    {
        _amazonS3 = amazonS3;
    }

    public async Task<S3ResponseDto> UploadFileAsync(IFormFile file)
    {
        var obj = new Domain.Dto.S3Object
        {
            File = file,
            Name = file.FileName,
            BucketName = "temp"
        };

        await using var newMemoryStream = new MemoryStream();
        await obj.File.CopyToAsync(newMemoryStream);

        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = newMemoryStream,
            Key = obj.Name,
            BucketName = obj.BucketName,
            CannedACL = S3CannedACL.PublicRead
        };

        var transferUtility = new TransferUtility(_amazonS3);


        await transferUtility.UploadAsync(uploadRequest);

        var response = new S3ResponseDto
        {
            StatusCode = 201,
            Message = $"{obj.Name} has been uploaded successfully",
            FileName = obj.Name
        };
        return response;
    }

    public async Task<GetObjectResponse> DownloadFileAsync(string objKey, string bucketName)
    {
        var downloadRequest = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = objKey
        };

        var response = await _amazonS3.GetObjectAsync(downloadRequest);

        return response;
    }

    public async Task<List<S3Object>> GetAllObjectFromBucketAsync(string bucketName)
    {
        var response = await _amazonS3.ListObjectsAsync(bucketName);

        return response.S3Objects;
    }

    public async Task CreateBucketAsync(string name)
    {
        var bucketRequest = new PutBucketRequest
        {
            BucketName = name,
            UseClientRegion = true
        };

        await _amazonS3.PutBucketAsync(bucketRequest);
    }

    public async Task<ListBucketsResponse> GetAllBuckets()
    {
        return await _amazonS3.ListBucketsAsync();
    }
}