﻿using Framework.EntityFrameworkCore;

namespace Framework.Migrations.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly FrameworkDbContext _context;

        public InitialHostDbBuilder(FrameworkDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();
            //Apartment Service
            new InitialApartmentServiceCreator(_context).Create();

            //Bill
            new InitialServiceBillCreator(_context).Create();
            new InitialBillingInformationCreator(_context).Create();

            _context.SaveChanges();
        }
    }
}
