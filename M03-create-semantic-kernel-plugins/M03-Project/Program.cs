using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

public class Program
{
    public static async Task Main(string[] args)
    {
        string filePath = Path.GetFullPath("/home/talha-naeem/Documents/LLM Work/handlebarstemplate/Handlebars/MSLearn-Develop-AI-Agents-with-Azure-OpenAI-and-Semantic-Kernel-SDK/appsettings.json");
        var config = new ConfigurationBuilder()
            .AddJsonFile(filePath)
            .Build();

        // Set your values in appsettings.json
        string modelId = config["modelId"]!;
        string endpoint = config["endpoint"]!;
        string apiKey = config["apiKey"]!;

        // Create a kernel with Azure OpenAI chat completion
        var builder = Kernel.CreateBuilder();
        builder.AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);

        var kernel = builder.Build();
        kernel.Plugins.AddFromType<FlightBookingPlugin>("FlightBooking");

        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        var history = new ChatHistory();
        // history.AddSystemMessage("The year is 2025 and the current month is June");

        // AddUserMessage("Find me a flight to Tokyo on the 19");
        // await GetReply();

        // GetInput();
        // await GetReply();

        // Helper methods defined *inside* Main so they can access local variables
        void AddUserMessage(string message)
        {
            history.AddUserMessage(message);
        }

        async Task GetReply()
        {
            var result = await chatCompletionService.GetChatMessageContentAsync(
                history,
                openAIPromptExecutionSettings,
                kernel // ← this was missing
            );

            if (!string.IsNullOrWhiteSpace(result.Content))
            {
                history.AddAssistantMessage(result.Content);
                Console.WriteLine($"Assistant: {result.Content}");
            }
        }
        history.AddSystemMessage("The year is 2025 and the current month is June");

        AddUserMessage("Find me a flight to Tokyo on the 19");
        await GetReply();

        GetInput();
        await GetReply();

        void GetInput()
        {
            Console.Write("User: ");
            string input = Console.ReadLine() ?? "";
            history.AddUserMessage(input);
        }
    }
}
