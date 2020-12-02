using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bolder_Blacksmith.Generators
{
    /*Element properties include:
     *       NOTE: some of these property descriptions are inaccurate
     *       They were written before implementation!
     *       
     *      -Number of covalent bonds. Ranges from
     *      1 to 8.
     *      
     *      -Catenation rate. Tendency to self-bind.
     *      Normalized percentage (from 0 to 1) based
     *      on how close the number of covalent bonds
     *      is to 4.
     *      
     *      -Catenation delta. Increases the probability
     *      of the base structure switching, dependent
     *      on symmetry of covalence, catenation/covalence,
     *      and a "dampener" 
     *      
     *      -Base structure. Depending on bonds,
     *      covalence, and random inputs, either
     *      cubic or crystalline. Cubic structures
     *      are more metallic, crystalline structures
     *      are either non-metals or more gas-like
     *      with high catenation and covalence.
     *      
     *      -Deformation tendency. Tendency for
     *      base structure to deform when
     *      bound. Ranges from 0.1 to 0.5, normalized
     *      from 0 to 1. 
     *      
     *      -Density. Ranges from 1 to 25, normalized
     *      from 0 to 1 with the median being
     *      3. Denser elements have lower deformative
     *      tendencies, a cubic base structure, and
     *      lower covalence.
     *      
     *      -Geometric modifier.
     *      For crystalline elements, ranges from a triangle (3)
     *      to a 12-gon (12). Lower density, higher catenation
     *      elements tend to have higher -gons, while higher
     *      density, medium catenation tend close to 4-6.
     *      For cubic elements, ranges from body centered (1),
     *      face-centered (2), or hexagonal (3).
     *      
     *      -Hardness. For crystalline elements, hardness
     *      cannot exceed 10. For cubic elements, hardness
     *      ranges from 3 to 9, normalized from 0 to 1.
     *      The hardest elements are hexagonal-cubics, then
     *      body centered, then face centered.
     *      
     *      -Pliance. For crystalline elements, pliance
     *      is typically low. For cubic elements, pliance
     *      is typically high.
     *      
     *      -Gravity. Likelihood of a material being
     *      heavier than the things it is bound to.
     *      Lowers with covalence.
     *      
     *      -Standard Phase. Likelihood of a material
     *      to vaporize dependent on temperature. The lower
     *      the threshold, the more likely it is to be a liquid or gas.
     *      
     *      -Heat resistance. Normalized 0-1. Heavy, hexagonal cubic elements
     *      tend to be much more temperature resistant. face-centered are typically
     *      not very heat resistant.
     *      
     *      -Heat-hardness ratio. Randomly modified ratio of heat/hardness.
     *      Elements with higher deformation tendencies become harder when
     *      exposed to heat. Elements with lower ones do not change.
     *      However, this ratio varies that by +-0.5.
     *      
     *      -Cleaving tendency. Normalized 0-1. Hard crystalline elements tend to
     *      cleave, while highly pliant elements tend to bend.
     *      
     *      -Standard pressure phase.
     *      -Solid, liquid, or gas. High covalence, low density, highly crystalline
     */
    public class Element
    {

        //independent properties. Stored as tuples for non-normal,normal value
        public string elementName;
        public (int original, double normalized) covalence;
        public double catenationRate;
        public (string asString, int asInt) baseStructure; //0 cubic, 1 crystalline
        public (double original, double normalized) deformation;


        //dependent properties
        public (int original, double normalized) density;
        public (string asString, int asInt) geometricStructure; //cubic: 1-3; crystalline, 3-12. --not normalized
        public (int original, double normalized) hardness; // 3-9 for cubic, <=10 for crystalline, tending towards <=5
        public double pliance;
        public double gravity;
        public double cleaveTendency;
        public (double meltingPoint, double boilingPoint) transitionalPoints; //temperatures when an element transitions phases
        public double heatRes; //modifies how slowly or quickly an element absorbs heat. Functions as a multiplier to transitionalPoints. e.g. (melting point)*1.05
        public double pressureRes; //linearly reduces or increases the melting and boiling point. measured as "degrees per atmosphere". e.g. .016 degree/atmosphere.

        public void initialize()
        {
            covalence = generateCovalentBonds();
            catenationRate = generateCatenationRate();
            baseStructure = generateBaseStructure();
            deformation = generateDeformation();
            density = generateDensity();
            geometricStructure = generateGeometricStructure();
            hardness = generateHardness();
            pliance = generatePliance();
            gravity = generateGravity();
        }

        public void initializeTestCase(int num)
        {
            covalence = generateCovalentBondsTest(num);
            catenationRate = generateCatenationRate();
            baseStructure = generateBaseStructure();
            deformation = generateDeformation();
            density = generateDensity();
            geometricStructure = generateGeometricStructure();
            hardness = generateHardness();
            pliance = generatePliance();
            gravity = generateGravity();
        }

        //selects an integer based on a probability of its occurrence.
        //See Dictionaries.weights
        (int,double) generateCovalentBonds()
        {
            double[] covalenceWeights = Dictionaries.weights["covalence"];
            int prob = utils.generateInverseDistro(8, covalenceWeights);
            return (prob, utils.normalize(prob, 1, 8));
        }

        //feeds an integer instead of selecting based on weights.
        //for testing purposes
        (int, double) generateCovalentBondsTest(int num)
        {
            return (num, utils.normalize(num, 1, 8));
        }

        //generates a value from 0-1, increasing based on how close an
        //element's number of covalent bonds is from 4.
        //Additionally, a random double is selected from
        //a uniform distribution and added or subtracted.
        //As the normalized distance would tend towards zero
        //the closer to 4, the final rate is 1 - the normalized distance
        //with the random uniform value applied.
        //e.g., if covalence.original = 4, dist = 0, therefore 1-dist = 1.
        double generateCatenationRate()
        {
            double catenation = 0;
            double uniformValue;
            double int_covalence = covalence.original;
            double dist = int_covalence - 4;

            while (true)
            {
                uniformValue = utils.getRandomDouble();
                if (uniformValue <= 0.15)
                {
                    catenation = Math.Abs(utils.normalize(dist, 0, 4));

                    if (catenation != 1)
                    {
                        catenation += +uniformValue;
                    } else
                    {
                        catenation -= uniformValue / 2;
                    }


                    if (catenation < 0 || catenation > 1)
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }

            }
                return 1 - catenation;
        }

        //generates a base structure, either cubic or crystalline.
        //Almost completely dependent on the number of covalent bonds.
        //As crystalline elements are more likely, there is a small
        //chance that a crystalline element will "flip". However,
        //if the catenation rate is too high, it cannot flip.
        (string,int) generateBaseStructure()
        {

            int unbiasedBase = (covalence.original <= 3) ? 0 : 1;
            string structureType = "cubic";

            double weight = utils.getRandomDouble();

            if (weight <= .2 && catenationRate <= .95)
            {
                return ("cubic",0);
            }else
            {
                if (unbiasedBase == 1)
                {
                    structureType = "crystalline";
                }
                return (structureType, unbiasedBase);
            }

        }

        //how likely the element is to deform when stressed.
        //The basis for this calculation is the normalized covalence/2.
        //Meaning, more covalent elements are more likely to deform.
        //Additionally, evenly covalent elements are more likely to deform than oddly covalent elements.
        //Lastly, a random uniform double is applied.
        (double, double) generateDeformation()
        {
            double baseDeformation = covalence.normalized / 2;
            double dampenedDeformation = (covalence.original % 2 == 0) ? baseDeformation+0.1 : baseDeformation-0.075;
            double dampener = utils.getRandomDouble(0.0,0.15);

            if (dampenedDeformation > 0.5)
            {
                dampenedDeformation = 0.5 - dampener;
            } else if (dampenedDeformation < 0.0)
            {
                dampenedDeformation = 0.0 + dampener;
            }

            double refinedDeformation = utils.normalize(dampenedDeformation,0.0,0.5);
            return (dampenedDeformation,refinedDeformation);
        }

        //How dense the element is. Cubic elements tend to be the most dense,
        //metalloids are moderately dense, and everything else is not very dense.
        //Evenly covalent elements are denser than odd.
        //highly covalent elements with low deformative tendencies tend to be denser than otherwise.
        //lowly covalent elements with low deformative tendencies do not typically increase beyond +1 as per the dampener.
        (int, double) generateDensity()
        {
            int baseDensity = 0; ;

            if (covalence.original <= 3)
            {
                baseDensity = utils.getRandomInt(6, 12);
            } else if (covalence.original > 3 && covalence.original <= 6)
            {
                baseDensity = utils.getRandomInt(3, 9);
            } else if (covalence.original > 6)
            {
                baseDensity = utils.getRandomInt(1, 6);
            }

            double symmetryMod = (covalence.original % 2 == 0) ? 0.75 : 0.5;
            double dampDensity = (baseDensity / symmetryMod) + (Math.Sqrt(covalence.original)-deformation.normalized);
            int dampDensityInt = (int)Math.Round(dampDensity);

            //The maximum value is rolling a 12 with 3 covalence. 12 / 0.5 = 24. 24 + sqrt(3) = 24.7. 24.7-0.0 = 24.7 which rounds to 25.
            //This line is solely due to paranoia.
            if (dampDensityInt > 25)
            {
                dampDensityInt = 25;
            }

            return (dampDensityInt, utils.normalize(dampDensityInt,1,25));
        }

        //Geometry is entirely dependent on density for cubic elements.
        //For crystalline elements, it is dependent on covalence and catenation.
        //If it is lowly catenating (below .5), density has a max value of 8*1.5, or 12.
        //If it is highly catenating (above .5), density has a max value of 5*1.85, or 9.25.
        //FAQ: WHY NOT JUST COVALENCE?! ISN'T CATENATION BASED ON COVALENCE?!
        //Answer: Recall- catenation can increase by a max of .15 and decrease by a max of .075.
        //        Though a small margin, this can flip the operation for values immediately
        //        next to 4.
        (string,int) generateGeometricStructure()
        {
            // dependent on catenation and density -- cannot exceed 12

            int baseGeometry;

            if (baseStructure.asInt == 0)
            {
                if (density.original <= 14) {baseGeometry = 2;}
                else if (density.original > 14 && density.original <= 21){ baseGeometry = 1;}
                else{baseGeometry = 3;}
                return (Dictionaries.cubics[baseGeometry],baseGeometry);

            } else
            {
                int operation = (int) Math.Round(catenationRate);
                int dampGeometry = (operation == 0) ?
                    (int) Math.Round((covalence.original * (1.25 + utils.getRandomDouble(0.0, .25)))) :
                    (int) Math.Round((covalence.original * (1.6 - utils.getRandomDouble(0.0, .25))));
                return (Dictionaries.polygons[dampGeometry], dampGeometry);
            }
        }

        //For cubic elements, hardness is entirely based on density,
        //though it has a 20% chance of being slighly reduced.
        //For crystalline elements, it is also based on density,
        //but has a 30% chance of being based on catenation instead.
        //This allows highly catenating elements to reach high densities
        //naturally (e.g. hyper-dense metalloids, like diamond).
        (int, double) generateHardness()
        {
            //basis for calculating edge.
            //harder elements have more tensile and compressive strength, but less shear strength.
            //softer elements, in turn, have less tensile and compressive strength, but more shear strength
            //harder elements must be shorter in length (when made into ingot) because they tend to snap instead of bend
            int baseHardness = (int) Math.Ceiling(density.normalized * 10);
            int dampener;
            if (baseStructure.asInt == 0)
            {
                dampener = utils.getRandomInt(0, 10);
                if (dampener < 2) { baseHardness -= dampener; }
                return (baseHardness, utils.normalize(baseHardness, 1, 10));
            } else
            {
                //diamonds! 30% chance to be highly catenating
                dampener = utils.getRandomInt(0, 10);
                if (dampener < 3) { baseHardness = (int)Math.Ceiling(catenationRate * 10); }
                return (baseHardness, utils.normalize(baseHardness, 1, 10));
            }

        }

        //For body centered and hexagonal cuboid elements, pliance is between .55 and .8.
        //For face-centered elements, it is between .75 and .9.
        //For crystalline elements, it is based on catenation and deformation.
        //
        //Recall that catenation increases the closer an element is to 4 covalent bonds.
        //Also recal that deformation increases for highly covalent and even elements.
        // There for, covalence+deformation/2 balances lowly covalent, moderately deforming
        // elements with highly covalent, highly deforming elements, and applies a random
        // uniform value between 0 and .2. NOTE that non normalized deformation cannot exceed .5.
        // So, the maximum theoretical value is .75 for crystalline elements, decreasing based
        // on the abovementioned factors.
        double generatePliance()
        {
            //workability. How "workable" is the element? pliability is essentially an elements
            //ability to retain base properties when modified. Less pliant elements are easier
            //destroyed when undergoing certain processes and more likely to fail when alloyed
            double basePliance;

            if (baseStructure.asInt == 0)
            {
                if (geometricStructure.asInt == 1 || geometricStructure.asInt == 3)
                    basePliance = (1 - utils.getRandomDouble(0.2, 0.45));
                else
                {
                    basePliance = 1 - utils.getRandomDouble(0.1, 0.25);
                }
            }
            else
            {
                basePliance = ((catenationRate+deformation.original)/2.0) - utils.getRandomDouble(0.0, 0.2);
            }
            return basePliance;
        }

        //More accurately, relative mass. Dependent on density and base structure.
        //For cubic elements, the base gravity is 0.05; 0 for crystalline.
        //For cubic elements, the upper bound for random generation is .0175 (smaller) compared to 0.02
        //This is because cubic elements are typically denser, so they will iterate more.
        //NOTE: though this could be written as density*the random uniform value,
        //      I chose to just leave the for loop to better illustrate the thought process behind it.
        //ALSO NOTE: This value is not normalized. Though, the theoretical maximum is 0.4875 for cubic elements
        //           and .28 for crystalline elements (PLEASE DO NOT ASK ME HOW I GOT THIS NUMBER. SEE DENSITY FUNCTION).
        //           Using these max values, we would say that the element accounts for 48.75% of the weight when
        //           when bonded at each point to other elements (and 28% for the crystalline max).
        double generateGravity()
        {
            //how heavy? In otherwords, what percentage of weight of a mineral will this element take up when bonded?
            double baseGravity = (baseStructure.asInt == 0) ? 0.05 : 0.0;
            double upperBound = (baseStructure.asInt == 0) ? 0.0175 : 0.02;
            for (int i=0; i<density.original; i++)
            {
                baseGravity += utils.getRandomDouble(0.01,upperBound);
            }

            return baseGravity;


        }

        double generateCleaveTendency()
        {
            throw new Exception("implement");
        }

       (double, double) generateTransitionalPoints()
        {
            throw new Exception("implement");
        }

        double generateHeatRes()
        {
            throw new Exception("implement");
        }

        double generatePressureRes()
        {
            throw new Exception("implement");
        }
    }
}