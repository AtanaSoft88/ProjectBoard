using ProjectBoard.API.Features.Assignments.Models;
using ProjectBoard.Data.Abstractions.Enums;
using ProjectBoard.Data.Abstractions.Models;

namespace ProjectBoard.API.Tests.Features.Projects.Handlers.Data;
public static class DataModelInitializer
{
    public static List<Project> GetProjectsData(int projectsCount)
    {
        var projects = Enumerable.Range(1, projectsCount).Select(i => new Project
        {
            Id = Guid.NewGuid().ToString(),
            Name = $"Project - {i}",
            Description = $"Test Description-{i}",
            Status = ProjectStatus.InProgress,
            ProjectManagerId = Guid.NewGuid().ToString(),
            TeamId = null,
        }).ToList();
        return projects;
    }
    public static List<AssignmentModel> GetAssignmentModelData(AssignmentStatus status)
    {

        var assignmentModels = new List<AssignmentModel>()
        {
            new AssignmentModel()
            {
                 Id = Guid.NewGuid().ToString(),
                 Name = "Task-1", Description = "Description-Test1",
                 DeveloperId =  Guid.NewGuid().ToString(),
                 Status = status
            },
            new AssignmentModel()
            {
                 Id = Guid.NewGuid().ToString(),
                 Name = "Task-2", Description = "Description-Test2",
                 DeveloperId =  Guid.NewGuid().ToString(),
                 Status = status
            }
        };
        return assignmentModels;
    }

    public static List<Assignment> GetAssignmentData(AssignmentStatus status)
    {

        var assignments = new List<Assignment>()
        {
            new Assignment()
            {
                 Id = Guid.NewGuid().ToString(),
                 Name = "Task-1", Description = "Description-Test1",
                 DeveloperId =  Guid.NewGuid().ToString(),
                 Status = status
            },
            new Assignment()
            {
                 Id = Guid.NewGuid().ToString(),
                 Name = "Task-2", Description = "Description-Test2",
                 DeveloperId =  Guid.NewGuid().ToString(),
                 Status = status
            }
        };
        return assignments;
    }
}
