using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Localization;
using Abp.Net.Mail;
using Framework.Chat;
using Framework.Editions;
using Framework.Localization;
using Framework.MultiTenancy;
using System.Net.Mail;
using System.Web;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Abp.UI;
using Framework.Net.Emailing;
using static Abp.Net.Mail.EmailSettingNames;
using Abp.Net.Mail.Smtp;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using MailKit.Security;
using MimeKit;
using Framework.Authorization.Accounts;

namespace Framework.Authorization.Users
{
    /// <summary>
    /// Used to send email to users.
    /// </summary>
    public class UserEmailer : FrameworkServiceBase, IUserEmailer, ITransientDependency
    {
        private readonly IEmailTemplateProvider _emailTemplateProvider;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly ICurrentUnitOfWorkProvider _unitOfWorkProvider;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ISettingManager _settingManager;
        private readonly EditionManager _editionManager;
        private readonly UserManager _userManager;
        private readonly IAbpSession _abpSession;

        // used for styling action links on email messages.
        private string _emailButtonStyle =
            "padding-left: 30px; padding-right: 30px; padding-top: 12px; padding-bottom: 12px; color: #ffffff; background-color: #00bb77; font-size: 14pt; text-decoration: none;";
        private string _emailButtonColor = "#00bb77";

        public UserEmailer(
            IEmailTemplateProvider emailTemplateProvider,
            IEmailSender emailSender,
            IRepository<Tenant> tenantRepository,
            ICurrentUnitOfWorkProvider unitOfWorkProvider,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager,
            EditionManager editionManager,
            UserManager userManager,
            IAbpSession abpSession)
        {
            _emailTemplateProvider = emailTemplateProvider;
            _emailSender = emailSender;
            _tenantRepository = tenantRepository;
            _unitOfWorkProvider = unitOfWorkProvider;
            _unitOfWorkManager = unitOfWorkManager;
            _settingManager = settingManager;
            _editionManager = editionManager;
            _userManager = userManager;
            _abpSession = abpSession;
        }

        /*
        *   01-2024 Lo Tri Thong
        *   From here
        */
        public virtual async Task SendEmailActivationOTPAsync(User user, string plainPassword = null)
        {
            await CheckMailSettingsEmptyOrNull();

            if (user.EmailConfirmationCode.IsNullOrEmpty())
            {
                throw new Exception("EmailConfirmationCode không tồn tại");
            }

            var subject = "[Apato] Xác nhận Email đăng ký tài khoản Apato";
            var mailBody = new StringBuilder();

            mailBody.AppendLine("Chào, " + user.FullName + "<br>");
            mailBody.AppendLine("Đây là mã kích hoạt tài khoản của bạn: <b>" + SimpleStringCipher.Instance.Decrypt(user.EmailConfirmationCode) + "</b>");
            
            mailBody.AppendLine("<br><br>Dùng OTP này để hoàn thành đăng ký tài khoản sử dụng Apato của bạn");
            mailBody.AppendLine("<br>Sau khi hoàn thành đăng ký vui lòng chờ một thời gian để ban quản trị phê duyệt.");
            mailBody.AppendLine("<br>Lưu ý: <ul><li>Giữ kĩ mật khẩu không để lọt vào tay người khác</li>");
            mailBody.AppendLine("<li>Ban quản lí hay ban quản trị đều không cần biết mật khẩu của bạn.</li>");
            
            await SendEmail(user.EmailAddress, subject, mailBody);
        }

        public virtual async Task SendOTPToEmailAsync(EmailAddressOTP emailAddressOTP)
        {
            await CheckMailSettingsEmptyOrNull();

            if (emailAddressOTP.IsConfirmed)
            {
                throw new Exception("Email đã được xác thực");
            }

            var subject = "[Apato] Xác nhận Email đăng ký tài khoản Apato";
            var mailBody = new StringBuilder();

            mailBody.AppendLine("Xin chào thành viên mới,<br>");
            mailBody.AppendLine("Đây là mã kích hoạt tài khoản của bạn: <b>" + SimpleStringCipher.Instance.Decrypt(emailAddressOTP.OTP) + "</b>");

            mailBody.AppendLine("<br><br>Dùng OTP này để tiến hành đăng ký tài khoản sử dụng Apato của bạn");
            mailBody.AppendLine("<br>Sau khi hoàn thành đăng ký vui lòng chờ một thời gian để ban quản trị phê duyệt.");
            mailBody.AppendLine("<br>Lưu ý: <ul><li>Giữ kĩ mật khẩu không để lọt vào tay người khác</li>");
            mailBody.AppendLine("<li>Ban quản lí hay ban quản trị đều không cần biết mật khẩu của bạn.</li>");

            await SendEmail(emailAddressOTP.EmailAddress, subject, mailBody);
        }

