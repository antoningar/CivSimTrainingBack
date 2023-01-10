using cst_back.DBServices;
using cst_back.Models;
using cst_back.Protos;
using cst_back.Services;
using cst_back.Validators;
using Grpc.Core;
using MongoDB.Driver;
using Moq;

namespace cst_back.tests.Services.InstanceServiceTest
{
    public class GetInstancesTest
    {
        [Theory]
        [InlineData("ok", "")]
        [InlineData("letsgo", "")]
        [InlineData("Goal", "")]

        public async Task GetInstances_ShouldCheckParam(string type, string value)
        {
            var context = Helper.GetServerCallContext(nameof(IInstanceService.GetInstances));
            InstancesRequest request = new()
            {
                Filter = new()
                {
                    Type = type,
                    Value = value
                }
            };

            Mock<IInstanceDBService> mockInstanceDBService = new();
            Mock<ILeaderboardDBService> mockLeaderboardDBService = new();
            Mock<IServerStreamWriter<InstancesResponse>> mockStreamWriter = new();

            RPCInstance.RPCInstanceBase rpcInstance = new InstanceService(mockInstanceDBService.Object, mockLeaderboardDBService.Object);

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
        public async Task GetInstances_ErrorOccuredWithDB()
        {
            var context = Helper.GetServerCallContext(nameof(IInstanceService.GetInstances));
            InstancesRequest request = new()
            {
                Filter = new()
                {
                    Type = ""
                }
            };

            Mock<ILeaderboardDBService> mockLeaderboardDBService = new();
            Mock<IInstanceDBService> mockInstanceDBService = new();
            mockInstanceDBService
                .Setup(x => x.GetInstances(It.IsAny<Filter>()))
                .ThrowsAsync(new MongoException("error"));
            Mock<IServerStreamWriter<InstancesResponse>> mockStreamWriter = new();

            RPCInstance.RPCInstanceBase rpcInstance = new InstanceService(mockInstanceDBService.Object, mockLeaderboardDBService.Object);

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
            InstancesRequest request = new()
            {
                Filter = new()
                {
                    Type = "Goal",
                    Value = "Science Victory"
                }
            };

            Mock<IServerStreamWriter<InstancesResponse>> mockStreamWriter = new();
            mockStreamWriter.Setup(x => x.WriteAsync(It.IsAny<InstancesResponse>()));

            Mock<ILeaderboardDBService> mockLeaderboardDBService = new();
            Mock<IInstanceDBService> mockInstanceDBService = new();
            mockInstanceDBService
                .Setup(x => x.GetInstances(It.IsAny<Filter>()))
                .ReturnsAsync(new List<Instance>() {
                    new Instance()
                    {
                        Id = "1",
                        Civilization = "Civilization",
                        Goal = "Goal",
                        Map = "Map"
                    }
                });

            RPCInstance.RPCInstanceBase rpcInstance = new InstanceService(mockInstanceDBService.Object, mockLeaderboardDBService.Object);

            await rpcInstance.GetInstances(request, mockStreamWriter.Object, context);

            mockStreamWriter.Verify(s => s.WriteAsync(It.IsAny<InstancesResponse>()), Times.Once());
        }

        [Theory]
        [InlineData("Scientific Victory")]
        public async Task GetInstances_ShouldReturnFilteredResponseByGoal(string value)
        {
            var context = Helper.GetServerCallContext(nameof(IInstanceService.GetInstances));
            InstancesRequest request = new()
            {
                Filter = new()
                {
                    Type = "Goal",
                    Value = value
                }
            };

            Mock<ILeaderboardDBService> mockLeaderboardDBService = new();
            Mock<IInstanceDBService> mockInstanceDBService = new();
            mockInstanceDBService
                .Setup(x => x.GetInstances(It.IsAny<Filter>()))
                .ReturnsAsync(Helper.GetListInstance());
            Mock<IServerStreamWriter<InstancesResponse>> mockStreamWriter = new();

            RPCInstance.RPCInstanceBase rpcInstance = new InstanceService(mockInstanceDBService.Object, mockLeaderboardDBService.Object);

            await rpcInstance.GetInstances(request, mockStreamWriter.Object, context);

            mockStreamWriter.Verify(x => x.WriteAsync(It.Is<InstancesResponse>(instance => instance.Goal == value)), Times.AtLeastOnce());
        }

        [Theory]
        [InlineData("Seven Seas")]
        public async Task GetInstances_ShouldReturnFilteredResponseByMap(string value)
        {
            var context = Helper.GetServerCallContext(nameof(IInstanceService.GetInstances));
            InstancesRequest request = new()
            {
                Filter = new()
                {
                    Type = "Map",
                    Value = value
                }
            };

            Mock<ILeaderboardDBService> mockLeaderboardDBService = new();
            Mock<IInstanceDBService> mockInstanceDBService = new();
            mockInstanceDBService
                .Setup(x => x.GetInstances(It.IsAny<Filter>()))
                .ReturnsAsync(Helper.GetListInstance());
            Mock<IServerStreamWriter<InstancesResponse>> mockStreamWriter = new();

            RPCInstance.RPCInstanceBase rpcInstance = new InstanceService(mockInstanceDBService.Object, mockLeaderboardDBService.Object);

            await rpcInstance.GetInstances(request, mockStreamWriter.Object, context);

            mockStreamWriter.Verify(x => x.WriteAsync(It.Is<InstancesResponse>(instance => instance.Map == value)), Times.AtLeastOnce());
        }

        [Theory]
        [InlineData("Dido")]
        public async Task GetInstances_ShouldReturnFilteredResponseByCivilization(string value)
        {
            var context = Helper.GetServerCallContext(nameof(IInstanceService.GetInstances));
            InstancesRequest request = new()
            {
                Filter = new()
                {
                    Type = "Civilization",
                    Value = value
                }
            };

            Mock<ILeaderboardDBService> mockLeaderboardDBService = new();
            Mock<IInstanceDBService> mockInstanceDBService = new();
            mockInstanceDBService
                .Setup(x => x.GetInstances(It.IsAny<Filter>()))
                .ReturnsAsync(Helper.GetListInstance());
            Mock<IServerStreamWriter<InstancesResponse>> mockStreamWriter = new();

            RPCInstance.RPCInstanceBase rpcInstance = new InstanceService(mockInstanceDBService.Object, mockLeaderboardDBService.Object);

            await rpcInstance.GetInstances(request, mockStreamWriter.Object, context);

            mockStreamWriter.Verify(x => x.WriteAsync(It.Is<InstancesResponse>(instance => instance.Civilization == value)), Times.AtLeastOnce());
        }
    }
}
