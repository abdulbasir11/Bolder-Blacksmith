using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bolder_Blacksmith.Generators
{
    /* Element generator. Generates elements from initialize() function, which calls all generation methods.
     */
    public class ElementGenerator
    {
        //get one element
        public Element getElement()
        {

            Element elem = new Element();
            elem.initialize();

            return elem;
        }

        //get a batch of elements
        public Element [] getBatchElements(int numberOfElements)
        {
            Element [] elems = new Element[numberOfElements];
            Element holder;

            for (int i = 0; i < elems.Length; i++)
            {
                holder = new Element();
                holder.initialize();
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
                    holder = new Element();
                    holder.initializeTestCase(i + 1);
                    testCase.Add(holder);
                }
            }
            return testCase.ToArray();
        }
    }
}
