using cst_back.Protos;
using FluentValidation;

namespace cst_back.Validators
{
    public class GetInstancesValidator : AbstractValidator<InstancesRequest>
    {
        public GetInstancesValidator()
        {
            RuleFor(x => x.Filter)
                .Must(x => ConstantsValidators.INSTANCES_FILTER.Contains(x))
                .WithMessage("Filter type not found");
        }
    }
}
