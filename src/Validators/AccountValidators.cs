using cst_back.Services;
using FluentValidation;

namespace cst_back.Validators
{
    public class UsernameValidator : AbstractValidator<string>
    {
        public UsernameValidator() {
            RuleFor(x => x)
                .MinimumLength(ConstantsValidators.USERNAME_MIN_LENGTH)
                .MaximumLength(ConstantsValidators.USERNAME_MAX_LENGTH)
                .WithMessage("Wrong username length");
        }
    }
    public class PasswordValidator : AbstractValidator<string>
    {
        public PasswordValidator()
        {
            RuleFor(x => x)
                .MinimumLength(ConstantsValidators.PASSWORD_MIN_LENGTH)
                .WithMessage("Wrong password length");
        }
    }

    public class CreateAccountValidator : AbstractValidator<CreateAccountRequest>
    {
        public CreateAccountValidator() 
        {
            RuleFor(x => x.Username)
                .SetValidator(new UsernameValidator());
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Field does not match email adress");
            RuleFor(x => x.Password)
                .SetValidator(new PasswordValidator());
            RuleFor(x => x.ConfPassword)
                .Equal(x => x.Password)
                .WithMessage("Passwords doesn't match");
        }
    }

    public class ConnectValidator : AbstractValidator<ConnectRequest>
    {
        public ConnectValidator()
        {
            RuleFor(x => x.Username)
                .SetValidator(new UsernameValidator());
            RuleFor(x => x.Password)
                .SetValidator(new PasswordValidator());
        }
    }
}
