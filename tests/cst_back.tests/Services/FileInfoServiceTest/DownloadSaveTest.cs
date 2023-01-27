using cst_back.DBServices;
using cst_back.Helpers;
using cst_back.Models;
using cst_back.Protos;
using cst_back.Services;
using Grpc.Core;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cst_back.tests.Services.FileInfoServiceTest
{
    public  class DownloadSaveTest
    {
        ServerCallContext context = Helper.GetServerCallContext(nameof(IFileInfoService.DownloadSave));

        [Fact]
        public async void DownloadSave_ShouldCheckIfInstanceIdExist()
        {
            DownloadSaveRequest request = new()
            {
                InstanceID = "123"
            };

            Mock<IServerStreamWriter<DownloadSaveResponse>> mockStreamWriter = new();
            Mock<IInstanceDBService> mockInstanceDBService = new();
            mockInstanceDBService
                .Setup(x => x.GetInstance(It.Is<string>(x =>  x == request.InstanceID)))
                .ReturnsAsync((Instance)null);

            FileInfoService service = Helper.GetFileInfoService(mockInstanceDBService: mockInstanceDBService);

            try
            {
                await service.DownloadSave(request, mockStreamWriter.Object, context);
                Assert.Fail("No exception Thrown");
            }
            catch (RpcException ex)
            {
                mockInstanceDBService.Verify(x => x.GetInstance(It.Is<string>(x => x == request.InstanceID)), Times.Once());
                Assert.Equal(StatusCode.NotFound, ex.StatusCode);
            }
        }

        [Fact]
        public async void DownloadSave_ShouldDealWithMongoException()
        {
            DownloadSaveRequest request = new()
            {
                InstanceID = "123"
            };

            Mock<IServerStreamWriter<DownloadSaveResponse>> mockStreamWriter = new();
            Mock<IInstanceDBService> mockInstanceDBService = new();
            mockInstanceDBService
                .Setup(x => x.GetInstance(It.Is<string>(x => x == request.InstanceID)))
                .ThrowsAsync(new MongoException(""));

            FileInfoService service = Helper.GetFileInfoService(mockInstanceDBService: mockInstanceDBService);

            try
            {
                await service.DownloadSave(request, mockStreamWriter.Object, context);
                Assert.Fail("No exception Thrown");
            }
            catch (RpcException ex)
            {
                mockInstanceDBService.Verify(x => x.GetInstance(It.Is<string>(x => x == request.InstanceID)), Times.Once());
                Assert.Equal(StatusCode.Internal, ex.StatusCode);
            }
        }

        [Fact]
        public async void DownloadSave_ShouldReturnFile()
        {
            DownloadSaveRequest request = new()
            {
                InstanceID = "123"
            };

            Mock<IServerStreamWriter<DownloadSaveResponse>> mockStreamWriter = new();
            Mock<IInstanceDBService> mockInstanceDBService = new();
            mockInstanceDBService
                .Setup(x => x.GetInstance(It.Is<string>(x => x == request.InstanceID)))
                .ReturnsAsync(Helper.GetInstance());
            Mock<IFileDBService> mockFileDBService= new();
            mockFileDBService
                .Setup(x => x.GetFileIdByInstanceId(It.Is<string>(x => x == request.InstanceID)))
                .ReturnsAsync(new ObjectId("63ac7ce23f69376723a4aca4"));
            mockFileDBService
                .Setup(x => x.DownloadFile(It.IsAny<ObjectId>(), It.IsAny<IServerStreamWriter<DownloadSaveResponse>>()));


            FileInfoService service = Helper.GetFileInfoService(mockInstanceDBService: mockInstanceDBService, mockFileDBService: mockFileDBService);

            await service.DownloadSave(request, mockStreamWriter.Object, context);
            mockInstanceDBService.Verify(x => x.GetInstance(It.Is<string>(x => x == request.InstanceID)), Times.Once());
            mockFileDBService.Verify(x => x.GetFileIdByInstanceId(It.Is<string>(x => x == request.InstanceID)), Times.Once());
            mockFileDBService.Verify(x => x.DownloadFile(It.IsAny<ObjectId>(), It.IsAny<IServerStreamWriter<DownloadSaveResponse>>()), Times.Once());
        }
    }
}
