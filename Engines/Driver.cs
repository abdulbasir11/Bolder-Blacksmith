﻿using Bolder_Blacksmith.Engines;
using Bolder_Blacksmith.Generators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bolder_Blacksmith
{
    class Driver
    {
        public static void Main()
        {
            //ElementGenerator elemGen = new ElementGenerator();
            MineralGenerator minGen = new MineralGenerator();
            while (true)
            {
                Mineral min = minGen.getMineral();
                min.printInfo();
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

            /*
            while (true)
            {
                //three of each
                Element[] elems = elemGen.getTestCase(3);

                for (int i = 0; i < elems.Length; i++)
                {
                    Console.WriteLine("Element " + (i + 1));
                    elems[i].printInfo();
                }

                int randomElem = utils.getRandomInt(0, 23); //spit out a random element's breakdown
                for (int i = GameConstants.MINIMUM_PRESSURE; i < GameConstants.MAXIMUM_PRESSURE + GameConstants.PRESSURE_DAMPENER; i++)
                {

                    Console.WriteLine("Normalized pressure change at " + i + ": " + GameInstance.getPressureCalc(i));

                    (double, double) newPoints = GameInstance.getPressuredTransitionalPoints(elems[randomElem], GameInstance.getPressureCalc(i));

                    Console.WriteLine("Element " + (randomElem + 1) + "s melting point at " + i + " change in atmospheres: (" + newPoints.Item1 + "," + newPoints.Item2 + ")");
                    Console.WriteLine("...");
                }

                Console.WriteLine("Making an isotope from element " + (randomElem+1) + ":");
                Isotope iso = new Isotope(elems[randomElem]);
                iso.printInfo();


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
            */
        }
    }
}
