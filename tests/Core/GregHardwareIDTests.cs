using System;
using System.Reflection;
using System.Runtime.Serialization;
using Xunit;
using FluentAssertions;

namespace gregCore.Tests.Core.Persistence
{
    public class GregHardwareIDTests
    {
        private readonly Type _hardwareIdType;

        public GregHardwareIDTests()
        {
            var assembly = Assembly.Load("gregCore");
            _hardwareIdType = assembly.GetType("gregCore.Core.Persistence.GregHardwareID")!;
        }

        [Fact]
        public void GetId_WhenCalledInitially_ShouldGenerateNewGuid()
        {
            // Arrange
            var hw = FormatterServices.GetUninitializedObject(_hardwareIdType);
            var fieldInfo = _hardwareIdType.GetField("_hardwareId", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo?.SetValue(hw, "");

            // Act
            var getMethod = _hardwareIdType.GetMethod("GetId");
            var result = getMethod?.Invoke(hw, null) as string;

            // Assert
            result.Should().NotBeNullOrEmpty();
            Guid.TryParse(result, out _).Should().BeTrue("because the generated ID should be a valid GUID");

            // Subsequent calls should return the same ID
            var secondResult = getMethod?.Invoke(hw, null) as string;
            secondResult.Should().Be(result, "because the ID should be cached after generation");
        }

        [Fact]
        public void GetId_WhenIdAlreadySet_ShouldReturnExistingId()
        {
            // Arrange
            var hw = FormatterServices.GetUninitializedObject(_hardwareIdType);
            var expectedId = "my-custom-id-123";
            var fieldInfo = _hardwareIdType.GetField("_hardwareId", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo?.SetValue(hw, expectedId);

            // Act
            var getMethod = _hardwareIdType.GetMethod("GetId");
            var result = getMethod?.Invoke(hw, null) as string;

            // Assert
            result.Should().Be(expectedId);
        }
    }
}
