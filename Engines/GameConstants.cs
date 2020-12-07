using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bolder_Blacksmith.Engines
{
    public static class GameConstants
    {
        //Game-breaking constants are marked with an "!".

        //Min and max pressure must have same abs value.
        public static int MINIMUM_PRESSURE = -5;
        public static int MAXIMUM_PRESSURE = MINIMUM_PRESSURE * -1;
        public static int PRESSURE_DAMPENER = 1; //!!! Must be >= 1 and < MAXIMUM_PRESSURE!

    }
}
