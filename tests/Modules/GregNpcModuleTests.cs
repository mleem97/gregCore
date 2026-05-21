using System;
using Xunit;
using FluentAssertions;
using NSubstitute;
using gregCore.PublicApi;
using gregCore.PublicApi.Modules;
using gregCore.Core.Abstractions;

namespace gregCore.Tests.Modules
{
    public class GregNpcModuleTests
    {
        private readonly GregApiContext _context;
        private readonly GregNpcModule _sut;
        private readonly IGregLogger _mockLogger;
        private readonly INpcSubsystem _mockSubsystem;

        public GregNpcModuleTests()
        {
            _mockLogger = Substitute.For<IGregLogger>();
            _context = new GregApiContext { Logger = _mockLogger };
            _mockSubsystem = Substitute.For<INpcSubsystem>();
            _sut = new GregNpcModule(_context, _mockSubsystem);
        }

        [Fact]
        public void UpdateNpcState_WhenExceptionThrown_LogsError()
        {
            // Arrange
            var exception = new Exception("Mock exception");
            _mockSubsystem.When(x => x.UpdateState(Arg.Any<string>(), Arg.Any<string>()))
                          .Do(x => throw exception);

            // Act
            _sut.UpdateNpcState("npc123", "idle");

            // Assert
            _mockLogger.Received(1).Error("NPC update failed: Mock exception");
        }

        [Fact]
        public void UpdateNpcState_WhenNoException_DoesNotLogError()
        {
            // Act
            _sut.UpdateNpcState("npc123", "idle");

            // Assert
            _mockLogger.DidNotReceiveWithAnyArgs().Error(default);
        }
    }
}
