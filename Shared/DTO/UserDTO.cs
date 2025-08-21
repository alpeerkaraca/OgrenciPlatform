using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Shared.DTO
{
    public class ListUsersDTO
    {
        public List<UserDTO> Users { get; set; }
    }

    public class UserDTO
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string StudentNo { get; set; }
        public Roles Role { get; set; }
        public string DepartmentName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsFirstLogin { get; set; }
    }

    public class LoginUserDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginSuccessResponse
    {
        public string Token { get; set; }
        public string Message { get; set; }
    }


    public class RegisterDataDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
        public string Password { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? AdvisorId { get; set; }
        public string StudentNo { get; set; }
        public IEnumerable<SelectListItem> RolesList { get; set; }
        public IEnumerable<SelectListItem> DepartmentsList { get; set; }
        public IEnumerable<SelectListItem> AdvisorsList { get; set; }
    }

    public class ChangePasswordDTO
    {
        public Guid UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}