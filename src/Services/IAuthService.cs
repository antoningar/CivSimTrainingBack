using Grpc.Core;

namespace cst_back.Services
{
    public interface IAuthService
    {
        public Task<CreateResponse> CreateAccount(CreateRequest request);
    }
}
