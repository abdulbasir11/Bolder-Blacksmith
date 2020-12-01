using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bolder_Blacksmith.Generators
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

        //deprecated. may potentially be useful for debugging, but mostly replaced with structs.
        public static double inverseNormalize(double input, double min, double max)
        {
            return ((max - min) * input) + min;
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

    }
}
