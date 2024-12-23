namespace tests;
using render_logic;
using SkiaSharp;
using Xunit;

public class MandelbrotLibTests
{
    [Theory]
    [InlineData(0, 0, 100, -2.5, 1.0, -2.5)]
    [InlineData(100, 0, 100, -2.5, 1.0, 1.0)]
    [InlineData(50, 0, 100, -2.5, 1.0, -0.75)]
    public void TestMap(int value, int minSrc, int maxSrc, double minDst, double maxDst, double expected)
    {
        double result = MandelbrotLib.MapToImaginary(value, minSrc, maxSrc, minDst, maxDst);
        Assert.Equal(expected, result, precision: 5);
    }

    [Theory]
    [InlineData(0.0, 0.0, 50, 50)] // Inside the set
    [InlineData(2.0, 2.0, 50, 1)] // Escapes quickly
    [InlineData(-0.75, 0.1, 50, 33)] // Near boundary
    public void EscapeTime_ShouldReturnCorrectIterations(double x0, double y0, int maxIterations, int expected)
    {
        int result = MandelbrotLib.EscapeTime(x0, y0, maxIterations);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, 50, 0, 0, 0)] // Fully escaped (black)
    [InlineData(25, 50, 143, 239, 135)] // Midpoint of iterations
    [InlineData(50, 50, 0, 0, 0)] // Max iterations (black)
    public void CalculateColor_ShouldReturnExpectedColor(int iterations, int maxIterations, byte expectedR, byte expectedG, byte expectedB)
    {
        SKColor color = MandelbrotLib.CalculateColor(iterations, maxIterations);
        Assert.Equal(expectedR, color.Red);
        Assert.Equal(expectedG, color.Green);
        Assert.Equal(expectedB, color.Blue);
    }
}