using System;
using UnityEngine;

public class PeriodAlignment : MonoBehaviour
{
    public static float FindAlignmentTime(float[] periods)
    {
        if (periods.Length == 0)
        {
            throw new ArgumentException("At least one period is required.");
        }

        float alignmentTime = periods[0];

        for (int i = 1; i < periods.Length; i++)
        {
            alignmentTime = FindAlignmentTimeBetween(alignmentTime, periods[i]);
        }

        return alignmentTime;
    }

    public static float FindAlignmentTimeBetween(float period1, float period2)
    {
        float time = 0;
        float increment = Mathf.Max(period1, period2);

        while (true)
        {
            if (Mathf.Approximately(time % period1, 0) && Mathf.Approximately(time % period2, 0))
            {
                return time;
            }

            time += increment;
        }
    }
}