        public virtual async Task SendPasswordResetOTPAsync(User user)
        {
            await CheckMailSettingsEmptyOrNull();

            if (user.PasswordResetCode.IsNullOrEmpty())
            {
                throw new Exception("PasswordResetCode không tồn tại.");
            }

            var subject = "[Apato] Xác nhận Email đã đăng ký để sử dụng phần mềm";
            var mailBody = new StringBuilder();

            mailBody.AppendLine("Chào, " + user.FullName + "<br>");
            mailBody.AppendLine("Đây là OTP xác nhận email của bạn: <b>" + SimpleStringCipher.Instance.Decrypt(user.PasswordResetCode) + "</b>");

            mailBody.AppendLine("<br><br>Dùng OTP này để tiến hành đặt lại mật khẩu cho tài khoản Apato của bạn.");
            mailBody.AppendLine("<br>Lưu ý: <ul><li>Giữ kĩ mật khẩu không để lọt vào tay người khác</li>");
            mailBody.AppendLine("<li>Ban quản lí hay ban quản trị đều không cần biết mật khẩu của bạn.</li>");

            await SendEmail(user.EmailAddress, subject, mailBody);
        }

        public async Task SendRegisterResponseEmail(User user, List<string> RejectedReadsons = null)
        {
            await CheckMailSettingsEmptyOrNull();

            string subject = "[Apato] ";
            var mailBody = new StringBuilder();

            if (user.IsActive)
            {
                subject += "Thông tin đăng ký của bạn đã được phê duyệt thành công";

                mailBody.AppendLine("Xin chào, " + user.FullName + "<br>");
                mailBody.AppendLine("Thông tin đăng ký tài khoản Apato của bạn đã được phê duyệt thành công, bạn đã có thể đăng nhập và sử dụng các dịch vụ của Apato" + "</b>");
                mailBody.AppendLine("<br><br>");
                mailBody.AppendLine("Lưu ý: <ul><li>Giữ kĩ mật khẩu không để lọt vào tay người khác</li>");
                mailBody.AppendLine("<li>Ban quản lí hay ban quản trị đều không cần biết mật khẩu của bạn.</li></ul>");
            }
            else
            {
                subject += "Rất tiếc, thông tin đăng ký của bạn không được chấp nhận";

                mailBody.AppendLine("Xin chào, " + user.FullName + "<br>");
                mailBody.AppendLine("Thông tin đăng ký của bạn không được chấp nhận vì các lý do sau:" + "</b>");
                mailBody.AppendLine("<ul>");

                foreach (var reason in RejectedReadsons)
                {
                    mailBody.Append("<li>" + reason + "</li>");
                }

                mailBody.AppendLine("</ul");
                mailBody.AppendLine("<br>Để có thể sử dụng các dịch vụ của Apato, bạn vui lòng khắc phục cái lý do trên và tiến hành đăng ký lại tại khoản.<br>");
            }

            mailBody.AppendLine("<br><i>[Chữ ký]</i>");

            await SendEmail(user.EmailAddress, subject, mailBody);
        }

        /*
        *   01-2024 Lo Tri Thong
        *   To here
        */


        /// <summary>
        /// Send email activation link to user's email address.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Email activation link</param>
        /// <param name="plainPassword">
        /// Can be set to user's plain password to include it in the email.
        /// </param>
        [UnitOfWork]
        public virtual async Task SendEmailActivationLinkAsync(User user, string link, string plainPassword = null)
        {
            await CheckMailSettingsEmptyOrNull();

            if (user.EmailConfirmationCode.IsNullOrEmpty())
            {
                throw new Exception("EmailConfirmationCode should be set in order to send email activation link.");
            }

            link = link.Replace("{userId}", user.Id.ToString());
            link = link.Replace("{confirmationCode}", Uri.EscapeDataString(user.EmailConfirmationCode));

            if (user.TenantId.HasValue)
            {
                link = link.Replace("{tenantId}", user.TenantId.ToString());
            }

            link = EncryptQueryParameters(link);

            var tenancyName = GetTenancyNameOrNull(user.TenantId);
            var emailTemplate = GetTitleAndSubTitle(user.TenantId, L("EmailActivation_Title"), L("EmailActivation_SubTitle"));
            var mailMessage = new StringBuilder();

            mailMessage.AppendLine("<b>" + L("NameSurname") + "</b>: " + user.Surname + " " + user.Name + "<br />");

            if (!tenancyName.IsNullOrEmpty())
            {
                mailMessage.AppendLine("<b>" + L("TenancyName") + "</b>: " + tenancyName + "<br />");
            }

            mailMessage.AppendLine("<b>" + L("UserName") + "</b>: " + user.UserName + "<br />");

            if (!plainPassword.IsNullOrEmpty())
            {
                mailMessage.AppendLine("<b>" + L("Password") + "</b>: " + plainPassword + "<br />");
            }

            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine(L("EmailActivation_ClickTheLinkBelowToVerifyYourEmail") + "<br /><br />");
            mailMessage.AppendLine("<a style=\"" + _emailButtonStyle + "\" bg-color=\"" + _emailButtonColor + "\" href=\"" + link + "\">" + L("Verify") + "</a>");
            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine("<br />");
            mailMessage.AppendLine("<span style=\"font-size: 9pt;\">" + L("EmailMessage_CopyTheLinkBelowToYourBrowser") + "</span><br />");
            mailMessage.AppendLine("<span style=\"font-size: 8pt;\">" + link + "</span>");

            await ReplaceBodyAndSend(user.EmailAddress, L("EmailActivation_Subject"), emailTemplate, mailMessage);
        }

