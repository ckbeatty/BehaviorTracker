using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BehaviorTracker.Repository.DatabaseModels;
using BehaviorTracker.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BehaviorTracker.Repository.Implementations
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(BehaviorTrackerDatabaseContext behaviorTrackerDatabaseContext) : base(
            behaviorTrackerDatabaseContext)
        {
        }

        public async Task<BehaviorTrackerUser> GetUserAsync(string email)
        {
            return await _behaviorTrackerDatabaseContext.BehaviorTrackerUsers.FirstOrDefaultAsync(user =>
                user.Email == email);
        }

        public async Task<BehaviorTrackerUser> SaveUserAsync(BehaviorTrackerUser user)
        {
            var savedUser = user.BehaviorTrackerUserKey > 0
                ? _behaviorTrackerDatabaseContext.BehaviorTrackerUsers.Update(user).Entity
                : (await _behaviorTrackerDatabaseContext.BehaviorTrackerUsers.AddAsync(user)).Entity;

            await _behaviorTrackerDatabaseContext.SaveChangesAsync();
            return savedUser;
        }

        public async Task<IEnumerable<BehaviorTrackerRole>> GetUserRolesAsync(long behaviorTrackerUserKey)
        {
            throw new NotImplementedException();
//            return await _behaviorTrackerDatabaseContext.BehaviorTrackerUserRoles.Where(userRoles =>
//                    userRoles.BehaviorTrackerUserKey == behaviorTrackerUserKey)
//                .Select(userRoles => userRoles.BehaviorTrackerRole).ToListAsync();
        }

        public IEnumerable<BehaviorTrackerUser> GetUsers()
        {
            throw new NotImplementedException();
//            return _behaviorTrackerDatabaseContext.BehaviorTrackerUsers.Include(user => user.BehaviorTrackerUserRoles)
//                .ThenInclude(userRole => userRole.BehaviorTrackerRole).AsEnumerable();
        }
    }
}