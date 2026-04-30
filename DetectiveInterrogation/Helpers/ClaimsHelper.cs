using System.Security.Claims;

namespace DetectiveInterrogation.Helpers;

public class ClaimsHelper
{
    public int? GetUserId(ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier);
        return int.TryParse(claim?.Value, out int userId) ? userId : null;
    }

    public string? GetUsername(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Name)?.Value;
    }

    public string? GetEmail(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Email)?.Value;
    }
}