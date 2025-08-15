using System;

namespace OgrenciPlatform.Shared.DTO
{
    public class ListDepartmentsDTO
    {
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }
        public bool IsActive { get; set; }
    }


    public class AddDepartmentDTO
    {
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }
    }

    public class EditDepartmentDTO
    {
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }
        public bool IsActive { get; set; }
    }
}