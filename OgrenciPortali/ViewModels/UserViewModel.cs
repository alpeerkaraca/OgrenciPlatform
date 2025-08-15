using Shared.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace OgrenciPortali.ViewModels
{
    /// <summary>
    /// Kullanıcı kayıt işlemi için view model
    /// </summary>
    public class RegisterViewModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
        public string Password { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid AdvisorId { get; set; }
        public string StudentNo { get; set; }
        public SelectList RolesList { get; set; }
        public SelectList DepartmentsList { get; set; }
        public SelectList AdvisorsList { get; set; }
    }

    /// <summary>
    /// Şifre değiştirme işlemi için view model
    /// </summary>
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Mevcut şifreniz gerekli.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mevcut Şifre")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Yeni şifre gerekli.")]
        [StringLength(100, ErrorMessage = "Şifre en az {2} karakter olmalıdır.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage =
                "Şifre en az 8 karakter olmalı ve en az 1 büyük harf, 1 küçük harf, 1 rakam ve 1 özel karakter içermelidir.")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifreyi Onayla")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string ConfirmPassword { get; set; }
    }


    /// <summary>
    /// Kullanıcı güncelleme işlemi için view model
    /// </summary>
    public class UpdateUserViewModel : BaseClass
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
        public string StudentNo { get; set; }

        public Guid? DepartmentId { get; set; }
        public Guid? AdvisorId { get; set; }
        public IEnumerable<SelectListItem> RolesList { get; set; }
        public IEnumerable<SelectListItem> DepartmentsList { get; set; }
        public IEnumerable<SelectListItem> AdvisorsList { get; set; }
    }

    //// <summary>
    /// Kullanıcı giriş view modeli
    /// </summary>
    public class LoginUserViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}