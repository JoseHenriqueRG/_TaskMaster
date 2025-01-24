using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskMaster.Domain.Entities;
using TaskMaster.Domain.Interfaces;
using TaskMaster.Domain.ValueObjects;
using TaskMaster.Infra.Repository;

namespace TaskMaster.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserBusiness userBusiness) : ControllerBase
    {
        private readonly IUserBusiness _userBusiness = userBusiness;

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> PostUser(UserModel userModel)
        {
            var result = await _userBusiness.CreateUser(userModel);

            if(result.Success)
                return Ok(result);

            return StatusCode(500, result);
        }
    }
}
