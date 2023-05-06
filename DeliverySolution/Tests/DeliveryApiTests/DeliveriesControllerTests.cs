using DeliveryApi.Controllers;
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
    private DeliveriesController _deliveriesController = null!;

    [SetUp]
    public void Setup()
    {
        _mockDeliveryBusiness = new Mock<IDeliveryBusiness>();
        _deliveriesController = new DeliveriesController(_mockDeliveryBusiness.Object);
    }

    [Test]
    public async Task Create_ReturnsNoContent()
    {
        // Arrange
        var createDelivery = new CreateDelivery();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _deliveriesController.Create(createDelivery, cancellationToken);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
        _mockDeliveryBusiness.Verify(x => x.Create(It.IsAny<CreateDeliveryDomain>(), cancellationToken), Times.Once);
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
}