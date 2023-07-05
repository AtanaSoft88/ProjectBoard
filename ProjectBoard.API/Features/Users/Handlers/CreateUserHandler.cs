using AutoMapper;
using MediatR;
using ProjectBoard.API.Features.Users.Models;
using ProjectBoard.API.Features.Users.Requests;
using ProjectBoard.API.Responses.Base;
using ProjectBoard.API.Utilities;
using ProjectBoard.Identity.Abstractions;
using ProjectBoard.Identity.Abstractions.Models;

namespace ProjectBoard.API.Features.Users.Handlers
{
    public class CreateUserHandler : IRequestHandler<CreateUserRequest, IResult>
    {
        private readonly IIdentity _identity;
        private readonly IMapper _mapper;

        public CreateUserHandler(IIdentity identity, IMapper mapper)
        {
            _identity = identity;
            _mapper = mapper;
        }

        public async Task<IResult> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            User? createUserResponse = await _identity.CreateUser(request.Username, request.Email, request.Password);
            if (createUserResponse is null) 
            {
                return Response.BadRequest(ErrorMessages.UserAlreadyExists);
            }
            UserModel responseModel = _mapper.Map<UserModel>(createUserResponse);
            return Response.OkData(responseModel);                 
        }
    }
}
