using Domain.Entity;
using FluentValidation;
using LargeLanguageModelClient.Dto.Model;

namespace Implementation.Validator;

public class LlmModelDtoValidator : AbstractValidator<LlmModelDto>
{
    public LlmModelDtoValidator()
    {
        this.RuleFor(model => model.Id)
            .Must(this.HaveANonEmptyIdentifier)
            .WithMessage("Model-id must have a value");
        
        this.RuleFor(model => model.ProviderName)
            .Must(this.HaveAValidProviderName)
            .WithMessage("providerName is not valid");
    }

    private bool HaveANonEmptyIdentifier(Guid id) =>
        id != Guid.Empty;

    private bool HaveAValidProviderName(string providerName) =>
        Enum.TryParse<LlmProvider>(providerName, out var _);
}
