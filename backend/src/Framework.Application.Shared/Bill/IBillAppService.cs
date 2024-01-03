using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.Application.Services;
using Framework.AService.Dto;
using System.Threading.Tasks;
using Framework.Dto;
using Framework.Admin.Dtos;
using Framework.Bill.Dto;

namespace Framework.Bill
{
    public interface IBillAppService : IApplicationService
    {
        //Create
        //Task CreateEAndWBill(CreateEAndWBillInput input);
        //Task CreateManageBill(CreateManageBillInput input);
        Task CreateServiceBill(CreateServiceBillInput input);
        Task CreateApartmentBill_EW(CreateApartmentBill input);
        Task CreateApartmentBill_Manage(CreateApartmentBill_Manage input);
        Task CreateInServiceRegister(CreateInServiceRegisterInput input);

        //Delete
        //Task DeleteEAndWBill(EntityDto input);
        //Task DeleteManageBill(EntityDto input);
        Task DeleteApartmentBill(EntityDto input);
        Task DeleteServiceBill(EntityDto input);
        Task MakeApartmentBillWaiting(NeedUpdateBillInput input);
        Task MakeServiceBillWaiting(NeedUpdateBillInput input);
        Task MakeApartmentBillToDo(NeedUpdateBillInput input);
        Task MakeServiceBillToDo(NeedUpdateBillInput input);

    }
}
