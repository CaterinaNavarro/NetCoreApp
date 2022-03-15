using Microsoft.AspNetCore.Mvc;
using NetCoreApp.API.Dtos;
using NetCoreApp.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly IUsersManagementService _userManagementService;

        public UserManagementController (IUsersManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        /// <summary>
        /// Creates an user
        /// </summary>
        /// <param name="request"> User information </param>
        /// <returns> User Id already created </returns>
        [HttpPost("users")]
        public async Task<ActionResult<long>> UserAdd([FromBody] UserAddRequestDto request)
        {
            var userId = (await _userManagementService.UserAdd(request.Name, request.HeightCm)).Id;

            if (request.FamilyId.HasValue && request.FamilyRoleId.HasValue)
            {
                await _userManagementService.FamilyUserAdd(request.FamilyId.Value, userId, request.FamilyRoleId.Value);
            }
            
            foreach(var userAddress in request.Addresses)
            {
                await _userManagementService.UserAddressAdd(userId, userAddress.AddressId, userAddress.Main);
            };

            return Ok(userId);
        }

        /// <summary>
        /// Gets an user
        /// </summary>
        /// <param name="userId"> User Id </param>
        /// <returns> User information </returns>
        [HttpGet("users/{userId}")]
        [ResponseCache(Duration = 29)]
        public async Task<ActionResult<UserGetByIdResponseDto>> UserGetById([FromRoute] long userId, [FromQuery] bool? mainAddress = null)
        {
            var user = await _userManagementService.UserGetById(userId);
            
            return Ok(new UserGetByIdResponseDto()
            {
                Name = user.Name,
                HeightCm = user.HeightCm,
                Adresses = (mainAddress.HasValue ? user.Addresses.Where(ua => ua.Main == mainAddress.Value) : user.Addresses)
                .Select(ua => new UserGetByIdAddressResponseDto()
                {
                    AddressId = ua.Id,
                    Name = ua.Address.Name,
                    Number = ua.Address.Number,
                    Main = ua.Main
                })
            });
        }

        /// <summary>
        /// Creates a family group with its members and roles
        /// </summary>
        /// <param name="userId"> User Id </param>
        /// <param name="userRelationships"> UserId and FamilyRole List </param>
        /// <returns> Family Id </returns>
        [HttpPost("familyGroups")]
        public async Task<ActionResult<long>> FamilyGroupCreate([FromBody] IEnumerable<FamilyGroupCreateRequestDto> userRelationships)
        {
            var familyGroup = await _userManagementService.FamilyGroupCreate(userRelationships.Select(ur => ur.UserId));

            foreach (var userRelationship in userRelationships)
            {
                await _userManagementService.FamilyUserRoleUpdate(familyGroup.Id, userRelationship.UserId, userRelationship.FamilyRoleId);
            }

            return Ok(familyGroup.Id);
        }

    }
}
