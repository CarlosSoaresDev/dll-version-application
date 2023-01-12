using dotnet.dll.version.messages.core;
using Microsoft.Extensions.Caching.Distributed;
using System.Reflection;
using System.Text.RegularExpressions;

namespace dotnet.dll.version.api.Sevices
{
    public class MessageService
    {
        private string BUCKET_NAME = Environment.GetEnvironmentVariable("BUCKET_NAME");
        private const string S3_TO_DIRECTORY_CACHE = "cache_key_s3_to_directory_";
        private const string DEFAULT_VERSION = "1.1";

        private readonly S3BucketService _bucketService;
        private readonly IDistributedCache _distributedCache;

        public MessageService(S3BucketService s3BucketService, IDistributedCache distributedCache)
        {
            _bucketService = s3BucketService;
            _distributedCache = distributedCache;
        }

        private MessageStructure SetNewInstance(string key, string version)
        {

            var cleanKey = Regex.Replace(key, "[^A-Za-z]", "");

            var type = Assembly
                .LoadFrom($@"{AppDomain.CurrentDomain.BaseDirectory}\\dotnet.dll.version.messages.v{version}.{cleanKey.ToUpper()}.dll")
                .GetTypes()
                .FirstOrDefault(f => f.Name == key.ToUpper());

            return (MessageStructure)Activator.CreateInstance(type);
        }

        public string GetMessageByKeyAndVersionReference(string key, string version)
        {
            if (version == null)
                version = DEFAULT_VERSION;

            var instance = SetNewInstance(key, version);

            return instance.GetMessageValue();
        }

        public async Task<string> GetMessageByKeyAndVersionS3ToDirectory(string key, string version)
        {
            if (version == null)
                version = DEFAULT_VERSION;

            var cache = await _distributedCache.GetStringAsync(S3_TO_DIRECTORY_CACHE + version);

            if (cache == null)
            {
                var listObjects = await _bucketService.ListObjectsAsync(BUCKET_NAME, version);

                foreach (var s3Object in listObjects.S3Objects)
                {
                    var cleanKey = s3Object.Key.Replace($"{version}/", "");
                    var file = await _bucketService.GetFileByKeyAsync(BUCKET_NAME, s3Object.Key);

                    var filePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\\{cleanKey}";

                    if (!File.Exists(filePath))
                    {
                        using Stream streamToWriteTo = File.Open(filePath, FileMode.Create);
                        await file.ResponseStream.CopyToAsync(streamToWriteTo);
                    }
                }

                await _distributedCache.SetStringAsync(S3_TO_DIRECTORY_CACHE + version, Convert.ToString(true), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                });
            }

            var instance = SetNewInstance(key, version);
            return instance.GetMessageValue();
        }
    }
}
