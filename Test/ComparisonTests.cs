using Application.CoachWeb.Services;
using Contracts.CoachWeb.DTO;
using Contracts.CoachWeb.ErrorHandeling;
using Contracts.CoachWeb.Interfaces.Services;
using Moq;

namespace Test
{
    public class DriverComparisonTests
    {
        private readonly Mock<IReportService> _mockReportService;
        private readonly Mock<IDriverComparisonCoordinator> _mockStrategy;
        private readonly DriverComparisonService _service;

        public DriverComparisonTests()
        {
            _mockReportService = new Mock<IReportService>();
            _mockStrategy = new Mock<IDriverComparisonCoordinator>();
            _mockStrategy.SetupGet(s => s.Name).Returns("Fastest Lap");

            _service = new DriverComparisonService(
                _mockReportService.Object
            );
        }

        private DriverReportDTO CreateMockDriverDTO(string driverName = "MockDriver") => new()
        {
            DriverName = driverName,
            SessionId = 1,
            CircuitName = "Test Circuit",
            SessionDate = DateTime.UtcNow,
            Heats = new List<HeatDTO>
            {
                new HeatDTO
                {
                    HeatNumber = 1,
                    Laps = new List<LapDTO>
                    {
                        new LapDTO { LapNumber = 1, LapTime = TimeSpan.FromSeconds(60) }
                    }
                }
            }
        };

        [Fact]
        public async Task CompareDrivers_ValidSessions_ReturnsSuccess()
        {
            var d1 = CreateMockDriverDTO("Driver 1");
            var d2 = CreateMockDriverDTO("Driver 2");

            _mockReportService.Setup(r => r.GetSessionReportAsync(10)).ReturnsAsync(d1);
            _mockReportService.Setup(r => r.GetSessionReportAsync(20)).ReturnsAsync(d2);

            _mockStrategy.Setup(s => s.Compare(d1, d2)).Returns(new ComparisonResultDTO
            {
                MetricName = "Fastest Lap",
                Driver1Value = "1:02.000",
                Driver2Value = "1:01.000",
                Winner = "Driver 2"
            });

            var selectedStrategies = new List<IDriverComparisonCoordinator> { _mockStrategy.Object };

            var result = await _service.CompareDrivers("Driver 1", 10, "Driver 2", 20, selectedStrategies);

            Assert.True(result.IsSuccess);
            Assert.Single(result.Value!.ComparisonResults);
            Assert.Equal("Driver 2", result.Value.ComparisonResults[0].Winner);
        }

        [Fact]
        public async Task CompareDrivers_Session1IsNull_ReturnsFailure()
        {
            _mockReportService.Setup(r => r.GetSessionReportAsync(10)).ReturnsAsync((DriverReportDTO?)null);
            _mockReportService.Setup(r => r.GetSessionReportAsync(20)).ReturnsAsync(CreateMockDriverDTO("Driver 2"));

            var result = await _service.CompareDrivers("Driver 1", 10, "Driver 2", 20, new List<IDriverComparisonCoordinator>());

            Assert.True(result.IsFailure);
            Assert.Equal("One or both sessions could not be found.", result.Error);
            Assert.Equal(ErrorType.UserError, result.ErrorCategory);
        }

        [Fact]
        public async Task CompareDrivers_Session2IsNull_ReturnsFailure()
        {
            _mockReportService.Setup(r => r.GetSessionReportAsync(10)).ReturnsAsync(CreateMockDriverDTO("Driver 1"));
            _mockReportService.Setup(r => r.GetSessionReportAsync(20)).ReturnsAsync((DriverReportDTO?)null);

            var result = await _service.CompareDrivers("Driver 1", 10, "Driver 2", 20, new List<IDriverComparisonCoordinator>());

            Assert.True(result.IsFailure);
            Assert.Equal("One or both sessions could not be found.", result.Error);
            Assert.Equal(ErrorType.UserError, result.ErrorCategory);
        }

        [Fact]
        public async Task CompareDrivers_NoSelectedStrategies_ReturnsEmptyResult()
        {
            var d1 = CreateMockDriverDTO("Driver 1");
            var d2 = CreateMockDriverDTO("Driver 2");

            _mockReportService.Setup(r => r.GetSessionReportAsync(10)).ReturnsAsync(d1);
            _mockReportService.Setup(r => r.GetSessionReportAsync(20)).ReturnsAsync(d2);

            var result = await _service.CompareDrivers("Driver 1", 10, "Driver 2", 20, new List<IDriverComparisonCoordinator>());

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value!.ComparisonResults);
        }

        [Fact]
        public async Task CompareDrivers_StrategyThrowsException_ReturnsSystemFailure()
        {
            var d1 = CreateMockDriverDTO("Driver 1");
            var d2 = CreateMockDriverDTO("Driver 2");

            _mockReportService.Setup(r => r.GetSessionReportAsync(10)).ReturnsAsync(d1);
            _mockReportService.Setup(r => r.GetSessionReportAsync(20)).ReturnsAsync(d2);

            _mockStrategy
                .Setup(s => s.Compare(d1, d2))
                .Throws(new Exception("Test failure"));

            var result = await _service.CompareDrivers("Driver 1", 10, "Driver 2", 20, new List<IDriverComparisonCoordinator> { _mockStrategy.Object });

            Assert.True(result.IsFailure);
            Assert.Equal("An unexpected error occurred while comparing drivers.", result.Error);
            Assert.Equal(ErrorType.SystemError, result.ErrorCategory);
        }
    }
}