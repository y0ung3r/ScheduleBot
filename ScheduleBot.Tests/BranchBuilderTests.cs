using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ScheduleBot.Extensions;
using ScheduleBot.Interfaces;

namespace ScheduleBot.Tests
{
    public class BranchBuilderTests
    {
        private IBranchBuilder CreateBranchBuilder()
        {
            var services = new ServiceCollection();
            var serviceProvider = services.BuildServiceProvider();
            return new BranchBuilder(serviceProvider);
        }

        [Test]
        public void Building_a_branch_with_a_handler()
        {
            // Arrange
            var stub = new FakeRequestHandler();
            var sut = CreateBranchBuilder();
            sut.UseHandler(stub);

            // Act
            var requestDelegate = sut.Build();

            // Assert
            requestDelegate.Should().NotBeNull();
        }

        [Test]
        public void Building_a_branch_with_a_command()
        {
            // Arrange
            var sut = CreateBranchBuilder();
            sut.UseCommand<FakeCommandHandler>();

            // Act
            var requestDelegate = sut.Build();

            // Assert
            requestDelegate.Should().NotBeNull();
        }

        [Test]
        public void Building_a_branch_with_a_handler_and_a_command()
        {
            // Arrange
            var requestHandlerStub = new FakeRequestHandler();
            var sut = CreateBranchBuilder();

            sut.UseHandler(requestHandlerStub)
               .UseCommand<FakeCommandHandler>();

            // Act
            var requestDelegate = sut.Build();

            // Assert
            requestDelegate.Should().NotBeNull();
        }
    }
}
