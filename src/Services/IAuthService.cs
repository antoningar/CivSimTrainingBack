using Grpc.Core;

namespace cst_back.Services
{
    public interface IAuthService
    {
        public Task<CreateAccountResponse> CreateAccount(CreateAccountRequest request, ServerCallContext context);
    }
}
