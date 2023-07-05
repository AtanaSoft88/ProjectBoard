namespace ProjectBoard.API.Utilities;

public static class ErrorMessages
{   
    //Team
    public const string InvalidTeamId = "Invalid team id.";
    public const string InvalidTeamLeadId = "Invalid Team Lead id.";
    public const string TeamNotFound = "Team with id {0} not found.";
    public const string TeamLeadNotFound = "Team Lead with id {0} not found.";
    public const string InvalidNextPageKey = "NextPageKey is not a valid Base-64 string.";
    public const string InvalidPageSize = "PageSize must be between 1 and 60.";

    //TeamMember
    public const string ExistingMember = "This member already exists in the team.";
    public const string NotExistingMember = "This member doesn't exists in the team.";

    // User
    public const string UsernameNotFound = "There is no such username";
    public const string UsernameSearchMinLength = "Enter at least 3 characters to be able to search for username";
    public const string UserIdNotFound = "A user with this id {0} doesn't exist.";
    public const string InvalidUserId = "Invalid user id.";
    public const string UsernameMinLengthErrorMessage = "Enter at least 3 characters to be able to search for username";
    public const string UsernameSpecialSymbolError = "Search query should not contain any special characters";
    public const string PageNumberExceedPageCountError = "Page number cannot be bigger than page count";
    public const string InvalidPaginationToken = "Pagination token entered is invalid or expired";
    public const string ExceededLastPageError = "There is nothing here, the last populated page is {0}";
    public const string PageSizeMaxValueError = "Page size should be less or equal to 60";
    public const string PageSizeMinValueError = "Page size should be greater or equal to 1";
    public const string UserAlreadyExists = "User with such Username or Email address already exists";

    public const string PasswordLength = "Password must be at least {0} characters long.";
    public const string PasswordRequirementsMismatch = 
        "Password must be at least 6 characters long, containing: One upper-case, One lower-case, one non-alphanumeric character and one digit";
    public const string PasswordRegex = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*\W).{6,}$";

    //Project        
    public const string ProjectNotFoundById = "Project with such Id could not be found";
    public const string UpdateFailed = "Project could not be updated";
    public const string InvalidInput = "Incorrect input parameter for {0}.";
    public const string ProjectCreationNotAllowed = "Project creation can be done only by a Project Manager";
    public const string ProjectUpdateNotAllowed = "Project update can be done only by a Project Manager";              
    public const string PorjectManagerMismatch = "Inappropriate assignment of ProjectManagerId.";
    public const string ProjectNotFinished = "All Project's Tasks must be Done in order to be able to mark the Project as Done or Approved.";
    public const string Project_Not_Found = "Project could not be found.";
    public const string Projects_Not_Found = "Projects could not be found.";
    public const string Project_Does_Not_Exist = "Project with such Id could not be found";
    public const string Update_Failed = "Project could not be updated";
    
    //Assignment
    public const string AssignmentNotFound = "Assignment with id {0} not found.";
    public const string ProjectNotFound = "Project with id {0} not found.";
    public const string UserIsNotTeamMember = "Unable to assign the task to the user with id {0} because they are not a team member.";
}