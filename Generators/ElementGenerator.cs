using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Bolder_Blacksmith.Generators
{
    /* Element generator. Generates elements from initialize() function, which calls all generation methods.
     */
    public class ElementGenerator
    {

        //isotope options
        public bool generatesIsotopes;
        public double isotopeFrequency;

        //critical value options
        public double[] covalenceWeights;

        //all optional!
        public ElementGenerator(double isoFreq = 0.0, double[] covWts =  null )
        {
            generatesIsotopes = (isoFreq == 0.0) ? false : true;
            isotopeFrequency = isoFreq;

            covalenceWeights = covWts ?? new double[0];

            if (!(covalenceWeights.Length == 8 || covalenceWeights.Length == 0))
            {
                covalenceWeights = new double[0];
            }

        }

        #region getters/setters

        public double getIsotopeFrequency()
        {
            return isotopeFrequency;
        }
        public void setIsotopeFrequency(double d)
        {
            if (utils.Between(d, 0.0, 1.0, true))
            {
                if (d == 0.0)
                {
                    generatesIsotopes = false;
                    isotopeFrequency = 0.0;
                }
                else
                {
                    generatesIsotopes = true;
                    isotopeFrequency = d;
                }
            }
        }

        public double [] getWeights()
        {
            return covalenceWeights;
        }

        public void setWeights(double [] w)
        {
            if (w.Length == 8)
            {
                covalenceWeights = w;
            }
        }

        #endregion

        public void printInfo()
        {
            Console.WriteLine("---ELEMENT GENERATOR SETTINGS START---");
            Console.WriteLine("Generates isotopes?: " + generatesIsotopes);
            Console.WriteLine("Isotope Frequency: " + isotopeFrequency);
            Console.WriteLine("Covalence weights: ");
            foreach (double d in covalenceWeights)
            {
                Console.WriteLine(d);
            }
            Console.WriteLine("---ELEMENT GENERATOR SETTINGS END---");
            Console.WriteLine();
        }

        //get one element
        public Element getElement()
        {
            if (generatesIsotopes)
            {
                if (utils.getRandomDouble() < isotopeFrequency)
                {
                    if (covalenceWeights.Length == 8)
                    {
                        return new Isotope(new Element(covalenceWeights));
                    }
                    else
                    {
                        return new Isotope(new Element());
                    }
                }

            }

            if (covalenceWeights.Length == 8)
            {
                return new Element(covalenceWeights);
            }
            else
            {
                return new Element();
            }

        }

        //get a batch of elements
        public Element [] getBatchElements(int numberOfElements)
        {
            Element [] elems = new Element[numberOfElements];
            Element holder;

            for (int i = 0; i < elems.Length; i++)
            {
                holder = getElement();
                elems[i] = holder;
            }

            return elems;
        }

        public Element[] getTestCase(int repeats)
        {
            List<Element> testCase = new List<Element>();
            Element holder;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < repeats; j++)
                {
                    holder = new Element(i + 1);
                    testCase.Add(holder);
                }
            }
            return testCase.ToArray();
        }
    }
}
