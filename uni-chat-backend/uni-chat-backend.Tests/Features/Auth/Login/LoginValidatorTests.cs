using FluentValidation.TestHelper;
using uni_chat_backend.Features.Auth.Login;
using uni_chat_backend.Features.Auth.Login.Validators;

namespace uni_chat_backend.Tests.Features.Auth.Login;

public class LoginValidatorTests
{
    private readonly LoginValidator _validator = new();

    [Fact]
    public void Should_pass_when_phone_is_valid_international_format()
    {
        var command = new LoginCommand("+573001234567");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_fail_when_phone_is_empty()
    {
        var command = new LoginCommand("");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage("El teléfono es obligatorio");
    }

    [Fact]
    public void Should_fail_when_phone_is_not_international_format()
    {
        var command = new LoginCommand("573001234567");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage("El teléfono debe estar en formato internacional, ejemplo: +573001234567");
    }
}
