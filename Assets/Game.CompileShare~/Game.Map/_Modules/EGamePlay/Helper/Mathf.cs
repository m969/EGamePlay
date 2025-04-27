#region 程序集 UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// 未知的位置
// Decompiled with ICSharpCode.Decompiler 8.1.1.7464
#endregion

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.IL2CPP.CompilerServices;

#if NOT_UNITY
namespace UnityEngine
{
    public struct Mathf
    {
        //
        // 摘要:
        //     The well-known 3.14159265358979... value (Read Only).
        public const float PI = (float)Math.PI;

        //
        // 摘要:
        //     A representation of positive infinity (Read Only).
        public const float Infinity = float.PositiveInfinity;

        //
        // 摘要:
        //     A representation of negative infinity (Read Only).
        public const float NegativeInfinity = float.NegativeInfinity;

        //
        // 摘要:
        //     Degrees-to-radians conversion constant (Read Only).
        public const float Deg2Rad = (float)Math.PI / 180f;

        //
        // 摘要:
        //     Radians-to-degrees conversion constant (Read Only).
        public const float Rad2Deg = 57.29578f;

        //
        // 摘要:
        //     Returns the sine of angle f.
        //
        // 参数:
        //   f:
        //     The input angle, in radians.
        //
        // 返回结果:
        //     The return value between -1 and +1.
        public static float Sin(float f)
        {
            return (float)Math.Sin(f);
        }

        //
        // 摘要:
        //     Returns the cosine of angle f.
        //
        // 参数:
        //   f:
        //     The input angle, in radians.
        //
        // 返回结果:
        //     The return value between -1 and 1.
        public static float Cos(float f)
        {
            return (float)Math.Cos(f);
        }

        //
        // 摘要:
        //     Returns the tangent of angle f in radians.
        //
        // 参数:
        //   f:
        public static float Tan(float f)
        {
            return (float)Math.Tan(f);
        }

        //
        // 摘要:
        //     Returns the arc-sine of f - the angle in radians whose sine is f.
        //
        // 参数:
        //   f:
        public static float Asin(float f)
        {
            return (float)Math.Asin(f);
        }

        //
        // 摘要:
        //     Returns the arc-cosine of f - the angle in radians whose cosine is f.
        //
        // 参数:
        //   f:
        public static float Acos(float f)
        {
            return (float)Math.Acos(f);
        }

        //
        // 摘要:
        //     Returns the arc-tangent of f - the angle in radians whose tangent is f.
        //
        // 参数:
        //   f:
        public static float Atan(float f)
        {
            return (float)Math.Atan(f);
        }

        //
        // 摘要:
        //     Returns the angle in radians whose Tan is y/x.
        //
        // 参数:
        //   y:
        //
        //   x:
        public static float Atan2(float y, float x)
        {
            return (float)Math.Atan2(y, x);
        }

        //
        // 摘要:
        //     Returns square root of f.
        //
        // 参数:
        //   f:
        public static float Sqrt(float f)
        {
            return (float)Math.Sqrt(f);
        }

        //
        // 摘要:
        //     Returns the absolute value of f.
        //
        // 参数:
        //   f:
        public static float Abs(float f)
        {
            return Math.Abs(f);
        }

        //
        // 摘要:
        //     Returns the absolute value of value.
        //
        // 参数:
        //   value:
        public static int Abs(int value)
        {
            return Math.Abs(value);
        }

        //
        // 摘要:
        //     Returns the smallest of two or more values.
        //
        // 参数:
        //   a:
        //
        //   b:
        //
        //   values:
        public static float Min(float a, float b)
        {
            return (a < b) ? a : b;
        }

        //
        // 摘要:
        //     Returns the smallest of two or more values.
        //
        // 参数:
        //   a:
        //
        //   b:
        //
        //   values:
        public static float Min(params float[] values)
        {
            int num = values.Length;
            if (num == 0)
            {
                return 0f;
            }

            float num2 = values[0];
            for (int i = 1; i < num; i++)
            {
                if (values[i] < num2)
                {
                    num2 = values[i];
                }
            }

            return num2;
        }

