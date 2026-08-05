using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HelpDesk.Api.Controllers;
using HelpDesk.Api.Models;
using HelpDesk.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HelpDesk.Tests.Controllers
{
    public class TicketsControllerTests
    {
        private readonly Mock<ITicketRepository> _mockRepository;
        private readonly TicketsController _controller;

        public TicketsControllerTests()
        {
            // Repository is mocked using Moq — these tests never touch SQL Server.
            _mockRepository = new Mock<ITicketRepository>();
            _controller = new TicketsController(_mockRepository.Object);
        }

        private static Ticket CreateSampleTicket(int id = 1, string status = "Open", string priority = "Medium")
        {
            return new Ticket
            {
                Id = id,
                Title = "Sample Ticket",
                Description = "Sample Description",
                Priority = priority,
                Status = status,
                RaisedBy = "John Doe",
                CreatedDate = DateTime.UtcNow
            };
        }

        // ---------------------------------------------------------------
        // Mandatory Test Cases
        // ---------------------------------------------------------------

        [Fact]
        public async Task GetAllTickets_ReturnsOkResult_WhenTicketsExist()
        {
            // Arrange
            var tickets = new List<Ticket> { CreateSampleTicket(1), CreateSampleTicket(2) };
            _mockRepository.Setup(r => r.GetAllTicketsAsync()).ReturnsAsync(tickets);

            // Act
            var result = await _controller.GetAllTickets();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTickets = Assert.IsAssignableFrom<List<Ticket>>(okResult.Value);
            Assert.Equal(2, returnedTickets.Count);
        }

        [Fact]
        public async Task GetTicketById_ReturnsOkResult_WhenTicketExists()
        {
            // Arrange
            var ticket = CreateSampleTicket(1);
            _mockRepository.Setup(r => r.GetTicketByIdAsync(1)).ReturnsAsync(ticket);

            // Act
            var result = await _controller.GetTicketById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTicket = Assert.IsType<Ticket>(okResult.Value);
            Assert.Equal(1, returnedTicket.Id);
        }

        [Fact]
        public async Task GetTicketById_ReturnsNotFound_WhenTicketDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetTicketByIdAsync(It.IsAny<int>())).ReturnsAsync((Ticket)null);

            // Act
            var result = await _controller.GetTicketById(99);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateTicket_ReturnsOkResult_WhenTicketIsCreatedSuccessfully()
        {
            // Arrange
            var newTicket = CreateSampleTicket(0);
            var createdTicket = CreateSampleTicket(1);

            _mockRepository.Setup(r => r.CreateTicketAsync(It.IsAny<Ticket>())).ReturnsAsync(1);
            _mockRepository.Setup(r => r.GetTicketByIdAsync(1)).ReturnsAsync(createdTicket);

            // Act
            var result = await _controller.CreateTicket(newTicket);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedTicket = Assert.IsType<Ticket>(okResult.Value);
            Assert.Equal(1, returnedTicket.Id);
            _mockRepository.Verify(r => r.CreateTicketAsync(It.IsAny<Ticket>()), Times.Once);
        }

        [Fact]
        public async Task CreateTicket_ReturnsBadRequest_WhenTicketIsNull()
        {
            // Act
            var result = await _controller.CreateTicket(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            _mockRepository.Verify(r => r.CreateTicketAsync(It.IsAny<Ticket>()), Times.Never);
        }

        [Fact]
        public async Task GetTicketsByStatus_ReturnsOkResult_WhenMatchingTicketsExist()
        {
            // Arrange
            var tickets = new List<Ticket> { CreateSampleTicket(1, "Open"), CreateSampleTicket(2, "Open") };
            _mockRepository.Setup(r => r.GetTicketsByStatusAsync("Open")).ReturnsAsync(tickets);

            // Act
            var result = await _controller.GetTicketsByStatus("Open");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTickets = Assert.IsAssignableFrom<List<Ticket>>(okResult.Value);
            Assert.Equal(2, returnedTickets.Count);
        }

        // ---------------------------------------------------------------
        // Optional Test Cases
        // ---------------------------------------------------------------

        [Fact]
        public async Task UpdateTicket_ReturnsOkResult_WhenUpdateIsSuccessful()
        {
            // Arrange
            var ticket = CreateSampleTicket(1);
            _mockRepository.Setup(r => r.UpdateTicketAsync(It.IsAny<Ticket>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateTicket(1, ticket);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockRepository.Verify(r => r.UpdateTicketAsync(It.IsAny<Ticket>()), Times.Once);
        }

        [Fact]
        public async Task UpdateTicket_ReturnsNotFound_WhenTicketDoesNotExist()
        {
            // Arrange
            var ticket = CreateSampleTicket(99);
            _mockRepository
                .Setup(r => r.UpdateTicketAsync(It.IsAny<Ticket>()))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.UpdateTicket(99, ticket);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteTicket_ReturnsOkResult_WhenTicketIsDeletedSuccessfully()
        {
            // Arrange
            _mockRepository.Setup(r => r.DeleteTicketAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteTicket(1);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockRepository.Verify(r => r.DeleteTicketAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteTicket_ReturnsNotFound_WhenTicketDoesNotExist()
        {
            // Arrange
            _mockRepository
                .Setup(r => r.DeleteTicketAsync(It.IsAny<int>()))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.DeleteTicket(99);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetAllTickets_ReturnsEmptyList_WhenNoTicketsExist()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllTicketsAsync()).ReturnsAsync(new List<Ticket>());

            // Act
            var result = await _controller.GetAllTickets();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTickets = Assert.IsAssignableFrom<List<Ticket>>(okResult.Value);
            Assert.Empty(returnedTickets);
        }

        [Fact]
        public async Task GetTicketsByStatus_ReturnsEmptyList_WhenNoMatchingTicketsExist()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetTicketsByStatusAsync("Closed")).ReturnsAsync(new List<Ticket>());

            // Act
            var result = await _controller.GetTicketsByStatus("Closed");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTickets = Assert.IsAssignableFrom<List<Ticket>>(okResult.Value);
            Assert.Empty(returnedTickets);
        }
    }
}
