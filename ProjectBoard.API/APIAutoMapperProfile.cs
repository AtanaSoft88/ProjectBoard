using AutoMapper;
using ProjectBoard.API.Features.Assignments.Models;
using ProjectBoard.API.Features.Assignments.Requests;
using ProjectBoard.API.Features.Projects.Requests;
using ProjectBoard.API.Features.Projects.Responses;
using ProjectBoard.API.Features.Teams.Responses;
using ProjectBoard.API.Features.Users.Models;
using ProjectBoard.Data.Abstractions.Models;
using ProjectBoard.Identity.Abstractions.Models;

namespace ProjectBoard.API
{
    public class APIAutoMapperProfile : Profile
    {
        public APIAutoMapperProfile()
        {
            CreateMap<Project, ProjectModel>().ReverseMap();
            CreateMap<UpdateProjectRequest, Project>().ReverseMap();                
            CreateMap<Team, TeamModel>().ReverseMap();            
            CreateMap<User, UserModel>().ReverseMap();
            CreateMap<Assignment, AssignmentModel>().ReverseMap();
            CreateMap<Assignment, CreateAssignmentRequest>().ReverseMap();
        }
    }
}