        //
        // 摘要:
        //     Returns the smallest of two or more values.
        //
        // 参数:
        //   a:
        //
        //   b:
        //
        //   values:
        public static int Min(int a, int b)
        {
            return (a < b) ? a : b;
        }

        //
        // 摘要:
        //     Returns the smallest of two or more values.
        //
        // 参数:
        //   a:
        //
        //   b:
        //
        //   values:
        public static int Min(params int[] values)
        {
            int num = values.Length;
            if (num == 0)
            {
                return 0;
            }

            int num2 = values[0];
            for (int i = 1; i < num; i++)
            {
                if (values[i] < num2)
                {
                    num2 = values[i];
                }
            }

            return num2;
        }

        //
        // 摘要:
        //     Returns largest of two or more values.
        //
        // 参数:
        //   a:
        //
        //   b:
        //
        //   values:
        public static float Max(float a, float b)
        {
            return (a > b) ? a : b;
        }

        //
        // 摘要:
        //     Returns largest of two or more values.
        //
        // 参数:
        //   a:
        //
        //   b:
        //
        //   values:
        public static float Max(params float[] values)
        {
            int num = values.Length;
            if (num == 0)
            {
                return 0f;
            }

            float num2 = values[0];
            for (int i = 1; i < num; i++)
            {
                if (values[i] > num2)
                {
                    num2 = values[i];
                }
            }

            return num2;
        }

        //
        // 摘要:
        //     Returns the largest of two or more values.
        //
        // 参数:
        //   a:
        //
        //   b:
        //
        //   values:
        public static int Max(int a, int b)
        {
            return (a > b) ? a : b;
        }

        //
        // 摘要:
        //     Returns the largest of two or more values.
        //
        // 参数:
        //   a:
        //
        //   b:
        //
        //   values:
        public static int Max(params int[] values)
        {
            int num = values.Length;
            if (num == 0)
            {
                return 0;
            }

            int num2 = values[0];
            for (int i = 1; i < num; i++)
            {
                if (values[i] > num2)
                {
                    num2 = values[i];
                }
            }

            return num2;
        }

        //
        // 摘要:
        //     Returns f raised to power p.
        //
        // 参数:
        //   f:
        //
        //   p:
        public static float Pow(float f, float p)
        {
            return (float)Math.Pow(f, p);
        }

        //
        // 摘要:
        //     Returns e raised to the specified power.
        //
        // 参数:
        //   power:
        public static float Exp(float power)
        {
            return (float)Math.Exp(power);
        }

        //
        // 摘要:
        //     Returns the logarithm of a specified number in a specified base.
        //
        // 参数:
        //   f:
        //
        //   p:
        public static float Log(float f, float p)
        {
            return (float)Math.Log(f, p);
        }

        //
        // 摘要:
        //     Returns the natural (base e) logarithm of a specified number.
        //
        // 参数:
        //   f:
        public static float Log(float f)
        {
            return (float)Math.Log(f);
        }

        //
        // 摘要:
        //     Returns the base 10 logarithm of a specified number.
        //
        // 参数:
        //   f:
        public static float Log10(float f)
        {
            return (float)Math.Log10(f);
        }

        //
        // 摘要:
        //     Returns the smallest integer greater to or equal to f.
        //
        // 参数:
        //   f:
        public static float Ceil(float f)
        {
            return (float)Math.Ceiling(f);
        }

        //
        // 摘要:
        //     Returns the largest integer smaller than or equal to f.
        //
        // 参数:
        //   f:
        public static float Floor(float f)
        {
            return (float)Math.Floor(f);
        }

        //
        // 摘要:
        //     Returns f rounded to the nearest integer.
        //
        // 参数:
        //   f:
        public static float Round(float f)
        {
            return (float)Math.Round(f);
        }

        //
        // 摘要:
        //     Returns the smallest integer greater to or equal to f.
        //
        // 参数:
        //   f:
        public static int CeilToInt(float f)
        {
            return (int)Math.Ceiling(f);
        }

        //
        // 摘要:
        //     Returns the largest integer smaller to or equal to f.
        //
        // 参数:
        //   f:
        public static int FloorToInt(float f)
        {
            return (int)Math.Floor(f);
        }

        //
        // 摘要:
        //     Returns f rounded to the nearest integer.
        //
        // 参数:
        //   f:
        public static int RoundToInt(float f)
        {
            return (int)Math.Round(f);
        }

