using Microsoft.AspNetCore.Http;

namespace TicTacToe.Web.Services;

public static class GuestIdentityExtensions
{
    private const string CookieName = "tictactoe.guest";
    private static readonly TimeSpan CookieLifetime = TimeSpan.FromHours(12);

    public static void SetGuestIdentity(this HttpContext context, string roomCode, Guid playerId)
    {
        string value = $"{roomCode.Trim().ToUpperInvariant()}:{playerId}";
        context.Response.Cookies.Append(CookieName, value, CreateCookieOptions(context));
    }

    public static bool TryGetGuestIdentity(
        this HttpContext context,
        out string roomCode,
        out Guid playerId)
    {
        roomCode = string.Empty;
        playerId = Guid.Empty;

        string? value = context.Request.Cookies[CookieName];
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        int separator = value.IndexOf(':');
        if (separator <= 0 || separator >= value.Length - 1)
        {
            return false;
        }

        string codePart = value[..separator].Trim();
        string idPart = value[(separator + 1)..];

        if (codePart.Length == 0 || !Guid.TryParse(idPart, out playerId))
        {
            playerId = Guid.Empty;
            return false;
        }

        roomCode = codePart.ToUpperInvariant();
        return true;
    }

    public static void ClearGuestIdentity(this HttpContext context)
    {
        context.Response.Cookies.Delete(CookieName, CreateCookieOptions(context));
    }

    private static CookieOptions CreateCookieOptions(HttpContext context)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Secure = context.Request.IsHttps,
            Expires = DateTimeOffset.UtcNow.Add(CookieLifetime)
        };
    }
}