        /// <summary>
        /// Sends a password reset link to user's email.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Reset link</param>
        public async Task SendPasswordResetLinkAsync(User user, string link = null)
        {
            await CheckMailSettingsEmptyOrNull();

            if (user.PasswordResetCode.IsNullOrEmpty())
            {
                throw new Exception("PasswordResetCode should be set in order to send password reset link.");
            }

            var tenancyName = GetTenancyNameOrNull(user.TenantId);
            var emailTemplate = GetTitleAndSubTitle(user.TenantId, L("PasswordResetEmail_Title"), L("PasswordResetEmail_SubTitle"));
            var mailMessage = new StringBuilder();

            mailMessage.AppendLine("<b>" + L("NameSurname") + "</b>: " + user.Name + " " + user.Surname + "<br />");

            if (!tenancyName.IsNullOrEmpty())
            {
                mailMessage.AppendLine("<b>" + L("TenancyName") + "</b>: " + tenancyName + "<br />");
            }

            mailMessage.AppendLine("<b>" + L("UserName") + "</b>: " + user.UserName + "<br />");
            mailMessage.AppendLine("<b>" + L("ResetCode") + "</b>: " + user.PasswordResetCode + "<br />");

            if (!link.IsNullOrEmpty())
            {
                link = link.Replace("{userId}", user.Id.ToString());
                link = link.Replace("{resetCode}", Uri.EscapeDataString(user.PasswordResetCode));

                if (user.TenantId.HasValue)
                {
                    link = link.Replace("{tenantId}", user.TenantId.ToString());
                }

                link = EncryptQueryParameters(link);

                mailMessage.AppendLine("<br />");
                mailMessage.AppendLine(L("PasswordResetEmail_ClickTheLinkBelowToResetYourPassword") + "<br /><br />");
                mailMessage.AppendLine("<a style=\"" + _emailButtonStyle + "\" bg-color=\"" + _emailButtonColor + "\" href=\"" + link + "\">" + L("Reset") + "</a>");
                mailMessage.AppendLine("<br />");
                mailMessage.AppendLine("<br />");
                mailMessage.AppendLine("<br />");
                mailMessage.AppendLine("<span style=\"font-size: 9pt;\">" + L("EmailMessage_CopyTheLinkBelowToYourBrowser") + "</span><br />");
                mailMessage.AppendLine("<span style=\"font-size: 8pt;\">" + link + "</span>");
            }

            await ReplaceBodyAndSend(user.EmailAddress, L("PasswordResetEmail_Subject"), emailTemplate, mailMessage);
        }

