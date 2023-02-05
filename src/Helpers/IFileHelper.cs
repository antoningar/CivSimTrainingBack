using cst_back.Models;
using cst_back.Protos;
using Grpc.Core;

namespace cst_back.Helpers
{
    public interface IFileHelper
    {
        public Task<FileStream?> WriteFile(IAsyncStreamReader<GetInfoRequest> requestStream, string suffix);

        public bool IsInstanceTmpFilesExist(string userId);
        public bool IsInstanceTmpFilesExistByInstanceId(string instancId);

        public Task<Instance> GetInstanceFromFile(string filePath);
        public int DeleteTmpFileByUsername(string username);
    }
}
