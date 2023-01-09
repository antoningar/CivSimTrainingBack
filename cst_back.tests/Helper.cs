using cst_back.Models;
using cst_back.Protos;
using Grpc.Core;
using Grpc.Core.Testing;

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
                    Civilization = "Dido",
                    Map = "Seven Seas",
                    Goal = "Scientific Victory"
                },
                new Instance()
                {
                    Civilization = "Dido",
                    Map = "Seven Seas",
                    Goal = "Cultural Victory"
                },
                new Instance()
                {
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
                    Civilization = "Di" + search + "do",
                    Map = "Seven Seas",
                    Goal = "Scientific Victory"
                },
                new Instance()
                {
                    Civilization = "Dido",
                    Map = "Seven " + search + " Seas",
                    Goal = "Cultural Victory"
                },
                new Instance()
                {
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
    }
}
