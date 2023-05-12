using AutoMapper;
using DeliveryApi.Models;
using DeliveryApi.Models.Deliveries;
using DeliveryApi.Models.Users;
using DeliveryDomain.DomainModels;
using DeliveryDomain.DomainModels.Deliveries;
using DeliveryDomain.DomainModels.Users;

namespace DeliveryApi.AutoMapperProfiles
{
    public class ApiModelProfiles : Profile
    {
        public ApiModelProfiles()
        {
            AddDeliveryMappings();
            AddUserMappings();
            AddPagedListMappings();
        }

        private void AddDeliveryMappings()
        {
            CreateMap<CreateDeliveryRequest, CreateDeliveryRequestDomain>();
            CreateMap<CreateDeliveryRequest.CreateDeliveryRecipient, CreateDeliveryRequestDomain.CreateDeliveryRecipientDomain>();
            CreateMap<CreateDeliveryRequest.CreateDeliveryOrder, CreateDeliveryRequestDomain.CreateDeliveryOrderDomain>();
            
            CreateMap<DeliveryDomain.DomainModels.DeliveryDomain, Delivery>();
            CreateMap<DeliveryDomain.DomainModels.DeliveryDomain.DeliveryAccessWindowDomain, Delivery.DeliveryAccessWindow>();
            CreateMap<DeliveryDomain.DomainModels.DeliveryDomain.DeliveryRecipientDomain, Delivery.DeliveryRecipient>();
            CreateMap<DeliveryDomain.DomainModels.DeliveryDomain.DeliveryOrderDomain, Delivery.DeliveryOrder>();
        }

        private void AddUserMappings()
        {
            CreateMap<AuthenticateUserRequest, AuthenticateUserRequestDomain>();

            CreateMap<AuthenticateUserResponseDomain, AuthenticateUserResponse>();

            CreateMap<UserDomain, User>();
        }
        
        private void AddPagedListMappings()
        {
            CreateMap(typeof(PagedListDomain<>), typeof(PagedList<>));
        }
    }
}