using AutoMapper;
using DeliveryApi.AutoMapperProfiles;

namespace DeliveryApiTests;

[TestFixture]
public class ApiModelProfilesTests
{
    private MapperConfiguration _mapperConfiguration = null!;

    [SetUp]
    public void Setup()
    {
        _mapperConfiguration = new MapperConfiguration(x =>
        {
            x.AddProfile(new ApiModelProfiles());
        });

        _mapperConfiguration.CreateMapper();
    }

    [Test]
    public void Assert_ValidProfile()
    {
        _mapperConfiguration.AssertConfigurationIsValid();
    }
}