        //
        // 摘要:
        //     Returns the sign of f.
        //
        // 参数:
        //   f:
        public static float Sign(float f)
        {
            return (f >= 0f) ? 1f : (-1f);
        }

        //
        // 摘要:
        //     Clamps the given value between the given minimum float and maximum float values.
        //     Returns the given value if it is within the minimum and maximum range.
        //
        // 参数:
        //   value:
        //     The floating point value to restrict inside the range defined by the minimum
        //     and maximum values.
        //
        //   min:
        //     The minimum floating point value to compare against.
        //
        //   max:
        //     The maximum floating point value to compare against.
        //
        // 返回结果:
        //     The float result between the minimum and maximum values.
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }

            return value;
        }

        //
        // 摘要:
        //     Clamps the given value between a range defined by the given minimum integer and
        //     maximum integer values. Returns the given value if it is within min and max.
        //
        //
        // 参数:
        //   value:
        //     The integer point value to restrict inside the min-to-max range.
        //
        //   min:
        //     The minimum integer point value to compare against.
        //
        //   max:
        //     The maximum integer point value to compare against.
        //
        // 返回结果:
        //     The int result between min and max values.
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }

            return value;
        }

        //
        // 摘要:
        //     Clamps value between 0 and 1 and returns value.
        //
        // 参数:
        //   value:
        public static float Clamp01(float value)
        {
            if (value < 0f)
            {
                return 0f;
            }

            if (value > 1f)
            {
                return 1f;
            }

            return value;
        }

        //
        // 摘要:
        //     Linearly interpolates between a and b by t.
        //
        // 参数:
        //   a:
        //     The start value.
        //
        //   b:
        //     The end value.
        //
        //   t:
        //     The interpolation value between the two floats.
        //
        // 返回结果:
        //     The interpolated float result between the two float values.
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * Clamp01(t);
        }

        //
        // 摘要:
        //     Linearly interpolates between a and b by t with no limit to t.
        //
        // 参数:
        //   a:
        //     The start value.
        //
        //   b:
        //     The end value.
        //
        //   t:
        //     The interpolation between the two floats.
        //
        // 返回结果:
        //     The float value as a result from the linear interpolation.
        public static float LerpUnclamped(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        //
        // 摘要:
        //     Same as Lerp but makes sure the values interpolate correctly when they wrap around
        //     360 degrees.
        //
        // 参数:
        //   a:
        //
        //   b:
        //
        //   t:
        public static float LerpAngle(float a, float b, float t)
        {
            float num = Repeat(b - a, 360f);
            if (num > 180f)
            {
                num -= 360f;
            }

            return a + num * Clamp01(t);
        }

        //
        // 摘要:
        //     Moves a value current towards target.
        //
        // 参数:
        //   current:
        //     The current value.
        //
        //   target:
        //     The value to move towards.
        //
        //   maxDelta:
        //     The maximum change that should be applied to the value.
        public static float MoveTowards(float current, float target, float maxDelta)
        {
            if (Abs(target - current) <= maxDelta)
            {
                return target;
            }

            return current + Sign(target - current) * maxDelta;
        }

        //
        // 摘要:
        //     Same as MoveTowards but makes sure the values interpolate correctly when they
        //     wrap around 360 degrees.
        //
        // 参数:
        //   current:
        //
        //   target:
        //
        //   maxDelta:
        public static float MoveTowardsAngle(float current, float target, float maxDelta)
        {
            float num = DeltaAngle(current, target);
            if (0f - maxDelta < num && num < maxDelta)
            {
                return target;
            }

            target = current + num;
            return MoveTowards(current, target, maxDelta);
        }

        //
        // 摘要:
        //     Interpolates between min and max with smoothing at the limits.
        //
        // 参数:
        //   from:
        //
        //   to:
        //
        //   t:
        public static float SmoothStep(float from, float to, float t)
        {
            t = Clamp01(t);
            t = -2f * t * t * t + 3f * t * t;
            return to * t + from * (1f - t);
        }

        public static float Gamma(float value, float absmax, float gamma)
        {
            bool flag = value < 0f;
            float num = Abs(value);
            if (num > absmax)
            {
                return flag ? (0f - num) : num;
            }

            float num2 = Pow(num / absmax, gamma) * absmax;
            return flag ? (0f - num2) : num2;
        }

        //
        // 摘要:
        //     Loops the value t, so that it is never larger than length and never smaller than
        //     0.
        //
        // 参数:
        //   t:
        //
        //   length:
        public static float Repeat(float t, float length)
        {
            return Clamp(t - Floor(t / length) * length, 0f, length);
        }

        //
        // 摘要:
        //     PingPong returns a value that will increment and decrement between the value
        //     0 and length.
        //
        // 参数:
        //   t:
        //
        //   length:
        public static float PingPong(float t, float length)
        {
            t = Repeat(t, length * 2f);
            return length - Abs(t - length);
        }

        //
        // 摘要:
        //     Determines where a value lies between two points.
        //
        // 参数:
        //   a:
        //     The start of the range.
        //
        //   b:
        //     The end of the range.
        //
        //   value:
        //     The point within the range you want to calculate.
        //
        // 返回结果:
        //     A value between zero and one, representing where the "value" parameter falls
        //     within the range defined by a and b.
        public static float InverseLerp(float a, float b, float value)
        {
            if (a != b)
            {
                return Clamp01((value - a) / (b - a));
            }

            return 0f;
        }

        //
        // 摘要:
        //     Calculates the shortest difference between two given angles given in degrees.
        //
        //
        // 参数:
        //   current:
        //
        //   target:
        public static float DeltaAngle(float current, float target)
        {
            float num = Repeat(target - current, 360f);
            if (num > 180f)
            {
                num -= 360f;
            }

            return num;
        }

        internal static long RandomToLong(System.Random r)
        {
            byte[] array = new byte[8];
            r.NextBytes(array);
            return (long)(BitConverter.ToUInt64(array, 0) & 0x7FFFFFFFFFFFFFFFL);
        }
    }
