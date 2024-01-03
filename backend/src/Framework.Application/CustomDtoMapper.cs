﻿using Framework.Admin.Dtos;
using Framework.Admin;
using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.DynamicEntityProperties;
using Abp.EntityHistory;
using Abp.Localization;
using Abp.Notifications;
using Abp.Organizations;
using Abp.UI.Inputs;
using Abp.Webhooks;
using AutoMapper;
using IdentityServer4.Extensions;
using Framework.Auditing.Dto;
using Framework.Authorization.Accounts.Dto;
using Framework.Authorization.Delegation;
using Framework.Authorization.Permissions.Dto;
using Framework.Authorization.Roles;
using Framework.Authorization.Roles.Dto;
using Framework.Authorization.Users;
using Framework.Authorization.Users.Delegation.Dto;
using Framework.Authorization.Users.Dto;
using Framework.Authorization.Users.Importing.Dto;
using Framework.Authorization.Users.Profile.Dto;
using Framework.Chat;
using Framework.Chat.Dto;
using Framework.DynamicEntityProperties.Dto;
using Framework.Editions;
using Framework.Editions.Dto;
using Framework.Friendships;
using Framework.Friendships.Cache;
using Framework.Friendships.Dto;
using Framework.Localization.Dto;
using Framework.MultiTenancy;
using Framework.MultiTenancy.Dto;
using Framework.MultiTenancy.HostDashboard.Dto;
using Framework.MultiTenancy.Payments;
using Framework.MultiTenancy.Payments.Dto;
using Framework.Notifications.Dto;
using Framework.Organizations.Dto;
using Framework.Sessions.Dto;
using Framework.WebHooks.Dto;
using Framework.AService.Dto;
using Framework.AService;
using Framework.ServiceRegister;
using Stripe;
using Framework.Feedback;
using Framework.Feedback.Dto;
using Framework.UserServiceRegister.Dto;
using Framework.Bill;
using Framework.Bill.Dto;
using Framework.ApartmentManagement;
using Framework.ApartmentManagement.Dto;

namespace Framework
{
    internal static class CustomDtoMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<CreateOrEditPeopleDto, People>().ReverseMap();
            configuration.CreateMap<PeopleDto, People>().ReverseMap();
            //Inputs
            configuration.CreateMap<CheckboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<SingleLineStringInputType, FeatureInputTypeDto>();
            configuration.CreateMap<ComboboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<IInputType, FeatureInputTypeDto>()
                .Include<CheckboxInputType, FeatureInputTypeDto>()
                .Include<SingleLineStringInputType, FeatureInputTypeDto>()
                .Include<ComboboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<StaticLocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>();
            configuration.CreateMap<ILocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>()
                .Include<StaticLocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>();
            configuration.CreateMap<LocalizableComboboxItem, LocalizableComboboxItemDto>();
            configuration.CreateMap<ILocalizableComboboxItem, LocalizableComboboxItemDto>()
                .Include<LocalizableComboboxItem, LocalizableComboboxItemDto>();

            //Chat
            configuration.CreateMap<ChatMessage, ChatMessageDto>();
            configuration.CreateMap<ChatMessage, ChatMessageExportDto>();

            //Feature
            configuration.CreateMap<FlatFeatureSelectDto, Feature>().ReverseMap();
            configuration.CreateMap<Feature, FlatFeatureDto>();

            //Role
            configuration.CreateMap<RoleEditDto, Role>().ReverseMap();
            configuration.CreateMap<Role, RoleListDto>();
            configuration.CreateMap<UserRole, UserListRoleDto>();

            //Edition
            configuration.CreateMap<EditionEditDto, SubscribableEdition>().ReverseMap();
            configuration.CreateMap<EditionCreateDto, SubscribableEdition>();
            configuration.CreateMap<EditionSelectDto, SubscribableEdition>().ReverseMap();
            configuration.CreateMap<SubscribableEdition, EditionInfoDto>();

            configuration.CreateMap<Edition, EditionInfoDto>().Include<SubscribableEdition, EditionInfoDto>();

            configuration.CreateMap<SubscribableEdition, EditionListDto>();
            configuration.CreateMap<Edition, EditionEditDto>();
            configuration.CreateMap<Edition, SubscribableEdition>();
            configuration.CreateMap<Edition, EditionSelectDto>();

            //Payment
            configuration.CreateMap<SubscriptionPaymentDto, SubscriptionPayment>().ReverseMap();
            configuration.CreateMap<SubscriptionPaymentListDto, SubscriptionPayment>().ReverseMap();
            configuration.CreateMap<SubscriptionPayment, SubscriptionPaymentInfoDto>();

            //Permission
            configuration.CreateMap<Permission, FlatPermissionDto>();
            configuration.CreateMap<Permission, FlatPermissionWithLevelDto>();

            //Language
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageEditDto>();
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageListDto>();
            configuration.CreateMap<NotificationDefinition, NotificationSubscriptionWithDisplayNameDto>();
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageEditDto>()
                .ForMember(ldto => ldto.IsEnabled, options => options.MapFrom(l => !l.IsDisabled));

            //Tenant
            configuration.CreateMap<Tenant, RecentTenant>();
            configuration.CreateMap<Tenant, TenantLoginInfoDto>();
            configuration.CreateMap<Tenant, TenantListDto>();
            configuration.CreateMap<TenantEditDto, Tenant>().ReverseMap();
            configuration.CreateMap<CurrentTenantInfoDto, Tenant>().ReverseMap();

