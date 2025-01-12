using UnityEngine;

public static class MathUtil
{
    public static float Map(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        // Ensure that the value is within the input range
        value = Mathf.Clamp(value, fromMin, fromMax);

        // Map the value from the input range to the output range
        float fromRange = fromMax - fromMin;
        float toRange = toMax - toMin;

        float mappedValue = toMin + (value - fromMin) / fromRange * toRange;

        return mappedValue;
    }
}
