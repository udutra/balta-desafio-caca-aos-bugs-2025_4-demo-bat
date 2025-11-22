using BugStore.Application.DTOs;

namespace BugStore.Api.Exceptions;

public class GlobalExceptionMiddleware : IMiddleware{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next){
        try{
            await next(context);
        }
        catch (Exception ex){
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var error = new ErrorDto(500, ex.Message);
            await context.Response.WriteAsJsonAsync(error);
        }
    }
}