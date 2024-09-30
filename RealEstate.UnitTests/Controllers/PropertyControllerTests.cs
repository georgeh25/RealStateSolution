using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealEstate.API;
using RealEstate.Application.DTOs;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Data;
using System.Net;
using System.Net.Http.Json;


namespace RealEstate.UnitTests.Controllers
{
    //
    //Important: The integration tests were implemented without JWT security verification, so the `[Authorize]` attribute in the PropertyController should be commented out.
    //
    [TestFixture]
    public class PropertyControllerTests
    {
        private CustomWebApplicationFactory<Program> _factory;
        private HttpClient _client;
        private OwnerDTO _testOwner;

        [SetUp]
        public async Task Setup()
        {
            //Configure the CustomWebApplicationFactory to create a test server
            //Access the scoped service to obtain the DbContext
            _factory = new CustomWebApplicationFactory<Program>();
            _client = _factory.CreateClient();

            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<RealEstateDbContext>();

                var owner = new Owner
                {
                    Name = "Test Owner",
                    Address = "456 Test Ave",
                    Photo = "testphoto.png",
                    Birthday = new DateTime(1990, 1, 1)
                };

                db.Owners.Add(owner);
                await db.SaveChangesAsync();

                _testOwner = new OwnerDTO
                {
                    Name = owner.Name,
                    Address = owner.Address,
                    Photo = owner.Photo,
                    Birthday = owner.Birthday
                };
            }
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task GetPropertyById_ShouldReturnProperty_WhenPropertyExists()
        {
            // Arrange
            var newProperty = new PropertyDTO
            {
                Name = "Test Property",
                Address = "123 Test St",
                Price = 300000,
                CodeInternal = "TP001",
                Year = 2021,
                IdOwner = _testOwner.IdOwner,
                Owner = _testOwner
            };

            var createResponse = await _client.PostAsJsonAsync("/api/Property", newProperty);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdProperty = await createResponse.Content.ReadFromJsonAsync<PropertyDTO>();
            var existingPropertyId = createdProperty.IdProperty;

            // Act
            var response = await _client.GetAsync($"/api/Property/{existingPropertyId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var property = await response.Content.ReadFromJsonAsync<PropertyDTO>();
            property.Should().NotBeNull();
            property.IdProperty.Should().Be(existingPropertyId);
            property.Name.Should().Be(newProperty.Name);
            property.Address.Should().Be(newProperty.Address);
            property.Price.Should().Be(newProperty.Price);
            property.CodeInternal.Should().Be(newProperty.CodeInternal);
            property.Year.Should().Be(newProperty.Year);
        }

        [Test]
        public async Task GetPropertyById_ShouldReturnNotFound_WhenPropertyDoesNotExist()
        {
            // Arrange
            int nonExistingPropertyId = 9999;

            // Act
            var response = await _client.GetAsync($"/api/Property/{nonExistingPropertyId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task CreateProperty_ValidData_ReturnsCreatedResponse()
        {
            // Arrange
            var newProperty = new PropertyDTO
            {
                Name = "Test Property",
                Address = "123 Test St",
                Price = 200000,
                CodeInternal = "TP001",
                Year = 2020,
                IdOwner = _testOwner.IdOwner,
                Owner = _testOwner
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Property", newProperty);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();

            var createdProperty = await response.Content.ReadFromJsonAsync<PropertyDTO>();
            createdProperty.Should().NotBeNull();
            createdProperty.IdProperty.Should().BeGreaterThan(0);
            createdProperty.Name.Should().Be(newProperty.Name);
            createdProperty.Address.Should().Be(newProperty.Address);
            createdProperty.Price.Should().Be(newProperty.Price);
            createdProperty.CodeInternal.Should().Be(newProperty.CodeInternal);
            createdProperty.Year.Should().Be(newProperty.Year);
        }

        [Test]
        public async Task ListProperties_ShouldReturnListOfProperties()
        {
            // Arrange
            var propertiesToCreate = new List<PropertyDTO>
            {
                new PropertyDTO { Name = "House 1", Address = "123 House St", Price = 200000, CodeInternal = "CB001", Year = 2020, IdOwner = _testOwner.IdOwner, Owner = _testOwner },
                new PropertyDTO { Name = "House 2", Address = "456 House Ave", Price = 300000, CodeInternal = "CM001", Year = 2021, IdOwner = _testOwner.IdOwner,  Owner = _testOwner }
            };

            foreach (var prop in propertiesToCreate)
            {
                var response = await _client.PostAsJsonAsync("/api/Property", prop);
                response.StatusCode.Should().Be(HttpStatusCode.Created);
            }

            var queryString = "?Name=House";

            // Act
            var responseGet = await _client.GetAsync($"/api/Property{queryString}");

            // Assert
            responseGet.StatusCode.Should().Be(HttpStatusCode.OK);

            var properties = await responseGet.Content.ReadFromJsonAsync<List<PropertyDTO>>();
            properties.Should().NotBeNull();
            properties.Count.Should().BeGreaterThan(0);
            properties.Should().OnlyContain(p => p.Name.Contains("House"));
        }

        [Test]
        public async Task ListProperties_WithPriceFilter_ReturnsFilteredProperties()
        {
            // Arrange
            var properties = new[]
            {
                new PropertyDTO { Name = "Cheap House", Address = "123 House St", Price = 200000, CodeInternal = "CB001", Year = 2020, IdOwner = _testOwner.IdOwner, Owner = _testOwner },
                new PropertyDTO { Name = "Expensive House", Address = "456 House Ave", Price = 300000, CodeInternal = "CM001", Year = 2021, IdOwner = _testOwner.IdOwner,  Owner = _testOwner },
                new PropertyDTO { Name = "Mid-range House", Address = "789 House Dr", Price = 400000, CodeInternal = "CX001", Year = 2022, IdOwner = _testOwner.IdOwner,  Owner = _testOwner }
            };

            foreach (var prop in properties)
            {
                var response = await _client.PostAsJsonAsync("/api/Property", prop);
                response.StatusCode.Should().Be(HttpStatusCode.Created);
            }

            // Act
            var responseGet = await _client.GetAsync("/api/Property?MinPrice=200000&MaxPrice=400000");

            // Assert
            responseGet.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedProperties = await responseGet.Content.ReadFromJsonAsync<List<PropertyDTO>>();
            returnedProperties.Should().NotBeNull();            
            returnedProperties[0].Price.Should().Be(200000);
            returnedProperties.Should().OnlyContain(p => p.Price >= 200000 && p.Price <= 400000);
        }

        [Test]
        public async Task CreateProperty_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var invalidProperty = new PropertyDTO
            {
                // Name Missing
                Address = "123 Test St",
                Price = -100000, // Negative Price
                CodeInternal = "TP001",
                Year = 2020,
                IdOwner = _testOwner.IdOwner,
                Owner = _testOwner
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Property", invalidProperty);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var errorContent = await response.Content.ReadAsStringAsync();
            errorContent.Should().Contain("Name");
            errorContent.Should().Contain("Price");
        }

    }
}
