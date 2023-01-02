using Castle.Core.Logging;
using cst_back.DBServices;
using cst_back.Models;
using cst_back.Protos;
using cst_back.Services;
using cst_back.Validators;
using Grpc.AspNetCore.Server.Model;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using static Google.Protobuf.WellKnownTypes.Field.Types;

namespace cst_back.tests.Services
{
    public class InstanceServiceTest
    {
        [Theory]
        [InlineData("ok")]
        [InlineData("letsgo")]

        public async Task GetInstances_ShouldCheckParam(string filter)
        {
            var context = Helper.GetServerCallContext(nameof(IInstanceService.GetInstances));
            InstancesRequest request = new() { Filter = filter };

            Mock<IInstanceDBService> mockInstanceDBService = new();
            Mock<IServerStreamWriter<InstancesResponse>> mockStreamWriter = new();

            RPCInstance.RPCInstanceBase rpcInstance = new InstanceService(new GetInstancesValidator(), mockInstanceDBService.Object);

            try
            {
                await rpcInstance.GetInstances(request, mockStreamWriter.Object, context);
                Assert.Fail("No exception");
            }
            catch(Exception ex)
            {
                Assert.IsType<RpcException>(ex);
            }
        }

        [Fact]
        public async Task GetInstances_ErrorOccuredWithDB()
        {
            var context = Helper.GetServerCallContext(nameof(IInstanceService.GetInstances));
            InstancesRequest request = new() { Filter = "" };

            Mock<IInstanceDBService> mockInstanceDBService = new();
            mockInstanceDBService
                .Setup(x => x.GetInstances(It.IsAny<string>()))
                .ThrowsAsync(new MongoException("error"));
            Mock<IServerStreamWriter<InstancesResponse>> mockStreamWriter = new();

            RPCInstance.RPCInstanceBase rpcInstance = new InstanceService(new GetInstancesValidator(), mockInstanceDBService.Object);

            try
            {
                await rpcInstance.GetInstances(request, mockStreamWriter.Object, context);
                Assert.Fail("No exception");
            }
            catch (Exception ex)
            {
                Assert.IsType<RpcException>(ex);
            }
        }

        [Fact]
        public async Task GetInstances_ShouldReturnResponse()
        {
            var context = Helper.GetServerCallContext(nameof(IInstanceService.GetInstances));
            InstancesRequest request = new() { Filter = "" };

            Mock<IServerStreamWriter<InstancesResponse>> mockStreamWriter = new();
            mockStreamWriter.Setup(x => x.WriteAsync(It.IsAny<InstancesResponse>()));

            Mock<IInstanceDBService> mockInstanceDBService = new();
            mockInstanceDBService
                .Setup(x => x.GetInstances(It.IsAny<string>()))
                .ReturnsAsync(new List<Instance>() { 
                    new Instance() {
                        Civilization= "Civilization",
                        Goal="Goal",
                        Map="Map"
                    }
                });

            RPCInstance.RPCInstanceBase rpcInstance = new InstanceService(new GetInstancesValidator(), mockInstanceDBService.Object);

            await rpcInstance.GetInstances(request, mockStreamWriter.Object, context);

            mockStreamWriter.Verify(s => s.WriteAsync(It.IsAny<InstancesResponse>()), Times.Once());
        }
    }
}
