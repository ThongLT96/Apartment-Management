using System.Linq;
using Abp.Configuration;
using Abp.Localization;
using Abp.MultiTenancy;
using Abp.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Framework.EntityFrameworkCore;
using Abp.Runtime.Security;
using Framework.MultiTenancy;

namespace Framework.Migrations.Seed.Host
{
    public class DefaultSettingsCreator
    {
        private readonly FrameworkDbContext _context;

        public DefaultSettingsCreator(FrameworkDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            int? tenantId = null;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (!FrameworkConsts.MultiTenancyEnabled)
#pragma warning disable 162
            {
                tenantId = MultiTenancyConsts.DefaultTenantId;
            }
#pragma warning restore 162

            //Emailing
            // Mail sinh viên vẫn còn dùng nha :( làm ơn đừng ai lấy nghịch
            AddSettingIfNotExists(EmailSettingNames.DefaultFromAddress, "20520194@gm.uit.edu.vn", tenantId);
            AddSettingIfNotExists(EmailSettingNames.DefaultFromDisplayName, "SE214.N12.PMCL Host", tenantId);
            AddSettingIfNotExists(EmailSettingNames.Smtp.Host, "smtp.gmail.com", tenantId);
            AddSettingIfNotExists(EmailSettingNames.Smtp.Port, "587", tenantId);
            AddSettingIfNotExists(EmailSettingNames.Smtp.UserName, "20520194@gm.uit.edu.vn", tenantId);
            AddSettingIfNotExists(EmailSettingNames.Smtp.Password, "7oViEJyZwB58LhQS7w6uKQ==", tenantId);
            AddSettingIfNotExists(EmailSettingNames.Smtp.Domain, "", tenantId);
            AddSettingIfNotExists(EmailSettingNames.Smtp.EnableSsl, "true", tenantId);
            AddSettingIfNotExists(EmailSettingNames.Smtp.UseDefaultCredentials, "false", tenantId);

            //Languages
            AddSettingIfNotExists(LocalizationSettingNames.DefaultLanguage, "vi", tenantId);
        }

        private void AddSettingIfNotExists(string name, string value, int? tenantId = null)
        {
            if (_context.Settings.IgnoreQueryFilters().Any(s => s.Name == name && s.TenantId == tenantId && s.UserId == null))
            {
                return;
            }

            _context.Settings.Add(new Setting(tenantId, null, name, value));
            _context.SaveChanges();
        }
    }
}