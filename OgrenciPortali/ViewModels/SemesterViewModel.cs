using System;
using OgrenciPortali.Models;

namespace OgrenciPortali.ViewModels
{
    public class SemesterListViewModel : BaseClass
    {
        public Guid SemesterId { get; set; }
        public string SemesterName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class SemesterAddViewModel : BaseClass
    {
        public string SemesterName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class SemesterUpdateviewModel : BaseClass
    {
        public Guid SemesterId { get; set; }
        public string SemesterName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}