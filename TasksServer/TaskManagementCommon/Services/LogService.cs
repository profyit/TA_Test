using Microsoft.Extensions.Logging;



public class LogWebEntity
{
    public string? DeveloperName { get; set; }
    public string? Method { get; set; }
    public string? Path { get; set; }
    public string? QueryString { get; set; } 
    public int StatusCode { get; set; }
}

public class LogInfo
{
    public string? FunctionName { get; set; }
    public string? UserName { get; set; }
    public string Message { get; set; }
}


public interface ICustomLogService
{
    void LogApiCall(LogWebEntity logEntity);
    void LogError(Exception exception, string? contextualMessage = null); 
    void LogInfo(LogInfo logInfo);
    void LogToConsole(LogInfo logInfo);
}


public class CustomLogService : ICustomLogService
{
   
    private readonly ILogger<CustomLogService> _logger;

   
    public CustomLogService(ILogger<CustomLogService> logger)
    {
        _logger = logger;
    }

    public void LogApiCall(LogWebEntity logEntity)
    {
        
        _logger.LogInformation(
            "API Call -> Developer: {DeveloperName}, Method: {Method}, Path: {Path}{QueryString}, Status: {StatusCode}",
            logEntity.DeveloperName,
            logEntity.Method,
            logEntity.Path,
            logEntity.QueryString, 
            logEntity.StatusCode);

       
    }

 
    public void LogError(Exception exception, string? message = null)
    {
       
        if (!string.IsNullOrEmpty(message))
        {
            _logger.LogError(exception, "Error Occurred: {Message}", message);
        }
        else
        {

            _logger.LogError(exception, "An unhandled exception occurred.");
        }

    }

    public void LogInfo(LogInfo logInfo)
    {

        _logger.LogInformation(
            "Info -> Function: {FunctionName}, User: {UserName}",
            logInfo.FunctionName,
            logInfo.UserName);


    }

    public void LogToConsole(LogInfo logInfo)
    {
        Console.Write($"Function name{logInfo.FunctionName}: Messsage: {logInfo.Message}");
    }
    public void LogError(string message)
    {
        _logger.LogError("Error: {ErrorMessage}", message);
    }

    public void LogWarning(string message)
    {
        _logger.LogWarning("Warning: {WarningMessage}", message);
    }
}