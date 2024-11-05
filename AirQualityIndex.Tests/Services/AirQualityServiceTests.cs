using AirQualityIndex.Models.OpenWeatherMap;
using Moq.Protected;

namespace AirQualityIndex.Tests.Services;

using Moq;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using AirQualityIndex.Services;

[TestFixture]
public class AirQualityServiceTests
{
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private HttpClient _httpClient;
    private AirQualityService _airQualityService;

    [SetUp]
    public void Setup()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        
        _mockHttpMessageHandler
            .Protected()
            .Setup("Dispose", ItExpr.IsAny<bool>());

        
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        
        _mockConfiguration.Setup(config => config.GetSection("OpenWeatherMapApiUrl").Value)
            .Returns("http://test-api-url.com");
        Environment.SetEnvironmentVariable("OpenWeatherMapApiKey", "test-api-key");
        
        _airQualityService = new AirQualityService(_mockConfiguration.Object, _httpClient);
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient.Dispose();
    }

    [Test]
    public async Task GetAirQuality_ShouldReturnListOfAirQualityResponseModels_WhenApiCallIsSuccessful()
    {
        // Arrange
        var fromDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var toDate = new DateTime(2023, 1, 2, 0, 0, 0, DateTimeKind.Utc);
        const decimal latitude = 51.5074m;
        const decimal longitude = -0.1278m;
        
        var apiResponse = new AirQualityResponse
        {
            List = [new AirQuality { Main = new Main { Aqi = 3 }, Dt = 1672444800 }]
        };
        var responseContent = JsonConvert.SerializeObject(apiResponse);
        
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });
        
        // Act
        var result = await _airQualityService.GetAirQuality(fromDate, toDate, latitude, longitude);
        
        // Assert
        result.Should().HaveCount(1);
        result[0].AirQualityIndex.Should().Be(3);
        result[0].Date.Should().Be(DateTimeOffset.FromUnixTimeSeconds(1672444800).UtcDateTime);
    }

    [Test]
    public async Task GetAirQuality_ShouldThrowHttpRequestException_WhenApiCallFails()
    {
        // Arrange
        var fromDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var toDate = new DateTime(2023, 1, 2, 0, 0, 0, DateTimeKind.Utc);
        const decimal latitude = 51.5074m;
        const decimal longitude = -0.1278m;

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Bad Request")
            });

        // Act
        Func<Task> act = async () => await _airQualityService.GetAirQuality(fromDate, toDate, latitude, longitude);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage($"Unable to get air qualities from OpenWeatherMap with code: {HttpStatusCode.BadRequest} and content: Bad Request");
    }

    [Test]
    public async Task GetCurrentAirQuality_ShouldReturnAirQualityResponseModel_WhenApiCallIsSuccessful()
    {
        // Arrange
        const decimal latitude = 51.5074m;
        const decimal longitude = -0.1278m;
        
        var apiResponse = new AirQualityResponse
        {
            List = [new AirQuality() { Main = new Main { Aqi = 4 }, Dt = 1672444800 }]
        };
        var responseContent = JsonConvert.SerializeObject(apiResponse);
        
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });
        
        // Act
        var result = await _airQualityService.GetCurrentAirQuality(latitude, longitude);
        
        // Assert
        result.AirQualityIndex.Should().Be(4);
        result.Date.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Test]
    public async Task GetCurrentAirQuality_ShouldThrowHttpRequestException_WhenApiCallFails()
    {
        // Arrange
        const decimal latitude = 51.5074m;
        const decimal longitude = -0.1278m;

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Bad Request")
            });

        // Act
        Func<Task> act = async () => await _airQualityService.GetCurrentAirQuality(latitude, longitude);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage($"Unable to get air quality from OpenWeatherMap with code: {HttpStatusCode.BadRequest} and content: Bad Request");
    }

    [Test]
    public void AirQualityIndexKey_ShouldReturnCorrectKeyFormat()
    {
        // Arrange
        var fromDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var toDate = new DateTime(2023, 1, 2, 0, 0, 0, DateTimeKind.Utc);
        const decimal latitude = 51.5074m;
        const decimal longitude = -0.1278m;

        // Act
        var result = _airQualityService.AirQualityIndexKey(fromDate, toDate, latitude, longitude);

        // Assert
        result.Should().Be("airqualityindex_2023-01-01_2023-01-02_51.5074_-0.1278");
    }
}
