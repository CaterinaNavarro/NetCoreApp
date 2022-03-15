using NetCoreApp.Domain.Entities;
using NetCoreApp.Infrastructure.Data;
using NetCoreApp.Services.Interfaces;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NetCoreApi.Crosscutting.Exceptions;
using System.Net;
using NetCoreApi.Crosscutting.Enums;
using System.Collections.Generic;
using NetCoreApi.Crosscutting.Helpers;

namespace NetCoreApp.Services
{
    public class UsersManagementService : IUsersManagementService
    {
        private readonly ApplicationDbContext _context;

        public UsersManagementService (ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> UserAdd(string name, decimal heightCm)
        {
            var user = new User()
            {
                Name = name,
                HeightCm = heightCm
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> UserGetById(long userId)
        {
            return await _context.Users
                .Include(u => u.Relationships).ThenInclude(uf => uf.Family)
                .Include(u => u.Relationships).ThenInclude(uf => uf.FamilyRole)
                .Include(u => u.Addresses).ThenInclude(ua => ua.Address)
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new ClientErrorException("User not found", (int)HttpStatusCode.NotFound);
        }

        public async Task UserAddressAdd(long userId, long addressId, bool main)
        {
            var user = await UserGetById(userId);

            var userFamilies = user.Relationships.Select(cuf => cuf.FamilyId);

            if (!await _context.Addresses.AnyAsync(a => a.Id == addressId)) throw new ClientErrorException("Address not found", (int)HttpStatusCode.NotFound);

            if (user.Addresses.Exists(ua => ua.AddressId == addressId)) throw new ClientErrorException("User already has the address", (int)HttpStatusCode.Conflict);

            if (user.Addresses.Exists(ua => ua.Main) && main) throw new ClientErrorException("User already has a main address", (int)HttpStatusCode.Conflict);

            if (await _context.Users.AnyAsync(u => u.Id != userId && u.Addresses.Any(ua => ua.AddressId == addressId) && (!u.Relationships.Any() || u.Relationships.Any(uf => !userFamilies.Contains(uf.FamilyId))))) 
                throw new ClientErrorException("There is an user with the same address and does not belong to any of the families of the current user or is not related to any", (int)HttpStatusCode.Conflict);

            _context.UsersAddresses.Add(new UserAddress()
            {
                UserId = userId,
                AddressId = addressId,
                Main = main
            });

            await _context.SaveChangesAsync();
        }

        public async Task<Family> FamilyGroupCreate(IEnumerable<long> userIds)
        {
            if (userIds == null || !userIds.Any()) throw new ClientErrorException("you must add at least one user to the family group", (int)HttpStatusCode.BadRequest);

            foreach (var userId in userIds)
            {
                if (!await _context.Users.AnyAsync(u => u.Id == userId)) throw new ClientErrorException($"User with userId {userId} not found", (int)HttpStatusCode.NotFound);
            }

            if (userIds.GroupBy(userId => userId).Exists(g => g.Count() > 1)) throw new ClientErrorException($"Users cannot be repeated", (int)HttpStatusCode.BadRequest);

            var family = new Family()
            {
                Name = "Family created",
                Description = "Family created",
                Members = userIds.Select(userId => new UserRelationship()
                {
                    UserId = userId,

                }).ToList()
            };

            _context.Families.Add(family);
            await _context.SaveChangesAsync();

            return family;
        }

        public async Task FamilyUserAdd(long familyId, long userId, long familyRoleId)
        {
            if (!await _context.Users.AnyAsync(u => u.Id == userId)) throw new ClientErrorException($"User not found", (int)HttpStatusCode.NotFound);

            var family = await FamilyGetById(familyId);

            if (!await _context.FamilyRoles.AnyAsync(fr => fr.Id == familyRoleId)) throw new ClientErrorException($"Family role not found", (int)HttpStatusCode.NotFound);

            if (family.Members.Exists(fm => fm.UserId == userId)) throw new ClientErrorException($"User is a family member", (int)HttpStatusCode.Conflict);

            if (family.Members.Exists(fm => fm.FamilyRoleId.HasValue && fm.FamilyRoleId == familyRoleId &&
               (fm.FamilyRoleId == (long)FamilyRoles.Mother || fm.FamilyRoleId == (long)FamilyRoles.Father)))
                throw new ClientErrorException($"There is a family member with the role {((FamilyRoles)familyRoleId).GetDescription()}", (int)HttpStatusCode.Conflict);

            _context.UserRelationships.Add(new UserRelationship()
            {
                UserId = userId,
                FamilyId = familyId,
                FamilyRoleId = familyRoleId
            });

            await _context.SaveChangesAsync();
        }

        public async Task FamilyUserRoleUpdate(long familyId, long userId, long familyRoleId)
        {
            if (!await _context.Users.AnyAsync(u => u.Id == userId)) throw new ClientErrorException($"User not found", (int)HttpStatusCode.NotFound);

            var family = await FamilyGetById(familyId);

            if (!await _context.FamilyRoles.AnyAsync(fr => fr.Id == familyRoleId)) throw new ClientErrorException($"Family role not found", (int)HttpStatusCode.NotFound);

            if (family.Members.Exists(fm => fm.FamilyRoleId.HasValue && fm.FamilyRoleId == familyRoleId && fm.UserId != userId &&
               (fm.FamilyRoleId == (long)FamilyRoles.Mother || fm.FamilyRoleId == (long)FamilyRoles.Father)))
                throw new ClientErrorException($"There is a family member with the role {((FamilyRoles)familyRoleId).GetDescription()}", (int)HttpStatusCode.Conflict);

            var userRelationship = family.Members.FirstOrDefault(uf => uf.UserId == userId) ?? throw new ClientErrorException($"User not related to family", (int)HttpStatusCode.Conflict);

            userRelationship.FamilyRoleId = familyRoleId;

            _context.UserRelationships.Update(userRelationship);
            await _context.SaveChangesAsync();
        }

        public async Task<Family> FamilyGetById(long familyId)
        {
            return await _context.Families.Include(f => f.Members).ThenInclude(fm => fm.User)
                .Include(f => f.Members).ThenInclude(fm => fm.FamilyRole)
                .FirstOrDefaultAsync(f => f.Id == familyId)
                ?? throw new ClientErrorException($"Family not found", (int)HttpStatusCode.NotFound);
        }
    }
}
