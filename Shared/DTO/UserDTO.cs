using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Message { get; set; }
    }


    public class RegisterDataDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
        public string Password { get; set; }
        public int? StudentYear { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? AdvisorId { get; set; }
        public IEnumerable<SelectListItem> RolesList { get; set; }
        public IEnumerable<SelectListItem> DepartmentsList { get; set; }
        public IEnumerable<SelectListItem> AdvisorsList { get; set; }
    }

    public class ChangePasswordDTO
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class ForgotPasswordRequestDto
    {
        public string Email { get; set; }
    }

    public class ResetPasswordRequestDto
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class SsoLoginRequestDTO
    {
        public string Email { get; set; }
        public string Name { get; set; }

        public static Tuple<string, string> ParseNameCompatible(string fullName)
        {
            // 1. Gelen verinin null veya boş olup olmadığını kontrol et
            if (string.IsNullOrWhiteSpace(fullName))
            {
                // Eski Tuple syntax'ı ile boş bir sonuç dön
                return new Tuple<string, string>("", "");
            }

            // 2. İsmi boşluklara göre kelimelere ayır
            var parts = fullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // 3. Kelime sayısına göre if/else ile kontrol et
            if (parts.Length == 0)
            {
                return new Tuple<string, string>("", "");
            }
            else if (parts.Length == 1)
            {
                // Tek kelime varsa Ad olarak kabul et
                return new Tuple<string, string>(parts[0], "");
            }
            else
            {
                // 4. Son kelimeyi Soyad olarak al (LINQ ile)
                string surname = parts.Last();

                // 5. Son kelime hariç diğer tüm kelimeleri Ad olarak birleştir (LINQ ile)
                string name = string.Join(" ", parts.Take(parts.Length - 1));

                return new Tuple<string, string>(name, surname);
            }
        }
    }
}