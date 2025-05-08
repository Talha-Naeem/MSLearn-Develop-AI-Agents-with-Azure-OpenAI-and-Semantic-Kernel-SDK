using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

public class SelectedAdvertise
{
    public static async Task Main(string[] args)
    {
        string filePath = Path.GetFullPath("/home/talha-naeem/Documents/LLM Work/handlebarstemplate/MSLearn-Develop-AI-Agents-with-Azure-OpenAI-and-Semantic-Kernel-SDK/appsettings.json");
        var config = new ConfigurationBuilder()
            .AddJsonFile(filePath)
            .Build();

        // Set your values in appsettings.json
        string modelId = config["modelId"]!;
        string endpoint = config["endpoint"]!;
        string apiKey = config["apiKey"]!;

        // OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new() 
        // {
        //     FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        // };

        var history = new ChatHistory();
        history.AddSystemMessage("You are an assistant that can convert currencies and book flights. Always perform currency conversion if the user mentions amounts and currency names or symbols (like $, EUR, JPY). Only use flight booking when the user talks explicitly about travel or destinations.");
        

        
        var builder = Kernel.CreateBuilder();
        builder.AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);

        var kernel = builder.Build();
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        kernel.Plugins.AddFromType<FlightBookingPlugin>("FlightBooking");
        kernel.Plugins.AddFromType<CurrencyExchangePlugin>("CurrencyExchange");
        
        KernelFunction searchFlight = kernel.Plugins.GetFunction("FlightBooking", "search_flights");
        KernelFunction convertCurrency = kernel.Plugins.GetFunction("CurrencyExchange", "convertcurrency");

        PromptExecutionSettings openAIPromptExecutionSettings = new() 
        { 
            // FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(functions: [searchFlight, convertCurrency]) 
            FunctionChoiceBehavior = FunctionChoiceBehavior.Required(functions: [getWeather]) 
        };

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
                Console.WriteLine($"Function Results: {result.Metadata?.ToString()}");
            }
        }
        history.AddSystemMessage("The year is 2025 and the current month is June");
        
       AddUserMessage("Please convert $30 USD to Japanese Yen");
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

