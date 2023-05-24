using ApiHost;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExtendingOAuthAndOpenIDConnect.API.Controllers;

[Route("api/claims")]
[ApiController]
[Authorize]
public class ClaimsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var claims = from c in User.Claims
                     select new
                     {
                         type = c.Type,
                         value = c.Value
                     };

        // if DPoP: 
        var proofToken = Request.GetDPoPProofToken();
        if (proofToken != null)
        {
            var claimsToReturn = claims.ToList();
            claimsToReturn.Add(new { type = "proofToken", value = proofToken });
            return new JsonResult(claimsToReturn);
        }

        return new JsonResult(claims);
    }
}