        public async Task TryToSendChatMessageMail(User user, string senderUsername, string senderTenancyName, ChatMessage chatMessage)
        {
            try
            {
                await CheckMailSettingsEmptyOrNull();

                var emailTemplate = GetTitleAndSubTitle(user.TenantId, L("NewChatMessageEmail_Title"), L("NewChatMessageEmail_SubTitle"));
                var mailMessage = new StringBuilder();

                mailMessage.AppendLine("<b>" + L("Sender") + "</b>: " + senderTenancyName + "/" + senderUsername + "<br />");
                mailMessage.AppendLine("<b>" + L("Time") + "</b>: " + chatMessage.CreationTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss") + " UTC<br />");
                mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + chatMessage.Message + "<br />");
                mailMessage.AppendLine("<br />");

                await ReplaceBodyAndSend(user.EmailAddress, L("NewChatMessageEmail_Subject"), emailTemplate, mailMessage);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        public async Task TryToSendSubscriptionExpireEmail(int tenantId, DateTime utcNow)
        {
            try
            {
                using (_unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        await CheckMailSettingsEmptyOrNull();

                        var tenantAdmin = await _userManager.GetAdminAsync();
                        if (tenantAdmin == null || string.IsNullOrEmpty(tenantAdmin.EmailAddress))
                        {
                            return;
                        }

                        var hostAdminLanguage = _settingManager.GetSettingValueForUser(LocalizationSettingNames.DefaultLanguage, tenantAdmin.TenantId, tenantAdmin.Id);
                        var culture = CultureHelper.GetCultureInfoByChecking(hostAdminLanguage);
                        var emailTemplate = GetTitleAndSubTitle(tenantId, L("SubscriptionExpire_Title"), L("SubscriptionExpire_SubTitle"));
                        var mailMessage = new StringBuilder();

                        mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + L("SubscriptionExpire_Email_Body", culture, utcNow.ToString("yyyy-MM-dd") + " UTC") + "<br />");
                        mailMessage.AppendLine("<br />");

                        await ReplaceBodyAndSend(tenantAdmin.EmailAddress, L("SubscriptionExpire_Email_Subject"), emailTemplate, mailMessage);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        public async Task TryToSendSubscriptionAssignedToAnotherEmail(int tenantId, DateTime utcNow, int expiringEditionId)
        {
            try
            {
                using (_unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        await CheckMailSettingsEmptyOrNull();

                        var tenantAdmin = await _userManager.GetAdminAsync();
                        if (tenantAdmin == null || string.IsNullOrEmpty(tenantAdmin.EmailAddress))
                        {
                            return;
                        }

                        var hostAdminLanguage = _settingManager.GetSettingValueForUser(LocalizationSettingNames.DefaultLanguage, tenantAdmin.TenantId, tenantAdmin.Id);
                        var culture = CultureHelper.GetCultureInfoByChecking(hostAdminLanguage);
                        var expringEdition = await _editionManager.GetByIdAsync(expiringEditionId);
                        var emailTemplate = GetTitleAndSubTitle(tenantId, L("SubscriptionExpire_Title"), L("SubscriptionExpire_SubTitle"));
                        var mailMessage = new StringBuilder();

                        mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + L("SubscriptionAssignedToAnother_Email_Body", culture, expringEdition.DisplayName, utcNow.ToString("yyyy-MM-dd") + " UTC") + "<br />");
                        mailMessage.AppendLine("<br />");

                        await ReplaceBodyAndSend(tenantAdmin.EmailAddress, L("SubscriptionExpire_Email_Subject"), emailTemplate, mailMessage);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        public async Task TryToSendFailedSubscriptionTerminationsEmail(List<string> failedTenancyNames, DateTime utcNow)
        {
            try
            {
                await CheckMailSettingsEmptyOrNull();

                var hostAdmin = await _userManager.GetAdminAsync();
                if (hostAdmin == null || string.IsNullOrEmpty(hostAdmin.EmailAddress))
                {
                    return;
                }

                var hostAdminLanguage = _settingManager.GetSettingValueForUser(LocalizationSettingNames.DefaultLanguage, hostAdmin.TenantId, hostAdmin.Id);
                var culture = CultureHelper.GetCultureInfoByChecking(hostAdminLanguage);
                var emailTemplate = GetTitleAndSubTitle(null, L("FailedSubscriptionTerminations_Title"), L("FailedSubscriptionTerminations_SubTitle"));
                var mailMessage = new StringBuilder();

                mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + L("FailedSubscriptionTerminations_Email_Body", culture, string.Join(",", failedTenancyNames), utcNow.ToString("yyyy-MM-dd") + " UTC") + "<br />");
                mailMessage.AppendLine("<br />");

                await ReplaceBodyAndSend(hostAdmin.EmailAddress, L("FailedSubscriptionTerminations_Email_Subject"), emailTemplate, mailMessage);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        public async Task TryToSendSubscriptionExpiringSoonEmail(int tenantId, DateTime dateToCheckRemainingDayCount)
        {
            try
            {
                using (_unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        await CheckMailSettingsEmptyOrNull();

                        var tenantAdmin = await _userManager.GetAdminAsync();
                        if (tenantAdmin == null || string.IsNullOrEmpty(tenantAdmin.EmailAddress))
                        {
                            return;
                        }

                        var tenantAdminLanguage = _settingManager.GetSettingValueForUser(LocalizationSettingNames.DefaultLanguage, tenantAdmin.TenantId, tenantAdmin.Id);
                        var culture = CultureHelper.GetCultureInfoByChecking(tenantAdminLanguage);

                        var emailTemplate = GetTitleAndSubTitle(null, L("SubscriptionExpiringSoon_Title"), L("SubscriptionExpiringSoon_SubTitle"));
                        var mailMessage = new StringBuilder();

                        mailMessage.AppendLine("<b>" + L("Message") + "</b>: " + L("SubscriptionExpiringSoon_Email_Body", culture, dateToCheckRemainingDayCount.ToString("yyyy-MM-dd") + " UTC") + "<br />");
                        mailMessage.AppendLine("<br />");

                        await ReplaceBodyAndSend(tenantAdmin.EmailAddress, L("SubscriptionExpiringSoon_Email_Subject"), emailTemplate, mailMessage);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        private string GetTenancyNameOrNull(int? tenantId)
        {
            if (tenantId == null)
            {
                return null;
            }

            using (_unitOfWorkProvider.Current.SetTenantId(null))
            {
                return _tenantRepository.Get(tenantId.Value).TenancyName;
            }
        }

        private StringBuilder GetTitleAndSubTitle(int? tenantId, string title, string subTitle)
        {
            var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(tenantId));
            emailTemplate.Replace("{EMAIL_TITLE}", title);
            emailTemplate.Replace("{EMAIL_SUB_TITLE}", subTitle);

            return emailTemplate;
        }

        private async Task ReplaceBodyAndSend(string emailAddress, string subject, StringBuilder emailTemplate, StringBuilder mailMessage)
        {
            emailTemplate.Replace("{EMAIL_BODY}", mailMessage.ToString());
            await SendEmail(emailAddress, subject, emailTemplate);
        }

        /*
        *   01-2024 Lo Tri Thong
        *   From here
        */   
        private async Task SendEmail(string receiverEmailAddress, string subject, StringBuilder mailBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                await SettingManager.GetSettingValueAsync(DefaultFromDisplayName),
                await SettingManager.GetSettingValueAsync(DefaultFromAddress)
                ));
            message.To.Add(new MailboxAddress("", receiverEmailAddress));
            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = mailBody.ToString()
            };

            using (var client = new SmtpClient())
            {
                client.Connect(
                    await SettingManager.GetSettingValueAsync(Smtp.Host),
                    await SettingManager.GetSettingValueAsync<int>(Smtp.Port),
                    SecureSocketOptions.StartTls
                    );

                var smtpPassword = await SettingManager.GetSettingValueAsync(Smtp.Password);

                client.Authenticate(
                    await SettingManager.GetSettingValueAsync(Smtp.UserName),
                    SimpleStringCipher.Instance.Decrypt(smtpPassword)
                    );

                client.Send(message);

                client.Disconnect(true);
            }
        }

        /*
        *   01-2024 Lo Tri Thong
        *   To here
        */

        /// <summary>
        /// Returns link with encrypted parameters
        /// </summary>
        /// <param name="link"></param>
        /// <param name="encrptedParameterName"></param>
        /// <returns></returns>
        private string EncryptQueryParameters(string link, string encrptedParameterName = "c")
        {
            if (!link.Contains("?"))
            {
                return link;
            }

            var basePath = link.Substring(0, link.IndexOf('?'));
            var query = link.Substring(link.IndexOf('?')).TrimStart('?');

            return basePath + "?" + encrptedParameterName + "=" + HttpUtility.UrlEncode(SimpleStringCipher.Instance.Encrypt(query));
        }

        private async Task CheckMailSettingsEmptyOrNull()
        {
#if DEBUG
            return;
#endif
            if (
                (await _settingManager.GetSettingValueAsync(EmailSettingNames.DefaultFromAddress)).IsNullOrEmpty() ||
                (await _settingManager.GetSettingValueAsync(EmailSettingNames.Smtp.Host)).IsNullOrEmpty()
            )
            {
                throw new UserFriendlyException(L("SMTPSettingsNotProvidedWarningText"));
            }

            if ((await _settingManager.GetSettingValueAsync<bool>(EmailSettingNames.Smtp.UseDefaultCredentials)))
            {
                return;
            }

            if (
                (await _settingManager.GetSettingValueAsync(EmailSettingNames.Smtp.UserName)).IsNullOrEmpty() ||
                (await _settingManager.GetSettingValueAsync(EmailSettingNames.Smtp.Password)).IsNullOrEmpty()
            )
            {
                throw new UserFriendlyException(L("SMTPSettingsNotProvidedWarningText"));
            }
        }
    }
}
