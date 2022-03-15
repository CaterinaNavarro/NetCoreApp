using NetCoreApp.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NetCoreApp.Services.Interfaces
{
    public interface IUsersManagementService
    {
        Task<User> UserAdd(string name, decimal heightCm);
        Task<User> UserGetById(long userId);
        Task UserAddressAdd(long userId, long addressId, bool main);
        Task<Family> FamilyGroupCreate(IEnumerable<long> userIds);
        Task FamilyUserAdd(long familyId, long userId, long familyRoleId);
        Task FamilyUserRoleUpdate(long familyId, long userId, long familyRoleId);
        Task<Family> FamilyGetById(long familyId);
    }
}
