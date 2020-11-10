using System;

namespace EGamePlay
{
    public static class RandomHelper
    {
        private static readonly Random random = new Random();

        /// <summary>
        /// 获取lower与Upper之间的随机数
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public static int RandomNumber(int lower, int upper)
        {
            int value = random.Next(lower, upper);
            return value;
        }

        public static int RandomRate()
        {
            int value = random.Next(1, 101);
            return value;
        }
    }
}