using FluentValidation;
using FluentValidation.Results;
using Grpc.Core;

namespace cst_back.Services
{
    public class AuthService : Auth.AuthBase, IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly IValidator<CreateAccountRequest> _createAccountValidator;

        public AuthService(ILogger<AuthService> logger, IValidator<CreateAccountRequest> createAccountValidator)
        {
            _logger = logger;
            _createAccountValidator = createAccountValidator;
        }

        public override Task<CreateAccountResponse> CreateAccount(CreateAccountRequest request, ServerCallContext context)
        {
            ValidationResult isValid = _createAccountValidator.Validate(request);
            if (!isValid.IsValid)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, isValid.Errors[0].ErrorMessage));
            }

            return Task.FromResult(new CreateAccountResponse()
            {
                Id = "5"
            });
        }
    }
}
