namespace Ventixe.Authentication.Models;

public class AuthResult
{
    public bool Succeeded { get; set; }
    public string? Message { get; set; }
}

public class AuthResult<T> : AuthResult
{
    public T? Content { get; set; }
}
