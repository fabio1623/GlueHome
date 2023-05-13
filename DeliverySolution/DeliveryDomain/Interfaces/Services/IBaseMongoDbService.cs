using DeliveryDomain.DomainModels;

namespace DeliveryDomain.Interfaces.Services
{
    public interface IBaseMongoDbService<TDomainModel, in TDomainModelCreate, in TDomainModelUpdate>
    {
        Task<PagedListDomain<TDomainModel?>> GetPaged(int? requestedPage, int? pageSize, CancellationToken cancellationToken);
        Task<TDomainModel?> Get(string? id, CancellationToken cancellationToken);
        Task<TDomainModel?> Create(TDomainModelCreate? domainModelCreate, CancellationToken cancellationToken);
        Task Update(string? id, TDomainModelUpdate? domainModelUpdate, CancellationToken cancellationToken);
        Task Delete(string? id, CancellationToken cancellationToken);
    }
}