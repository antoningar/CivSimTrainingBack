using FluentValidation;
using FluentValidation.Results;
using Grpc.Core;

namespace cst_back.Services
{
    public class AuthService : Auth.AuthBase, IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly IValidator<CreateAccountRequest> _createAccountValidator;
        private readonly IAccountDBService _accountDBService;

        public AuthService(ILogger<AuthService> logger, IValidator<CreateAccountRequest> createAccountValidator, IAccountDBService accountDBService)
        {
            _logger = logger;
            _createAccountValidator = createAccountValidator;
            _accountDBService = accountDBService;
        }

        private async Task CheckCreateConditions(CreateAccountRequest request)
        {
            ValidationResult isValid = _createAccountValidator.Validate(request);
            if (!isValid.IsValid)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, isValid.Errors[0].ErrorMessage));
            }

            var byEmail = await _accountDBService.GetAccountByEmailAsync(request.Email);
            var byUsername = await _accountDBService.GetAccountByUsernameAsync(request.Username);
            if (await _accountDBService.GetAccountByEmailAsync(request.Email) != null)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, "email still in base"));
            }
            if (await _accountDBService.GetAccountByUsernameAsync(request.Username) != null)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, "usernaem stil in base"));
            }
        }

        public override async Task<CreateAccountResponse> CreateAccount(CreateAccountRequest request, ServerCallContext context)
        {
            await CheckCreateConditions(request);

            return await Task.FromResult(new CreateAccountResponse()
            {
                Id = "5"
            });
        }
    }
}
