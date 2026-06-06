namespace AccpacGraphqlClean.Domain;

public sealed record ProcessOut(
    string ReturnCode,
    string ReturnMessage,
    string? DocumentNumber = null,
    string? BatchNumber = null,
    string? ReferenceNumber = null,
    string? ErrorCode = null
)
{
    public static ProcessOut Ok(string message = "OK", string? documentNumber = null) =>
        new("0000", message, DocumentNumber: documentNumber, ErrorCode: "0000");
    public static ProcessOut Fail(string code, string message) => new(code, message, ErrorCode: code);
}

public sealed record AccpacOperationResult(ProcessOut Response, object? Data);

public sealed record AuthToken(string AccessToken, string TokenType, int ExpiresInSeconds);

public sealed record CompanyConnectionDetails(string CompanyId, string UserName, string Password);
