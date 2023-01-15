namespace Commons.Exceptions;

public sealed class RejectException : ApplicationException
{
    public RejectException(string? message) : base(message)
    {
    }
}