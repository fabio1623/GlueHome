using AutoMapper;
using DeliveryInfrastructure.AutoMapperProfiles;

namespace DeliveryInfrastructureTests;

[TestFixture]
public class Tests
{
    private MapperConfiguration _mapperConfiguration = null!;

    [SetUp]
    public void Setup()
    {
        _mapperConfiguration = new MapperConfiguration(x =>
        {
            x.AddProfile(new InfrastructureModelProfiles());
        });

        _mapperConfiguration.CreateMapper();
    }

    [Test]
    public void Assert_ValidProfile()
    {
        _mapperConfiguration.AssertConfigurationIsValid();
    }
}