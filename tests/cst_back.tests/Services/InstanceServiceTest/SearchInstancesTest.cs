using cst_back.DBServices;
using cst_back.Protos;
using cst_back.Services;
using cst_back.Validators;
using Grpc.Core;
using MongoDB.Driver;
using Moq;

namespace cst_back.tests.Services.InstanceServiceTest
{
    public class SearchInstanceTest
    {
        [Theory]
        [InlineData("abcsafg'af")]
        [InlineData("abc*asd")]
        [InlineData("abc|asd")]
        public async Task SearchtInstances_ShouldCheckSearchIsAlphaNum(string search)
        {
            var context = Helper.GetServerCallContext(nameof(IInstanceService.SearchInstances));
            SearchInstancesRequest request = new()
            {
                Search = search
            };

            Mock<IServerStreamWriter<InstancesResponse>> mockStreamWriter = new();

            RPCInstance.RPCInstanceBase rpcInstance = Helper.GetInstanceService();

            try
            {
                await rpcInstance.SearchInstances(request, mockStreamWriter.Object, context);
                Assert.Fail("No exception");
            }
            catch (RpcException ex)
            {
                Assert.Equal(StatusCode.FailedPrecondition, ex.Status.StatusCode);
            }
        }

        [Theory]
        [InlineData("Did")]
        public async Task SearchtInstances_ShouldDealWithMongoException(string search)
        {
            var context = Helper.GetServerCallContext(nameof(IInstanceService.SearchInstances));
            SearchInstancesRequest request = new()
            {
                Search = search
            };

            Mock<IInstanceDBService> mockInstanceDBService = new();
            mockInstanceDBService
                .Setup(x => x.SearchInstances(It.IsAny<string>()))
                .ThrowsAsync(new MongoException(""));
            Mock<IServerStreamWriter<InstancesResponse>> mockStreamWriter = new();

            RPCInstance.RPCInstanceBase rpcInstance = Helper.GetInstanceService(mockInstanceDBService: mockInstanceDBService);

            try
            {
                await rpcInstance.SearchInstances(request, mockStreamWriter.Object, context);
                Assert.Fail("No exception");
            }
            catch (RpcException ex)
            {
                Assert.Equal(StatusCode.Internal, ex.Status.StatusCode);
                mockInstanceDBService.Verify(x => x.SearchInstances(request.Search), Times.Once());
            }
        }

        [Theory]
        [InlineData("Did")]
        public async Task SearchtInstances_ShouldReturnResponse(string search)
        {
            var context = Helper.GetServerCallContext(nameof(IInstanceService.SearchInstances));
            SearchInstancesRequest request = new()
            {
                Search = search
            };

            Mock<IInstanceDBService> mockInstanceDBService = new();
            mockInstanceDBService
                .Setup(x => x.SearchInstances(search))
                .ReturnsAsync(Helper.GetListInstanceBySearch(search));
            Mock<IServerStreamWriter<InstancesResponse>> mockStreamWriter = new();

            RPCInstance.RPCInstanceBase rpcInstance = Helper.GetInstanceService(mockInstanceDBService: mockInstanceDBService);

            await rpcInstance.SearchInstances(request, mockStreamWriter.Object, context);

            mockStreamWriter.Verify(x => x.WriteAsync(It.Is<InstancesResponse>(instance => Helper.InstancesResponseContainsSearch(instance, search))), Times.AtLeastOnce());
        }
    }
}
