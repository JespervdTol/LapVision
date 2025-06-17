using Application.CoachWeb.Services;
using Contracts.CoachWeb.ErrorHandeling;
using Contracts.CoachWeb.Interfaces.Services;
using Contracts.CoachWeb.ViewModels.Comparison;
using Contracts.CoachWeb.ViewModels.Report;
using Moq;
using Xunit;

namespace Test
{
    public class DriverComparisonTests
    {
        private readonly Mock<IReportService> _mockReportService;
        private readonly Mock<IDriverComparisonService> _mockStrategy;
        private readonly DriverComparisonService _service;

        public DriverComparisonTests()
        {
            _mockReportService = new Mock<IReportService>();
            _mockStrategy = new Mock<IDriverComparisonService>();
            _mockStrategy.SetupGet(s => s.Name).Returns("Fastest Lap");

            _service = new DriverComparisonService(
                new List<IDriverComparisonService> { _mockStrategy.Object },
                _mockReportService.Object
            );
        }

        [Fact]
        public async Task CompareDrivers_ValidSessions_ReturnsSuccess()
        {
            var d1 = new DriverReportViewModel();
            var d2 = new DriverReportViewModel();

            _mockReportService.Setup(r => r.GetSessionReportAsync(10)).ReturnsAsync(d1);
            _mockReportService.Setup(r => r.GetSessionReportAsync(20)).ReturnsAsync(d2);

            _mockStrategy.Setup(s => s.Compare(d1, "Driver 1", d2, "Driver 2"))
                .Returns(new ComparisonResultViewModel
                {
                    MetricName = "Fastest Lap",
                    Driver1Value = "1:02.000",
                    Driver2Value = "1:01.000",
                    Winner = "Driver 2"
                });

            var result = await _service.CompareDrivers("Driver 1", 10, "Driver 2", 20, new List<string> { "Fastest Lap" });

            Assert.True(result.IsSuccess);
            Assert.Single(result.Value!.ComparisonResults);
            Assert.Equal("Driver 2", result.Value.ComparisonResults[0].Winner);
        }

        [Fact]
        public async Task CompareDrivers_Session1IsNull_ReturnsFailure()
        {
            _mockReportService.Setup(r => r.GetSessionReportAsync(10)).ReturnsAsync((DriverReportViewModel?)null);
            _mockReportService.Setup(r => r.GetSessionReportAsync(20)).ReturnsAsync(new DriverReportViewModel());

            var result = await _service.CompareDrivers("Driver 1", 10, "Driver 2", 20, new List<string> { "Fastest Lap" });

            Assert.True(result.IsFailure);
            Assert.Equal("One or both sessions could not be found.", result.Error);
            Assert.Equal(ErrorType.UserError, result.ErrorCategory);
        }

        [Fact]
        public async Task CompareDrivers_Session2IsNull_ReturnsFailure()
        {
            _mockReportService.Setup(r => r.GetSessionReportAsync(10)).ReturnsAsync(new DriverReportViewModel());
            _mockReportService.Setup(r => r.GetSessionReportAsync(20)).ReturnsAsync((DriverReportViewModel?)null);

            var result = await _service.CompareDrivers("Driver 1", 10, "Driver 2", 20, new List<string> { "Fastest Lap" });

            Assert.True(result.IsFailure);
            Assert.Equal("One or both sessions could not be found.", result.Error);
            Assert.Equal(ErrorType.UserError, result.ErrorCategory);
        }

        [Fact]
        public async Task CompareDrivers_NoSelectedMetrics_ReturnsEmptyResult()
        {
            var d1 = new DriverReportViewModel();
            var d2 = new DriverReportViewModel();

            _mockReportService.Setup(r => r.GetSessionReportAsync(10)).ReturnsAsync(d1);
            _mockReportService.Setup(r => r.GetSessionReportAsync(20)).ReturnsAsync(d2);

            var result = await _service.CompareDrivers("Driver 1", 10, "Driver 2", 20, new List<string>());

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value!.ComparisonResults);
        }

        [Fact]
        public async Task CompareDrivers_UnknownMetric_ReturnsEmptyResult()
        {
            var d1 = new DriverReportViewModel();
            var d2 = new DriverReportViewModel();

            _mockReportService.Setup(r => r.GetSessionReportAsync(10)).ReturnsAsync(d1);
            _mockReportService.Setup(r => r.GetSessionReportAsync(20)).ReturnsAsync(d2);

            var result = await _service.CompareDrivers("Driver 1", 10, "Driver 2", 20, new List<string> { "Nonexistent Metric" });

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value!.ComparisonResults);
        }

        [Fact]
        public async Task CompareDrivers_DuplicateMetrics_ShouldRunOnlyOnce()
        {
            var d1 = new DriverReportViewModel();
            var d2 = new DriverReportViewModel();

            _mockReportService.Setup(r => r.GetSessionReportAsync(10)).ReturnsAsync(d1);
            _mockReportService.Setup(r => r.GetSessionReportAsync(20)).ReturnsAsync(d2);

            _mockStrategy.Setup(s => s.Compare(d1, "Driver 1", d2, "Driver 2"))
                .Returns(new ComparisonResultViewModel
                {
                    MetricName = "Fastest Lap",
                    Driver1Value = "1:01.000",
                    Driver2Value = "1:00.000",
                    Winner = "Driver 2"
                });

            var result = await _service.CompareDrivers("Driver 1", 10, "Driver 2", 20, new List<string> { "Fastest Lap", "Fastest Lap" });

            Assert.True(result.IsSuccess);
            Assert.Single(result.Value!.ComparisonResults);
        }

        [Fact]
        public async Task CompareDrivers_StrategyThrowsException_ReturnsSystemFailure()
        {
            var d1 = new DriverReportViewModel();
            var d2 = new DriverReportViewModel();

            _mockReportService.Setup(r => r.GetSessionReportAsync(10)).ReturnsAsync(d1);
            _mockReportService.Setup(r => r.GetSessionReportAsync(20)).ReturnsAsync(d2);

            _mockStrategy.Setup(s => s.Compare(d1, "Driver 1", d2, "Driver 2"))
                .Throws(new Exception("Something failed inside strategy"));

            var result = await _service.CompareDrivers("Driver 1", 10, "Driver 2", 20, new List<string> { "Fastest Lap" });

            Assert.True(result.IsFailure);
            Assert.Equal("An unexpected error occurred while comparing drivers.", result.Error);
            Assert.Equal(ErrorType.SystemError, result.ErrorCategory);
        }
    }
}