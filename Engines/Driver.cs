using Bolder_Blacksmith.Generators;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bolder_Blacksmith
{
    class Driver
    {
        public static void Main()
        {

            ElementGenerator elemGen = new ElementGenerator();

            while (true)
            {
                Element[] elems = elemGen.getTestCase(3);

                for (int i = 0; i < elems.Length; i++)
                {
                        Console.WriteLine("Element " + (i + 1));
                        Console.WriteLine("Normal Covalence: " + elems[i].covalence.normalized);
                        Console.WriteLine("Integer Covalence: " + elems[i].covalence.original);
                        Console.WriteLine("...");
                        Console.WriteLine("Catenation Rate: " + elems[i].catenationRate);
                        Console.WriteLine("...");
                        Console.WriteLine("Base Structure: " + elems[i].baseStructure.asString);
                        Console.WriteLine("...");
                        Console.WriteLine("Original Deformation: " + elems[i].deformation.original);
                        Console.WriteLine("Normal Deformation: " + elems[i].deformation.normalized);
                        Console.WriteLine("...");
                        Console.WriteLine("Original Density: " + elems[i].density.original);
                        Console.WriteLine("Normal Density: " + elems[i].density.normalized);
                        Console.WriteLine("...");
                        Console.WriteLine("Geometric Structure: " + elems[i].geometricStructure.asString);
                        Console.WriteLine("Geometric Structure -gons: " + elems[i].geometricStructure.asInt);
                        Console.WriteLine("...");
                        Console.WriteLine("Hardness Original: " + elems[i].hardness.original);
                        Console.WriteLine("Hardness Normalized: " + elems[i].hardness.normalized);
                        Console.WriteLine("...");
                        Console.WriteLine("Pliance: " + elems[i].pliance);
                        Console.WriteLine("...");
                        Console.WriteLine("Gravity: " + elems[i].gravity);
                        Console.WriteLine("...");
                        Console.WriteLine("Melting Resistance: " + elems[i].heatRes.meltingRes);
                        Console.WriteLine("Boiling Resistance: " + elems[i].heatRes.boilingRes);
                        Console.WriteLine("...");
                        Console.WriteLine("Melting Point: "+elems[i].transitionalPoints.meltingPoint);
                        Console.WriteLine("Boiling Point: " + elems[i].transitionalPoints.boilingPoint);
                        Console.WriteLine("---------------------------------------");
                }

                Console.WriteLine("Again?");
                string again = Console.ReadLine();
                if (again.Equals("y"))
                {
                    continue;
                }
                else
                {
                    break;
                }

            }

        }

    }
}
