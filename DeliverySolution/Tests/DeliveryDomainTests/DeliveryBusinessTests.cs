using DeliveryDomain.Businesses;
using DeliveryDomain.DomainModels.Deliveries;
using DeliveryDomain.DomainModels.RabbitMqMessages;
using DeliveryDomain.Exceptions;
using DeliveryDomain.Interfaces.Services;
using Moq;

namespace DeliveryDomainTests;

[TestFixture]
public class DeliveryBusinessTests
{
    private Mock<IDeliveryService> _deliveryServiceMock;
    private Mock<IRabbitMqService> _rabbitMqServiceMock;
    private DeliveryBusiness _deliveryBusiness;

    [SetUp]
    public void Setup()
    {
        _deliveryServiceMock = new Mock<IDeliveryService>();
        _rabbitMqServiceMock = new Mock<IRabbitMqService>();
        _deliveryBusiness = new DeliveryBusiness(_deliveryServiceMock.Object, _rabbitMqServiceMock.Object);
    }

    [Test]
    public async Task Create_Successful()
    {
        // Arrange
        var deliveryDomain = new CreateDeliveryDomain
        {
            Order = new OrderDomain
            {
                OrderNumber = "1234"
            }
        };

        // Act
        await _deliveryBusiness.Create(deliveryDomain, CancellationToken.None);

        // Assert
        _deliveryServiceMock.Verify(mock => mock.Create(deliveryDomain, CancellationToken.None), Times.Once);
        _rabbitMqServiceMock.Verify(mock => mock.ProduceMessage(It.Is<DeliveryCreatedMessage>(message => message.OrderNumber == "1234")), Times.Once);
    }
    
    [Test]
    public async Task Get_Successful()
    {
        // Arrange
        const string orderNumber = "1234";
        var delivery = new DeliveryDomain.DomainModels.Deliveries.DeliveryDomain
        {
            Order = new OrderDomain
            {
                OrderNumber = orderNumber
            },
            State = StateDomain.Created
        };
        
        
        _deliveryServiceMock.Setup(mock => mock.Get(orderNumber, CancellationToken.None)).ReturnsAsync(delivery);

        // Act
        var result = await _deliveryBusiness.Get(orderNumber, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(delivery));
        _deliveryServiceMock.Verify(mock => mock.Get(orderNumber, CancellationToken.None), Times.Once);
    }

    [Test]
    public async Task Approve_Successful()
    {
        // Arrange
        const string orderNumber = "1234";
        var delivery = new DeliveryDomain.DomainModels.Deliveries.DeliveryDomain
        {
            Order = new OrderDomain
            {
                OrderNumber = orderNumber
            },
            State = StateDomain.Created
        };
        _deliveryServiceMock.Setup(mock => mock.Get(orderNumber, CancellationToken.None)).ReturnsAsync(delivery);

        // Act
        await _deliveryBusiness.Approve(orderNumber, CancellationToken.None);

        // Assert
        _deliveryServiceMock.Verify(mock => mock.Update(orderNumber, It.Is<DeliveryUpdateDomain>(domain => domain.State == StateDomain.Approved), CancellationToken.None), Times.Once);
        _rabbitMqServiceMock.Verify(mock => mock.ProduceMessage(It.Is<DeliveryUpdatedMessage>(message => message.OrderNumber == orderNumber && message.NewState == StateDomain.Approved.ToString())), Times.Once);
    }

    [TestCase(StateDomain.Approved)]
    [TestCase(StateDomain.Completed)]
    [TestCase(StateDomain.Cancelled)]
    [TestCase(StateDomain.Expired)]
    public Task Approve_ThrowsException_StateNotCreated(StateDomain state)
    {
        // Arrange
        const string orderNumber = "1234";
        var delivery = new DeliveryDomain.DomainModels.Deliveries.DeliveryDomain
        {
            Order = new OrderDomain
            {
                OrderNumber = orderNumber
            },
            State = state
        };
        _deliveryServiceMock.Setup(mock => mock.Get(orderNumber, CancellationToken.None)).ReturnsAsync(delivery);

        // Act and Assert
        Assert.ThrowsAsync<DomainException>(() => _deliveryBusiness.Approve(orderNumber, CancellationToken.None));
        return Task.CompletedTask;
    }

