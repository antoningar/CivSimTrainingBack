using cst_back.Protos;
using Grpc.Core;
using MongoDB.Bson;

namespace cst_back.DBServices
{
    public interface IFileDBService
    {
        public Task SaveFile(string userId, string instanceId);
        public Task<ObjectId> GetFileIdByInstanceId(string instanceId);
        public Task DownloadFile(ObjectId objectId, IServerStreamWriter<DownloadSaveResponse> streamReponse);
    }
}
