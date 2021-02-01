using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using static Bolder_Blacksmith.Generators.ElementConstants;

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
     *      -Base structure. Cubic structures
     *      are more metallic, crystalline structures
     *      are either non-metals or metalloids.
     *      Either cubic (0) or crystalline (1).
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
     *      is typically high. Although the name might
     *      suggest, pliance is not inversely proportional
     *      to hardness. Pliance is the likelihood of
     *      an element to retain its properties when
     *      undergoing processes. Basically, the % impact
     *      of an elements properties when alloyed.
     *      
     *      -Gravity. Likelihood of a material being
     *      heavier than the things it is bound to.
     *      Essentially relative gravity as a percentage
     *      
     *      -Cleaving tendency. Normalized 0-1. Hard crystalline elements tend to
     *      cleave, while soft cubic elements tend to bend.
     *      
     *      -Transitional points. Includes melting and boiling point. The
     *      point when a element switches phases. Represented as temperatures (F).
     *      Most often, the melting point is the desired phase change. However,
     *      boiling can occur as well. Vaporized elements are much harder to
     *      handle in-game.
     *      
     *      -Heat resistance. Heavy, hexagonal cubic elements
     *      tend to be much more temperature resistant. face-centered are typically
     *      not very heat resistant. Functions as a multiplier or divider. There are no
     *      explicit bounds, but unreasonable heat resistance would result in
     *      an element thats phase does not change.
     *      
     *      -Pressure resistance. Functions as a multiplier or divider, raising or lowering
     *      transitional points conditionally, based on atmospheric pressure. Represented
     *      as a ratio.
     *      
     *      -Heat Absorption. Represented as the degrees subtracted per tick. For example,
     *      if the heat absorption is 12, then every tick, the temperature of the element
     *      will increase by 12 degrees. Calculated based upon heat resistance, geometric
     *      structure (i.e., -gons), and deformation (as this affects the integrity of
     *      the structure). Base structure is an indirect factor seeing as cuboids
     *      are always between 1-3.
     */

    public class Element
    {

        //independent properties. Stored as tuples for non-normal,normal value
        public string elementName;
        public double[] covalenceWeights = Dictionaries.weights["covalence"];
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
        public (double meltingRes, double boilingRes) heatRes; //e.g. (melting point)*1.05
        public double pressureRes; //measured as "degrees per atmosphere". e.g. .016 degree/atmosphere.
        public double heatAbsorption; //Rate of heat absorption, e.g. 12/tick
        public double radioactivity = 0.0; //0.0 to 1.0, very rare, causes problems
        public double magnetism = 0.0; //0.0 to 1.0,  does something?

        //basic constructor
        public Element(double [] weights = null) {
            covalenceWeights = weights ?? covalenceWeights;
            covalence = generateCovalentBonds();
            catenationRate = generateCatenationRate();
            baseStructure = generateBaseStructure();
            deformation = generateDeformation();
            density = generateDensity();
            geometricStructure = generateGeometricStructure();
            hardness = generateHardness();
            pliance = generatePliance();
            gravity = generateGravity();
            cleaveTendency = generateCleaveTendency();
            heatRes = generateHeatRes();
            pressureRes = generatePressureRes();
            transitionalPoints = generateTransitionalPoints();
            heatAbsorption = generateHeatAbsorption();
            checkParameters();
        }

        //test case init
        public Element(int num)
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
            cleaveTendency = generateCleaveTendency();
            heatRes = generateHeatRes();
            pressureRes = generatePressureRes();
            transitionalPoints = generateTransitionalPoints();
            heatAbsorption = generateHeatAbsorption();
            checkParameters();
        }

        //copy constructor
        public Element (Element other)
        {
            elementName = other.elementName;
            covalence = other.covalence;
            catenationRate = other.catenationRate;
            baseStructure = other.baseStructure;
            deformation = other.deformation;
            density = other.density;
            geometricStructure = other.geometricStructure;
            hardness = other.hardness;
            pliance = other.pliance;
            gravity = other.gravity;
            cleaveTendency = other.cleaveTendency;
            transitionalPoints = other.transitionalPoints;
            heatAbsorption = other.heatAbsorption;
            heatRes = other.heatRes;
            pressureRes = other.pressureRes;
            radioactivity = other.radioactivity;
            magnetism = other.magnetism;
            checkParameters();
        }

        public void printInfo()
        {
            Console.WriteLine("Normal Covalence: " + covalence.normalized);
            Console.WriteLine("Integer Covalence: " + covalence.original);
            Console.WriteLine("...");
            Console.WriteLine("Catenation Rate: " + catenationRate);
            Console.WriteLine("...");
            Console.WriteLine("Base Structure: " + baseStructure.asString);
            Console.WriteLine("...");
            Console.WriteLine("Original Deformation: " + deformation.original);
            Console.WriteLine("Normal Deformation: " + deformation.normalized);
            Console.WriteLine("...");
            Console.WriteLine("Original Density: " + density.original);
            Console.WriteLine("Normal Density: " + density.normalized);
            Console.WriteLine("...");
            Console.WriteLine("Geometric Structure: " + geometricStructure.asString);
            Console.WriteLine("Geometric Structure -gons: " + geometricStructure.asInt);
            Console.WriteLine("...");
            Console.WriteLine("Hardness Original: " + hardness.original);
            Console.WriteLine("Hardness Normalized: " + hardness.normalized);
            Console.WriteLine("...");
            Console.WriteLine("Pliance: " + pliance);
            Console.WriteLine("...");
            Console.WriteLine("Gravity: " + gravity);
            Console.WriteLine("...");
            Console.WriteLine("Cleaving Tendency: " + cleaveTendency);
            Console.WriteLine("...");
            Console.WriteLine("Melting Resistance: " + heatRes.meltingRes);
            Console.WriteLine("Boiling Resistance: " + heatRes.boilingRes);
            Console.WriteLine("...");
            Console.WriteLine("Melting Point: " + transitionalPoints.meltingPoint);
            Console.WriteLine("Boiling Point: " + transitionalPoints.boilingPoint);
            Console.WriteLine("...");
            Console.WriteLine("Pressure Resistance: " + pressureRes);
            Console.WriteLine("...");
            Console.WriteLine("Heat Absorption: " + heatAbsorption);
            Console.WriteLine("...");
            Console.WriteLine("Radioactivity: " + radioactivity);
            Console.WriteLine("...");
            Console.WriteLine("Magnetism: " + magnetism);
            Console.WriteLine("---------------------------------------");
        }

        //selects an integer based on a probability of its occurrence.
        //See Dictionaries.weights
        (int,double) generateCovalentBonds()
        {
            int prob = utils.generateInverseDistro(covalence_max, covalenceWeights);
            return (prob, utils.normalize(prob, covalence_min, covalence_max));
        }

        //feeds an integer instead of selecting based on weights.
        //for testing purposes
        (int, double) generateCovalentBondsTest(int num)
        {
            return (num, utils.normalize(num, covalence_min, covalence_max));
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
            double dist = int_covalence - catenation_max;

            while (true)
            {
                uniformValue = utils.getRandomDouble();
                if (uniformValue <= 0.15)
                {
                    catenation = Math.Abs(utils.normalize(dist, catenation_min, catenation_max));

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

            int unbiasedBase = (covalence.original <= 3) ? is_cubic : is_crystal;
            string structureType = "cubic";

            double weight = utils.getRandomDouble();

            if (weight <= .18 && catenationRate <= structure_cubic_limit)
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
            double dampenedDeformation = (covalence.original % 2 == 0) ? baseDeformation+deformation_even_symmetry_multiplier
                                                                       : baseDeformation+deformation_odd_symmetry_multiplier;
            double dampener = utils.getRandomDouble(0.0,0.15);

            if (dampenedDeformation > deformation_max)
            {
                dampenedDeformation = deformation_max - dampener;
            } else if (dampenedDeformation < deformation_min)
            {
                dampenedDeformation = deformation_min + dampener;
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

            double symmetryMod = (covalence.original % 2 == 0) ? density_even_symmetry_multiplier : density_odd_symmetry_multiplier;
            double dampDensity = (baseDensity / symmetryMod) + (Math.Sqrt(covalence.original)-deformation.normalized);
            int dampDensityInt = (int)Math.Round(dampDensity);

            //The maximum value is rolling a 12 with 3 covalence. 12 / 0.5 = 24. 24 + sqrt(3) = 24.7. 24.7-0.0 = 24.7 which rounds to 25.
            //This line is solely due to paranoia.
            if (dampDensityInt > density_max)
            {
                dampDensityInt = density_max;
            }

            return (dampDensityInt, utils.normalize(dampDensityInt, density_min, density_max));
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

            if (baseStructure.asInt == is_cubic)
            {
                if (density.original <= 14) {baseGeometry = is_face_centered;}
                else if (density.original > 14 && density.original <= 21){ baseGeometry = is_body_centered;}
                else{baseGeometry = is_hexagonal_cuboid;}
                return (Dictionaries.cubics[baseGeometry],baseGeometry);

            } else
            {
                int operation = (int) Math.Round(catenationRate);
                int dampGeometry = (operation == 0) ?
                    (int) Math.Round((covalence.original * (geometry_add_op + utils.getRandomDouble(0.0, .25)))) :
                    (int) Math.Round((covalence.original * (geometry_sub_op - utils.getRandomDouble(0.0, .25))));
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
            if (baseStructure.asInt == is_cubic)
            {
                dampener = utils.getRandomInt(0, 10);
                if (dampener < hardness_cubic_damp_limit) { baseHardness -= dampener; }
            } else
            {
                //diamonds! 30% chance to be highly catenating
                dampener = utils.getRandomInt(0, 10);
                if (dampener < hardness_crystalline_damp_limit) { baseHardness = (int)Math.Ceiling(catenationRate * 10); }
            }
            if (baseHardness < hardness_min)
            {
                baseHardness = hardness_min;
            }
            return (baseHardness, utils.normalize(baseHardness, hardness_min, hardness_max));
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

            if (baseStructure.asInt == is_cubic)
            {
                if (geometricStructure.asInt == is_body_centered || geometricStructure.asInt == is_hexagonal_cuboid)
                    basePliance = (1 - utils.getRandomDouble(pliance_hard_cubics_min, pliance_hard_cubics_max));
                else
                {
                    basePliance = 1 - utils.getRandomDouble(pliance_soft_cubics_min, pliance_soft_cubics_max);
                }
            }
            else
            {
                basePliance = ((catenationRate+deformation.original)/2.0) - utils.getRandomDouble(pliance_crystalline_min, pliance_crystalline_max);
                basePliance = utils.Clamp(basePliance,0.0,1.0);
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
            double baseGravity = (baseStructure.asInt == is_cubic) ? gravity_cubic_base_weight : gravity_crystalline_base_weight;
            double upperBound = (baseStructure.asInt == is_cubic) ? gravity_cubic_iter_max : gravity_crystalline_iter_max;
            for (int i=0; i<density.original; i++)
            {
                baseGravity += utils.getRandomDouble(gravity_iter_min,upperBound);
            }

            return baseGravity;


        }

        //based on base structure and hardness.
        //This number is the likelihood that the
        //element will cleave instead of bend.
        double generateCleaveTendency()
        {
            double structureMod = (baseStructure.asInt == is_cubic) 
                ? utils.getRandomDouble(cleave_cubic_min, cleave_cubic_max) 
                : utils.getRandomDouble(cleave_crystal_min + (covalence.normalized/cleave_crystal_damp), cleave_crystal_max);
            double baseCleave = structureMod - (hardness.normalized / cleave_damp);

            double dampCleave = utils.Clamp(baseCleave, cleave_min, cleave_max);

            return dampCleave;
        }

        //Generates multipliers for transitional points
        //depending on specified bounds for cubics,
        //covalence for crystallines, and a mixture of both for cubic with >3 covalence.
        //The boiling point multiplier is entirely based upon the melting point multiplier
        (double, double) generateHeatRes()
        {
            double baseMeltingRes = (1 + (1 - covalence.normalized));
            double baseBoilingRes;

            if (baseStructure.asInt == is_cubic && covalence.original <= 3)
            {
                switch (geometricStructure.asInt)
                {
                    case is_body_centered:
                        baseMeltingRes = utils.getRandomDouble
                            (heat_res_body_centered_min, heat_res_body_centered_max);
                        break;
                    case is_face_centered:
                        baseMeltingRes = utils.getRandomDouble
                            (heat_res_face_centered_min, heat_res_face_centered_max);
                        break;
                    case is_hexagonal_cuboid:
                        baseMeltingRes = utils.getRandomDouble
                            (heat_res_hexagonal_cuboid_min, heat_res_hexagonal_cuboid_max);
                        break;
                }

            }
            else if (baseStructure.asInt == is_cubic && covalence.original >= 4)
            {
                baseMeltingRes += utils.getRandomDouble
                    (heat_res_abnormal_cuboid_min, heat_res_abnormal_cuboid_max);
            }

            double divisor = (baseMeltingRes > 1.0) ?
                heat_res_non_zero_boiling_scaler : heat_res_zero_boiling_scaler;
            baseBoilingRes = Math.Truncate(baseMeltingRes) + ((baseMeltingRes % 1.0) / divisor);

            return (baseMeltingRes, baseBoilingRes);
        }

        //atmosphere is normalized number from -5 to 5; see Engines.GameConstants
        //At 0, the base melting/boiling point are used; see Engines.GameInstance
        //Pressure resistance increases based on covalence
        double generatePressureRes()
        {
            double basePressureRes;
            double dampPressureRes;

            int sign = (covalence.original > 3) ? 1 : -1;
            basePressureRes = ((covalence.normalized/2) * sign) + (utils.getRandomDouble(pressure_damp_min, pressure_damp_max) * (sign));

            dampPressureRes = (sign == 1) ? 1.0 + basePressureRes : 1.0 / 1.0 + basePressureRes;

            return dampPressureRes;
                                
        }

        //Generates melting and boiling points for an element
        //based on density and hardness.
        //See utils for function specifics.
        //
        //NOTE: I could go into great depth in what the function is intended to do,
        //      but to put it simply:
        //
        //      The "dampener" is log function that gradually
        //      decreases from 1 towards 0 depending on the input
        //      (only verified for 1 to 25). It is averaged
        //      with the hardness to generate a multiplier
        //      that prevents less dense elements from producing values
        //      that are too small and denser elements from producing
        //      values that are too large.
        //
        //      The actual function is just exponential
        //      function. The dampener multiplies the constant
        //      and adds it to the base function of 20x^2/sqrt(x)
        (double, double) generateTransitionalPoints()
        {
            double d = density.original;
            double s = hardness.normalized;
            double c1 = melting_point_density_scaler;
            double c2 = boiling_point_density_scaler;
            double m = utils.getTransitionalPointDampener(density.original, 0);
            double b = utils.getTransitionalPointDampener(density.original, 1);

            //melting
            double t1 = utils.getTransitionalPoint(d, c1, s, m);

            //boiling
            double t2 = utils.getTransitionalPoint(d, c2, s, b);

            return (t1 * heatRes.meltingRes, t2 * heatRes.boilingRes);

        }

        double generateHeatAbsorption()
        {

            return heatRes.meltingRes * (geometricStructure.asInt * (deformation.original + 1));

        }

        // Makes sure that all of the generated parameters are within their
        // respective bounds, throwing an error if that is not the case
        // The warn option will also throw warnings for parameters
        // that are exactly at the maximum or minimum value for floating-
        // point entries, since that should probably never happen
        public void checkParameters()
        {
            Debug.Assert(covalence.original >= covalence_min);
            Debug.Assert(covalence.original <= covalence_max);
            Debug.Assert(covalence.normalized >= normalized_min);
            Debug.Assert(covalence.normalized <= normalized_max);
            Debug.Assert(catenationRate >= catenation_min);
            Debug.Assert(catenationRate <= catenation_max);
            Debug.Assert(deformation.original >= deformation_min);
            Debug.Assert(deformation.original <= deformation_max);
            Debug.Assert(deformation.normalized >= normalized_min);
            Debug.Assert(deformation.normalized <= normalized_max);
            Debug.Assert(density.original >= density_min);
            Debug.Assert(density.original <= density_max);
            Debug.Assert(density.normalized >= normalized_min);
            Debug.Assert(density.normalized <= normalized_max);
            bool validStructure = false;
            if (baseStructure.asInt == 0)
            {
                foreach (KeyValuePair<int, string> entry in Dictionaries.cubics)
                {
                    if (geometricStructure.asInt == entry.Key)
                    {
                        Debug.Assert(geometricStructure.asString == entry.Value);
                        validStructure = true;
                    }
                }
            } else if (baseStructure.asInt == 1)
            {
                Debug.Assert(baseStructure.asString == "crystalline");
                foreach (KeyValuePair<int, string> entry in Dictionaries.polygons)
                {
                    if (geometricStructure.asInt == entry.Key)
                    {
                        Debug.Assert(geometricStructure.asString == entry.Value);
                        validStructure = true;
                    }
                }
            }
            Debug.Assert(validStructure);
            Debug.Assert(hardness.original >= hardness_min);
            Debug.Assert(hardness.original <= hardness_max);
            Debug.Assert(hardness.normalized >= normalized_min);
            Debug.Assert(hardness.normalized <= normalized_max);
            Debug.Assert(pliance >= pliance_min);
            Debug.Assert(pliance <= pliance_max);
            Debug.Assert(gravity >= gravity_min);
            Debug.Assert(gravity <= gravity_max);
            Debug.Assert(cleaveTendency >= cleave_min);
            Debug.Assert(cleaveTendency <= cleave_max);
            Debug.Assert(heatRes.meltingRes > 0);
            Debug.Assert(heatRes.boilingRes > 0);
            Debug.Assert(transitionalPoints.meltingPoint > 0);
            Debug.Assert(transitionalPoints.boilingPoint > transitionalPoints.meltingPoint);
            Debug.Assert(pressureRes > 0);
            Debug.Assert(heatAbsorption > 0);
            Debug.Assert(radioactivity >= 0);
            Debug.Assert(magnetism >= 0);
        }
    }
}