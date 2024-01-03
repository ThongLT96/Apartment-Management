using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.Runtime.Security;
using Abp.Runtime.Validation;
using Framework.Authorization.Users;
using Microsoft.Extensions.Options;

namespace Framework.Authorization.Accounts.Dto
{
    public class ResetPasswordInput: IShouldNormalize
    {
        public long UserId { get; set; }

        public string ResetCode { get; set; }

        [StringLength(UserConsts.MaxPlainPasswordLength, MinimumLength = UserConsts.MinPlainPasswordLength,
            ErrorMessage = "Mật khẩu phải có 6 đến 20 ký tự")]
        [DisableAuditing]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public string SingleSignIn { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(AbpUserBase.MaxEmailAddressLength, ErrorMessage = "Email quá dài")]
        public string EmailAddress { get; set; }

        public string OTP { get; set; }

        /// <summary>
        /// Encrypted values for {TenantId}, {UserId} and {ResetCode}
        /// </summary>
        public string c { get; set; }

        public void Normalize()
        {
            ResolveParameters();
        }

        protected virtual void ResolveParameters()
        {
            if (!string.IsNullOrEmpty(c))
            {
                var parameters = SimpleStringCipher.Instance.Decrypt(c);
                var query = HttpUtility.ParseQueryString(parameters);

                if (query["userId"] != null)
                {
                    UserId = Convert.ToInt32(query["userId"]);
                }

                if (query["resetCode"] != null)
                {
                    //ResetCode = query["resetCode"];
                }
            }
        }
    }
}