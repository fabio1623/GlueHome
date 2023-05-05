using DeliveryDomain.DomainModels.Deliveries;

namespace DeliveryApi.Models.Deliveries;

public class AccessWindow
{
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public AccessWindow(AccessWindowDomain? accessWindowDomain)
    {
        StartTime = accessWindowDomain?.StartTime;
        EndTime = accessWindowDomain?.EndTime;
    }

    public AccessWindowDomain ToDomain()
    {
        return new AccessWindowDomain
        {
            StartTime = StartTime,
            EndTime = EndTime
        };
    }
}