using cst_back;
using Grpc.Core;

namespace cst_back.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;

        public AuthService(ILogger<AuthService> logger)
        {
            _logger = logger;
        }

        public Task<CreateResponse> CreateAccount(CreateRequest request)
        {
            throw new RpcException(new Status(StatusCode.Unimplemented, ""));
            /*
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request
            });
            */
        }
    }
}
