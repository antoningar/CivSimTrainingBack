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
            return Task.FromResult(new CreateResponse()
            {
                Id = "4"
            });
        }
    }
}
