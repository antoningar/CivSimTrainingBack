using cst_back.Protos;
using cst_back.Settings;
using Grpc.Core;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
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
                BucketName= dbSettings.Value.BaseSaveBucketName
            });
        }

        public async Task SaveFile(string userId, string instanceId)
        {
            string[] files = Directory.GetFiles(TMP_PATH);
            string filename = files.First(x => x.Contains(userId));
            byte[] fileBytes  = File.ReadAllBytes(filename);
            await _bucket.UploadFromBytesAsync(filename.Replace(".Civ6Save", instanceId  + ".Civ6Save"), fileBytes);
        }

        public async Task<ObjectId> GetFileIdByInstanceId(string instanceId)
        {
            var filter = Builders<GridFSFileInfo>.Filter.Regex("Filename", new BsonRegularExpression(".*_" + instanceId + ".*"));
            GridFSFileInfo file = await _bucket.Find(filter).FirstOrDefaultAsync();
            return file.Id;
        }

        public async Task DownloadFile(ObjectId objectId, IServerStreamWriter<DownloadSaveResponse> streamReponse)
        {
            using (var stream = await _bucket.OpenDownloadStreamAsync(objectId))
            {
                var buffer = new byte[1024];
                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    DownloadSaveResponse response = new()
                    {
                        File = Google.Protobuf.ByteString.CopyFrom(buffer, 0, bytesRead)
                    };
                    await streamReponse.WriteAsync(response);
                }
            }
        }
    }
}
