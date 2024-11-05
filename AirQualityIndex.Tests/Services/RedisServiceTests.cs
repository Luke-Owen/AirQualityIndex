using AirQualityIndex.Interfaces;
using AirQualityIndex.Models.OpenWeatherMap;
using AirQualityIndex.Services;

namespace AirQualityIndex.Tests.Services;

using Moq;
using NUnit.Framework;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;

[TestFixture]
public class RedisServiceTests
{
    private Mock<IConnectionMultiplexer> _mockConnectionMultiplexer;
    private Mock<IDatabase> _mockDatabase;
    private IRedisService _redisService;

    [SetUp]
    public void Setup()
    {
        _mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
        _mockDatabase = new Mock<IDatabase>();
        _mockConnectionMultiplexer.Setup(conn => conn.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(_mockDatabase.Object);
        _redisService = new RedisService(_mockConnectionMultiplexer.Object);
    }

    [Test]
    public async Task SetAsync_ShouldStoreSerializedValue()
    {
        // Arrange
        const string key = "testKey";
        var value = new { Name = "Test" };
        var serializedValue = JsonSerializer.Serialize(value);

        // Act
        await _redisService.SetAsync(key, value);

        // Assert
        _mockDatabase.Verify(db => db.StringSetAsync(key, serializedValue, null, It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Test]
    public async Task GetAsync_ShouldReturnDeserializedValue()
    {
        // Arrange
        const string key = "testKey";
        var expectedValue = new Main { Aqi = 123 };
        var serializedValue = JsonSerializer.Serialize(expectedValue);
        _mockDatabase.Setup(db => db.StringGetAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(serializedValue);

        // Act
        var result = await _redisService.GetAsync<Main>(key);

        // Assert
        result.Aqi.Should().Be(expectedValue.Aqi);
    }

    [Test]
    public async Task AddToSetAsync_ShouldAddMemberToSet()
    {
        // Arrange
        const string key = "testSet";
        const string value = "setValue";

        // Act
        await _redisService.AddToSetAsync(key, value);

        // Assert
        _mockDatabase.Verify(db => db.SetAddAsync(key, value, It.IsAny<CommandFlags>()), Times.Once);
    }

    [Test]
    public async Task GetSetMembersAsync_ShouldReturnAllSetMembers()
    {
        // Arrange
        const string key = "testSet";
        var expectedMembers = new RedisValue[] { "value1", "value2" };
        _mockDatabase.Setup(db => db.SetMembersAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(expectedMembers);

        // Act
        var result = await _redisService.GetSetMembersAsync(key);

        // Assert
        result.Should().BeEquivalentTo("value1", "value2");
    }

    [Test]
    public async Task SetHashFieldAsync_ShouldStoreHashField()
    {
        // Arrange
        const string hashKey = "testHash";
        const string field = "field1";
        const string value = "value1";

        // Act
        await _redisService.SetHashFieldAsync(hashKey, field, value);

        // Assert
        _mockDatabase.Verify(db => db.HashSetAsync(hashKey, field, value, It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Test]
    public async Task GetHashFieldAsync_ShouldReturnHashFieldValue()
    {
        // Arrange
        const string hashKey = "testHash";
        const string field = "field1";
        const string expectedValue = "value1";
        _mockDatabase.Setup(db => db.HashGetAsync(hashKey, field, It.IsAny<CommandFlags>()))
            .ReturnsAsync(expectedValue);

        // Act
        var result = await _redisService.GetHashFieldAsync(hashKey, field);

        // Assert
        result.Should().Be(expectedValue);
    }

    [Test]
    public async Task PushToListAsync_ShouldPushValueToList()
    {
        // Arrange
        const string key = "testList";
        const string value = "listValue";

        // Act
        await _redisService.PushToListAsync(key, value);

        // Assert
        _mockDatabase.Verify(db => db.ListRightPushAsync(key, value, It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Test]
    public async Task GetListAsync_ShouldReturnAllListValues()
    {
        // Arrange
        const string key = "testList";
        var expectedValues = new RedisValue[] { "value1", "value2" };
        _mockDatabase.Setup(db => db.ListRangeAsync(key, 0, -1, It.IsAny<CommandFlags>()))
            .ReturnsAsync(expectedValues);

        // Act
        var result = await _redisService.GetListAsync(key);

        // Assert
        result.Should().BeEquivalentTo("value1", "value2");
    }

    [Test]
    public async Task SetKeyExpiryAsync_ShouldSetKeyExpiry()
    {
        // Arrange
        const string key = "testKey";
        var expiry = TimeSpan.FromMinutes(5);

        // Act
        await _redisService.SetKeyExpiryAsync(key, expiry);

        // Assert
        _mockDatabase.Verify(db => db.KeyExpireAsync(key, expiry, It.IsAny<ExpireWhen>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Test]
    public async Task KeyExistsAsync_ShouldReturnIfKeyExists()
    {
        // Arrange
        const string key = "testKey";
        _mockDatabase.Setup(db => db.KeyExistsAsync(key, It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        var result = await _redisService.KeyExistsAsync(key);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public async Task DeleteKeyAsync_ShouldDeleteKey()
    {
        // Arrange
        const string key = "testKey";

        // Act
        await _redisService.DeleteKeyAsync(key);

        // Assert
        _mockDatabase.Verify(db => db.KeyDeleteAsync(key, It.IsAny<CommandFlags>()), Times.Once);
    }
}