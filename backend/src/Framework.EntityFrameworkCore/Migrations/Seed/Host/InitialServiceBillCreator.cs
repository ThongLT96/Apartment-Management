using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.EntityFrameworkCore;
using Framework.Bill;

namespace Framework.Migrations.Seed.Host
{
    internal class InitialServiceBillCreator
    {
        private readonly FrameworkDbContext _context;

        public InitialServiceBillCreator(FrameworkDbContext context)
        {
            _context = context;
        }
        public void Create()
        {
            var Managebill = _context.ServiceBills.FirstOrDefault(p => p.State == "Chưa thanh toán");
            if (Managebill == null)
            {
                _context.ServiceBills.Add(
                    new ServiceBill
                    {
                        BillID = "DV2511001",
                        BillName = "Hóa đơn thuê sân bóng",
                        ServiceName = "Dịch vụ bóng đá theo ngày",
                        EmailAddress = "ptp@gmail.com",
                        OwnerName = "Phong",
                        CreateName = "Dang",
                        CreateDay = new DateTime(2022, 11, 25),
                        StartDay = new DateTime(2022, 11, 25),
                        EndDay = new DateTime(2022, 11, 25),
                        Price = 300000,
                        PaymentTerm = new DateTime(2022, 11, 25),
                        State = "Chưa thanh toán"
                    });
            }
        }
    }
}
