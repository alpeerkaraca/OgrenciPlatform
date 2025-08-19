using System;

namespace Shared.DTO
{
    public class BaseClass
    {
        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }


        public string DeletedBy { get; set; }
    }
}