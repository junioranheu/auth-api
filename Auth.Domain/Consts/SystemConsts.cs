namespace Auth.Domain.Consts;

public static class SystemConsts
{
    public const string Name = "Auth.API";
    public const string AzureUserIdClaims = "http://schemas.microsoft.com/identity/claims/objectidentifier";

    public const int OneMinuteInSec = 60;
    public const int TenMinutesInSec = 600;
    public const int OneHourInSec = 3600;
    public const int OneDayInSec = 86400;
    public const int OneMonthInSec = 2629800;

    public const string RefreshTokenJWTCustomHeader = "X-New-JWT";
}