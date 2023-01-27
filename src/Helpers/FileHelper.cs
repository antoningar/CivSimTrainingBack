using cst_back.Models;
using cst_back.Protos;
using Grpc.Core;

namespace cst_back.Helpers
{
    public class FileHelper : IFileHelper
    {
        private const string TMP_PATH = "tmp/";

        public Task<Instance> GetInstanceFromFile(string filePath)
        {
            return Task.FromResult(new Instance()
            {
                Civilization = "Dido",
                Map = "Seven Seas",
                Mods = new[] {"BBG, BBS"}
            });
        }

        public bool IsInstanceTmpFilesExist(string username)
        {
            string[] files = Directory.GetFiles(TMP_PATH);
            return files.Any(x => x.Contains("_" + username + "_", StringComparison.OrdinalIgnoreCase));
        }

        public bool IsInstanceTmpFilesExistByInstanceId(string instancId)
        {
            string[] files = Directory.GetFiles(TMP_PATH);
            return files.Any(x => x.Contains("_" + instancId, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<FileStream?> WriteFile(IAsyncStreamReader<GetInfoRequest> requestStream, string suffix)
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
            string filename = string.Join("_", Path.GetRandomFileName(), userId, suffix, ".Civ6Save");
            return File.Create(TMP_PATH + filename); ;
        }

        public int DeleteTmpFileByUsername(string username)
        {
            int count = 0;
            string[] files = Directory.GetFiles(TMP_PATH);
            foreach (string file in files) 
            {
                if (file.ToLower().Contains("_"+ username.ToLower() + "_"))
                {
                    File.Delete(file);
                    count++;
                }
            }
            return count;
        }
    }
}
