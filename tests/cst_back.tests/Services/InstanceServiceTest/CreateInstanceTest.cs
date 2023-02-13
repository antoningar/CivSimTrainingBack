using cst_back.DBServices;
using cst_back.Helpers;
using cst_back.Models;
using cst_back.Protos;
using cst_back.Services;
using Grpc.Core;
using MongoDB.Driver;
using Moq;

namespace cst_back.tests.Services.InstanceServiceTest
{
    public class CreateInstanceTest
    {
        ServerCallContext context = Helper.GetServerCallContext(nameof(IInstanceService.CreateInstance));

        [Fact]
        public async void CreateInstance_ShouldDealWithMongoException()
        {
            CreateInstanceRequest request = new()
            {
                Username = "userId",
                Goal = "Max Science turn 50"
            };

            Mock<IAccountDBService> mockAccountDbService = new();
            mockAccountDbService
                .Setup(x => x.GetAccountByUsernameAsync(It.Is<string>(x => x == request.Username)))
                .ThrowsAsync(new MongoException(""));

            InstanceService service = Helper.GetInstanceService(mockAccountDBService: mockAccountDbService);

            try
            {
                await service.CreateInstance(request, context);
                Assert.Fail("No exception thrown");
            }
            catch (RpcException ex)
            {
                mockAccountDbService.Verify(x => x.GetAccountByUsernameAsync(It.Is<string>(x => x == request.Username)), Times.Once);
                Assert.Equal(StatusCode.Internal, ex.StatusCode);
            }
        }

        [Fact]
        public async void CreateInstance_ShouldCheckIfUserExist()
        {
            CreateInstanceRequest request = new()
            {
                Username = "userId",
                Goal = "Max Science turn 50"
            };

            Mock<IAccountDBService> mockAccountDbService = new();
            mockAccountDbService
                .Setup(x => x.GetAccountByUsernameAsync(It.Is<string>(x => x == request.Username)))
                .ReturnsAsync((Account)null);

            InstanceService service = Helper.GetInstanceService(mockAccountDBService: mockAccountDbService);

            try
            {
                await service.CreateInstance(request, context);
                Assert.Fail("No exception thrown");
            }
            catch (RpcException ex)
            {
                mockAccountDbService.Verify(x => x.GetAccountByUsernameAsync(It.Is<string>(x => x == request.Username)), Times.Once);
                Assert.Equal(StatusCode.FailedPrecondition, ex.StatusCode);
            }
        }

        [Fact]
        public async void CreateInstance_ShouldCheckIfGoalIsValid()
        {
            CreateInstanceRequest request = new()
            {
                Username = "username",
                Goal = "okletsgo"
            };

            Mock<IAccountDBService> mockAccountDbService = new();
            mockAccountDbService
                .Setup(x => x.GetAccountByUsernameAsync(It.Is<string>(x => x == request.Username)))
                .ReturnsAsync(new Account() { Id = "123"});

            InstanceService service = Helper.GetInstanceService(mockAccountDBService: mockAccountDbService);

            try
            {
                await service.CreateInstance(request, context);
                Assert.Fail("No exception thrown");
            }
            catch (RpcException ex)
            {
                mockAccountDbService.Verify(x => x.GetAccountByUsernameAsync(It.Is<string>(x => x == request.Username)), Times.Once);
                Assert.Equal(StatusCode.FailedPrecondition, ex.StatusCode);
            }
        }

        [Fact]
        public async void CreateInstance_ShouldCheckIfFilesExists()
        {
            CreateInstanceRequest request = new()
            {
                Username = "username",
                Goal = "Max science turn 60"
            };

            Mock<IAccountDBService> mockAccountDbService = new();
            mockAccountDbService
                .Setup(x => x.GetAccountByUsernameAsync(It.Is<string>(x => x == request.Username)))
                .ReturnsAsync(new Account() { Id = "123" });
            Mock<IFileHelper> mockFileHelper = new();
            mockFileHelper
                .Setup(x => x.IsInstanceTmpFilesExist(It.Is<string>(x => x == request.Username)))
                .Returns(false);

            InstanceService service = Helper.GetInstanceService(mockAccountDBService: mockAccountDbService, mockFileHelper: mockFileHelper);

            try
            {
                await service.CreateInstance(request, context);
                Assert.Fail("No exception thrown");
            }
            catch (RpcException ex)
            {
                mockAccountDbService.Verify(x => x.GetAccountByUsernameAsync(It.Is<string>(x => x == request.Username)), Times.Once);
                mockFileHelper.Verify(x => x.IsInstanceTmpFilesExist(It.Is<string>(x => x == request.Username)), Times.Once);
                Assert.Equal(StatusCode.FailedPrecondition, ex.StatusCode);
            }
        }

