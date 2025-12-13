using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;

namespace Reserva.Domain.Services.Storage;

/// <summary>
/// Implementación de IStorageService para Cloudflare R2 usando API compatible con S3
/// </summary>
public class CloudflareR2StorageService : IStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _publicUrl;

    public CloudflareR2StorageService(IConfiguration configuration)
    {
        var accountId = configuration["CloudflareR2:AccountId"]
            ?? throw new InvalidOperationException("CloudflareR2:AccountId no está configurado");
        var accessKeyId = configuration["CloudflareR2:AccessKeyId"]
            ?? throw new InvalidOperationException("CloudflareR2:AccessKeyId no está configurado");
        var secretAccessKey = configuration["CloudflareR2:SecretAccessKey"]
            ?? throw new InvalidOperationException("CloudflareR2:SecretAccessKey no está configurado");
        _bucketName = configuration["CloudflareR2:BucketName"]
            ?? throw new InvalidOperationException("CloudflareR2:BucketName no está configurado");
        _publicUrl = configuration["CloudflareR2:PublicUrl"]
            ?? throw new InvalidOperationException("CloudflareR2:PublicUrl no está configurado");

        var credentials = new BasicAWSCredentials(accessKeyId, secretAccessKey);
        var config = new AmazonS3Config
        {
            ServiceURL = $"https://{accountId}.r2.cloudflarestorage.com",
            ForcePathStyle = true,
            AuthenticationRegion = "auto"
        };

        _s3Client = new AmazonS3Client(credentials, config);
    }

    /// <summary>
    /// Sube un archivo a Cloudflare R2 y retorna su URL pública
    /// </summary>
    public async Task<string> UploadAsync(Stream file, string path, string contentType)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        ms.Position = 0;

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = path,
            InputStream = ms,
            ContentType = contentType,
            DisablePayloadSigning = true,
            DisableDefaultChecksumValidation = true,
            Headers = { ContentLength = ms.Length }//,
            //CannedACL = S3CannedACL.PublicRead
        };

        await _s3Client.PutObjectAsync(request);
        return GetPublicUrl(path);
    }

    /// <summary>
    /// Elimina un archivo de Cloudflare R2
    /// </summary>
    public async Task DeleteAsync(string path)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = path
        };

        await _s3Client.DeleteObjectAsync(request);
    }

    /// <summary>
    /// Elimina todos los archivos dentro de una carpeta en Cloudflare R2
    /// </summary>
    public async Task DeleteFolderAsync(string folderPath)
    {
        // Listar todos los objetos en la carpeta
        var listRequest = new ListObjectsV2Request
        {
            BucketName = _bucketName,
            Prefix = folderPath
        };

        var objects = await _s3Client.ListObjectsV2Async(listRequest);

        if (objects.S3Objects.Count == 0)
            return;

        // Eliminar todos los objetos encontrados
        var deleteRequest = new DeleteObjectsRequest
        {
            BucketName = _bucketName,
            Objects = objects.S3Objects.Select(o => new KeyVersion { Key = o.Key }).ToList()
        };

        await _s3Client.DeleteObjectsAsync(deleteRequest);
    }

    /// <summary>
    /// Lista todos los archivos dentro de una carpeta
    /// </summary>
    public async Task<IEnumerable<string>> ListAsync(string folderPath)
    {
        var request = new ListObjectsV2Request
        {
            BucketName = _bucketName,
            Prefix = folderPath
        };

        var response = await _s3Client.ListObjectsV2Async(request);
        return response.S3Objects.Select(o => o.Key);
    }

    /// <summary>
    /// Obtiene la URL pública completa de un archivo
    /// </summary>
    public string GetPublicUrl(string path)
    {
        return $"{_publicUrl}/{path}";
    }
}