#if false // 反编译日志
缓存中的 227 项
------------------
解析: "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
找到单个程序集: "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
警告: 版本不匹配。应为: "2.0.0.0"，实际为: "4.0.0.0"
从以下位置加载: "C:\Program Files\Unity\Hub\Editor\2021.3.1f1c1\Editor\Data\NetStandard\compat\2.1.0\shims\netfx\mscorlib.dll"
------------------
解析: "System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
找到单个程序集: "System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
警告: 版本不匹配。应为: "3.5.0.0"，实际为: "4.0.0.0"
从以下位置加载: "C:\Program Files\Unity\Hub\Editor\2021.3.1f1c1\Editor\Data\NetStandard\compat\2.1.0\shims\netfx\System.Core.dll"
------------------
解析: "UnityEngine.SharedInternalsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
找到单个程序集: "UnityEngine.SharedInternalsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
从以下位置加载: "C:\Program Files\Unity\Hub\Editor\2021.3.1f1c1\Editor\Data\Managed\UnityEngine\UnityEngine.SharedInternalsModule.dll"
------------------
解析: "System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
找到单个程序集: "System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
警告: 版本不匹配。应为: "2.0.0.0"，实际为: "4.0.0.0"
从以下位置加载: "C:\Program Files\Unity\Hub\Editor\2021.3.1f1c1\Editor\Data\NetStandard\compat\2.1.0\shims\netfx\System.dll"
------------------
解析: "Microsoft.Win32.Registry, Version=4.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
无法按名称“Microsoft.Win32.Registry, Version=4.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a”查找 
------------------
解析: "netstandard, Version=2.1.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51"
找到单个程序集: "netstandard, Version=2.1.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51"
从以下位置加载: "C:\Program Files\Unity\Hub\Editor\2021.3.1f1c1\Editor\Data\NetStandard\ref\2.1.0\netstandard.dll"
------------------
解析: "System.Security.Principal.Windows, Version=4.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
无法按名称“System.Security.Principal.Windows, Version=4.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a”查找 
------------------
解析: "System.Security.AccessControl, Version=4.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
无法按名称“System.Security.AccessControl, Version=4.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a”查找 
------------------
解析: "System.CodeDom, Version=4.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51"
无法按名称“System.CodeDom, Version=4.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51”查找 
#endif

}
#endif