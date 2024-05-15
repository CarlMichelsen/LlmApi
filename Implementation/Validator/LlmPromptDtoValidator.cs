using FluentValidation;
using LargeLanguageModelClient;
using LargeLanguageModelClient.Dto.Prompt;
using LargeLanguageModelClient.Dto.Prompt.Content;

namespace Implementation.Validator;

public class LlmPromptDtoValidator : AbstractValidator<LlmPromptDto>
{
    private static List<string> supportedMediaTypes = new List<string>
    {
        "image/jpeg",
    };

    public LlmPromptDtoValidator()
    {
        this.RuleFor(prompt => prompt.ModelIdentifier)
            .NotEmpty()
            .WithMessage("Prompt model identifier is invalid");

        this.RuleFor(prompt => prompt.Messages)
            .NotEmpty()
            .WithMessage("There are no messages in this prompt");
        
        this.RuleForEach(prompt => prompt.Messages)
            .NotNull();
        
        this.RuleForEach(prompt => prompt.Messages)
            .Must(this.OnlyHaveSupportedImageTypes)
            .WithMessage($"Unsupported media type, supported types: {string.Join(", ", supportedMediaTypes)}");
        
        this.RuleFor(prompt => prompt.Messages)
            .Must(this.StartWithUserMessage)
            .WithMessage("Initial message must be from the user");
        
        this.RuleFor(prompt => prompt.Messages)
            .Must(this.HaveAlternatingRoles)
            .WithMessage("Messages must have alternating roles");
    }

    private bool OnlyHaveSupportedImageTypes(LlmPromptMessageDto message)
    {
        foreach (var content in message.Content)
        {
            if (content.Type == LlmContentType.Image)
            {
                var imageContent = content as LlmImageContent;
                if (!supportedMediaTypes.Contains(imageContent!.MediaType))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private bool StartWithUserMessage(List<LlmPromptMessageDto> messages)
    {
        return messages.FirstOrDefault()?.IsUserMessage ?? false;
    }

    private bool HaveAlternatingRoles(List<LlmPromptMessageDto> messages)
    {
        var role = true;
        foreach (var message in messages)
        {
            if (role == message.IsUserMessage)
            {
                role = !role;
                continue;
            }

            return false;
        }

        return true;
    }
}
