using Microsoft.AspNetCore.Mvc;

namespace TrackingApi.Helpers;

public static class ApiResponse
{
    public static ProblemDetails BadRequest(string detail)
    {
        return new ProblemDetails
        {
            Title = "Not Found",
            Status = StatusCodes.Status400BadRequest,
            Detail = detail
        };
    }
}
