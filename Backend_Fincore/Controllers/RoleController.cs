
using Microsoft.AspNetCore.Mvc;
using Backend_Fincore.Models;
using Backend_Fincore.DTOs;
using Backend_Fincore.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;


namespace Backend_Fincore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        AppDbContext db;
        private readonly IMapper mapper;

        public RoleController(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        [HttpPost("/ap/v1/roles")]
        public async Task<ActionResult<Role>> CreateRole(RoleDTO dto)
        {
            var role = mapper.Map<Role>(dto);
            db.Role.Add(role);
            await db.SaveChangesAsync();
            return Ok(new {message= "Role created successfully",
                role });
            }
    }
}
