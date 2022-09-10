using System.Text.Json;
namespace OpenDSM.NET.Exceptions;

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException(UserCredentials credentials) :
    base($"OpenDSM Authentication Credentials provided were invalid!\n{JsonSerializer.Serialize(credentials)}")
    { }
}
