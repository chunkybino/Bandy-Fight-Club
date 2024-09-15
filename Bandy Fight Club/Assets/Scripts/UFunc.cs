using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFunc
{
    public static float StopFloatOvershoot(float num1, float numDontOvershootThis, float direction)
    {
        return StopFloatOvershoot(num1, numDontOvershootThis, Mathf.RoundToInt(direction));
    }
    public static float StopFloatOvershoot(float num1, float numDontOvershootThis, int direction) //make sure the number doesnt go past the other number
    {
        if (direction == 1)
        {
            return Mathf.Min(num1, numDontOvershootThis);
        }
        else if (direction == -1)
        {
            return Mathf.Max(num1, numDontOvershootThis);
        }
        return num1;
    }

    public static float FloatToDirection(float num)
    {
        if (num > 0)
        {
            return 1;
        }
        else if (num < 0)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    public static bool CheckIsSameDirection(float num1, float num2) //are both numbers the same positive or negative
    {
        return FloatToDirection(num1) == FloatToDirection(num2);
    }

    public static void TickTimer(ref float num) //tick down by 1 every second
    {
        num = Mathf.Max(num - Time.deltaTime, 0);
    }
    public static void TickTimerFixed(ref float num) //tick down by 1 every second
    {
        num = Mathf.Max(num - Time.fixedDeltaTime, 0);
    }

    public static float RoundToDecimal(float num, float digits)
    {
        float degree = Mathf.Pow(10, digits);
        return Mathf.Round(num * degree) / degree;
    }

    public static Vector2 AddVector3ToVector2(Vector2 vec2, Vector3 vec3)
    {
        return new Vector2(vec2.x + vec3.x, vec2.y + vec3.y);
    }
    public static float DistanceBetweenVec2AndVec3(Vector2 vec2, Vector3 vec3)
    {
        return Vector2.Distance(vec2, new Vector2(vec3.x, vec3.y));
    }

    public static float Pythag(float a, float b)
    {
        return Mathf.Sqrt((a*a) + (b*b));
    }

    public static Vector2 AngleToVec(float angle)
    {
        return new Vector2(Mathf.Cos(Mathf.Deg2Rad*angle), Mathf.Sin(Mathf.Deg2Rad*angle));
    }
    public static float VecToAngle(Vector2 vec)
    {
        return Mathf.Rad2Deg * Mathf.Atan2(vec.y, vec.x);
    }

    public static Vector3 LocalToWorldScale(Transform thing)
    {
        Vector3 scale = new Vector3(thing.localScale.x/thing.lossyScale.x, thing.localScale.y/thing.lossyScale.y, thing.localScale.z/thing.lossyScale.z);
        if (thing.localScale.z == 0) scale = new Vector3(scale.x,scale.y,0); //for fixing nans
        return scale; 
    }

    public static Vector2 RotateVec(Vector2 vec, float rot)
    {
        return AngleToVec(VecToAngle(vec)+rot) * vec.magnitude;
    }
    public static Vector2 RotateVecByVec(Vector2 vec, Vector2 rotVec)
    {
        if (rotVec.y == 0) return vec;
        if (rotVec.x == 0) return new Vector2(-vec.y,vec.x);

        float slope = rotVec.y/rotVec.x;
        float perpSlope = -1/slope;
        float slope1x = vec.x/Mathf.Sqrt(slope*slope + 1);
        float slope2x = vec.y/Mathf.Sqrt(1/(slope*slope) + 1);
        float perpSlopex = slope1x-(slope2x*Mathf.Sign(slope));

        return new Vector2(perpSlopex, -((perpSlopex-slope1x)/slope)+(slope*slope1x));
    }

    public static List<string> SeperateStringByUnderscore(string thing)
    {
        List<string> words = new List<string>(); //make list
        int wordIndex = 0;

        words.Add(""); //add first word

        for (int i = 0; i < thing.Length; i++)
        {
            if (thing[i] == '_') //start new word if find underscore
            {
                wordIndex++;
                words.Add("");
            }
            else
            {
                words[wordIndex] = words[wordIndex] + thing[i]; //add letter to current work if not fund underscore
            }
        }

        return words;
    }

    public static float AddAwayFromZero(ref float num, float add) //if positive, add, if negative, subtract
    {
        num = Mathf.Max(Mathf.Abs(num) + add, 0) * Mathf.Sign(num);
        return num;
    }

    public static Color SetColorAlpha(Color color, float alpha)
    {
        color = new Vector4(color.r,color.g,color.b,alpha);
        return color;
    }

    public static Vector3 LerpVector3(Vector3 n1, Vector3 n2, float t)
    {
        return new Vector3(Mathf.Lerp(n1.x,n2.x,t),Mathf.Lerp(n1.y,n2.y,t),Mathf.Lerp(n1.z,n2.z,t));
    }

    public static float InverseLerpUnclamped(float n1, float n2, float n3)
    {
        return (n3-n1)/(n2-n1);
    }

    public static float EaseOutPoly(float t, float exp)
    {
        return -Mathf.Pow(-Mathf.Clamp(t,0,1) + 1, exp) + 1;
    }
    public static float EaseOutExpo(float t, float exp)
    {
        return Mathf.Clamp(((1/Mathf.Pow(exp,t)) - (1/exp)) / (1-(1/exp)), 0, 1);
    }
    public static float EaseOutLog(float t, float bas)
    {
        return Mathf.Clamp((t + (1/(bas-1))) * (1-(1/bas)), 0, 1);
    }
    public static float EaseOutExpoLinear(float n1, float n2, float exp, float t)
    {
        return Mathf.Lerp(n1, n2, (t/(exp*exp) - (1/(Mathf.Pow(exp,2*t)))) + 1);
    }

    public static float FullToNegativeRangeAngle(float a)
    {
        if (a > 180) a -= 360;
        if (a < -180) a += 360;
        return a;
    }

    public static Vector2 SlopeIntersect(float slope1, Vector2 pos1, float slope2, Vector2 pos2)
    {
        float x = (pos2.y + (slope2*-pos2.x) - pos1.y - (slope1*-pos1.x)) / (slope1 - slope2);

        return new Vector2(x, slope1*(x-pos1.x) + pos1.y);
    }

    public static float SlopeDifference(float s1, float s2)
    {
        float perpSlope = 1/s1;
        float x = 1/Mathf.Sqrt(s1*s1 + 1);
        Vector2 perpPos = new Vector2(x, s1*x);
        return Vector2.Distance(SlopeIntersect(perpSlope, perpPos, s2, new Vector2(0,0)),perpPos);
    }

    public static T[] ListToArray<T>(List<T> list)
    {
        T[] array = new T[list.Count];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = list[i];
        }
        return array;
    }
    public static List<T> FlipList<T>(List<T> list)
    {
        List<T> newList = new List<T>();
        for (int i = list.Count-1; i >= 0; i--)
        {
            newList.Add(list[i]);
        }
        return newList;
    }
    public static List<T> AppendArrayToList<T>(List<T> list, T[] array)
    {
        foreach (T a in array)
        {
            list.Add(a);
        }
        return list;
    }

    public static bool Between01(float n)
    {
        return n >= 0 && n <= 1;
    }
    public static bool IsBetweenVector2(Vector2 vec, Vector2 low, Vector2 high)
    {
        bool yes = true;
        if (low.x < high.x) {
            if (vec.x < low.x || vec.x > high.x) yes = false;
        } else {
            if (vec.x > low.x || vec.x < high.x) yes = false;
        }
        if (low.y < high.y) {
            if (vec.y < low.y || vec.y > high.y) yes = false;
        } else {
            if (vec.y > low.y || vec.y < high.y) yes = false;
        }
        return yes;
    }
}
