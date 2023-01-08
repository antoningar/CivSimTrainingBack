using cst_back.Protos;
using FluentValidation;

namespace cst_back.Validators
{
    public class GetInstancesValidator : AbstractValidator<InstancesRequest>
    {
        public GetInstancesValidator()
        {
            RuleFor(x => x.Filter.Type)
                .Must(x => ConstantsValidators.INSTANCES_FILTER.Contains(x))
                .WithMessage("Filter type not found");
            RuleFor(x => x.Filter.Value)
                .Must(x => !string.IsNullOrEmpty(x))
                .When(x => !string.IsNullOrWhiteSpace(x.Filter.Type));
        }
    }
}
