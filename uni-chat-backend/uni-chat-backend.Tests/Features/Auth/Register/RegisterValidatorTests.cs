using FluentValidation.TestHelper;
using uni_chat_backend.Features.Auth.Register;
using uni_chat_backend.Features.Auth.Register.Validators;

namespace uni_chat_backend.Tests.Features.Auth.Register;

public class RegisterValidatorTests
{
    private readonly RegisterValidator _validator = new();

    private static RegisterCommand ValidCommand() =>
        new("usuario", "+573001234567", "test@example.com");

    [Fact]
    public void Should_pass_when_all_fields_are_valid()
    {
        var result = _validator.TestValidate(ValidCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_fail_when_email_is_invalid()
    {
        var command = ValidCommand() with { Email = "not-an-email" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("El formato del email no es válido");
    }

    [Fact]
    public void Should_fail_when_phone_is_invalid()
    {
        var command = ValidCommand() with { Phone = "3001234567" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage("El teléfono debe estar en formato internacional, ejemplo: +573001234567");
    }

    [Fact]
    public void Should_fail_when_username_is_too_short()
    {
        var command = ValidCommand() with { Username = "ab" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("El username debe tener al menos 3 caracteres");
    }
}
