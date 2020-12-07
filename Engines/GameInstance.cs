using Bolder_Blacksmith.Engines;
using Bolder_Blacksmith.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bolder_Blacksmith
{
    public class GameInstance
    {
        
        //returns the pressure resistance modifier given the number of atmospheres
        //This will move somewhere else eventually. Although changing the pressure
        //will occur within a game instance, it'll be via a tool, as base transitional
        //points are not stored when modified.
        //In other words, if you apply a pressure change, it will only calculate it
        //at the moment of smelting. For e.g., if "Oven 1" does not have a pressure
        //change option, then it will not calculate. If "Oven 2" does, it will.
        //But it is entirely dependent on the tool. Those have not been written yet.
        public static double getPressureCalc(int numAtmospheres)
        {
            if (numAtmospheres < GameConstants.MINIMUM_PRESSURE || numAtmospheres > GameConstants.MAXIMUM_PRESSURE + 1)
            {
                throw new Exception("Atmospheric conditions be outside of min/max bounds.");

            }
            else if (numAtmospheres < 0)
            {
                return 1 - utils.normalize(Math.Abs(numAtmospheres), 0, GameConstants.MAXIMUM_PRESSURE + GameConstants.PRESSURE_DAMPENER);
            }
            else if (numAtmospheres > 0)
            {
                return 1 + utils.normalize(numAtmospheres, 0, GameConstants.MAXIMUM_PRESSURE + GameConstants.PRESSURE_DAMPENER);
            }
            else
            {
                return 0;
            }

        }

        //return melting and boiling point with pressure changes applied. NOTE that
        //new values DO NOT modify the original element.
        public static(double, double) getPressuredTransitionalPoints(Element e, double pressureCalc)
        {
            //string[] acceptedTypes = {"Element","Isotope","Mineral","Rock","Ingot"};

            double mp = e.transitionalPoints.meltingPoint;
            double bp = e.transitionalPoints.boilingPoint;
            double pr = e.pressureRes;
            double pt = pressureCalc;
            double newmp = 0;
            double newbp = 0;

            if (pt == 0)
            {
                newmp = mp;
                newbp = bp;
            }
            if (pr < 1 && pt != 0)
            {
                newmp = mp * (pr / pt);
                newbp = bp * (pr / pt);
            }
            if (pr >= 1 && pt != 0)
            {
                newmp = mp * (pr * pt);
                newbp = bp * (pr * pt);
            }
            (double meltingPoint, double boilingPoint) transPoints = (newmp, newbp);
            return transPoints;
        }

    }
}
