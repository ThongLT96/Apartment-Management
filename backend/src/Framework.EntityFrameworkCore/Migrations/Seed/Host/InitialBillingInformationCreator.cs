using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.EntityFrameworkCore;
using Framework.Bill;

namespace Framework.Migrations.Seed.Host
{
    internal class InitialBillingInformationCreator
    {
        private readonly FrameworkDbContext _context;
        public InitialBillingInformationCreator(FrameworkDbContext context)
        {
            _context = context;
        }
        public void Create()
        {
            var bill = _context.BillingInformations.FirstOrDefault(p => p.AccountName == "Phong");
            if (bill == null)
            {
                _context.BillingInformations.Add(
                    new BillingInformation
                    {
                        AccountName = "Phong",
                        AccountNumber = "123456789",
                        Bank = "Vietcombank",
                        Content = "abc",
                        DayTrading = new DateTime(2022, 11, 28)
                    });

            }
            var abill = _context.BillingInformations.FirstOrDefault(p => p.AccountName == "Khanh");
            if (abill == null)
            {
                _context.BillingInformations.Add(
                    new BillingInformation
                    {
                        AccountName = "Khanh",
                        AccountNumber = "987654321",
                        Bank = "Viettinbank",
                        Content = "abcxyz",
                        DayTrading = new DateTime(2022, 11, 28)
                    });
            }
        }
    }
}
