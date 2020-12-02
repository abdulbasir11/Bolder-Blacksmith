using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bolder_Blacksmith.Generators
{
    //Generates an isotope for a provided element
    //Isotopes can be used in the place of an element
    //to generate minerals. An element can have up to 3 isotopes.
    //Isotope properties are all randomized and have a slim chance of applying.
    //ALL modified values do not recalculate dependent values unless specified.
    /*
     * radioactive - normalized 0-1. Based on the symmetry of the element,
     *                 a slim chance exist for it to become radioactive.
     *                 Radioactive elements can inflict status effects.
     * 
     * floridian - Multiplier to heat resistance. Makes element more resistant to heat.
     * 
     * whiteKnuckled - Multiplier to pressure resistance. Makes elements more resistant to pressure
     * 
     * gregarious - reduces catenation rate
     * 
     * antisocial - reduces number of covalent bonds
     * 
     * paleoDiet - decreases gravity
     * 
     * softy - increases pliance
     * 
     * glassBones - increases cleaving tendency
     * 
     */
    public class Isotope : Element
    {
        public Isotope(Element baseElem)
        {
            //add additional properties to base element and return isotope
        }

    }
}
