namespace BugStore.Application.DTOs;

public class ErrorDto(int statusCode, string? message){
    public int StatusCode { get; set; } = statusCode;
    public string? Message { get; set; } = message;
}