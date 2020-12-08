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
        /*
         * All bonuses are in the range [0.0, 1.0].
         * 
         * Negative modifiers (things that reduce the given property) are easy: the corresponding 
         * property is multiplied by the bonus rate to achieve the new property value.
         * 
         * For positive modifiers, the bonus represents the amount of
         * remaining "space" between the given value and 1.0 to eliminate.
         * 
         * For example: let's say the floridian heat resistance bonus is 0.5. An element with 0 heat
         * resistance would have it boosted to 0.5; an element with 0.5 heat resistance would have it
         * boosted to 0.75; an element with 1.0 heat resistance would have it boosted to 1.0. In each
         * example, the boost halves the distance between the original value and the maximum value.
         * 
         * Therefore, setting a bonus to 0.0 will do nothing, while setting it to 1.0 will always
         * maximize the corresponding value for any element that rolls the bonus.
         */
        public bool floridian;
        public static double floridianChance = 0.01;
        public static double floridianHeatResBonus = 0.5;

        public bool whiteKnuckled;
        public static double whiteKnuckledChance = 0.01;
        public static double whiteKnuckledPressureResBonus = 0.5;

        public bool gregarious;
        public static double gregariousChance = 0.01;
        public static double gregariousCatenationRateMult = 0.7;

        public bool antisocial;
        public static double antisocialChance = 0.01;
        public static double antisocialCovalenceMult = 0.5;

        public bool paleoDiet;
        public static double paleoDietChance = 0.01;
        public static double paleoDietGravityMult = 0.7;

        public bool softy;
        public static double softyChance = 0.01;
        public static double softyPlianceBonus = 0.5;

        public bool glassBones;
        public static double glassBonesChance = 0.01;
        public static double glassBonesCleaveTendencyBonus = 0.5;

        public static double radioactiveChance = 0.005;
        public static double magneticChance = 0.3;
        public Isotope(Element baseElem) : base(baseElem)
        {
            //Add additional properties to base element and return isotope
            //TODO remove hardcoded 1.0s and replace with things from Constants, based on the element type
            floridian = utils.getRandomDouble() < floridianChance;
            if (floridian)
            {
                heatRes.boilingRes += (1.0 - heatRes.boilingRes) * floridianHeatResBonus;
                heatRes.meltingRes += (1.0 - heatRes.meltingRes) * floridianHeatResBonus;
            }

            whiteKnuckled = utils.getRandomDouble() < whiteKnuckledChance;
            if (whiteKnuckled)
            {
                pressureRes += (1.0 - pressureRes) * whiteKnuckledPressureResBonus;
            }

            gregarious = utils.getRandomDouble() < gregariousChance;
            if (gregarious)
            {
                catenationRate *= gregariousCatenationRateMult;
            }

            antisocial = utils.getRandomDouble() < antisocialChance;
            if (antisocial)
            {
                covalence.original = (int)(covalence.original * antisocialCovalenceMult);
                covalence.normalized = utils.normalize(covalence.original, 1, 8);
            }

            paleoDiet = utils.getRandomDouble() < paleoDietChance;
            if (paleoDiet)
            {
                gravity *= paleoDietGravityMult;
            }

            softy = utils.getRandomDouble() < softyChance;
            if (softy)
            {
                pliance += (1.0 - pliance) * softyPlianceBonus;
            }

            glassBones = utils.getRandomDouble() < glassBonesChance;
            if (glassBones)
            {
                cleaveTendency += (1.0 - cleaveTendency) * glassBonesCleaveTendencyBonus;
            }

            if (utils.getRandomDouble() < radioactiveChance)
            {
                radioactivity = utils.getRandomDouble();
            }

            if (utils.getRandomDouble() < magneticChance)
            {
                magnetism = utils.getRandomDouble();
            }
        }

    }
}
