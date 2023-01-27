using cst_back.Protos;
using Grpc.Core;

namespace cst_back.Services
{
    public interface IFileInfoService
    {
        public Task<GetBaseInfoResponse> GetBaseInfo(IAsyncStreamReader<GetInfoRequest> requestStream, ServerCallContext context);
        public Task<GetFinalInfoResponse> GetFinalInfo(IAsyncStreamReader<GetInfoRequest> requestStream, ServerCallContext context);
        public Task<DeleteTmpFilesResponse> DeleteTmpFiles(DeleteTmpFilesRequest request, ServerCallContext context);
        public Task DownloadSave(DownloadSaveRequest request, IServerStreamWriter<DownloadSaveResponse> responseStream, ServerCallContext context);
    }
}
