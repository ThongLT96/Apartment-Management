using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Framework.Feedback
{
    [Table("AbpFeedback")]
    public class UserFeedBack: FullAuditedEntity
    {

        [Required]
        [StringLength(256)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }

        [Required]
        public string Content { get; set; }

        [StringLength(200)]
        public string Respond { get; set; }

        public string Image { get; set; }

    }
}
