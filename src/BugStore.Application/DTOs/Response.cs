using System.Text.Json.Serialization;

namespace BugStore.Application.DTOs;

public class Response<TData>{
    public readonly int Code;
    public TData? Data { get; set; }
    public string? Message { get; set; }

    [JsonIgnore]
    public bool IsSuccess => Code is >= 200 and <= 299;

    [JsonConstructor]
    public Response() => Code = Configuration.DefaultStatusCode;

    public Response(TData? data, int code = Configuration.DefaultStatusCode, string? message = null){
        Code = code;
        Data = data;
        Message = message;
    }
}