using System.ComponentModel.DataAnnotations;

namespace Framework.Authorization.Accounts.Dto
{
    public class CheckPasswordResetOTPOutput
    {
        public string EmailAddress { get; set; }

        public bool OTPIsCorrect { get; set; }

        public string Message { get; set; }
    }
}
