using cst_back.Models;
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
    }
}
