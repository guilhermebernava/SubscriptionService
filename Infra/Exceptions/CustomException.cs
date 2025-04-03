namespace Infra.Exceptions;
public class CustomException : Exception
{
    public CustomException(Exception e) : base("", e) { }
}
