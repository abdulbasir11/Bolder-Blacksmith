using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Bolder_Blacksmith.ColorUtilities;
using Pastel;

namespace Bolder_Blacksmith.Generators
{
    /* 
     * Mineral properties include:    
     *      -Number of elements. Ranges from 1-4.
     *      
     *      -Element distribution. Sets distributions of each
     *      mineral based on covalence, catenation, and gravity, with some
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
     *      -Instability. Residual weight. Calculated as
     *      (netweight % 1)/2
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

        //element generator
        public ElementGenerator elemGen;

        //for indexing
        char[] elemNames = { 'a', 'b', 'c', 'd', 'e' };

        //fields
        public int numElements;
        public Element[] elements;
        public Dictionary<int, int> atoms;
        public Dictionary<int, double> atomDistribution;
        public int reactivity; //MAY rename this. But, essentially the number of remaining bonds after atoms are distributed

        public int complexity; //avg geometric structure
        public (double satMod, double valMod) reflectance;
        public string color; //hex code? there is a System.(something).Color but, would it be better to just store it as a string for now?
        public (double average, double weighted) structure; //avg base structure
        public double luster; //based on avg base structure and complexity
        public double iridescence;
        public double deformation;
        public double density;
        public double hardness;
        public double pliance;
        public double cleaveTendency;
        public (double meltingPoint, double boilingPoint) transitionalPoints;
        public (double meltingRes, double boilingRes) heatRes;
        public double pressureRes;
        public double heatAbsorption;

        public double stability; //basis for calculation for powder-filament continuum.

        public (int asInt, string asString) classification; //1 - metal, 2 - metalloid, 3 - non-metal

        public double radioactivity = 0.0;
        public double magnetism = 0.0;

        public Mineral()
        {
            elemGen = new ElementGenerator();
      
            numElements = generateNumElements();
            elements = elemGen.getBatchElements(numElements);

            (Dictionary<int, int>, int) atomsAndResids = generateAtoms(); //this kinda sucks but it works
            atoms = atomsAndResids.Item1;
            reactivity = atomsAndResids.Item2;

            atomDistribution = generateElementDistro();

            //one liners for summing and averaging any particular field
            // the Select grabs all of a particular field, then it is converted to an array.
            // this is effectively creating a new array from the list of elements with just the selected field
            // it is then averaged, rounded, and cast to an integer, in this case
            complexity = (int) Math.Round(utils.Average(elements.Select(e => e.geometricStructure.asInt).ToArray()));
            structure = generateStructure();
            deformation = utils.Average(elements.Select(e => e.deformation.original).ToArray());
            density = utils.Average(elements.Select(e => e.density.original).ToArray());
            hardness = utils.Average(elements.Select(e => e.hardness.original).ToArray());
            pliance = utils.Average(elements.Select(e => e.pliance).ToArray());
            cleaveTendency = utils.Average(elements.Select(e => e.cleaveTendency).ToArray());
            transitionalPoints = (utils.Average(elements.Select(e => e.transitionalPoints.meltingPoint).ToArray()),
                utils.Average(elements.Select(e => e.transitionalPoints.boilingPoint).ToArray()));
            heatRes = (utils.Average(elements.Select(e => e.heatRes.meltingRes).ToArray()),
                utils.Average(elements.Select(e => e.heatRes.boilingRes).ToArray()));
            pressureRes = utils.Average(elements.Select(e => e.pressureRes).ToArray());
            heatAbsorption = utils.Average(elements.Select(e => e.heatAbsorption).ToArray());

            stability = generateStability();
            reflectance = generateReflectance();
            color = generateColor();
            luster = generateLuster();
            iridescence = generateIridescence();
            classification = generateClassifcation();

        }

        public void printInfo()
        {
            Console.WriteLine("Number of elements: " + numElements);
            Console.WriteLine("...");
            
            
            Console.WriteLine("Individual elements: ");
            foreach (Element s in elements)
            {
                s.printInfo();
            }
            Console.WriteLine("...");
            
            
            Console.WriteLine("Atoms");
            foreach (KeyValuePair<int,int> e in atoms)
            {
                Console.WriteLine("Element name: " + elemNames[e.Key - 1] + " - Assoc. Atoms: " + e.Value);
            }
            Console.WriteLine("...");
            Console.WriteLine("Atom Distribution");
            foreach (KeyValuePair<int, double> e in atomDistribution)
            {
                Console.WriteLine("Element name: " + elemNames[e.Key - 1] + " ; Assoc. Atoms %: " + e.Value);
            }
            Console.WriteLine("...");
            Console.WriteLine("Reactivity: " + reactivity);
            Console.WriteLine("...");
            Console.WriteLine("Complexity: " + complexity);
            Console.WriteLine("...");
            Console.WriteLine("Structure (avg): " + structure.average);
            Console.WriteLine("Structure (wt): " + structure.weighted);
            Console.WriteLine("...");
            Console.WriteLine("Stability: " + stability);
            Console.WriteLine("...");
            Console.WriteLine("Reflectance: " + reflectance.satMod + ", " + reflectance.valMod);
            Console.WriteLine("...");
            Console.WriteLine("Color: " + color);
            Console.WriteLine("                        ".PastelBg(color));
            Console.WriteLine("...");
            Console.WriteLine("Luster: " + luster);
            Console.WriteLine("...");
            Console.WriteLine("Iridescence: " + iridescence);
            Console.WriteLine("...");
            Console.WriteLine("Classification: " + classification.asString);
            Console.WriteLine("...");
            Console.WriteLine("Original Deformation: " + deformation);
            Console.WriteLine("...");
            Console.WriteLine("Original Density: " + density);
            Console.WriteLine("...");
            Console.WriteLine("Hardness Original: " + hardness);
            Console.WriteLine("...");
            Console.WriteLine("Pliance: " + pliance);
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

        //See Dictionaries.cs
        public int generateNumElements()
        {
            double[] elementWeights = Dictionaries.weights["mineralElements"];
            return utils.generateInverseDistro(5, elementWeights);
            
        }

        //In other words, how much of one element is the mineral made up of?
        //Originally, this was going to be represented as a percentage.
        //e.g., H2O being 66% hydrogen and 33% oxygen.
        //
        //However, assigning an integer to each element based on
        //weight, covalence, and some random modding prevents the
        //summation of each percentage from exceeding 100%, intrinsically.
        //
        //For example, if you randomly generate 8 atoms for Element A,
        //3 for Element B, and 34 for Element C, you could calculate the
        //percentages under the assumption you will never exceed 100%.
        //This would not be the case if the percentages were generated
        //directly.
        //Also, this allows for elements to have a discernable chemical makeup.
        //e.g. A8B3C34.

        public (Dictionary<int,int>, int) generateAtoms()
        {
            //recall: covalence: how many bonds an element can have; use as an iterator
            //        catenation rate: % chance that it bonds to something other than itself; use for rejection
            //        gravity/weight: relative mass as a percentage, use for limiting atom generation

            //first: find how many atoms a single element would have

            List<int> singleCat = new List<int>();
            int counter;
            double comparator;
            foreach (Element e in elements)
            {
                counter = 1; //there's at least 1 atom!
                while (true) {
                    comparator = utils.getRandomDouble(); //new roll each iteration!
                    if (comparator < e.catenationRate)
                    {
                        counter++;
                        if (counter * e.gravity > 1)
                        {
                            singleCat.Add(counter - 1);
                            break;
                        }
                    }
                    else
                    {
                        singleCat.Add(counter);
                        break;
                    }
                }
            }

            List<Tuple<int, int, int, int>> test = new List<Tuple<int, int, int, int>>(); //basically a sortable dictionary

            //id, self-catenated atoms, covalence, and available atoms
            for (int i = 0; i < singleCat.Count; i++)
            {
                test.Add(new Tuple<int, int, int, int>(i + 1, singleCat[i], elements[i].covalence.original, singleCat[i] - elements[i].covalence.original));
            }


            //second: cascade them - elements with available bonds (positive atoms - covalence) give to negative ones until they have
            //        no more available bonds. if all are the same sign (bar 0) then return.

            test.Sort((x,y) => y.Item4.CompareTo(x.Item4)); //init sort by bonds

            //uh... basically, compare the largest negative number to the largest positive
            //if the result is negative, compare it to the next positive number
            //if this isn't possible, then return

            int min;
            int max;
            int remainder;

            //if >1 element
            while (test.Count != 1)
            {
                min = test.Min((x) => x.Item4); //minimum available bonds
                max = test.Max((x) => x.Item4); //max available bonds

                //if the max is not yet 0 AND a minimum <0 still exists
                if(max > 0 && min < 0)
                {
                    //deduct the minimum from the maximum. Recall that the minimum is always negative when this is executed
                    remainder = max + min;

                    //if the maximum is greater than 0 after deducting the minimum
                    if (remainder > 0)
                    {
                        //Recall that due to the initial sort, the maximum is the first element and the minimum is the last, always
                        test[0] = Tuple.Create(test[0].Item1, test[0].Item2+min, test[0].Item3, remainder); //Maximum element is the non-zero remainder
                        test[test.Count - 1] = Tuple.Create(test[test.Count - 1].Item1, test[test.Count - 1].Item2, test[test.Count - 1].Item3, 0); //minimum element is 0
                        test.Sort((x, y) => y.Item4.CompareTo(x.Item4)); //sort again to ensure maximum is the first element and the minimum is the last 

                    //if the maximum is less than or equal to 0 after deducting the minimum
                    } else
                    {
                        test[0] = Tuple.Create(test[0].Item1, test[0].Item3, test[0].Item3, 0); //all available bonds are consumed and the maximum is now 0
                        test[test.Count - 1] = Tuple.Create(test[test.Count - 1].Item1, test[test.Count - 1].Item2, test[test.Count - 1].Item3, remainder); //the minimum is now equal to the minimum plus the remainder
                        test.Sort((x, y) => y.Item4.CompareTo(x.Item4)); //sort again to ensure maximum is the first element and minimum is the last
                    }
                }
                else
                {
                    //leave the loop if all available bonds have been consumed
                    //(meaning the maximum would be negative or 0 AND the minimum would be positive or 0 with the next iteration)
                    break;
                }

            }

            //convert to dict
            int residuals = 0;
            Dictionary<int, int> returnme = new Dictionary<int, int>();
            foreach (Tuple<int,int,int,int> s in test)
            {
                returnme.Add(s.Item1, s.Item2);
                residuals += s.Item4;
            }
            return (returnme, residuals);
        }

        //the percentage of each atom takes up, as abovementioned
        public Dictionary<int,double> generateElementDistro()
        {
            Dictionary<int, double> distro = new Dictionary<int, double>();
            int sum = 0;
            foreach (int i in atoms.Values)
            {
                sum += i;
            }

            foreach (KeyValuePair<int,int> a in atoms)
            {
                distro.Add(a.Key, (double) a.Value / (double) sum);
            }

            return distro;
        }

        public (double, double) generateStructure()
        {
            List<Tuple<int, int>> sortedDistro = new List<Tuple<int, int>>();
            foreach (KeyValuePair<int, int> kv in atoms)
            {
                sortedDistro.Add(Tuple.Create(kv.Key, kv.Value));
            }
            sortedDistro.Sort((x, y) => x.Item1.CompareTo(y.Item1));

            double weightedStructure = 0;
            int counter = 0;
            int totalAtoms = utils.Sum(atoms.Select(e => e.Value).ToArray());


            foreach (Element e in elements)
            {

                for (int i = 0; i < sortedDistro[counter].Item2; i++)
                {
                    weightedStructure += e.baseStructure.asInt;
                }
                counter++;

            }

            return (utils.Average(elements.Select(e => e.baseStructure.asInt).ToArray()), weightedStructure/totalAtoms);
        }

        //determines the key value for the color calculation
        //Essentially, this is adapting the Lightness field from HSL
        //to HSV. Moving diagonally in the color space
        //i.e., decreasing the saturation and increasing the value
        //mimick the effect of Lightness.
        // (#000000 == 0% sat and 100% val, #FFFFFF == 100% sat and 0% val)
        public (double, double) generateReflectance()
        {
            double baseReflectance = (stability + utils.normalize(density, 1, 25)) / 2.0;
            baseReflectance -= 0.5;
            return (baseReflectance, baseReflectance * -1); //when signs are opposite, they travel diagonally in the color space
        }

        public string generateColor()
        {
            //the base color ranges from rgb(150,150,150), or hsv(0,0,59) to hsv(40,60,59) depending on structure
            (double h, double s, double v) baseColor;

            double baseHue = utils.inverseNormalize(structure.average, 0, 40); //yields the same value as rescale(structure, (0.0, 1.0), (0.0, 40.0);
            double baseSaturation = utils.inverseNormalize(structure.average, 0, 60) / 100.0;
            double baseValue = 0.59;

            //complexity: scales H in any particular direction between 0 and 360
                //The complexity, absolute value of the reactivity/residuals, and a random number between 1 and the number of elements are clamped between 1 and 12
                //this number is then rescaled to fit between 0 and 360, corresponding to the hue
                //12 is the maximum complexity (and also the maximum geometric structure of an element)
            double complexityMod = utils.rescale(
                utils.Clamp(complexity + (Math.Abs(reactivity) / utils.getRandomDouble(1.0, numElements)), 1.0, 12.0),
                (1.0, 12.0),
                (0, 360)
                );

            //atom sum: scales S value in positively between 0 and 40 -- base value if already 60 at max
            //issue: the theoretical maximum number of atoms is 3,333.
            //       though the low catenation rate of high covalence elements
            //       makes this extremely unlikely, it is technically possible.
            //       Therefore, in order to avoid that headache, it's just capped at 15.
            //
            //       15 can easily be exceeded by a mineral with 3+ highly catenating
            //       elements, but their gravity, and the bond cascading tends to limit them to 5-8
            int totalAtoms = utils.Sum(atoms.Select(e => e.Value).ToArray());
            int atomsCapped = (totalAtoms > 15) ? 15 : totalAtoms;

            double saturationMod = utils.rescale(atomsCapped, (1, 15), (0, 40)) / 100;

            //net density / 25 : scales the V value positively between 0 and 41 -- base value is 59 at max
            double valueMod = utils.rescale(density, (3, 25), (0, 41)) / 100;
            
            //apply and adjust
            baseColor.h = baseHue + complexityMod;
            baseColor.s = baseSaturation + saturationMod;
            baseColor.v = baseValue + valueMod;

            //add in key value
            (double h, double s, double v) keyedColor;
            keyedColor.h = baseColor.h;
            keyedColor.s = utils.Clamp(baseColor.s + reflectance.satMod, 0.0, 1.0);
            keyedColor.v = utils.Clamp(baseColor.v + reflectance.valMod, 0.0, 1.0);

            (int r,int g,int b) rgb = ColorUtils.HsvToRgb(baseColor.h,baseColor.s,baseColor.v);
            (int r, int g, int b) rgbKeyed = ColorUtils.HsvToRgb(keyedColor.h, keyedColor.s, keyedColor.v);

            /*
            Console.WriteLine("Struct: " + structure);
            Console.WriteLine("Complexity: " + complexity);
            Console.WriteLine("HSV: (" + baseColor.h + ", " + baseColor.s + ", " + baseColor.v + ")");
            Console.WriteLine("RGB: (" + rgb.r + ", " + rgb.g + ", " + rgb.b + ")");
            Console.WriteLine("Hex: " + ColorUtils.rgbToHex(rgb.r, rgb.g, rgb.b));
            Console.Write("Mineral Color (org):");
            Console.WriteLine("                        ".PastelBg(ColorUtils.rgbToHex(rgb.r, rgb.g, rgb.b)));
            Console.Write("Mineral Color (mod):");
            Console.WriteLine("                        ".PastelBg(ColorUtils.rgbToHex(rgbKeyed.r, rgbKeyed.g, rgbKeyed.b)));
            */

