using DeliveryDomain.Businesses;
using DeliveryDomain.DomainEnums;
using DeliveryDomain.DomainModels;
using DeliveryDomain.DomainModels.BrokerMessages;
using DeliveryDomain.DomainModels.Deliveries;
using DeliveryDomain.Exceptions;
using DeliveryDomain.Interfaces.Services;
using Moq;

namespace DeliveryDomainTests;

[TestFixture]
public class DeliveryBusinessTests
{
    private Mock<IDeliveryService> _deliveryServiceMock = null!;
    private Mock<IRabbitMqService> _rabbitMqServiceMock = null!;
    
    private DeliveryBusiness _deliveryBusiness = null!;

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
        var cancellationToken = CancellationToken.None;
        var createDeliveryRequestDomain = new CreateDeliveryRequestDomain();
        var expectedDelivery = new DeliveryDomain.DomainModels.DeliveryDomain
        {
            Id = Guid.NewGuid().ToString()
        };

        _deliveryServiceMock
            .Setup(x => x.Create(It.IsAny<CreateDeliveryRequestDomain>(), cancellationToken))
            .ReturnsAsync(expectedDelivery);

        // Act
        var result = await _deliveryBusiness.Create(createDeliveryRequestDomain, cancellationToken);

        // Assert
        Assert.That(result?.Id, Is.EqualTo(expectedDelivery.Id));
        _deliveryServiceMock.Verify(mock => mock.Create(createDeliveryRequestDomain, cancellationToken), Times.Once);
        _rabbitMqServiceMock.Verify(mock => mock.ProduceMessage(It.Is<DeliveryCreatedMessage>(message => message.DeliveryId == expectedDelivery.Id)), Times.Once);
    }
    
    [Test]
    public async Task GetPaged_Successful()
    {
        // Arrange
        const int requestedPage = 1;
        const int pageSize = 1;
        const string orderNumber = "1234";
        
        var pagedList = new PagedListDomain<DeliveryDomain.DomainModels.DeliveryDomain?>
        {
            CurrentPage = requestedPage,
            TotalPages = 1,
            TotalResults = 1,
            Results = new List<DeliveryDomain.DomainModels.DeliveryDomain?>
            {
                new()
                {
                    Order = new DeliveryDomain.DomainModels.DeliveryDomain.DeliveryOrderDomain
                    {
                        OrderNumber = orderNumber
                    },
                    State = StateDomain.Created
                }
            }
        };

        _deliveryServiceMock
            .Setup(mock => mock.GetPaged(requestedPage, pageSize, CancellationToken.None))
            .ReturnsAsync(pagedList);

        // Act
        var result = await _deliveryBusiness.GetPaged(requestedPage, pageSize, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(pagedList));
        _deliveryServiceMock.Verify(mock => mock.GetPaged(requestedPage, pageSize, CancellationToken.None), Times.Once);
    }
    
    [Test]
    public async Task Get_Successful()
    {
        // Arrange
        const string orderNumber = "1234";
        var delivery = new DeliveryDomain.DomainModels.DeliveryDomain
        {
            Order = new DeliveryDomain.DomainModels.DeliveryDomain.DeliveryOrderDomain
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
        var delivery = new DeliveryDomain.DomainModels.DeliveryDomain
        {
            Order = new DeliveryDomain.DomainModels.DeliveryDomain.DeliveryOrderDomain
            {
                OrderNumber = orderNumber
            },
            State = StateDomain.Created
        };
        _deliveryServiceMock.Setup(mock => mock.Get(orderNumber, CancellationToken.None)).ReturnsAsync(delivery);

        // Act
        await _deliveryBusiness.Approve(orderNumber, CancellationToken.None);

        // Assert
        _deliveryServiceMock.Verify(mock => mock.Get(orderNumber, CancellationToken.None), Times.Once);
        _deliveryServiceMock.Verify(mock => mock.Update(orderNumber, It.Is<UpdateDeliveryRequestDomain>(domain => domain.State == StateDomain.Approved), CancellationToken.None), Times.Once);
        _rabbitMqServiceMock.Verify(mock => mock.ProduceMessage(It.Is<DeliveryUpdatedMessage>(message => message.DeliveryId == orderNumber && message.NewState == StateDomain.Approved.ToString())), Times.Once);
    }

    [TestCase(StateDomain.Approved)]
    [TestCase(StateDomain.Completed)]
    [TestCase(StateDomain.Cancelled)]
    [TestCase(StateDomain.Expired)]
    public Task Approve_ThrowsException_StateNotCreated(StateDomain state)
    {
        // Arrange
        const string orderNumber = "1234";
        var delivery = new DeliveryDomain.DomainModels.DeliveryDomain
        {
            Order = new DeliveryDomain.DomainModels.DeliveryDomain.DeliveryOrderDomain
            {
                OrderNumber = orderNumber
            },
            State = state
        };
        _deliveryServiceMock.Setup(mock => mock.Get(orderNumber, CancellationToken.None)).ReturnsAsync(delivery);

        // Act and Assert
        Assert.ThrowsAsync<DomainException>(() => _deliveryBusiness.Approve(orderNumber, CancellationToken.None));
        _deliveryServiceMock.Verify(mock => mock.Update(It.IsAny<string>(), It.IsAny<UpdateDeliveryRequestDomain>(), It.IsAny<CancellationToken>()), Times.Never);
        _rabbitMqServiceMock.Verify(mock => mock.ProduceMessage(It.IsAny<DeliveryUpdatedMessage>()), Times.Never);
        return Task.CompletedTask;
    }

    [Test]
    public async Task Complete_Successful()
    {
        // Arrange
        const string orderNumber = "1234";
        var delivery = new DeliveryDomain.DomainModels.DeliveryDomain
        {
            Order = new DeliveryDomain.DomainModels.DeliveryDomain.DeliveryOrderDomain
            {
                OrderNumber = orderNumber
            },
            State = StateDomain.Approved
        };
        _deliveryServiceMock.Setup(mock => mock.Get(orderNumber, CancellationToken.None)).ReturnsAsync(delivery);

        // Act
        await _deliveryBusiness.Complete(orderNumber, CancellationToken.None);

        // Assert
        _deliveryServiceMock.Verify(mock => mock.Get(orderNumber, CancellationToken.None), Times.Once);
        _deliveryServiceMock.Verify(mock => mock.Update(orderNumber, It.Is<UpdateDeliveryRequestDomain>(domain => domain.State == StateDomain.Completed), CancellationToken.None), Times.Once);
        _rabbitMqServiceMock.Verify(mock => mock.ProduceMessage(It.Is<DeliveryUpdatedMessage>(message => message.DeliveryId == orderNumber && message.NewState == StateDomain.Completed.ToString())), Times.Once);
    }
    
    [TestCase(StateDomain.Created)]
    [TestCase(StateDomain.Completed)]
    [TestCase(StateDomain.Cancelled)]
    [TestCase(StateDomain.Expired)]
    public Task Complete_ThrowsException_StateNotApproved(StateDomain state)
    {
        // Arrange
        const string orderNumber = "1234";
        var delivery = new DeliveryDomain.DomainModels.DeliveryDomain
        {
            Order = new DeliveryDomain.DomainModels.DeliveryDomain.DeliveryOrderDomain
            {
                OrderNumber = orderNumber
            },
            State = state
        };
        _deliveryServiceMock.Setup(mock => mock.Get(orderNumber, CancellationToken.None)).ReturnsAsync(delivery);

        // Act and Assert
        Assert.ThrowsAsync<DomainException>(() => _deliveryBusiness.Complete(orderNumber, CancellationToken.None));
        _deliveryServiceMock.Verify(mock => mock.Update(It.IsAny<string>(), It.IsAny<UpdateDeliveryRequestDomain>(), It.IsAny<CancellationToken>()), Times.Never);
        _rabbitMqServiceMock.Verify(mock => mock.ProduceMessage(It.IsAny<DeliveryUpdatedMessage>()), Times.Never);
        return Task.CompletedTask;
    }
    
    [Test]
    public async Task Cancel_Successful()
    {
        // Arrange
        const string orderNumber = "1234";
        var delivery = new DeliveryDomain.DomainModels.DeliveryDomain
        {
            Order = new DeliveryDomain.DomainModels.DeliveryDomain.DeliveryOrderDomain
            {
                OrderNumber = orderNumber
            },
            State = StateDomain.Created
        };
        _deliveryServiceMock.Setup(mock => mock.Get(orderNumber, CancellationToken.None)).ReturnsAsync(delivery);

        // Act
        await _deliveryBusiness.Cancel(orderNumber, CancellationToken.None);

        // Assert
        _deliveryServiceMock.Verify(mock => mock.Get(orderNumber, CancellationToken.None), Times.Once);
        _deliveryServiceMock.Verify(mock => mock.Update(orderNumber, It.Is<UpdateDeliveryRequestDomain>(domain => domain.State == StateDomain.Cancelled), CancellationToken.None), Times.Once);
        _rabbitMqServiceMock.Verify(mock => mock.ProduceMessage(It.Is<DeliveryUpdatedMessage>(message => message.DeliveryId == orderNumber && message.NewState == StateDomain.Cancelled.ToString())), Times.Once);
    }
    
    [TestCase(StateDomain.Completed)]
    [TestCase(StateDomain.Cancelled)]
    [TestCase(StateDomain.Expired)]
    public Task Cancel_ThrowsException_StateNotCreatedNorApproved(StateDomain state)
    {
        // Arrange
        const string orderNumber = "1234";
        var delivery = new DeliveryDomain.DomainModels.DeliveryDomain
        {
            Order = new DeliveryDomain.DomainModels.DeliveryDomain.DeliveryOrderDomain
            {
                OrderNumber = orderNumber
            },
            State = state
        };
        _deliveryServiceMock.Setup(mock => mock.Get(orderNumber, CancellationToken.None)).ReturnsAsync(delivery);

        // Act and Assert
        Assert.ThrowsAsync<DomainException>(() => _deliveryBusiness.Complete(orderNumber, CancellationToken.None));
        _deliveryServiceMock.Verify(mock => mock.Update(It.IsAny<string>(), It.IsAny<UpdateDeliveryRequestDomain>(), It.IsAny<CancellationToken>()), Times.Never);
        _rabbitMqServiceMock.Verify(mock => mock.ProduceMessage(It.IsAny<DeliveryUpdatedMessage>()), Times.Never);
        return Task.CompletedTask;
    }
    
    [Test]
    public Task Cancel_ThrowsException_DeliveryDoesNotExist()
    {
        // Arrange
        const string orderNumber = "1234";
        _deliveryServiceMock.Setup(mock => mock.Get(orderNumber, CancellationToken.None)).ReturnsAsync((DeliveryDomain.DomainModels.DeliveryDomain?)null);

        // Act and Assert
        Assert.ThrowsAsync<DomainException>(() => _deliveryBusiness.Complete(orderNumber, CancellationToken.None));
        _deliveryServiceMock.Verify(mock => mock.Update(It.IsAny<string>(), It.IsAny<UpdateDeliveryRequestDomain>(), It.IsAny<CancellationToken>()), Times.Never);
        _rabbitMqServiceMock.Verify(mock => mock.ProduceMessage(It.IsAny<DeliveryUpdatedMessage>()), Times.Never);
        return Task.CompletedTask;
    }
    
    [Test]
    public async Task Delete_Successful()
    {
        // Arrange
        const string orderNumber = "1234";
        var delivery = new DeliveryDomain.DomainModels.DeliveryDomain
        {
            Order = new DeliveryDomain.DomainModels.DeliveryDomain.DeliveryOrderDomain
            {
                OrderNumber = orderNumber
            },
            State = StateDomain.Created
        };
        _deliveryServiceMock.Setup(mock => mock.Get(orderNumber, CancellationToken.None)).ReturnsAsync(delivery);

        // Act
        await _deliveryBusiness.Delete(orderNumber, CancellationToken.None);

        // Assert
        _deliveryServiceMock.Verify(mock => mock.Get(orderNumber, CancellationToken.None), Times.Once);
        _deliveryServiceMock.Verify(mock => mock.Delete(orderNumber, CancellationToken.None), Times.Once);
        _rabbitMqServiceMock.Verify(mock => mock.ProduceMessage(It.Is<DeliveryDeletedMessage>(message => message.DeliveryId == orderNumber)), Times.Once);
    }
    
    [Test]
    public Task Delete_ThrowsException_DeliveryDoesNotExist()
    {
        // Arrange
        const string orderNumber = "1234";
        _deliveryServiceMock
            .Setup(mock => mock.Get(orderNumber, CancellationToken.None))
            .ReturnsAsync((DeliveryDomain.DomainModels.DeliveryDomain?)null);

        // Act and Assert
        Assert.ThrowsAsync<DomainException>(() => _deliveryBusiness.Delete(orderNumber, CancellationToken.None));
        _deliveryServiceMock.Verify(mock => mock.Delete(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _rabbitMqServiceMock.Verify(mock => mock.ProduceMessage(It.IsAny<DeliveryDeletedMessage>()), Times.Never);
        return Task.CompletedTask;
    }
}