using M450.UnitTesting.App;

namespace M450.UnitTesting.App.Tests;

public class PriceCalculatorTests
{
    [Theory]
    [InlineData(10.00, 2, 0, 20.00)]
    [InlineData(10.00, 2, 10, 18.00)]
    [InlineData(10.00, 0, 0, 0.00)]
    [InlineData(10.00, 2, 100, 0.00)]
    public void CalculateTotal_WithValidInputs_ReturnsExpectedTotal(
        decimal unitPrice, int quantity, decimal discountPercentage, decimal expectedTotal)
    {
        // Arrange
        var calculator = new PriceCalculator();

        // Act
        var actualTotal = calculator.CalculateTotal(unitPrice, quantity, discountPercentage);

        // Assert
        Assert.Equal(expectedTotal, actualTotal);
    }

    [Fact]
    public void CalculateTotal_WithNegativeUnitPrice_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var calculator = new PriceCalculator();

        // Act
        Action act = () => calculator.CalculateTotal(-1m, 2);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public void CalculateTotal_WithNegativeQuantity_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var calculator = new PriceCalculator();

        // Act
        Action act = () => calculator.CalculateTotal(10m, -1);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void CalculateTotal_WithInvalidDiscountPercentage_ThrowsArgumentOutOfRangeException(
        decimal discountPercentage)
    {
        // Arrange
        var calculator = new PriceCalculator();

        // Act
        Action act = () => calculator.CalculateTotal(10m, 2, discountPercentage);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(act);
    }
}
