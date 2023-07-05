namespace ProjectBoard.API;

public class Constants
{
    public const string UserPolicyName = "User";
    public const string AdminPolicyName = "Admin";

    public const string GetAllOperation = "GetAll";
    public const string GetOperation = "Get";
    public const string CreateOperation = "Create";
    public const string UpdateOperation = "Update";
    public const string SearchOperation = "Search";
    public const string RemoveOperation = "Remove";

    public const int PageNumberDefaultValue = 1;
    public const int PageSizeDefaultValue = 10;
    public const int MinPageSize = 1;
    public const int MaxPageSize = 60;
    public const int DefaultPageSize = 10;
}
