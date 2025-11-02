namespace BugStore.Application.DTOs.Customers;

public class GetAllCustomersResponseDto(IEnumerable<CustomerDto> data, int totalCount, int pageNumber,
    int pageSize, int totalPages, int statusCode, string? message){

    public IEnumerable<CustomerDto> Data { get; set; } = data;
    public int TotalCount { get; set; } = totalCount;
    public int PageNumber { get; set; } = pageNumber;
    public int PageSize { get; set; } = pageSize;
    public int TotalPages { get; set; } = totalPages;
    public int StatusCode { get; set; } = statusCode;
    public string? Message { get; set; } = message;
}