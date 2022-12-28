using cst_back.Services;
using Grpc.Core;
using Grpc.Core.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
