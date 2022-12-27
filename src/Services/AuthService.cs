using Grpc.Core;

namespace cst_back.Services
{
    public class AuthService : Auth.AuthBase, IAuthService
    {
        private readonly ILogger<AuthService> _logger;

        public AuthService(ILogger<AuthService> logger)
        {
            _logger = logger;
        }

        public override Task<CreateResponse> CreateAccount(CreateRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Auth/CreateAccount request");
            return Task.FromResult(new CreateResponse()
            {
                Id = "5"
            });
        }
    }
}
