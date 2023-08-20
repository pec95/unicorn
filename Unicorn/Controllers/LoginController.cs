using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Unicorn.Models;
using Microsoft.AspNetCore.Identity;
using Unicorn.Entities;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Unicorn.Controllers
{
    [AllowAnonymous]
    [EnableCors("AllowLogin")]
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public LoginController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        [HttpPost("loginUser")]
        public async Task<bool> Login(UserWeb user)
        {
            if(ModelState.IsValid)
            {
                var password = user.Password;
                
                var userFound = await _userManager.FindByNameAsync(user.UserName);

                if (userFound == null)
                {
                    return false;
                }

                var passwordCorrect = await _userManager.CheckPasswordAsync(userFound, password);
                if (!passwordCorrect)
                {
                    return false;
                }
                
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpGet("createUsers")]
        public async Task<IActionResult> createUsers()
        {
            User user1 = new User()
            {
                UserName = "sales",
                Email = "sales@mail.com"
            };

            User user2 = new User()
            {
                UserName = "warehouse",
                Email = "warehouse@mail.com"
            };

            User user3 = new User()
            {
                UserName = "customer",
                Email = "customer@mail.com"
            };

            var result1 = await _userManager.CreateAsync(user1, "Prodaja1_");

            if(!result1.Succeeded)
            {
                return BadRequest(result1.Errors);
            }

            var result2 = await _userManager.CreateAsync(user2, "Skladište2_");

            if (!result2.Succeeded)
            {
                return BadRequest(result2.Errors);
            }

            var result3 = await _userManager.CreateAsync(user3, "Kupac3_");

            if (!result3.Succeeded)
            {
                return BadRequest(result3.Errors);
            }

            return Ok();
        }

        [HttpGet("createRoles")]
        public async Task<IActionResult> createRoles()
        {
            IdentityRole role1 = new IdentityRole() { Name = "Warehouse" };
            IdentityRole role2 = new IdentityRole() { Name = "Sales" };

            var result1 = await _roleManager.CreateAsync(role1);

            if(!result1.Succeeded)
            {
                return BadRequest(result1.Errors);
            }

            var result2 = await _roleManager.CreateAsync(role2);

            if (!result2.Succeeded)
            {
                return BadRequest(result2.Errors);
            }

            var user1 = await _userManager.FindByNameAsync("warehouse");
            var user2 = await _userManager.FindByNameAsync("sales");
            var user3 = await _userManager.FindByNameAsync("customer");

            var result3 = await _userManager.AddToRoleAsync(user1, "Warehouse");
            if(!result3.Succeeded)
            {
                return BadRequest(result3.Errors);
            }

            var result4 = await _userManager.AddToRoleAsync(user2, "Sales");
            if (!result4.Succeeded)
            {
                return BadRequest(result4.Errors);
            }

            var result5 = await _userManager.AddToRoleAsync(user3, "Sales");
            if (!result5.Succeeded)
            {
                return BadRequest(result5.Errors);
            }

            return Ok();

        }
    }
}
