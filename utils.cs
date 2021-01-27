using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bolder_Blacksmith
{
    public static class utils
    {

        //return a random number within a range
        private static Random random;
        private static void Init()
        {
            if (random == null) random = new Random();
        }

        public static int getRandomInt(int min, int max)
        {
            Init();
            return random.Next(min, max);
        }

        //return a uniformally distributed variable
        public static double getRandomDouble()
        {
            Init();
            return random.NextDouble();
        }

        public static double getRandomDouble(double min, double max)
        {
            Init();
            return random.NextDouble() * (max - min) + min;
        }

        //Return a float between 0 and 1, assuming min <= input <= max
        public static double normalize(double input, double min, double max)
        {
            return (input - min) / (max - min);
        }

        //More specifically, undoes minmax scaling for fractional values

        //again, this only "undoes" values that are between 0 and 1.
        //giving this a number outside of that range will yield garbage.
        //see utils.rescale is that is what you want to do
        public static double inverseNormalize(double input, double min, double max)
        {
            return ((max - min) * input) + min;
        }

        //for non-fractional values that you want to normalize, this changes the range while maintaining the ratio.
        //it is not necessary for fractional values because they are inherently between 0 and 1. see utils.inverseNormalize
        public static double rescale(double input, (double oldMin, double oldMax) oldRange, (double newMin, double newMax) newRange)
        {
            return (((input - oldRange.oldMin) * (newRange.newMax - newRange.newMin)) / (oldRange.oldMax - oldRange.oldMin)) + newRange.newMin;
        }



        //Given an upper bound and an array of weights for each probability, selects a biased random number within the range
        public static int generateInverseDistro(int factorsUpperBound, double[] orderedWeights)
        {
            bool valueSelected = false;
            double uniformValue;

            if (factorsUpperBound != orderedWeights.Length)
            {
                throw new System.Exception("Upper bound and weights must match.");
            }

            while (!(valueSelected))
            {
                //uniformValue = getRandomDouble();
                for (int i = 1; i < factorsUpperBound + 1; i++)
                {
                    uniformValue = getRandomDouble();
                    if (uniformValue <= orderedWeights[i - 1])
                    {
                        return i;
                    }
                }
            }

            //error!
            return 0;

        }

        //C# doesn't have a built in clamp method lol
        public static T Clamp<T>(T value, T min, T max)
            where T : System.IComparable<T>
        {
            T result = value;
            if (value.CompareTo(max) > 0)
                result = max;
            if (value.CompareTo(min) < 0)
                result = min;
            return result;
        }

        public static double Average(this int[] arr)
        {
            if (arr.Length > 0)
            {
                int sum = 0;
                foreach (int i in arr) {
                    sum += i;
                }
                return (double)sum / arr.Length;
            }
            else
            {
                return 0;
            }
        }

        public static double Average(this double[] arr)
        {
            if (arr.Length > 0)
            {
                double sum = 0;
                foreach (double i in arr)
                {
                    sum += i;
                }
                return sum / arr.Length;
            }
            else
            {
                return 0.0;
            }
        }

        public static int Sum(this int[] arr)
        {
            if (arr.Length > 0)
            {
                int sum = 0;
                foreach (int i in arr)
                {
                    sum += i;
                }
                return sum;
            }
            else
            {
                return 0;
            }
        }

        public static double Sum(this double[] arr)
        {
            if (arr.Length > 0)
            {
                double sum = 0;
                foreach (double i in arr)
                {
                    sum += i;
                }
                return sum;
            }
            else
            {
                return 0.0;
            }
        }

        //fun boolean magic
        //works with everything that is comparable (int, double, char, etc)
        //but only intended for numbers!
        //seems interestingly NAND-y on booleans (inclusively)
        public static bool Between<T>(this T num, T lower, T upper, bool inclusive) where T : System.IComparable<T>
        {
            return inclusive
                ? lower.CompareTo(num) <= 0 && upper.CompareTo(num) >= 0
                : lower.CompareTo(num) < 0 && upper.CompareTo(num) > 0;
                                             
        }
        public static double getTransitionalPointDampener(double d, int operation)
        {
            //0 - light scaling
            if (operation == 0)
            {
                return (1 - ((1.0 / 6.0) * Math.Log10(4.0 * d))) + (1.0 / 10.0);
            }
            //1 - heavy scaling
            else if (operation == 1)
            {
                return (0.8 - ((1.0 / 3.0) * Math.Log10(4.0 * d))) + (1.0 / 10.0);
            }
            else
            {
                throw new Exception("Unknown operation");
            }
        }

        public static double getTransitionalPoint(double d, double c, double s, double damp)
        {

         return ((20 * Math.Pow(d, 2)) / Math.Sqrt(d)) +
            ((c * d) / ((s + damp) / 2));

        }
    }
}
