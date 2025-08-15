using System;
using System.Web;
using Shared.DTO;

namespace OgrenciPortali.Utils
{
    public class Utils
    {
        public static Guid? GetActiveUserId()
        {
            try
            {
                var user = HttpContext.Current.User;
                if (user == null || !user.Identity.IsAuthenticated) return null;
                var userIdClaim = user.Identity.Name;
                if (Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return userId;
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static void SetAuditFieldsForCreate(BaseClass entitiy)
        {
            entitiy.CreatedAt = DateTime.Now;
            entitiy.UpdatedAt = DateTime.Now;
            entitiy.CreatedBy = GetActiveUserId().ToString();
            entitiy.UpdatedBy = GetActiveUserId().ToString();
            entitiy.IsDeleted = false;
        }

        public static void SetAuditFieldsForUpdate(BaseClass entitiy)
        {
            entitiy.UpdatedAt = DateTime.Now;
            entitiy.UpdatedBy = GetActiveUserId().ToString();
        }

        public static void SetAuditFieldsForDelete(BaseClass entitiy)
        {
            entitiy.DeletedAt = DateTime.Now;
            entitiy.DeletedBy = GetActiveUserId().ToString();
            entitiy.IsDeleted = true;
        }
    }
}