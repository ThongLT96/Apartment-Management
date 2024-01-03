using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using Framework.Authorization.Accounts;
using Framework.Chat;

namespace Framework.Authorization.Users
{
    public interface IUserEmailer
    {
        // Send email activation OTP to user's email address
        Task SendEmailActivationOTPAsync(User user, string plainPassword = null);

        Task SendOTPToEmailAsync(EmailAddressOTP emailAddressOTP);

        // Send and OTP to user's email address
        Task SendPasswordResetOTPAsync(User user);

        // Send an email to respond whether the account is approved or rejected
        Task SendRegisterResponseEmail(User user, List<string>RejectedReasons = null);

        /// <summary>
        /// Send email activation link to user's email address.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Email activation link</param>
        /// <param name="plainPassword">
        /// Can be set to user's plain password to include it in the email.
        /// </param>
        Task SendEmailActivationLinkAsync(User user, string link, string plainPassword = null);

        /// <summary>
        /// Sends a password reset link to user's email.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Password reset link (optional)</param>
        Task SendPasswordResetLinkAsync(User user, string link = null);

        /// <summary>
        /// Sends an email for unread chat message to user's email.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="senderUsername"></param>
        /// <param name="senderTenancyName"></param>
        /// <param name="chatMessage"></param>
        Task TryToSendChatMessageMail(User user, string senderUsername, string senderTenancyName, ChatMessage chatMessage);
    }
}
