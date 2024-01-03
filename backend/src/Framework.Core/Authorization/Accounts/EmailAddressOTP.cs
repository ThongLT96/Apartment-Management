using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;
namespace Framework.Authorization.Accounts
{
    [Table("EmailAddressOTP")]
    public class EmailAddressOTP : FullAuditedEntity
    {
        public virtual string EmailAddress { get; set; }
        public virtual string OTP { get; set; }
        public virtual bool IsConfirmed { get; set; }
    }
}
