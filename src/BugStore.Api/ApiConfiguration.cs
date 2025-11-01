namespace BugStore.Api;

public static class ApiConfiguration{
    public static string ConnectionString { get; set; } = string.Empty;
    public static string BackendUrl { get; set; } = string.Empty;
    public const string CorsPolicyName = "desafio-caca-aos-bugs-2025-cors";
}