        [Fact]
        public async void CreateInstance_ShouldInsertInstances()
        {
            CreateInstanceRequest request = new()
            {
                Username = "userId",
                Goal = "Max science turn 60"
            };

            Instance instanceResponse = Helper.GetInstance();

            Mock<IAccountDBService> mockAccountDbService = new();
            mockAccountDbService
                .Setup(x => x.GetAccountByUsernameAsync(It.Is<string>(x => x == request.Username)))
                .ReturnsAsync(new Account() { Id = "123" });
            Mock<IFileHelper> mockFileHelper = new();
            mockFileHelper
                .Setup(x => x.IsInstanceTmpFilesExist(It.Is<string>(x => x == request.Username)))
                .Returns(true);
            mockFileHelper
                .Setup(x => x.GetInstanceFromFile(It.Is<string>(x => x == request.Username)))
                .ReturnsAsync(instanceResponse);
            Mock<IFileDBService> mockFileDBService = new();
            mockFileDBService
                .Setup(x => x.SaveFile(It.Is<string>(x  => x == request.Username), It.IsAny<string>()));
            Mock<IInstanceDBService> mockInstanceDBService = new();
            mockInstanceDBService
                .Setup(x => x.InsertInstance(It.IsAny<Instance>()))
                .ReturnsAsync(new Instance() { Id = "12" });

            InstanceService service = Helper.GetInstanceService(mockAccountDBService: mockAccountDbService, mockFileHelper: mockFileHelper, mockFileDBService: mockFileDBService, mockInstanceDBService: mockInstanceDBService);
            
            await service.CreateInstance(request, context);

            mockAccountDbService.Verify(x => x.GetAccountByUsernameAsync(It.Is<string>(x => x == request.Username)), Times.AtLeastOnce);
            mockFileHelper.Verify(x => x.IsInstanceTmpFilesExist(It.Is<string>(x => x == request.Username)), Times.AtLeastOnce);
            mockInstanceDBService.Verify(x => x.InsertInstance(It.IsAny<Instance>()), Times.AtLeastOnce);
            mockFileDBService.Verify(x => x.SaveFile(It.Is<string>(x => x == request.Username), It.IsAny<string>()));
        }

        [Fact]
        public async void CreateInstance_ShouldCreateLeaderboard()
        {
            CreateInstanceRequest request = new()
            {
                Username = "userId",
                Goal = "Max science turn 60"
            };

            Instance instanceResponse = Helper.GetInstance();

            Mock<IAccountDBService> mockAccountDbService = new();
            mockAccountDbService
                .Setup(x => x.GetAccountByUsernameAsync(It.Is<string>(x => x == request.Username)))
                .ReturnsAsync(new Account() { Id = "123" });
            Mock<IFileHelper> mockFileHelper = new();
            mockFileHelper
                .Setup(x => x.IsInstanceTmpFilesExist(It.Is<string>(x => x == request.Username)))
                .Returns(true);
            mockFileHelper
                .Setup(x => x.GetInstanceFromFile(It.Is<string>(x => x == request.Username)))
                .ReturnsAsync(instanceResponse);
            Mock<IFileDBService> mockFileDBService = new();
            mockFileDBService
                .Setup(x => x.SaveFile(It.Is<string>(x => x == request.Username), It.IsAny<string>()));
            Mock<IInstanceDBService> mockInstanceDBService = new();
            mockInstanceDBService
                .Setup(x => x.InsertInstance(It.IsAny<Instance>()))
                .ReturnsAsync(new Instance() { Id = "12" });

            InstanceService service = Helper.GetInstanceService(mockAccountDBService: mockAccountDbService, mockFileHelper: mockFileHelper, mockFileDBService: mockFileDBService, mockInstanceDBService: mockInstanceDBService);

            await service.CreateInstance(request, context);

            mockAccountDbService.Verify(x => x.GetAccountByUsernameAsync(It.Is<string>(x => x == request.Username)), Times.AtLeastOnce);
            mockFileHelper.Verify(x => x.IsInstanceTmpFilesExist(It.Is<string>(x => x == request.Username)), Times.AtLeastOnce);
            mockInstanceDBService.Verify(x => x.InsertInstance(It.IsAny<Instance>()), Times.AtLeastOnce);
            mockFileDBService.Verify(x => x.SaveFile(It.Is<string>(x => x == request.Username), It.IsAny<string>()));
        }
    }
}