    [Test]
    public async Task Complete_Successful()
    {
        // Arrange
        const string orderNumber = "1234";
        var delivery = new DeliveryDomain.DomainModels.Deliveries.DeliveryDomain
        {
            Order = new OrderDomain
            {
                OrderNumber = orderNumber
            },
            State = StateDomain.Approved
        };
        _deliveryServiceMock.Setup(mock => mock.Get(orderNumber, CancellationToken.None)).ReturnsAsync(delivery);

        // Act
        await _deliveryBusiness.Complete(orderNumber, CancellationToken.None);

        // Assert
        _deliveryServiceMock.Verify(mock => mock.Update(orderNumber, It.Is<DeliveryUpdateDomain>(domain => domain.State == StateDomain.Completed), CancellationToken.None), Times.Once);
        _rabbitMqServiceMock.Verify(mock => mock.ProduceMessage(It.Is<DeliveryUpdatedMessage>(message => message.OrderNumber == orderNumber && message.NewState == StateDomain.Completed.ToString())), Times.Once);
    }
    
    [TestCase(StateDomain.Created)]
    [TestCase(StateDomain.Completed)]
    [TestCase(StateDomain.Cancelled)]
    [TestCase(StateDomain.Expired)]
    public Task Complete_ThrowsException_StateNotApproved(StateDomain state)
    {
        // Arrange
        const string orderNumber = "1234";
        var delivery = new DeliveryDomain.DomainModels.Deliveries.DeliveryDomain
        {
            Order = new OrderDomain
            {
                OrderNumber = orderNumber
            },
            State = state
        };
        _deliveryServiceMock.Setup(mock => mock.Get(orderNumber, CancellationToken.None)).ReturnsAsync(delivery);

        // Act and Assert
        Assert.ThrowsAsync<DomainException>(() => _deliveryBusiness.Complete(orderNumber, CancellationToken.None));
        return Task.CompletedTask;
    }
    
    [Test]
    public async Task Cancel_Successful()
    {
        // Arrange
        const string orderNumber = "1234";
        var delivery = new DeliveryDomain.DomainModels.Deliveries.DeliveryDomain
        {
            Order = new OrderDomain
            {
                OrderNumber = orderNumber
            },
            State = StateDomain.Created
        };
        _deliveryServiceMock.Setup(mock => mock.Get(orderNumber, CancellationToken.None)).ReturnsAsync(delivery);

        // Act
        await _deliveryBusiness.Cancel(orderNumber, CancellationToken.None);

        // Assert
        _deliveryServiceMock.Verify(mock => mock.Update(orderNumber, It.Is<DeliveryUpdateDomain>(domain => domain.State == StateDomain.Cancelled), CancellationToken.None), Times.Once);
        _rabbitMqServiceMock.Verify(mock => mock.ProduceMessage(It.Is<DeliveryUpdatedMessage>(message => message.OrderNumber == orderNumber && message.NewState == StateDomain.Cancelled.ToString())), Times.Once);
    }
    
    [TestCase(StateDomain.Completed)]
    [TestCase(StateDomain.Cancelled)]
    [TestCase(StateDomain.Expired)]
    public Task Cancel_ThrowsException_StateNotCreatedNorApproved(StateDomain state)
    {
        // Arrange
        const string orderNumber = "1234";
        var delivery = new DeliveryDomain.DomainModels.Deliveries.DeliveryDomain
        {
            Order = new OrderDomain
            {
                OrderNumber = orderNumber
            },
            State = state
        };
        _deliveryServiceMock.Setup(mock => mock.Get(orderNumber, CancellationToken.None)).ReturnsAsync(delivery);

        // Act and Assert
        Assert.ThrowsAsync<DomainException>(() => _deliveryBusiness.Complete(orderNumber, CancellationToken.None));
        return Task.CompletedTask;
    }
    
    [Test]
    public async Task Delete_Successful()
    {
        // Arrange
        const string orderNumber = "1234";
        var delivery = new DeliveryDomain.DomainModels.Deliveries.DeliveryDomain
        {
            Order = new OrderDomain
            {
                OrderNumber = orderNumber
            },
            State = StateDomain.Created
        };
        _deliveryServiceMock.Setup(mock => mock.Get(orderNumber, CancellationToken.None)).ReturnsAsync(delivery);

        // Act
        await _deliveryBusiness.Delete(orderNumber, CancellationToken.None);

        // Assert
        _deliveryServiceMock.Verify(mock => mock.Delete(orderNumber, CancellationToken.None), Times.Once);
        _rabbitMqServiceMock.Verify(mock => mock.ProduceMessage(It.Is<DeliveryDeletedMessage>(message => message.OrderNumber == orderNumber)), Times.Once);
    }
    
    [Test]
    public Task Cancel_ThrowsException_OrderDoesNotExist()
    {
        // Arrange
        const string orderNumber = "1234";
        _deliveryServiceMock.Setup(mock => mock.Get(orderNumber, CancellationToken.None)).ReturnsAsync((DeliveryDomain.DomainModels.Deliveries.DeliveryDomain?)null);

        // Act and Assert
        Assert.ThrowsAsync<DomainException>(() => _deliveryBusiness.Complete(orderNumber, CancellationToken.None));
        return Task.CompletedTask;
    }
}