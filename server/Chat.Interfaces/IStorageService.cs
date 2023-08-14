using Amazon.S3.Model;
using Chat.Domain.Dto;
using Microsoft.AspNetCore.Http;
using S3Object = Amazon.S3.Model.S3Object;

namespace Chat.Interfaces;

public interface IStorageService
{
    Task<S3ResponseDto> UploadFileAsync(IFormFile file);
    Task<GetObjectResponse> DownloadFileAsync(string objKey, string bucketName);
    Task<List<S3Object>> GetAllObjectFromBucketAsync(string bucketName);
    Task CreateBucketAsync(string name);

    Task<ListBucketsResponse> GetAllBuckets();
}