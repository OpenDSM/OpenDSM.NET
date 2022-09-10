
using System.Diagnostics.Contracts;

namespace OpenDSM.NET.Exceptions;

public class UnresolvedQueryResultException : Exception
{
    public UnresolvedQueryResultException(string message = "") :
    base($"The query posted returned with no results.  This usually means that the query was invalid or the information requested was not available.\n{message}")
    { }
}
