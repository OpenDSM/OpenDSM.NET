using System.Text.Json;
namespace OpenDSM.NET.Exceptions;

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException(UserCredentials credentials, string message = "") :
    base($"OpenDSM Authentication Credentials provided were invalid!{(string.IsNullOrWhiteSpace(message) ? "" : $"\n{message}")}\n{JsonSerializer.Serialize(credentials)}")
    { }
}
