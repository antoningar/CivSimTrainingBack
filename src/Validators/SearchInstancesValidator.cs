using cst_back.Protos;
using FluentValidation;

namespace cst_back.Validators
{
    public class SearchInstancesValidator : AbstractValidator<SearchInstancesRequest>
    {
        public SearchInstancesValidator()
        {
            RuleFor(x => x.Search)
                .Matches("^[a-zA-Z0-9]*$")
                .WithMessage("Search must be alphanumeric");
        }
    }
}
