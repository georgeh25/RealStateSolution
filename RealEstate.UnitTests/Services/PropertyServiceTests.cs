using AutoMapper;
using Moq;
using RealEstate.Application.DTOs;
using RealEstate.Application.Interfaces;
using RealEstate.Application.Mapping;
using RealEstate.Application.Services;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Interfaces;

using ApplicationFilters = RealEstate.Application.Filters;
using DomainFilters = RealEstate.Domain.Filters;

namespace RealEstate.UnitTests.Services
{
    [TestFixture]
    public class PropertyServiceTests
    {
        private Mock<IPropertyRepository> _propertyRepositoryMock;
        private IMapper _mapper;
        private PropertyService _propertyService;

        [SetUp]
        public void Setup()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = config.CreateMapper();
                        
            _propertyRepositoryMock = new Mock<IPropertyRepository>();

            _propertyService = new PropertyService(_propertyRepositoryMock.Object, _mapper);
        }

        [Test]
        public async Task CreatePropertyAsync_ValidPropertyDTO_AddsPropertyAndReturnsDTO()
        {
            // Arrange
            var propertyDto = new PropertyDTO
            {
                IdProperty = 0,
                Name = "New Property",
                Address = "123 Main St",
                Price = 250000,
                CodeInternal = "NP-001",
                Year = 2023,
                IdOwner = 1,
                Owner = new OwnerDTO { IdOwner = 1, Name = "John Connor" },
                PropertyImages = new List<PropertyImageDTO>(),
                PropertyTraces = new List<PropertyTraceDTO>()
            };

            var addedPropertyEntity = new Property
            {
                IdProperty = 1, // ID generated
                Name = "New Property",
                Address = "123 Main St",
                Price = 250000,
                CodeInternal = "NP-001",
                Year = 2023,
                IdOwner = 1,
                Owner = new Owner { IdOwner = 1, Name = "John Connor" },
                PropertyImages = new List<PropertyImage>(),
                PropertyTraces = new List<PropertyTrace>()
            };

            _propertyRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Property>()))
                .ReturnsAsync(addedPropertyEntity);

            // Act
            var result = await _propertyService.CreatePropertyAsync(propertyDto);

            // Assert
            _propertyRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Property>()), Times.Once);

            Assert.That(result, Is.Not.Null, "Returned PropertyDTO should not be null");
            Assert.That(result.Name, Is.EqualTo(propertyDto.Name));
            Assert.That(result.Address, Is.EqualTo(propertyDto.Address));
            Assert.That(result.Price, Is.EqualTo(propertyDto.Price));
            Assert.That(result.CodeInternal, Is.EqualTo(propertyDto.CodeInternal));
            Assert.That(result.Year, Is.EqualTo(propertyDto.Year));
            Assert.That(result.IdOwner, Is.EqualTo(propertyDto.IdOwner));
        }

        [Test]
        public async Task ChangePriceAsync_ValidPrice_ReturnsTrue()
        {
            // Arrange
            int propertyId = 1;
            decimal newPrice = 150000;

            _propertyRepositoryMock.Setup(repo => repo.ChangePriceAsync(propertyId, newPrice))
                .ReturnsAsync(true);

            // Act
            var result = await _propertyService.ChangePriceAsync(propertyId, newPrice);

            // Assert

            Assert.That(result, Is.True, "Expected ChangePriceAsync to return true");

            _propertyRepositoryMock.Verify(repo => repo.ChangePriceAsync(propertyId, newPrice), Times.Once);
        }

        [Test]
        public async Task ChangePriceAsync_PropertyDoesNotExist_ReturnsFalse()
        {
            // Arrange
            int propertyId = 2;
            decimal newPrice = 150000;

            _propertyRepositoryMock.Setup(repo => repo.ChangePriceAsync(propertyId, newPrice))
                .ReturnsAsync(false);

            // Act
            var result = await _propertyService.ChangePriceAsync(propertyId, newPrice);

            // Assert

            Assert.That(result, Is.False, "Expected ChangePriceAsync to return false for non-existent property");

            _propertyRepositoryMock.Verify(repo => repo.ChangePriceAsync(propertyId, newPrice), Times.Once);
        }

        [Test]
        public void ChangePriceAsync_RepositoryThrowsException_PropagatesException()
        {
            // Arrange
            int propertyId = 3;
            decimal newPrice = 150000;

            _propertyRepositoryMock.Setup(repo => repo.ChangePriceAsync(propertyId, newPrice))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _propertyService.ChangePriceAsync(propertyId, newPrice));
            Assert.That(ex.Message, Is.EqualTo("Database error"));

            _propertyRepositoryMock.Verify(repo => repo.ChangePriceAsync(propertyId, newPrice), Times.Once);
        }

        [Test]
        public async Task ListPropertiesAsync_ValidFilter_ReturnsListOfPropertyDTO()
        {
            // Arrange
            var applicationFilter = new ApplicationFilters.PropertyFilter
            {
                Name = "Test",
                MinPrice = 100000,
                MaxPrice = 200000                
            };

            var domainFilter = _mapper.Map<DomainFilters.PropertyFilter>(applicationFilter);

            var properties = new List<Property>
            {
                new Property { IdProperty = 1, Name = "Test Property 1", Price = 150000 },
                new Property { IdProperty = 2, Name = "Test Property 2", Price = 180000 }
            };

            _propertyRepositoryMock.Setup(repo => repo.ListAsync(It.Is<DomainFilters.PropertyFilter>(f =>
                f.Name == domainFilter.Name &&
                f.MinPrice == domainFilter.MinPrice &&
                f.MaxPrice == domainFilter.MaxPrice
            ))).ReturnsAsync(properties);

            // Act
            var result = await _propertyService.ListPropertiesAsync(applicationFilter);

            // Assert
            Assert.That(result, Is.Not.Null, "Result should not be null");
            Assert.That(result, Has.Count.EqualTo(2), "Expected two properties in the result");

            Assert.That(result[0].Name, Is.EqualTo("Test Property 1"));
            Assert.That(result[0].Price, Is.EqualTo(150000));

            Assert.That(result[1].Name, Is.EqualTo("Test Property 2"));
            Assert.That(result[1].Price, Is.EqualTo(180000));

            _propertyRepositoryMock.Verify(repo => repo.ListAsync(It.Is<DomainFilters.PropertyFilter>(f =>
                f.Name == domainFilter.Name &&
                f.MinPrice == domainFilter.MinPrice &&
                f.MaxPrice == domainFilter.MaxPrice)), Times.Once);
        }

        [Test]
        public async Task ListPropertiesAsync_NoMatchingProperties_ReturnsEmptyList()
        {
            // Arrange
            var applicationFilter = new ApplicationFilters.PropertyFilter
            {
                Name = "NonExistent",
                MinPrice = 500000,
                MaxPrice = 600000
            };

            var domainFilter = _mapper.Map<DomainFilters.PropertyFilter>(applicationFilter);

            var properties = new List<Property>();

            _propertyRepositoryMock.Setup(repo => repo.ListAsync(It.Is<DomainFilters.PropertyFilter>(f =>
                f.Name == domainFilter.Name &&
                f.MinPrice == domainFilter.MinPrice &&
                f.MaxPrice == domainFilter.MaxPrice
            ))).ReturnsAsync(properties);

            // Act
            var result = await _propertyService.ListPropertiesAsync(applicationFilter);


            // Assert
            Assert.That(result, Is.Not.Null, "Result should not be null");
            Assert.That(result, Is.Empty, "Expected an empty list of properties");

            _propertyRepositoryMock.Verify(repo => repo.ListAsync(It.Is<DomainFilters.PropertyFilter>(f =>
                f.Name == domainFilter.Name &&
                f.MinPrice == domainFilter.MinPrice &&
                f.MaxPrice == domainFilter.MaxPrice)), Times.Once);
        }

        [Test]
        public async Task ListPropertiesAsync_ShouldReturnListOfPropertyDTOs()
        {
            // Arrange
            var applicationFilter = new ApplicationFilters.PropertyFilter
            {
                Name = "House",
                MinPrice = 100000,
                MaxPrice = 500000
            };

            var domainFilter = _mapper.Map<DomainFilters.PropertyFilter>(applicationFilter);

            var properties = new List<Property>
                            {
                                new Property { IdProperty = 1, Name = "House 1", Price = 200000 },
                                new Property { IdProperty = 2, Name = "House 2", Price = 300000 }
                            };

            _propertyRepositoryMock.Setup(repo => repo.ListAsync(It.Is<DomainFilters.PropertyFilter>(f =>
                f.Name == domainFilter.Name &&
                f.MinPrice == domainFilter.MinPrice &&
                f.MaxPrice == domainFilter.MaxPrice
            ))).ReturnsAsync(properties);

            // Act
            var result = await _propertyService.ListPropertiesAsync(applicationFilter);

            // Assert
            Assert.That(result, Is.Not.Null, "The result should not be null");
            Assert.That(result.Count, Is.EqualTo(2), "Expected two properties in the result");
            Assert.That(result[0].Name, Is.EqualTo("House 1"), "The first property should be 'House 1'");
            Assert.That(result[1].Name, Is.EqualTo("House 2"), "The second property should be 'House 2'");

            _propertyRepositoryMock.Verify(repo => repo.ListAsync(It.Is<DomainFilters.PropertyFilter>(f =>
                f.Name == domainFilter.Name &&
                f.MinPrice == domainFilter.MinPrice &&
                f.MaxPrice == domainFilter.MaxPrice
            )), Times.Once, "ListAsync should be called once with the correct filter");
        }

        [Test]
        public async Task UpdatePropertyAsync_ValidPropertyDTO_UpdatesProperty()
        {
            // Arrange
            var propertyDto = new PropertyDTO
            {
                IdProperty = 1,
                Name = "Updated Property",
                Address = "456 Elm St",
                Price = 200000,
                CodeInternal = "UP-001",
                Year = 2024,
                IdOwner = 1,
                Owner = new OwnerDTO { IdOwner = 1, Name = "John Connor" },
                PropertyImages = new List<PropertyImageDTO>(),
                PropertyTraces = new List<PropertyTraceDTO>()
            };

            _propertyRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Property>()))
                 .ReturnsAsync(true);

            // Act
            var result = await _propertyService.UpdatePropertyAsync(propertyDto);


            // Assert

            Assert.That(result, Is.True, "Expected UpdatePropertyAsync to return true");

            _propertyRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Property>(p =>
                p.IdProperty == propertyDto.IdProperty &&
                p.Name == propertyDto.Name &&
                p.Address == propertyDto.Address &&
                p.Price == propertyDto.Price &&
                p.CodeInternal == propertyDto.CodeInternal &&
                p.Year == propertyDto.Year &&
                p.IdOwner == propertyDto.IdOwner
            )), Times.Once, "UpdateAsync should be called once with correct Property entity");
        }

        [Test]
        public void UpdatePropertyAsync_RepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var propertyDto = new PropertyDTO
            {
                IdProperty = 3,
                Name = "Faulty Property",
                Address = "789 Pine St",
                Price = 300000,
                CodeInternal = "FP-001",
                Year = 2025,
                IdOwner = 2,
                Owner = new OwnerDTO { IdOwner = 2, Name = "Jane Connor" },
                PropertyImages = new List<PropertyImageDTO>(),
                PropertyTraces = new List<PropertyTraceDTO>()
            };

            _propertyRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Property>()))
                .ThrowsAsync(new Exception("Database update error"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _propertyService.UpdatePropertyAsync(propertyDto));            
            Assert.That(ex.Message, Is.EqualTo("Database update error"));

            _propertyRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Property>()), Times.Once);
        }
        
        [Test]
        public async Task GetPropertyByIdAsync_ExistingProperty_ReturnsPropertyDto()
        {
            // Arrange
            int propertyId = 1;
            var property = new Property { IdProperty = propertyId, Name = "Test Property", Price = 100000 };
            _propertyRepositoryMock.Setup(repo => repo.GetByIdAsync(propertyId))
                .ReturnsAsync(property);

            // Act
            var result = await _propertyService.GetPropertyByIdAsync(propertyId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IdProperty, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("Test Property"));
        }

        [Test]
        public async Task GetPropertyByIdAsync_PropertyDoesNotExist_ReturnsNull()
        {
            int propertyId = 2;
            _propertyRepositoryMock.Setup(repo => repo.GetByIdAsync(propertyId))
                .ReturnsAsync((Property)null);

            // Act
            var result = await _propertyService.GetPropertyByIdAsync(propertyId);

            Assert.That(result, Is.Null, "Expected PropertyDTO to be null for non-existent property");

            _propertyRepositoryMock.Verify(repo => repo.GetByIdAsync(propertyId), Times.Once);
        }

    }
}
