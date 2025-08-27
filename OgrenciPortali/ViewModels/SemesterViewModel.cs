using Shared.DTO;
using System;
using System.ComponentModel.DataAnnotations;
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
        [Display(Name = "Başlangıç Tarihi")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Bitiş Tarihi")]
        [DataType(DataType.Date)] // Bu satır, verinin sadece bir tarih olduğunu belirtir.
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)] // Bu satır, düzenleme modunda formatı zorunlu kılar.
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}