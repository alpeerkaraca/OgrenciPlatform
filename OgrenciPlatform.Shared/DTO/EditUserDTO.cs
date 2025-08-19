using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OgrenciPlatform.Shared.DTO
{
    public class EditUserDTO : BaseClass
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public Role { get; set; }
        public string StudentNo { get; set; }

        public Guid? DepartmentId { get; set; }
        public Guid? AdvisorId { get; set; }
        public IEnumerable<SelectListItem> RolesList { get; set; }
        public IEnumerable<SelectListItem> DepartmentsList { get; set; }
        public IEnumerable<SelectListItem> AdvisorsList { get; set; }
    }
}