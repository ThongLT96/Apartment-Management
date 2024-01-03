using Abp.Domain.Repositories;
using Framework.Feedback;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.InterfaceFeedback;
using Framework.Bill;
using Abp.Runtime.Session;
using Framework.Authorization.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Framework.Feedback.Dto;
using AutoMapper;
using Abp.Application.Services.Dto;
using Framework.Bill.Dto;
using Abp.Collections.Extensions;

namespace Framework.UserFeedback
{
    public class UserFeedbackAppService: FrameworkAppServiceBase, IFeedbackAppService
    {
        private readonly IRepository<UserFeedBack> _feedbackRepository;
        public UserManager _UserManager { get; set; }
        public IAbpSession _AbpSession { get; set; }

        public UserFeedbackAppService(
            IRepository<UserFeedBack> feedbackRepository,
            UserManager UserManager,
            IAbpSession AbpSession
            )
        {
            _feedbackRepository= feedbackRepository;
            _UserManager = UserManager;
            _AbpSession = AbpSession;
        }

        public async Task<long> AddFeedback(UserFeedbackDto input)
        {
            var mappedInput = ObjectMapper.Map<UserFeedBack>(input);
            return await _feedbackRepository.InsertAndGetIdAsync(mappedInput);
        }
        public async Task<ListResultDto<UserFeedbackWithIdDto>> GetCurrentUserFeedback()
        {
            var currentUser = await UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());

            var feedbacksByCurrentEmail = _feedbackRepository.GetAll()
            .WhereIf(
                true,
                p => p.Email.Equals(currentUser.EmailAddress)
            )
            .OrderBy(p => p.Email)
            .ToList();

            return new ListResultDto<UserFeedbackWithIdDto>(ObjectMapper.Map<List<UserFeedbackWithIdDto>>(feedbacksByCurrentEmail));
        }
        public async Task UpdateFeedback(UserFeedbackWithIdDto request)
        {
            var userFeedBack = await _feedbackRepository.GetAsync(request.Id);

            userFeedBack.Type = request.Type;   
            userFeedBack.Title = request.Title;
            userFeedBack.Time = request.Time;
            userFeedBack.Content = request.Content;
            userFeedBack.Status = request.Status;
            userFeedBack.Respond = request.Respond;

            await _feedbackRepository.UpdateAsync(userFeedBack);
        }

        public async Task DeleteFeedback(int id)
        {
            await _feedbackRepository.DeleteAsync(id);
        }
        public async Task<ListResultDto<UserFeedbackWithIdDto>> GetAllFeedback()
        {
            var feedbackList = await _feedbackRepository.GetAll().ToListAsync();
            return new ListResultDto<UserFeedbackWithIdDto>(ObjectMapper.Map<List<UserFeedbackWithIdDto>>(feedbackList));
        }
    }
   


}
