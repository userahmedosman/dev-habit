using FluentValidation;

namespace DevHabit.Api.DTO.Tags;

public sealed class CreateTagDtoValidator: AbstractValidator<CreateTagDto>
{
    public CreateTagDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(15);
        RuleFor(x => x.Description).MaximumLength(50).MinimumLength(10).When(x => x.Description is not null);
    }
}
