using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gifter.Models;
using Gifter.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gifter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileRepository _userProfileRepository;

        public UserProfileController(IUserProfileRepository userProfileRepository)
        {
            _userProfileRepository = userProfileRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_userProfileRepository.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            UserProfile userProfile = _userProfileRepository.GetById(id);
            if (userProfile == null)
            {
                return NotFound();
            }
            return Ok(userProfile);
        }

        [HttpGet("GetWithPosts/{id}")]
        public IActionResult GetUserProfileWithComments(int id)
        {
            var userProfile = _userProfileRepository.GetWithPosts(id);
            return Ok(userProfile);
        }

        [HttpPost]
        public IActionResult Post(UserProfile userProfile)
        {
            _userProfileRepository.Add(userProfile);
            //produces a status code of 201, which means userProfile object created sucessfully
            return CreatedAtAction("Get", new { id = userProfile.Id }, userProfile);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, UserProfile userProfile)
        {
            if (id != userProfile.Id)
            {
                return BadRequest();
            }
            _userProfileRepository.Update(userProfile);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _userProfileRepository.Delete(id);
            //return status 204
            return NoContent();
        }


    }
}