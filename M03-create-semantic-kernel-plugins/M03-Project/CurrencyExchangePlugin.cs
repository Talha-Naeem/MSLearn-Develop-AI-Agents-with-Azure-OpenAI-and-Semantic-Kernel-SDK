using System.ComponentModel;
using Microsoft.SemanticKernel;

public class CurrencyExchangePlugin
{
    // A dictionary that stores exchange rates for demonstration
    private static Dictionary<string, decimal> exchangeRates = new Dictionary<string, decimal>
    {
        { "USD-EUR", 0.85m },
        { "USD-GBP", 0.75m },
        { "USD-JPY", 110.50m },
        { "EUR-USD", 1.18m },
        { "GBP-USD", 1.33m },
        { "JPY-USD", 1 / 110.50m }
    };

    // Function to get the exchange rate between two currencies
    public static decimal GetExchangeRate(string fromCurrency, string toCurrency)
    {
        string key = $"{fromCurrency}-{toCurrency}";
        if (exchangeRates.ContainsKey(key))
        {
            return exchangeRates[key];
        }
        else
        {
            throw new Exception("Exchange rate not available for this currency pair.");
        }
    }

    

    [KernelFunction, Description("Converts an amount from one currency to another, for example USD to EUR")]
    public string convert_currency(
        [Description("Amount to convert")] decimal amount,
        [Description("Currency to convert from, like USD")] string from,
        [Description("Currency to convert to, like JPY")] string to)
    {
        decimal rate = GetExchangeRate(from, to);
        decimal result = amount * rate;
        return $"{amount} {from} = {result} {to}";
        
    }
}