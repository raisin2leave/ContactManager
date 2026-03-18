using AppCore.Dto;
using FluentValidation;

namespace AppCore.Validators.Shared;

public class AddressDtoValidator : AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Ulica jest wymagana.")
            .MaximumLength(200);

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("Miasto jest wymagane.")
            .MaximumLength(100);

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Kod pocztowy jest wymagany.")
            .Matches(@"^\d{2}-\d{3}$").WithMessage("Kod pocztowy musi być w formacie xx-xxx.");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Kraj jest wymagany.")
            .MaximumLength(100);

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Nieprawidłowy typ adresu.");
    }
}