            return ColorUtils.rgbToHex(rgbKeyed.r, rgbKeyed.g, rgbKeyed.b);

        }

        //tl;dr how shiny it is. This differs from reflectance
        //in that reflectance refers to how much of the color
        //is being reflected as opposed to how much light
        //luster is not affected by the surface tendencies
        public double generateLuster()
        {

            double baseLuster = 1 - structure.weighted;
            double lusterMod = (structure.weighted < 0.25) //problem here
                ? utils.getRandomDouble(0.0, 0.35) * -1
                : utils.getRandomDouble(0.0, 0.15);

            double luster = baseLuster + lusterMod;

            //high complexity minerals are also lustrous
            //e.g. graphite
            if (baseLuster + lusterMod < 0.5 && complexity >= 6)
            {
                luster += utils.getRandomDouble(0.2, 0.45);
            }
            return luster;

        }

        public double generateStability()
        {
            double baseStability = (pliance * 1.2);
            double symmetryMod = (complexity % 2 == 0) ? utils.getRandomDouble(-0.2, -0.1) : utils.getRandomDouble(0.1, 0.2);

            return utils.Clamp(baseStability + symmetryMod, 0.0, 1.0);

        }

        //note: -maximum reflectance is 0.5
        //      -maximum luster is 1.0
        public double generateIridescence()
        {
            double baseIridescence = 0;
            if (complexity <= 2 || complexity >= 6)
            {
                baseIridescence = utils.Clamp(((Math.Abs(reflectance.valMod) + luster) / 2.0), 0.0, 0.75);
                double iridMod = (baseIridescence > 0.5)
                    ? utils.getRandomDouble(0.0, 0.45) * -1
                    : utils.getRandomDouble(0.0, 0.15);

                return (baseIridescence + iridMod);

            }
            return baseIridescence;
        }
        public (int,string) generateClassifcation()
        {
            //metal
            if (utils.Between(structure.average, 0.0, 0.34, true)){
                return (1, Dictionaries.mineralClassifications[1]);
            }

            //metalloid
            else if (utils.Between(structure.average, .34, 0.67, true))
            {
                return (2, Dictionaries.mineralClassifications[2]);
            }

            //non-metal
            else
            {
                return (3, Dictionaries.mineralClassifications[3]);
            }

        }

    }
}
