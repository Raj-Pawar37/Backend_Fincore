using Backend_Fincore.Application.Interface;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Backend_Fincore.Infrastucture.Service
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor accessor;

        public CurrentUserService(IHttpContextAccessor accessor)
        {
            this.accessor = accessor;
        }

        public int UserId =>
            int.Parse(accessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        public string? UserName =>
            accessor.HttpContext!.User.FindFirst(ClaimTypes.Name)?.Value;

        public string? Email =>
            accessor.HttpContext!.User.FindFirst(ClaimTypes.Email)?.Value;

        public int MasterId =>
            int.Parse(accessor.HttpContext!.User.FindFirst("masterId")!.Value);

        public string? MasterType =>
            accessor.HttpContext!.User.FindFirst("masterType")?.Value;

        public int RoleId =>
            int.Parse(accessor.HttpContext!.User.FindFirst(ClaimTypes.Role)!.Value);
    }
}