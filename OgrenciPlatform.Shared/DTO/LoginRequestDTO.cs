using System;

namespace OgrenciPlatform.Shared.DTO
{
    internal class LoginRequestDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    internal class LoginSuccessResponse
    {
        public string Token { get; set; }
        public string Message { get; set; }
    }

    internal class ChangePasswordRequestDTO
    {
        public Guid UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}