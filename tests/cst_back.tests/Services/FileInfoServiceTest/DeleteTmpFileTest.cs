using cst_back.DBServices;
using cst_back.Helpers;
using cst_back.Models;
using cst_back.Protos;
using cst_back.Services;
using Grpc.Core;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cst_back.tests.Services.FileInfoServiceTest
{
    public class DeleteTmpFileTest
    {
        ServerCallContext context = Helper.GetServerCallContext(nameof(IFileInfoService.DeleteTmpFiles));

        [Fact]
        public async Task DeleteTmpFileTest_ShouldCheckIfUserExist()
        {
            DeleteTmpFilesRequest request = new()
            {
                Username  = "fakeUsername"
            };
            
            Mock<IAccountDBService> mockAccountDBService  = new();
            mockAccountDBService
                .Setup(x => x.GetAccountByUsernameAsync(It.Is<string>(x => x == request.Username)))
                .ReturnsAsync((Account)null);

            FileInfoService service = Helper.GetFileInfoService(mockAccountDBService: mockAccountDBService);

            try
            {
                await service.DeleteTmpFiles(request,  context);
                Assert.Fail("No exception thrown");
            }
            catch (RpcException ex)
            {
                mockAccountDBService.Verify(x => x.GetAccountByUsernameAsync(It.Is<string>(x => x == request.Username)), Times.Once());
                Assert.Equal(StatusCode.NotFound, ex.StatusCode);
            }
        }
        [Fact]
        public async Task DeleteTmpFileTest_ShouldDealWithMongoException()
        {
            DeleteTmpFilesRequest request = new()
            {
                Username = "fakeUsername"
            };

            Mock<IAccountDBService> mockAccountDBService = new();
            mockAccountDBService
                .Setup(x => x.GetAccountByUsernameAsync(It.Is<string>(x => x == request.Username)))
                .ThrowsAsync(new MongoException(""));

            FileInfoService service = Helper.GetFileInfoService(mockAccountDBService: mockAccountDBService);

            try
            {
                await service.DeleteTmpFiles(request, context);
                Assert.Fail("No exception thrown");
            }
            catch (RpcException ex)
            {
                mockAccountDBService.Verify(x => x.GetAccountByUsernameAsync(It.Is<string>(x => x == request.Username)), Times.Once());
                Assert.Equal(StatusCode.Internal, ex.StatusCode);
            }
        }
        [Fact]
        public async Task DeleteTmpFileTest_ShouldDeleteFiles()
        {
            DeleteTmpFilesRequest request = new()
            {
                Username = "fakeUsername"
            };

            Mock<IAccountDBService> mockAccountDBService = new();
            mockAccountDBService
                .Setup(x => x.GetAccountByUsernameAsync(It.Is<string>(x => x == request.Username)))
                .ReturnsAsync(new Account() { Username = "123" });
            Mock<IFileHelper> mockFileHelper = new();
            mockFileHelper
                .Setup(x => x.DeleteTmpFileByUsername(It.Is<string>(x => x == request.Username)))
                .Returns(1);

            FileInfoService service = Helper.GetFileInfoService(mockAccountDBService: mockAccountDBService, mockFileHelper: mockFileHelper);

            await service.DeleteTmpFiles(request, context);

            mockAccountDBService.Verify(x => x.GetAccountByUsernameAsync(It.Is<string>(x => x == request.Username)), Times.Once());
            mockFileHelper.Verify(x => x.DeleteTmpFileByUsername(It.Is<string>(x => x == request.Username)), Times.Once());
        }
    }
}
