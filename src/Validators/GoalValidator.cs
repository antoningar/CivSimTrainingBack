using cst_back.Services;
using FluentValidation;

namespace cst_back.Validators
{
    public class GoalValidator : AbstractValidator<string>
    {
        public GoalValidator() {
            RuleFor(x => x)
                .Must(BeValid)
                .WithMessage("Goasl not found");
        }

        private bool BeValid(string goal)
        {
            foreach (string pattern in ConstantsValidators.GOALS)
            {
                if (goal.ToLower().Contains(pattern.ToLower()))
                {
                    return int.TryParse(goal.Split(" ").Last(), out _);
                }
            }
            return false;
        }
    }
}
