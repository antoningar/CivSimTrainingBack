using cst_back.DBServices;
using cst_back.Helpers;
using cst_back.Models;
using cst_back.Protos;
using Grpc.Core;
using MongoDB.Driver;

namespace cst_back.Services
{
    public class FileInfoService : RPCFileInfo.RPCFileInfoBase, IFileInfoService
    {
        private IFileHelper _fileHelper;
        private IAccountDBService _accounDBService;

        public FileInfoService(IFileHelper fileHelper, IAccountDBService accountDBService)
        {
            _fileHelper = fileHelper;
            _accounDBService = accountDBService;
        }

        public override async Task<GetBaseInfoResponse> GetBaseInfo(IAsyncStreamReader<GetInfoRequest> requestStream, ServerCallContext context)
        {
            FileStream? file = await _fileHelper.WriteFile(requestStream, "base");
            
            if (file != null)
            {
                GetBaseInfoResponse response = new()
                {
                    MapInfo = await GetMapInfosAsync(file!)
                };
                file.Close();
                return response;
            }
            else
            {
                file!.Close();
                throw new RpcException(new Status(StatusCode.Aborted, "Filesystem error"));
            }
        }

        public override async Task<GetFinalInfoResponse> GetFinalInfo(IAsyncStreamReader<GetInfoRequest> requestStream, ServerCallContext context)
        {
            FileStream? file = await _fileHelper.WriteFile(requestStream, "final");

            if (file != null)
            {
                GetFinalInfoResponse response =  new()
                {
                    MapInfo = await GetMapInfosAsync(file),
                    GameStats = await GetGameStatsAsync(file)
                };
                file.Close();
                return response;
            }
            else
            {
                file!.Close();
                throw new RpcException(new Status(StatusCode.Aborted, "Filesystem error"));
            }
        }

        private Task<GameStats> GetGameStatsAsync(FileStream file)
        {
            return Task.FromResult(new GameStats()
            {
                Science = 74,
                Culture = 63,
                Food = 87,
                Production = 116,
                Faith = 4,
                Gold = 4
            });
        }

        private Task<MapInfo> GetMapInfosAsync(FileStream file)
        {
            var result = new MapInfo()
            {
                Civilization = "Dido",
                Turn = 1,
                Map = "Seven Seas"
            };
            result.Mods.Add("BBS");
            result.Mods.Add("BBG");

            return Task.FromResult(result);
        }

        private async Task CheckUsernameExist(string username)
        {
            Account? account = await _accounDBService.GetAccountByUsernameAsync(username);
            if (account == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Account not found"));
        }

        public override async Task<DeleteTmpFilesResponse> DeleteTmpFiles(DeleteTmpFilesRequest request, ServerCallContext context)
        {
            DeleteTmpFilesResponse response = new();
            
            try
            {
                await CheckUsernameExist(request.Username);
                response.NumberDeleted = _fileHelper.DeleteTmpFileByUsername(request.Username);
            }
            catch (MongoException ex) 
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }

            return response;
        }
    }
}
