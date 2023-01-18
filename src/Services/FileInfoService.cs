using cst_back.Protos;
using Grpc.Core;

namespace cst_back.Services
{
    public class FileInfoService : RPCFileInfo.RPCFileInfoBase, IFileInfoService
    {
        private const string TMP_PATH = "tmp/";
        public override async Task<GetBaseInfoResponse> GetBaseInfo(IAsyncStreamReader<GetInfoRequest> requestStream, ServerCallContext context)
        {
            FileStream? file = await WriteFile(requestStream, "base");
            
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
            FileStream? file = await WriteFile(requestStream, "final");

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

        private async Task<FileStream?> WriteFile(IAsyncStreamReader<GetInfoRequest> requestStream, string suffix)
        {
            FileStream? file = null;
            await foreach (var message in requestStream.ReadAllAsync())
            {
                if (!string.IsNullOrWhiteSpace(message.Id) && file == null)
                {
                    file = CreateFile(message.Id, suffix);
                }
                if (!message.File.IsEmpty && file != null)
                {
                    await file.WriteAsync(message.File.Memory);
                }
            }

            return file;
        }

        private FileStream CreateFile(string userId, string suffix)
        {
            string filename = string.Join("_",Path.GetRandomFileName(), userId, suffix, ".Civ6Save");
            return File.Create(TMP_PATH + filename); ;
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
    }
}
