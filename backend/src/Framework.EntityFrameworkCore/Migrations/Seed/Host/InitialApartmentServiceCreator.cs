using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.EntityFrameworkCore;
using Framework.AService;

namespace Framework.Migrations.Seed.Host
{
    internal class InitialApartmentServiceCreator
    {
        private readonly FrameworkDbContext _context;

        public InitialApartmentServiceCreator(FrameworkDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            var park = _context.AparmentServices.FirstOrDefault(p => p.TypeService == "An ninh");
            if (park == null)
            {
                _context.AparmentServices.Add(
                    new ApartmentService
                    {
                        ServiceName = "Gửi xe máy theo tháng",
                        Describe = " ",
                        ServiceCharge = 300000,
                        Cycle = " ",
                        TypeService = "An ninh",
                        ResponsibleUnit = " "
                    });
            }
        }
    }
}
