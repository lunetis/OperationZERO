using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathHelper
{
    public static void CalcAvgPos(ref Vector2 avgPos, ref int frameCount, Vector2 pos)
    {
        frameCount++;
        if(avgPos == Vector2.zero) avgPos = pos;
        else
        {
            avgPos = ((frameCount - 1) / frameCount) * avgPos + (pos / frameCount);
        }
    }

    public static float GetTimeToCatchUp(Rigidbody target, Rigidbody follow)
    {
        Vector3 d = target.position - follow.position;  // d = distance between two objects
        Vector3 v = target.velocity;                    // v = target velocity
        float s = follow.velocity.magnitude;            // s = following object's speed

        float a = (v.x * v.x + v.y * v.y + v.z * v.z - s * s);
        float b = (d.x * v.x + d.y * v.y + d.z * v.z);
        float c = (d.x * d.x + d.y * d.y + d.z * d.z);

        float root = b * b - a * c;

        if(root < 0)
        {
            return -1;
        }

        // In most cases, a will be negative value
        float value1 = (-b - Mathf.Sqrt(root)) / a;
        float value2 = (-b + Mathf.Sqrt(root)) / a;

        if(value1 < 0 && value2 < 0)
        {
            return -1;
        }
        else if(value1 > 0 && value2 < 0)
        {
            return value1;
        }
        else if(value1 < 0 && value2 > 0)
        {
            return value2;
        }
        else
        {
            return Mathf.Min(value1, value2);
        }
    }
}
