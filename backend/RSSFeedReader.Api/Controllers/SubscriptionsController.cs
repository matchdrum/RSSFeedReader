using Microsoft.AspNetCore.Mvc;
using RSSFeedReader.Api.Models;
using RSSFeedReader.Api.Services;

namespace RSSFeedReader.Api.Controllers;

[ApiController]
[Route("api/subscriptions")]
public sealed class SubscriptionsController(SubscriptionStore store) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<SubscriptionResponse>> Get()
    {
        var subscriptions = store.GetAll()
            .Select(url => new SubscriptionResponse(url))
            .ToArray();

        return Ok(subscriptions);
    }

    [HttpPost]
    public ActionResult<SubscriptionResponse> Add([FromBody] SubscriptionRequest? request)
    {
        if (!SubscriptionUrlValidator.TryValidate(request?.Url, out var url, out var error))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid subscription URL",
                Detail = error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (!store.TryAdd(url))
        {
            return Conflict(new ProblemDetails
            {
                Title = "Duplicate subscription",
                Detail = "This URL is a duplicate subscription.",
                Status = StatusCodes.Status409Conflict
            });
        }

        return Ok(new SubscriptionResponse(url));
    }
}
