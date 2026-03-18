using AppCore.Dto;
using FluentValidation;

namespace AppCore.Validators;

public class UpdatePersonDtoValidator : AbstractValidator<UpdatePersonDto>
{
    public UpdatePersonDtoValidator()
    {
        RuleFor(x => x.FirstName).MaximumLength(100).Matches(@"^[\p{L}\s\-]+$").When(x => x.FirstName != null);
        RuleFor(x => x.LastName).MaximumLength(200).Matches(@"^[\p{L}\s\-]+$").When(x => x.LastName != null);
        RuleFor(x => x.Email).EmailAddress().When(x => x.Email != null);
    }
}