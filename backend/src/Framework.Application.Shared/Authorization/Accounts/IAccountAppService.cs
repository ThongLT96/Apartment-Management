using System.Threading.Tasks;
using Abp.Application.Services;
using Framework.Authorization.Accounts.Dto;

namespace Framework.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<int?> ResolveTenantId(ResolveTenantIdInput input);

        Task<RegisterOutput> Register(RegisterInput input);

        Task<RegisterOutput> RegisterByPhoneNumber(RegisterInput input);

        Task<bool> EmailAddressIsExist(CheckEmailExistInput input);

        Task SendPasswordResetOTP(SendPasswordResetCodeInput input);

        Task<ResetPasswordOutput> ResetPassword(ResetPasswordInput input);

        Task SendEmailActivationLink(SendEmailActivationLinkInput input);

        Task ActivateEmail(ActivateEmailInput input);

        Task<ImpersonateOutput> Impersonate(ImpersonateInput input);

        Task<ImpersonateOutput> BackToImpersonator();

        Task<SwitchToLinkedAccountOutput> SwitchToLinkedAccount(SwitchToLinkedAccountInput input);

        Task SendEmailActivationOTP(RegisterInput input);

        Task SendEmailAddressOTP(RegisterByEmailInput input);

        Task<ConfirmEmailAddressOTPOutput> ConfirmEmailAddressOTP(ConfirmEmailAddressOTPInput input);

        Task<RegisterOutput> RegisterByEmailAddress(RegisterInput input);
    }
}
