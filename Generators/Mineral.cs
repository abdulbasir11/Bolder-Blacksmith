using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Bolder_Blacksmith.Generators
{
    /* 
     * Mineral properties include:    
     *      -Number of elements. Ranges from 1-4.
     *      
     *      -Element distribution. Sets distributions of each
     *      mineral based on covalence and gravity, with some
     *      random modification.
     *      
     *      -Complexity. Also known as Net geomertic structure
     *      
     *      -Color. Based on complexity and randomization (to generate rgb values)
     *      
     *      -Net structure. Normalized 0-1 of sum of each structure,
     *      with 0 being crystalline and 1 being cubic.
     *      
     *      -Luster. Based on net structure and complexity
     *      
     *      -Net deformation.
     *      
     *      -Net density.
     *      
     *      -Net hardness
     *      
     *      -Net pliance
     *      
     *      -Net cleave tendency
     *      
     *      -Net transitional points
     *      
     *      -Net heat resistance
     *      
     *      -Net pressure resistance
     *      
     *      -Classification. Based on net structure, either a metal, metalloid, or non-metal.
     *      
     *      -Radioactivity. If an isotope is used, the percentage of the isotope mix that is radioactive.
     *                      Default: 0.
     */
    public class Mineral
    {
        public void initialize()
        {

        }

        public void initializeIsotopic()
        {

        }
    }
}
