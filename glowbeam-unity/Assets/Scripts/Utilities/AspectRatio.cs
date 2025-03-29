using UnityEngine;

public enum AspectRatio
{
    AR_16x9,
    AR_16x10,
    AR_4x3,
    AR_Nonstandard
}

public static class AspectRatioUtility
{
    // Helper function to compute the greatest common divisor (GCD)
    private static int GCD(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    /// <summary>
    /// Returns the AspectRatio enum value based on the given width and height.
    /// </summary>
    /// <param name="width">The width value</param>
    /// <param name="height">The height value</param>
    /// <returns>AspectRatio enum representing AR_16x9, AR_16x10, AR_4x3, or AR_Nonstandard</returns>
    public static AspectRatio GetAspectRatio(int width, int height)
    {
        int gcd = GCD(width, height);
        int reducedWidth = width / gcd;
        int reducedHeight = height / gcd;

        if (reducedWidth == 16 && reducedHeight == 9)
        {
            return AspectRatio.AR_16x9;
        }
        else if (reducedWidth == 16 && reducedHeight == 10)
        {
            return AspectRatio.AR_16x10;
        }
        else if (reducedWidth == 4 && reducedHeight == 3)
        {
            return AspectRatio.AR_4x3;
        }
        else
        {
            return AspectRatio.AR_Nonstandard;
        }
    }
}
