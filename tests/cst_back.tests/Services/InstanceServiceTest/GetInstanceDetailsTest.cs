using cst_back.DBServices;
using cst_back.Models;
using cst_back.Protos;
using cst_back.Services;
using Grpc.Core;
using MongoDB.Driver;
using Moq;

namespace cst_back.tests.Services.InstanceServiceTest
{
    public class GetInstanceDetailsTest
    {

        ServerCallContext context = Helper.GetServerCallContext(nameof(IInstanceService.GetInstancesDetails));

        [Fact]
        public async Task GetInstancesDetails_ShouldDealWithMogoExceptions()
        {
            InstancesDetailsRequest request = new()
            {
                Id = "12",
            };

            Mock<ILeaderboardDBService> mockLeaderboardDBService = new();
            Mock<IInstanceDBService> mockDbService = new();
            mockDbService.
                Setup(x => x.GetInstance(It.IsAny<string>()))
                .ThrowsAsync(new MongoException(""));

            InstanceService service = new(mockDbService.Object, mockLeaderboardDBService.Object);

            try
            {
                await service.GetInstancesDetails(request, context);
                Assert.Fail("Not exception throwm");
            }
            catch (RpcException ex)
            {
                mockDbService.Verify(x => x.GetInstance(request.Id), Times.Once());
                Assert.Equal(StatusCode.Internal, ex.StatusCode);
            }
        }

        [Fact]
        public async Task GetInstancesDetails_ShouldCheckIfInstanceExist()
        {
            InstancesDetailsRequest request = new()
            {
                Id = "12",
            };

            Mock<ILeaderboardDBService> mockLeaderboardDBService = new();
            Mock<IInstanceDBService> mockDbService = new();
            mockDbService.
                Setup(x => x.GetInstance(It.IsAny<string>()))
                .ReturnsAsync((Instance)null);

            InstanceService service = new(mockDbService.Object, mockLeaderboardDBService.Object);

            try
            {
                await service.GetInstancesDetails(request, context);
                Assert.Fail("Not exception throwm");
            }
            catch (RpcException ex)
            {
                mockDbService.Verify(x=> x.GetInstance(request.Id), Times.Once());
                Assert.Equal(StatusCode.NotFound, ex.StatusCode);
            }
        }

        [Fact]
        public async Task GetInstancesDetails_ShouldReturnReponseWithEmptyLeaderboard()
        {
            InstancesDetailsRequest request = new()
            {
                Id = "12",
            };

            Mock<IInstanceDBService> mockDbService = new();
            Mock<ILeaderboardDBService> mockLeaderboardDBService = new();
            mockDbService.
                Setup(x => x.GetInstance(It.IsAny<string>()))
                .ReturnsAsync(Helper.GetInstance());
            mockLeaderboardDBService.
                Setup(x => x.GetLeaderboard(It.IsAny<string>()))
                .ReturnsAsync((Models.Leaderboard)null);

            InstanceService service = new(mockDbService.Object, mockLeaderboardDBService.Object);

            InstancesDetailsResponse response = await service.GetInstancesDetails(request, context);

            mockLeaderboardDBService.Verify(x => x.GetLeaderboard(request.Id), Times.Once());
            mockDbService.Verify(x => x.GetInstance(request.Id), Times.Once());
            Assert.Null(response.Leaderboard);
        }

        [Fact]
        public async Task GetInstancesDetails_ShouldReturnCompleteResponse()
        {
            InstancesDetailsRequest request = new()
            {
                Id = "12",
            };

            Mock<IInstanceDBService> mockDbService = new();
            mockDbService.
                Setup(x => x.GetInstance(It.IsAny<string>()))
                .ReturnsAsync(Helper.GetInstance());
            Mock<ILeaderboardDBService> mockLeaderboardDBService = new();
            mockLeaderboardDBService.
                Setup(x => x.GetLeaderboard(It.IsAny<string>()))
                .ReturnsAsync(Helper.GetLeaderBoard());

            InstanceService service = new(mockDbService.Object, mockLeaderboardDBService.Object);

            InstancesDetailsResponse response = await service.GetInstancesDetails(request, context);

            mockLeaderboardDBService.Verify(x => x.GetLeaderboard(request.Id), Times.Once());
            mockDbService.Verify(x => x.GetInstance(request.Id), Times.Once());
            Assert.NotNull(response.Leaderboard);
        }
    }
}
