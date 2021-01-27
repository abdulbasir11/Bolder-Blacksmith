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
        public Mineral getMineral()
        {
            return new Mineral();
        }

        public Mineral getIsotopicMineral()
        {
            //Mineral min = new Mineral();
            //min.initializeIsotopic();
            //return min;
            throw new Exception("implement");
        }

        public Mineral [] getBatchMinerals(int numberOfMinerals)
        {
            
            Mineral[] mins = new Mineral[numberOfMinerals];
            Mineral holder;

            for (int i = 0; i < mins.Length; i++)
            {
                holder = new Mineral();
                mins[i] = holder;
            }

            return mins;

        }

        public Mineral[] getTestCase(int repeats)
        {
            throw new Exception("implement");
        }
    }
}
