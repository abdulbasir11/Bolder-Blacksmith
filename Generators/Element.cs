using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bolder_Blacksmith.Generators
{
    /*Element properties include:
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
        public double hardness; // 3-9 for cubic, <=10 for crystalline, tending towards <=5
        public double pliance;
        public double gravity;
        public double stdPhase; //0-0.2: gas, 0.2-0.4: liquid, >0.4, solid, >0.8, supersolid
        public int phase; //int representaion of phase; see above
        public double heatRes;
        public double heatHardness;
        public double cleaveTendency;

        public void initialize()
        {
            covalence = generateCovalentBonds();
            catenationRate = generateCatenationRate();
            baseStructure = generateBaseStructure();
            deformation = generateDeformation();
            density = generateDensity();
            geometricStructure = generateGeometricStructure();
        }

        public void initializeTestCase(int num)
        {
            covalence = generateCovalentBondsTest(num);
            catenationRate = generateCatenationRate();
            baseStructure = generateBaseStructure();
            deformation = generateDeformation();
            density = generateDensity();
            geometricStructure = generateGeometricStructure();
        }

        (int,double) generateCovalentBonds()
        {
            double[] covalenceWeights = Dictionaries.weights["covalence"];
            int prob = utils.generateInverseDistro(8, covalenceWeights);
            return (prob, utils.normalize(prob, 1, 8));
        }

        (int, double) generateCovalentBondsTest(int num)
        {
            return (num, utils.normalize(num, 1, 8));
        }

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


        (int, double) generateDensity()
        {
            //dense if:
            /*
             * cubic
             * odd symmetry
             * lower deformation
             * lower covalence?
             */

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
    }
}