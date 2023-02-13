using cst_back.DBServices;
using cst_back.Helpers;
using cst_back.Models;
using cst_back.Protos;
using cst_back.Services;
using Grpc.Core;
using Grpc.Core.Testing;
using Moq;

namespace cst_back.tests
{
    public static class Helper
    {
        public static ServerCallContext GetServerCallContext(string method)
        {
            return TestServerCallContext.Create(
                method,
                "localhost",
                DateTime.Now.AddMinutes(15),
                new Metadata(),
                CancellationToken.None,
                "", null, null,
                (metadata) => Task.CompletedTask,
                () => new WriteOptions(),
                (writeOptions) => { });

        }

        public static List<Instance> GetListInstance()
        {
            return new List<Instance>()
            {
                new Instance()
                {
                    Id = "1",
                    Civilization = "Dido",
                    Map = "Seven Seas",
                    Goal = "Scientific Victory"
                },
                new Instance()
                {
                    Id = "2",
                    Civilization = "Dido",
                    Map = "Seven Seas",
                    Goal = "Cultural Victory"
                },
                new Instance()
                {
                    Id = "3",
                    Civilization = "Trajan",
                    Map = "Pangea",
                    Goal = "Scientific Victory"
                },
            };
        }


        public static List<Instance> GetListInstanceBySearch(string search)
        {
            return new List<Instance>()
            {
                new Instance()
                {
                    Id = "1",
                    Civilization = "Di" + search + "do",
                    Map = "Seven Seas",
                    Goal = "Scientific Victory"
                },
                new Instance()
                {
                    Id = "2",
                    Civilization = "Dido",
                    Map = "Seven " + search + " Seas",
                    Goal = "Cultural Victory"
                },
                new Instance()
                {
                    Id = "3",
                    Civilization = "Trajan",
                    Map = "Pangea",
                    Goal = "Scientific " + search + " Victory"
                },
            };
        }

        public static bool InstancesResponseContainsSearch(InstancesResponse instance, string search)
        {
            return instance.Civilization.Contains(search) || instance.Map.Contains(search) || instance.Goal.Contains(search);
        }

        public static InstanceDetails GetDetailsInstance()
        {
            Instance instance = new()
            {
                Id = "1",
                Civilization = "Dido",
                Map = "Pangea",
                Goal = "Scientific victory",
                Creator = "sil2ob",
                Mods = new string[] { "BBS", "BBG" }
            };
            Models.Leaderboard leaderboard = new()
            {
                Results = new Models.Result[]
                {
                    new Models.Result()
                    {
                        Position = 1,
                        Username = "okletsgo",
                        Value = "Turn 115"
                    },
                    new Models.Result()
                    {
                        Position = 2,
                        Username = "bahtiens",
                        Value = "Turn 122"
                    },
                    new Models.Result()
                    {
                        Position = 3,
                        Username = "zbraa",
                        Value = "Turn 137"
                    },
                }
            };

            return new InstanceDetails(instance, leaderboard);
        }

        public static Instance GetInstance() 
        {
            return new Instance()
            {
                Id = "2",
                Civilization = "Dido",
                Map = "Seven Seas",
                Goal = "Cultural Victory"
            };
        }

        public static Models.Leaderboard GetLeaderBoard()
        {
            return new Models.Leaderboard()
            {
                Results = new Models.Result[]
                {
                    new Models.Result()
                    {
                        Position = 1,
                        Username = "toi",
                        Value = "Value"
                    }
                }
            };
        }

        public static InstanceService GetInstanceService(
            Mock<IInstanceDBService>? mockInstanceDBService = null,
            Mock<ILeaderboardDBService>? mockLeaderboardDBService = null,
            Mock<IAccountDBService>? mockAccountDBService = null,
            Mock<IFileHelper>? mockFileHelper = null,
            Mock<IFileDBService>? mockFileDBService = null
            )
        {
            mockInstanceDBService = (mockInstanceDBService == null) ? new Mock<IInstanceDBService>() : mockInstanceDBService;
            mockLeaderboardDBService = (mockLeaderboardDBService == null) ? new Mock<ILeaderboardDBService>() : mockLeaderboardDBService;
            mockAccountDBService = (mockAccountDBService == null) ? new Mock<IAccountDBService>() : mockAccountDBService;
            mockFileHelper = (mockFileHelper == null) ? new Mock<IFileHelper>() : mockFileHelper;
            mockFileHelper
                .Setup(x => x.GetGameStatsFromfile(It.IsAny<string>()))
                .ReturnsAsync(new Stats() { Culture = 104.2F, Faith = 14, Gold = 43, Science = 111.3F });
            mockFileDBService = (mockFileDBService == null) ? new Mock<IFileDBService>() : mockFileDBService;

            return new InstanceService(mockInstanceDBService.Object, mockLeaderboardDBService.Object, mockAccountDBService.Object, mockFileHelper.Object, mockFileDBService.Object);
        }

        public static FileInfoService GetFileInfoService(
            Mock<IFileHelper>? mockFileHelper = null,
            Mock<IAccountDBService>? mockAccountDBService = null,
            Mock<IInstanceDBService>? mockInstanceDBService = null,
            Mock<IFileDBService>? mockFileDBService = null
            )
        {
            mockInstanceDBService = (mockInstanceDBService == null) ? new Mock<IInstanceDBService>() : mockInstanceDBService;
            mockAccountDBService = (mockAccountDBService == null) ? new Mock<IAccountDBService>() : mockAccountDBService;
            mockFileHelper = (mockFileHelper == null) ? new Mock<IFileHelper>() : mockFileHelper;
            mockFileDBService = (mockFileDBService == null) ? new Mock<IFileDBService>() : mockFileDBService;

            return new FileInfoService(mockFileHelper.Object, mockAccountDBService.Object,  mockInstanceDBService.Object, mockFileDBService.Object);
        }
    }
}
