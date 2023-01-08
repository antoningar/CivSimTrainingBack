using cst_back.DBServices;
using cst_back.Helpers;
using cst_back.Models;
using FluentValidation;
using FluentValidation.Results;
using Grpc.Core;

namespace cst_back.Services
{
    public class AuthService : Auth.AuthBase, IAuthService
    {
        private readonly IValidator<CreateAccountRequest> _createAccountValidator;
        private readonly IValidator<ConnectRequest> _connectAccountValidator;
        private readonly IAccountDBService _accountDBService;
        private readonly ICryptoHelper _cryptoHelper;

        public AuthService(
            IValidator<CreateAccountRequest> createAccountValidator,
            IValidator<ConnectRequest> connectValidator,
            IAccountDBService accountDBService,
            ICryptoHelper cryptoHelper)
        {
            _createAccountValidator = createAccountValidator;
            _connectAccountValidator = connectValidator;
            _accountDBService = accountDBService;
            _cryptoHelper = cryptoHelper;
        }

        private async Task CheckCreateConditions(CreateAccountRequest request)
        {
            ValidationResult isValid = _createAccountValidator.Validate(request);
            if (!isValid.IsValid)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, isValid.Errors[0].ErrorMessage));
            }

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

            Account account = new()
            {
                Username = request.Username,
                Email = request.Email,
                Password = _cryptoHelper.Hash(request.Password),
            };

            int? accountId = await _accountDBService.InsertAccountAsync(account);
            return new CreateAccountResponse()
            {
                Id = (int)accountId,
            };
        }

        public override async Task<ConnectResponse> Connect(ConnectRequest request, ServerCallContext context)
        {
            checkConnectConditions(request);

            Account? account = await _accountDBService.GetAccountByUsernameAsync(request.Username);
            if (account == null || account!.AccountId == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Account not found"));
            }

            if (!_cryptoHelper.Verify(request.Password, account!.Password!))
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Account not found"));
            }

            return new ConnectResponse()
            {
                Id = (int)account!.AccountId!
            };
        }

        private void checkConnectConditions(ConnectRequest request)
        {
            ValidationResult isValid = _connectAccountValidator.Validate(request);
            if (!isValid.IsValid)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, isValid.Errors[0].ErrorMessage));
            }
        }
    }
}
