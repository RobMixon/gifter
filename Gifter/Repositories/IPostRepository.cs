using Gifter.Models;
using System.Collections.Generic;

namespace Gifter.Repositories
{
    public interface IPostRepository
    {

        void Add(Post post);
        void Update(Post post);
        void Delete(int id);
        List<Post> GetAll();
        Post GetById(int id);
        List<Post> GetAllWithComments();
    }
}