            //User
            configuration.CreateMap<User, UserEditDto>()
                .ForMember(dto => dto.Password, options => options.Ignore())
                .ReverseMap()
                .ForMember(user => user.Password, options => options.Ignore());
            configuration.CreateMap<User, UserLoginInfoDto>();
            configuration.CreateMap<User, UserListDto>();
            configuration.CreateMap<User, ChatUserDto>();
            configuration.CreateMap<User, OrganizationUnitUserListDto>();
            configuration.CreateMap<Role, OrganizationUnitRoleListDto>();
            configuration.CreateMap<CurrentUserProfileEditDto, User>().ReverseMap();
            configuration.CreateMap<UserLoginAttemptDto, UserLoginAttempt>().ReverseMap();
            configuration.CreateMap<ImportUserDto, User>();

            //AuditLog
            configuration.CreateMap<AuditLog, AuditLogListDto>();
            configuration.CreateMap<EntityChange, EntityChangeListDto>();
            configuration.CreateMap<EntityPropertyChange, EntityPropertyChangeDto>();

            //Friendship
            configuration.CreateMap<Friendship, FriendDto>();
            configuration.CreateMap<FriendCacheItem, FriendDto>();

            //OrganizationUnit
            configuration.CreateMap<OrganizationUnit, OrganizationUnitDto>();

            //Webhooks
            configuration.CreateMap<WebhookSubscription, GetAllSubscriptionsOutput>();
            configuration.CreateMap<WebhookSendAttempt, GetAllSendAttemptsOutput>()
                .ForMember(webhookSendAttemptListDto => webhookSendAttemptListDto.WebhookName,
                    options => options.MapFrom(l => l.WebhookEvent.WebhookName))
                .ForMember(webhookSendAttemptListDto => webhookSendAttemptListDto.Data,
                    options => options.MapFrom(l => l.WebhookEvent.Data));

            configuration.CreateMap<WebhookSendAttempt, GetAllSendAttemptsOfWebhookEventOutput>();

            configuration.CreateMap<DynamicProperty, DynamicPropertyDto>().ReverseMap();
            configuration.CreateMap<DynamicPropertyValue, DynamicPropertyValueDto>().ReverseMap();
            configuration.CreateMap<DynamicEntityProperty, DynamicEntityPropertyDto>()
                .ForMember(dto => dto.DynamicPropertyName,
                    options => options.MapFrom(entity => entity.DynamicProperty.DisplayName.IsNullOrEmpty() ? entity.DynamicProperty.PropertyName : entity.DynamicProperty.DisplayName));
            configuration.CreateMap<DynamicEntityPropertyDto, DynamicEntityProperty>();

            configuration.CreateMap<DynamicEntityPropertyValue, DynamicEntityPropertyValueDto>().ReverseMap();

            //User Delegations
            configuration.CreateMap<CreateUserDelegationDto, UserDelegation>();

            /* ADD YOUR OWN CUSTOM AUTOMAPPER MAPPINGS HERE */
            //Apartment Service
            configuration.CreateMap<ApartmentService, ApartmentServiceListDto>();
            configuration.CreateMap<CreateApartmentServiceInput, ApartmentService>();
            configuration.CreateMap<ApartmentService, GetApartmentServiceForEditOutput>();

            //Service Register
            configuration.CreateMap<UserServiceRegister2, UserRegisterDto>();
            configuration.CreateMap<UserRegisterDto, UserServiceRegister2>();
            configuration.CreateMap<UserServiceRegister2, RegisterServiceInput>();
            configuration.CreateMap<RegisterServiceInput, UserServiceRegister2>();


            //User
            configuration.CreateMap<User, GetAllUsersOutput>();

            //Apartment
            configuration.CreateMap<Apartment, GetApartmentOutputDto>();
            configuration.CreateMap<AddApartmentInputDto, Apartment>();
            configuration.CreateMap<Apartment, GetApartmentOutputDto>();


            //Bill
            //configuration.CreateMap<CreateEAndWBillInput, EAndWBill>();
            //configuration.CreateMap<CreateManageBillInput, ManageBill>();
            configuration.CreateMap<CreateApartmentBill, ApartmentBill>();
            configuration.CreateMap<CreateApartmentBill_Manage, ApartmentBill>();
            configuration.CreateMap<CreateServiceBillInput, ServiceBill>();
            //configuration.CreateMap<EAndWBill, EAndWBillListDto>();
            //configuration.CreateMap<ManageBill, ManageBillListDto>();
            configuration.CreateMap<ApartmentBill, ApartmentBillListDto>();
            configuration.CreateMap<ServiceBill, ServiceBillListDto>();
            /////configuration.CreateMap<EAndWBill, GetEAndWBillForEditOutput>();
            //configuration.CreateMap<ManageBill, GetManageBillForEditOutput>();
            configuration.CreateMap<ApartmentBill, GetApartmentBillForEditOutput>();
            configuration.CreateMap<ServiceBill, GetServiceBillForEditOutput>();
            configuration.CreateMap<ApartmentService, ApartmentServiceForBillListDto>();
            configuration.CreateMap<User, UserForBillListDto>();
            configuration.CreateMap<BillingInformation, BillingInformationOutput>();
            configuration.CreateMap<CreateInServiceRegisterInput, UserServiceRegister2>();

            configuration.CreateMap<UserFeedBack, UserFeedbackDto>();
            configuration.CreateMap<UserFeedbackDto, UserFeedBack>();
            configuration.CreateMap<UserFeedbackWithIdDto, UserFeedBack>();
            configuration.CreateMap<UserFeedBack, UserFeedbackWithIdDto>();
            configuration.CreateMap<UserForBillListDto, User>();

        }
    }
}