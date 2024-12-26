namespace Embedings.Interfaces;

public interface IChatBotService
{
    Task<string> ProcessTextAsync(string input, Dictionary<string, string>? metadataDict = null);
}
