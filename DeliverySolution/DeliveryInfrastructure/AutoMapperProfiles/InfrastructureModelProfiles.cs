using AutoMapper;
using DeliveryDomain.DomainEnums;
using DeliveryDomain.DomainModels;
using DeliveryDomain.DomainModels.Deliveries;
using DeliveryDomain.DomainModels.Users;
using DeliveryInfrastructure.Enums;
using DeliveryInfrastructure.InfrastructureModels;
using MongoDB.Bson;

namespace DeliveryInfrastructure.AutoMapperProfiles
{
    public class InfrastructureModelProfiles : Profile
    {
        public InfrastructureModelProfiles()
        {
            AddObjectIdMappings();
            AddUserMappings();
            AddDeliveryMappings();
            AddRoleMappings();
            AddStateMappings();
        }

        private void AddObjectIdMappings()
        {
            CreateMap<string, ObjectId>()
                .ConvertUsing(x => ParseToObjectId(x));

            CreateMap<ObjectId, string>()
                .ConvertUsing(x => x.ToString());
        }

        private void AddUserMappings()
        {
            CreateMap<CreateUserRequestDomain, UserInfra>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<UserInfra, UserDomain>();
        }

        private void AddDeliveryMappings()
        {
            CreateMap<CreateDeliveryRequestDomain, DeliveryInfra>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            CreateMap<CreateDeliveryRequestDomain.CreateDeliveryAccessWindowDomain, DeliveryInfra.DeliveryAccessWindowInfra>();
            CreateMap<CreateDeliveryRequestDomain.CreateDeliveryRecipientDomain, DeliveryInfra.DeliveryRecipientInfra>();
            CreateMap<CreateDeliveryRequestDomain.CreateDeliveryOrderDomain, DeliveryInfra.DeliveryOrderInfra>();
            
            CreateMap<DeliveryInfra, DeliveryDomain.DomainModels.DeliveryDomain>();
            CreateMap<DeliveryInfra.DeliveryAccessWindowInfra, DeliveryDomain.DomainModels.DeliveryDomain.DeliveryAccessWindowDomain>();
            CreateMap<DeliveryInfra.DeliveryRecipientInfra, DeliveryDomain.DomainModels.DeliveryDomain.DeliveryRecipientDomain>();
            CreateMap<DeliveryInfra.DeliveryOrderInfra, DeliveryDomain.DomainModels.DeliveryDomain.DeliveryOrderDomain>();
        }
        
        private void AddRoleMappings()
        {
            CreateMap<RoleDomain, RoleInfra>();
        }

        private void AddStateMappings()
        {
            CreateMap<StateDomain, StateInfra>();
        }

        private static ObjectId ParseToObjectId(string stringValue)
        {
            _ = ObjectId.TryParse(stringValue, out var objectId);

            return objectId;
        }
    }
}