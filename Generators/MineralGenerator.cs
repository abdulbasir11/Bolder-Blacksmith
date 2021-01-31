using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bolder_Blacksmith.Generators
{
    /* Mineral generator. Generates minerals from initialize() function, which calls all generation methods.
     */
    public class MineralGenerator
    {
        public ElementGenerator elemGen;
        public double[] mineralWeights;

        public MineralGenerator(ElementGenerator gen = null, double[] mineralWts = null)
        {
            elemGen = gen ?? new ElementGenerator();
            mineralWeights = mineralWts ?? new double[0];
        }

        #region getters/setters
        public ElementGenerator getElementGenerator()
        {
            return elemGen;
        }

        public void setElementGenerator(ElementGenerator e)
        {
            elemGen = e;
        }

        public double[] getWeights()
        {
            return mineralWeights;
        }

        public void setWeights(double[] w)
        {
            if (w.Length == 5)
            {
                mineralWeights = w;
            }
        }

        #endregion

        public void printInfo()
        {
            Console.WriteLine("---MINERAL GENERATOR SETTINGS START---");
            Console.WriteLine("Mineral Weights: ");
            foreach (double d in mineralWeights)
            {
                Console.WriteLine(d);
            }
            Console.WriteLine("---MINERAL GENERATOR SETTINGS END---");
            Console.WriteLine();
        }

        public Mineral getMineral()
        {
            if (mineralWeights.Length == 5)
            {
                return new Mineral(elemGen, mineralWeights);
            }
            else
            {
                return new Mineral(elemGen);
            }
        }

        public Mineral [] getBatchMinerals(int numberOfMinerals)
        {
            
            Mineral[] mins = new Mineral[numberOfMinerals];
            Mineral holder;

            for (int i = 0; i < mins.Length; i++)
            {
                holder = getMineral();
                mins[i] = holder;
            }

            return mins;

        }

    }
}
