using UnityEngine;

public static class AxisRounder
{

    public static float Round(float roundDownDecimal, float roundUpDecimal, float num)
    {
        float remainder = num % 1;

        if (remainder <= roundDownDecimal)
            return num - remainder;
        else if (remainder >= roundUpDecimal)
            return num + ((1.0f - remainder));

        return (int)num;
    }

	public static float Round(float num)
	{
		float roundDownDecimal = 0.49999f;
		float roundUpDecimal = 0.50001f;
		float remainder = num % 1;
		
		if (remainder <= roundDownDecimal)
			return num - remainder;
		else if (remainder >= roundUpDecimal)
			return num + ((1.0f - remainder));
		
		return (int)num;
	}

    public static float SmoothRound(float roundDownDecimal, float roundUpDecimal, float num)
    {
        float remainder = num % 1;

        if (remainder <= roundDownDecimal)
            return num - remainder / 8.0f;
        else if (remainder >= roundUpDecimal)
            return num + ((1.0f - remainder) / 8.0f);
		
        return num;
    }
}
