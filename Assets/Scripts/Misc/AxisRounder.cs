using UnityEngine;

public static class AxisRounder
{

    public static float Round(float roundDownDecimal, float roundUpDecimal, float num)
    {
        float sign = num < 0.0f ? -1.0f : 1.0f;
        num = Mathf.Abs(num);
        float remainder = num % 1;

        if (remainder <= roundDownDecimal)
            return sign * (num - remainder);
        else if (remainder >= roundUpDecimal)
            return sign * (num + ((1.0f - remainder)));

        return (int)num;
    }

    public static float Round(float num)
    {
        return Round(0.49999f, 0.50001f, num);
    }

    public static float SmoothRound(float roundDownDecimal, float roundUpDecimal, float num)
    {
        float sign = num < 0.0f ? -1.0f : 1.0f;
        num = Mathf.Abs(num);
        float remainder = num % 1;

        if (remainder <= roundDownDecimal)
            return sign * (num - remainder / 2.0f);
        else if (remainder >= roundUpDecimal)
            return sign * (num + ((1.0f - remainder) / 8.0f));

        return num;
    }
}
