namespace render_logic;
using SkiaSharp;

public static class MandelbrotLib
{

    public static void GenerateMandelbrotSet(int width, int height, int maxIterations, double zoom, string filePath)
    {
        double centerReal = -0.75;
        double centerImaginary = 0.0;
        double realRange = 3.5 / zoom;
        double imaginaryRange = 2.25 / zoom;

        double minReal = centerReal - realRange / 2;
        double maxReal = centerReal + realRange / 2;
        double minImaginary = centerImaginary - imaginaryRange / 2;
        double maxImaginary = centerImaginary + imaginaryRange / 2;

        using var bitmap = new SKBitmap(width, height);

        // Access the raw pixel buffer
        var pixels = bitmap.Pixels;

        // Parallelize computation across rows of pixels
        Parallel.For(0, height, py =>
        {
            int rowOffset = py * width;

            for (int px = 0; px < width; px++)
            {
                double x0 = MapToImaginary(px, 0, width, minReal, maxReal);
                double y0 = MapToImaginary(py, 0, height, minImaginary, maxImaginary);
                int iterations = EscapeTime(x0, y0, maxIterations);
                SKColor color = CalculateColor(iterations, maxIterations);
                pixels[rowOffset + px] = color;
            }
        });

        // Write the buffer back to the bitmap
        bitmap.Pixels = pixels;

        using var data = SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100);
        File.WriteAllBytes(filePath, data.ToArray());
    }

    public static double MapToImaginary(int value, int minSrc, int maxSrc, double minDst, double maxDst)
    {
        return minDst + (value - minSrc) * (maxDst - minDst) / (maxSrc - minSrc);
    }

    public static int EscapeTime(double x0, double y0, int maxIterations)
    {
        double x = 0, y = 0;
        int iterations = 0;

        // loop until point escapes set or hits max iterations (whichever comes first)
        while (x * x + y * y <= 4 && iterations < maxIterations)
        {
            double tempX = x * x - y * y + x0;
            y = 2 * x * y + y0;
            x = tempX;
            iterations++;
        }
        return iterations;
    }

    public static SKColor CalculateColor(int iterations, int maxIterations)
    {
        if (iterations == maxIterations)
        {
            return SKColors.Black;
        }

        double t = (double)iterations / maxIterations;
        byte r = (byte)(9 * (1 - t) * t * t * t * 255);
        byte g = (byte)(15 * (1 - t) * (1 - t) * t * t * 255);
        byte b = (byte)(8.5 * (1 - t) * (1 - t) * (1 - t) * t * 255);

        return new SKColor(r, g, b);
    }
}

