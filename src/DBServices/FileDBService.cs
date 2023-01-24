using cst_back.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace cst_back.DBServices
{
    public class FileDBService : IFileDBService
    {
        private const string TMP_PATH = "tmp/";
        private GridFSBucket _bucket;

        public FileDBService(IOptions<DatabaseSettings> dbSettings)
        {
            MongoClient client = new(dbSettings.Value.ConnectionString);
            IMongoDatabase database = client.GetDatabase(dbSettings.Value.DatabaseName);

            _bucket = new GridFSBucket(database, new GridFSBucketOptions
            {
                BucketName= dbSettings.Value.FileBucketName
            });
        }

        public async Task SaveFile(string userId, string instanceId)
        {
            string[] files = Directory.GetFiles(TMP_PATH);
            string filename = files.First(x => x.Contains(userId));
            byte[] fileBytes  = File.ReadAllBytes(filename);
            await _bucket.UploadFromBytesAsync(filename, fileBytes);
        }
    }
}
