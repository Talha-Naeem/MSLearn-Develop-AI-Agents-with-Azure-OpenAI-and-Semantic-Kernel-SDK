using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;

public class DateTimePlugin
{
    [KernelFunction("get_current_time")]
    [Description("Returns the current UTC date and time")]
    public string GetCurrentTime()
    {
        return $"The current UTC date and time is {DateTime.UtcNow}.";
    }
}

public class FunctionAdvertising
{
    public static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("/home/talha-naeem/Documents/LLM Work/handlebarstemplate/MSLearn-Develop-AI-Agents-with-Azure-OpenAI-and-Semantic-Kernel-SDK/appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        string modelId = config["modelId"] ?? throw new Exception("modelId is missing in appsettings.json");
        string endpoint = config["endpoint"] ?? throw new Exception("endpoint is missing in appsettings.json");
        string apiKey = config["apiKey"] ?? throw new Exception("apiKey is missing in appsettings.json");

        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var endpointUri) || endpointUri.Scheme != "https")
        {
            throw new Exception("Invalid endpoint URL. It must be a valid HTTPS URL.");
        }

        IKernelBuilder builder = Kernel.CreateBuilder();
        builder.AddAzureOpenAIChatCompletion(deploymentName: modelId, endpoint: endpoint, apiKey: apiKey);
        builder.Plugins.AddFromType<WeatherPlugin>();
        builder.Plugins.AddFromType<FlightBookingPlugin>();
        builder.Plugins.AddFromType<DateTimePlugin>();

        Kernel kernel = builder.Build();

        // Advertising All Functions
        // This behavior allows the kernel to automatically select the most relevant function based on the prompt.
        // The kernel will analyze the prompt and choose the function that best fits the request.

        PromptExecutionSettings Advertisingall = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };

        var result = await kernel.InvokePromptAsync("What is the likely color of the sky in Boston?", new(Advertisingall));
        Console.WriteLine(result);
        
        // Advertising Selected Functions
        // This behavior allows the kernel to automatically select the most relevant function based on the prompt.
        // The kernel will analyze the prompt and choose the function that best fits the request.

        KernelFunction getWeather = kernel.Plugins.GetFunction("WeatherPlugin", "get_weather");
        KernelFunction getCurrentTime = kernel.Plugins.GetFunction("DateTimePlugin", "get_current_time");

        PromptExecutionSettings advertisingselect = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(functions: [getWeather, getCurrentTime]) };

        var result1 = await kernel.InvokePromptAsync("What is the likely color of the sky in Boston?", new(advertisingselect));
        Console.WriteLine(result1);
        
         // Disable Behavior
        // This behavior disables the function selection process, and the prompt is treated as a regular text input.
        // The kernel will not attempt to select any functions based on the prompt. 
        

        PromptExecutionSettings disableFunction = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(functions: []) };

        var result2 = await kernel.InvokePromptAsync("What is the likely color of the sky in Boston?", new(disableFunction));
        Console.WriteLine(result2);
        
        // Auto Behavior
        // This behavior automatically selects the most relevant function based on the prompt.
        // The kernel will analyze the prompt and choose the function that best fits the request.
        

        PromptExecutionSettings AutoBehaviour = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };
        var result3 = await kernel.InvokePromptAsync("What is the likely color of the sky in Boston?", new(AutoBehaviour));
        Console.WriteLine(result3);
        
        // Required Behavior
        // This behavior requires the user to specify which functions are needed to answer the prompt.
        // The kernel will not select any functions automatically, and the user must explicitly indicate which functions to use.

        PromptExecutionSettings RequiredBehaviour = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Required(functions: [getWeather]) };

        var result4 = await kernel.InvokePromptAsync("What is the likely color of the sky in Boston?", new(RequiredBehaviour));
        Console.WriteLine(result4);

        // None Behavior
        // This behavior does not allow any functions to be selected, and the prompt is treated as a regular text input.

        PromptExecutionSettings Nonesettings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.None() };

        var result5 = await kernel.InvokePromptAsync("Specify which provided functions are needed to determine the sky’s color in Boston.", new(Nonesettings));
        Console.WriteLine(result5);
    }
}
