using Xunit;
using Moq;
using Lama.Application.Services;
using Lama.Domain.Enums;
using Lama.Infrastructure.Services;

namespace Lama.UnitTests.Services;

/// <summary>
/// Unit tests para PointsCalculatorService
/// </summary>
public class PointsCalculatorServiceTests
{
    private readonly Mock<IAppConfigProvider> _mockConfigProvider;
    private readonly PointsCalculatorService _service;

    public PointsCalculatorServiceTests()
    {
        _mockConfigProvider = new Mock<IAppConfigProvider>();
        _service = new PointsCalculatorService(_mockConfigProvider.Object);

        // Configurar defaults para el mock
        _mockConfigProvider.Setup(x => x.GetIntAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns<string, int, CancellationToken>((key, defaultValue, _) =>
                Task.FromResult(defaultValue));

        _mockConfigProvider.Setup(x => x.GetDoubleAsync(It.IsAny<string>(), It.IsAny<double>(), It.IsAny<CancellationToken>()))
            .Returns<string, double, CancellationToken>((key, defaultValue, _) =>
                Task.FromResult(defaultValue));
    }

    #region Points Per Class Tests

    [Fact]
    public async Task CalculateAsync_Class1_ReturnsCorrectPointsPerEvent()
    {
        // Arrange
        double mileage = 100;
        int eventClass = 1;

        // Act
        var result = await _service.CalculateAsync(
            mileage, eventClass, "USA", "Americas", "USA", "Americas", CancellationToken.None);

        // Assert
        Assert.Equal(1, result.PointsPerEvent); // Class 1 = 1 punto
    }

    [Fact]
    public async Task CalculateAsync_Class2_ReturnsCorrectPointsPerEvent()
    {
        // Arrange
        double mileage = 100;
        int eventClass = 2;

        // Act
        var result = await _service.CalculateAsync(
            mileage, eventClass, "USA", "Americas", "USA", "Americas", CancellationToken.None);

        // Assert
        Assert.Equal(3, result.PointsPerEvent); // Class 2 = 3 puntos
    }

    [Fact]
    public async Task CalculateAsync_Class5_ReturnsCorrectPointsPerEvent()
    {
        // Arrange
        double mileage = 100;
        int eventClass = 5;

        // Act
        var result = await _service.CalculateAsync(
            mileage, eventClass, "USA", "Americas", "USA", "Americas", CancellationToken.None);

        // Assert
        Assert.Equal(15, result.PointsPerEvent); // Class 5 = 15 puntos
    }

    #endregion

    #region Points Per Distance Tests

    [Fact]
    public async Task CalculateAsync_SmallDistance_ReturnsZeroPointsPerDistance()
    {
        // Arrange: < 200 millas
        double mileage = 150;
        int eventClass = 1;

        // Act
        var result = await _service.CalculateAsync(
            mileage, eventClass, "USA", "Americas", "USA", "Americas", CancellationToken.None);

        // Assert
        Assert.Equal(0, result.PointsPerDistance);
    }

    [Fact]
    public async Task CalculateAsync_MediumDistance_ReturnsOnePointPerDistance()
    {
        // Arrange: 200-800 millas
        double mileage = 300;
        int eventClass = 1;

        // Act
        var result = await _service.CalculateAsync(
            mileage, eventClass, "USA", "Americas", "USA", "Americas", CancellationToken.None);

        // Assert
        Assert.Equal(1, result.PointsPerDistance);
    }

    [Fact]
    public async Task CalculateAsync_LongDistance_ReturnsTwoPointsPerDistance()
    {
        // Arrange: > 800 millas
        double mileage = 1000;
        int eventClass = 1;

        // Act
        var result = await _service.CalculateAsync(
            mileage, eventClass, "USA", "Americas", "USA", "Americas", CancellationToken.None);

        // Assert
        Assert.Equal(2, result.PointsPerDistance);
    }

    [Fact]
    public async Task CalculateAsync_ExactlyThreshold1_ReturnsOnePoint()
    {
        // Arrange
        double mileage = 200;
        int eventClass = 1;

        // Act
        var result = await _service.CalculateAsync(
            mileage, eventClass, "USA", "Americas", "USA", "Americas", CancellationToken.None);

        // Assert
        Assert.Equal(0, result.PointsPerDistance); // Exacto no supera
    }

    [Fact]
    public async Task CalculateAsync_JustAboveThreshold1_ReturnsOnePoint()
    {
        // Arrange
        double mileage = 200.1;
        int eventClass = 1;

        // Act
        var result = await _service.CalculateAsync(
            mileage, eventClass, "USA", "Americas", "USA", "Americas", CancellationToken.None);

        // Assert
        Assert.Equal(1, result.PointsPerDistance);
    }

    #endregion

    #region Visitor Bonus Tests

    [Fact]
    public async Task CalculateAsync_SameCountry_ReturnsLocalZeroBonus()
    {
        // Arrange
        double mileage = 100;
        int eventClass = 1;

        // Act
        var result = await _service.CalculateAsync(
            mileage, eventClass, "Colombia", "Americas", "Colombia", "Americas", CancellationToken.None);

        // Assert
        Assert.Equal(0, result.VisitorBonus);
        Assert.Equal(VisitorClass.Local, result.VisitorClassification);
    }

    [Fact]
    public async Task CalculateAsync_SameContinentDifferentCountry_ReturnsVisitorABonus()
    {
        // Arrange
        double mileage = 100;
        int eventClass = 1;

        // Act
        var result = await _service.CalculateAsync(
            mileage, eventClass, "Colombia", "Americas", "USA", "Americas", CancellationToken.None);

        // Assert
        Assert.Equal(1, result.VisitorBonus);
        Assert.Equal(VisitorClass.VisitorA, result.VisitorClassification);
    }

    [Fact]
    public async Task CalculateAsync_DifferentContinents_ReturnsVisitorBBonus()
    {
        // Arrange
        double mileage = 100;
        int eventClass = 1;

        // Act
        var result = await _service.CalculateAsync(
            mileage, eventClass, "Colombia", "Americas", "France", "Europe", CancellationToken.None);

        // Assert
        Assert.Equal(2, result.VisitorBonus);
        Assert.Equal(VisitorClass.VisitorB, result.VisitorClassification);
    }

    #endregion

    #region Total Points Tests

    [Fact]
    public async Task CalculateAsync_ComplexScenario_CalculatesTotalCorrectly()
    {
        // Arrange
        double mileage = 500; // 1 punto por distancia (200-800)
        int eventClass = 3; // 5 puntos por clase
        // Visitante A = 1 punto bonus

        // Act
        var result = await _service.CalculateAsync(
            mileage, eventClass, "Mexico", "Americas", "USA", "Americas", CancellationToken.None);

        // Assert
        Assert.Equal(5, result.PointsPerEvent);
        Assert.Equal(1, result.PointsPerDistance);
        Assert.Equal(1, result.VisitorBonus);
        Assert.Equal(7, result.TotalPoints); // 5 + 1 + 1
    }

    #endregion

    #region Conversion Tests

    [Fact]
    public void ConvertMilesToKilometers_Returns_CorrectValue()
    {
        // Arrange
        double miles = 100;

        // Act
        var km = _service.ConvertMilesToKilometers(miles);

        // Assert
        Assert.Equal(160.9344, km, precision: 2);
    }

    [Fact]
    public void ConvertKilometersToMiles_Returns_CorrectValue()
    {
        // Arrange
        double kilometers = 100;

        // Act
        var miles = _service.ConvertKilometersToMiles(kilometers);

        // Assert
        Assert.Equal(62.1371, miles, precision: 2);
    }

    #endregion
}
