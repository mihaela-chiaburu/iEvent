using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using iEvent.WebApi.Controllers;
using iEvent.Application.Interfaces.Services;
using iEvent.Application.DTOs.Event;
using iEvent.Application.DTOs.Common;
using iEvent.Application.DTOs.Tickets;

namespace iEvent.Tests.Controllers
{
    public class EventsControllerTests
    {
        private readonly Mock<IEventService> _eventServiceMock;
        private readonly Mock<ITicketTypeService> _ticketTypeServiceMock;
        private readonly EventsController _controller;

        public EventsControllerTests()
        {
            _eventServiceMock = new Mock<IEventService>();
            _ticketTypeServiceMock = new Mock<ITicketTypeService>();

            _controller = new EventsController(_eventServiceMock.Object, _ticketTypeServiceMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WithPagedResult()
        {
            // Arrange
            var queryDto = new EventQueryDto();
            var expectedPagedResult = new PagedResultDto<EventRespDto>
            {
                Items = new List<EventRespDto>(),
                TotalCount = 0
            };

            _eventServiceMock
                .Setup(s => s.GetAllAsync(queryDto))
                .ReturnsAsync(expectedPagedResult);

            // Act
            var result = await _controller.GetAll(queryDto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedPagedResult);
        }

        [Fact]
        public async Task GetAll_WithAllFiltersPopulated_ShouldPassQueryToServiceAndReturnOk()
        {
            // Arrange
            var fullQueryDto = new EventQueryDto
            {
                City = "Chisinau",
                VenueId = Guid.NewGuid(),
                Category = (iEvent.Domain.Enums.EventCategory)2,
                ToDate = new DateOnly(2026, 07, 31),
                MinPrice = 50.0m,
                MaxPrice = 500.0m,
                SortBy = "date_desc",
                Page = 1,
                PageSize = 10
            };

            var expectedPagedResult = new PagedResultDto<EventRespDto>
            {
                TotalCount = 0,
                Items = new List<EventRespDto>()
            };

            _eventServiceMock
                .Setup(s => s.GetAllAsync(fullQueryDto))
                .ReturnsAsync(expectedPagedResult);

            // Act
            var result = await _controller.GetAll(fullQueryDto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedPagedResult);

            _eventServiceMock.Verify(s => s.GetAllAsync(It.Is<EventQueryDto>(q =>
                q.City == "Chisinau" &&
                q.MinPrice == 50.0m &&
                (int?)q.Category == 2 &&
                q.SortBy == "date_desc" &&
                q.PageSize == 10
            )), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WithEventData()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var expectedEvent = new EventRespDto { EventId = eventId, Name = "Test Event" };

            _eventServiceMock
                .Setup(s => s.GetByIdAsync(eventId))
                .ReturnsAsync(expectedEvent);

            // Act
            var result = await _controller.GetById(eventId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedEvent);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WithCreatedEvent()
        {
            // Arrange
            var createDto = new EventCreateDto { Name = "New Event" };
            var createdEvent = new EventRespDto { EventId = Guid.NewGuid(), Name = "New Event" };

            _eventServiceMock
                .Setup(s => s.CreateAsync(createDto))
                .ReturnsAsync(createdEvent);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdResult.RouteValues["id"].Should().Be(createdEvent.EventId);
            createdResult.Value.Should().BeEquivalentTo(createdEvent);
        }

        [Fact]
        public async Task Patch_ShouldReturnNoContent_AndCallService()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var patchDto = new EventPatchDto
            {
                Name = "Updated Title",
                Description = "Updated description",
                VenueId = Guid.NewGuid(),
                Category = Domain.Enums.EventCategory.Conference,
                EventDates = new List<EventDateCreateDto>(),
                Images = new List<EventImageCreateDto>()
            };

            _eventServiceMock
                .Setup(s => s.PatchAsync(eventId, patchDto))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Patch(eventId, patchDto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _eventServiceMock.Verify(s => s.PatchAsync(eventId, patchDto), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_AndCallService()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            _eventServiceMock
                .Setup(s => s.DeleteAsync(eventId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(eventId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _eventServiceMock.Verify(s => s.DeleteAsync(eventId), Times.Once);
        }

        [Fact]
        public async Task GetTicketTypes_ShouldReturnOk_WithTicketTypesList()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var expectedTickets = new List<TicketTypeRespDto> { new TicketTypeRespDto { Name = "VIP" } };

            _ticketTypeServiceMock
                .Setup(s => s.GetAllAsync(eventId))
                .ReturnsAsync(expectedTickets);

            // Act
            var result = await _controller.GetTicketTypes(eventId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedTickets);
        }

        [Fact]
        public async Task GetPreviewByCategory_ShouldReturnBadRequest_WhenCategoryIsEmpty()
        {
            // Arrange
            string emptyCategory = "";

            // Act
            var result = await _controller.GetPreviewByCategory(emptyCategory);

            // Assert
            var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Category is required.");
        }

        [Fact]
        public async Task GetPreviewByCategory_ShouldReturnOk_WhenCategoryIsValid()
        {
            // Arrange
            string category = "Concerts";
            var expectedEvents = new List<EventRespDto> { new EventRespDto { Name = "Rock Show" } };

            _eventServiceMock
                .Setup(s => s.GetPreviewByCategoryAsync(category, 4))
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.GetPreviewByCategory(category);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedEvents);
        }
    }
}