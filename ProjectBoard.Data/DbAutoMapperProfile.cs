using Amazon.DynamoDBv2.Model;
using AutoMapper;
using ProjectBoard.Data.Abstractions.Models;
using ProjectBoard.Data.DynamoModels;

namespace ProjectBoard.Data;
public class DbAutoMapperProfile : Profile
{
    public DbAutoMapperProfile()
    {
        CreateMap<DynamoProject, Project>().ReverseMap();
        CreateMap<DynamoTeam, Team>().ReverseMap();
        CreateMap<DynamoAssignment, Assignment>().ReverseMap();
        CreateMap<Dictionary<string, AttributeValue>, Team>()
         .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ContainsKey("Id") ? src["Id"].S : null))
         .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ContainsKey("Name") ? src["Name"].S : null))
         .ForMember(dest => dest.TeamLeadId, opt => opt.MapFrom(src => src.ContainsKey("TeamLeadId") ? src["TeamLeadId"].S : null))
         .ForMember(dest => dest.DeveloperIds, opt => opt.MapFrom(src => src.ContainsKey("DeveloperIds") ? src["DeveloperIds"].SS : null));

        CreateMap<Dictionary<string, AttributeValue>, Project>()
         .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ContainsKey("Id") ? src["Id"].S : null))
         .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ContainsKey("Name") ? src["Name"].S : null))
         .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ContainsKey("Description") ? src["Description"].S : null))
         .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.ContainsKey("Status") ? src["Status"].S : null))
         .ForMember(dest => dest.TeamId, opt => opt.MapFrom(src => src.ContainsKey("TeamId") ? src["TeamId"].S : null))
         .ForMember(dest => dest.ProjectManagerId, opt => opt.MapFrom(src => src.ContainsKey("ProjectManagerId") ? src["ProjectManagerId"].S : null))
         .ForMember(dest => dest.Assignments, opt => opt.MapFrom(src => src.ContainsKey("Assignments") ? src["Assignments"].SS.ToList() : null))
         ;
    }
}
