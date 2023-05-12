using AutoMapper;
using DeliveryApi.Controllers;
using DeliveryApi.Models;
using DeliveryApi.Models.Deliveries;
using DeliveryDomain.DomainModels.Deliveries;
using DeliveryDomain.Interfaces.Businesses;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DeliveryApiTests;

[TestFixture]
public class DeliveriesControllerTests
{
    private Mock<IDeliveryBusiness> _mockDeliveryBusiness = null!;
    private Mock<IMapper> _mockMapper = null!;
    
    private DeliveriesController _deliveriesController = null!;

    [SetUp]
    public void Setup()
    {
        _mockDeliveryBusiness = new Mock<IDeliveryBusiness>();
        _mockMapper = new Mock<IMapper>();
        _deliveriesController = new DeliveriesController(_mockDeliveryBusiness.Object, _mockMapper.Object);
    }

    [Test]
    public async Task Create_ReturnsNoContent()
    {
        // Arrange
        var createDelivery = new CreateDeliveryRequest();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _deliveriesController.Create(createDelivery, cancellationToken);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
        _mockDeliveryBusiness.Verify(x => x.Create(It.IsAny<CreateDeliveryRequestDomain>(), cancellationToken), Times.Once);
    }
    
    [Test]
    public async Task GetPaged_ReturnsPagedListOfDeliveries()
    {
        // Arrange
        const int requestedPage = 1;
        const int pageSize = 1;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _deliveriesController.GetPaged(requestedPage, pageSize, cancellationToken);

        // Assert
        Assert.That(result, Is.InstanceOf<ActionResult<PagedList<Delivery>>>());
        _mockDeliveryBusiness.Verify(x => x.GetPaged(It.IsAny<int>(), It.IsAny<int>(), cancellationToken), Times.Once);
    }
    
    [Test]
    public async Task Get_ReturnsDelivery()
    {
        // Arrange
        const string deliveryId = "12345";
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _deliveriesController.Get(deliveryId, cancellationToken);

        // Assert
        Assert.That(result, Is.InstanceOf<ActionResult<Delivery?>>());
        _mockDeliveryBusiness.Verify(x => x.Get(It.IsAny<string>(), cancellationToken), Times.Once);
    }

    [Test]
    public async Task Approve_ReturnsNoContent()
    {
        // Arrange
        const string orderNumber = "12345";
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _deliveriesController.Approve(orderNumber, cancellationToken);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
        _mockDeliveryBusiness.Verify(x => x.Approve(orderNumber, cancellationToken), Times.Once);
    }

    [Test]
    public async Task Complete_ReturnsNoContent()
    {
        // Arrange
        const string orderNumber = "12345";
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _deliveriesController.Complete(orderNumber, cancellationToken);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
        _mockDeliveryBusiness.Verify(x => x.Complete(orderNumber, cancellationToken), Times.Once);
    }

    [Test]
    public async Task Cancel_ReturnsNoContent()
    {
        // Arrange
        const string orderNumber = "12345";
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _deliveriesController.Cancel(orderNumber, cancellationToken);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
        _mockDeliveryBusiness.Verify(x => x.Cancel(orderNumber, cancellationToken), Times.Once);
    }
    
    [Test]
    public async Task Delete_ReturnsNoContent()
    {
        // Arrange
        const string orderNumber = "12345";
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _deliveriesController.Delete(orderNumber, cancellationToken);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
        _mockDeliveryBusiness.Verify(x => x.Delete(orderNumber, cancellationToken), Times.Once);
    }
}