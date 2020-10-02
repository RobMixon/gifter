using Gifter.Models;
using System.Collections.Generic;

namespace Gifter.Repositories
{
    public interface IUserProfileRepository
    {
        List<UserProfile> GetAll();
        UserProfile GetById(int id);
        UserProfile GetWithPosts(int id);
        void Add(UserProfile userProfile);
        void Update(UserProfile userProfile);

        void Delete(int id);
    }
}