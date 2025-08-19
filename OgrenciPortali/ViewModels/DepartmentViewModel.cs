using System;
using System.Collections.Generic;
using Shared.DTO;

namespace OgrenciPortali.ViewModels
{
    public class DepartmentListViewModel
    {
        public List<ListDepartmentsDTO> Departments { get; set; }
    }

    public class DepartmentEditViewModel
    {
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }
        public bool IsActive { get; set; }
    }

    public class DepartmentAddViewModel
    {
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }
    }
}