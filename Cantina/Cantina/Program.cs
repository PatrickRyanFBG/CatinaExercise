using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace Cantina
{
    class Program
    {
        static void Main(string[] args)
        {
            // Can grab a file from a URL. Note for github content this is the link to the RAW file.
            //JSONSelector selector = new JSONSelector("https://raw.githubusercontent.com/jdolan/quetoo/master/src/cgame/default/ui/settings/SystemViewController.json");

            // The default constructor uses the file I downloaded so I don't have to ping github constantly while developing this application.
            JSONSelector selector = new JSONSelector();

            // Debug checking JSON.
            //selector.PrintJSON();

            string selectorResponse = null;

            while(true)
            {
                if (!string.IsNullOrEmpty(selectorResponse))
                {
                    Console.Clear();
                    Console.WriteLine("*ERROR* " + selectorResponse + " *ERROR*");
                }
                else
                {
                    Console.WriteLine();
                }

                Console.WriteLine("Please enter in a selector. Type exit to close the program.");
                Console.Write("Input: ");
                string userInput = Console.ReadLine();

                if (userInput.ToLower() == "exit")
                {
                    break;
                }

                selectorResponse = selector.FindViews(userInput);
            }
        }
    }
}
