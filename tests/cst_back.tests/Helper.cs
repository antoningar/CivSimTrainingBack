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
            IInstanceDBService instanceDBService = (mockInstanceDBService == null) ? new Mock<IInstanceDBService>().Object : mockInstanceDBService!.Object;
            ILeaderboardDBService leaderboardDBService = (mockLeaderboardDBService == null) ? new Mock<ILeaderboardDBService>().Object: mockLeaderboardDBService!.Object;
            IAccountDBService accountDBService = (mockAccountDBService == null) ? new Mock<IAccountDBService>().Object : mockAccountDBService!.Object;
            IFileHelper fileHelper = (mockFileHelper == null) ? new Mock<IFileHelper>().Object : mockFileHelper!.Object;
            IFileDBService fileDBService = (mockFileDBService == null) ? new  Mock<IFileDBService>().Object : mockFileDBService!.Object;

            return new InstanceService(instanceDBService,leaderboardDBService,accountDBService, fileHelper, fileDBService);
        }
    }
}
