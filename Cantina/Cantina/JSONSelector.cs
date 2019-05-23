using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// SimpleJSON is my go-to for anything related to JSON in C#
using SimpleJSON;
using System.IO;
using System.Net;

namespace Cantina
{
    /// <summary>
    /// JSONSelector handles the loading and search of selectors.
    /// </summary>
    class JSONSelector
    {
        private JSONNode m_jsonParent;

        /// <summary>
        /// Default constructor grabs downloaded file
        /// </summary>
        public JSONSelector()
        {
            // Stored locally in the bin, so if this application was pulled, it would still work.
            string fileContent = File.ReadAllText("SystemViewController.json");
            m_jsonParent = JSON.Parse(fileContent);
        }

        /// <summary>
        /// Constructor with URL will grab webpage and store it locally.
        /// </summary>
        /// <param name="a_url"></param>
        public JSONSelector(string a_url)
        {
            WebClient client = new WebClient();
            string targetWebFile = client.DownloadString(a_url);
            m_jsonParent = JSON.Parse(targetWebFile);
        }

        /// <summary>
        /// Prints the JSON. (Debug)
        /// </summary>
        public void PrintJSON()
        {
            Console.WriteLine("** RAW JSON **");
            Console.WriteLine(m_jsonParent.ToString());
        }

        /// <summary>
        /// Will find Views based on the selector string passed in.
        /// Handled Compounds and Chain Selectors
        /// </summary>
        /// <param name="a_selector">The entire string of what the user entered</param>
        /// <returns>Returns and error message if needed.</returns>
        public string FindViews(string a_selector)
        {
            string errorMessage = null;

            // At this moment the only "error" is that the user enters in an empty string.
            if (string.IsNullOrEmpty(a_selector))
            {
                errorMessage = "Please enter in a non-empty string.";
            }
            else
            {
                Console.Clear();

                // First check if there is a selector chain.
                List<string> multiSelector = new List<string>(a_selector.Split(' '));

                // Dictionary is <selector, type> as the selectors are unique and the types are not.
                Dictionary<string, string> selectorWithType = new Dictionary<string, string>();

                for (int i = 0; i < multiSelector.Count; ++i)
                {
                    // First we are checking if this is a compound selector.
                    // Checking for classNames first.
                    int index = multiSelector[i].IndexOf('.', 1);
                    while (index > 0)
                    {
                        string[] split = multiSelector[i].Split('.');

                        multiSelector[i] = split[0];
                        multiSelector.Add("." + split[1]);
                        index = multiSelector[i].IndexOf('.');
                    }

                    // Checking for identifiers second.
                    index = multiSelector[i].IndexOf('#', 1);
                    while (index > 0)
                    {
                        string[] split = multiSelector[i].Split('#');

                        multiSelector[i] = split[0];
                        multiSelector.Add("#" + split[1]);
                        index = multiSelector[i].IndexOf('#');
                    }

                    switch (multiSelector[i][0])
                    {
                        case '.':
                            // Looking for a className
                            selectorWithType[multiSelector[i].Substring(1)] = "classNames";
                            break;
                        case '#':
                            // Looking for a identifier
                            selectorWithType[multiSelector[i].Substring(1)] = "identifier";
                            break;
                        default:
                            // Looking for a class
                            selectorWithType[multiSelector[i]] = "class";
                            break;
                    }
                }

                // To let the user know how their input worked out.
                Console.WriteLine("Searching for: ");

                foreach (KeyValuePair<string, string> kvp in selectorWithType)
                {
                    Console.WriteLine("\t" + kvp.Value + " - " + kvp.Key);
                }

                PrintSelectorRecursive(selectorWithType, m_jsonParent);
            }

            return errorMessage;
        }

        /// <summary>
        /// Recursviely travels through the tree checking each Node to see if it matches all selectors
        /// </summary>
        /// <param name="a_selectorsWithType">Dictionary of (selector, type) to check each node.</param>
        /// <param name="a_node">The current node to search through</param>
        private void PrintSelectorRecursive(Dictionary<string, string> a_selectorsWithType, JSONNode a_node)
        {
            #region SelectorCheck
            // Loops through the dictionary to make sure all selectors are found.
            bool found = true;

            foreach (KeyValuePair<string, string> selectorWithType in a_selectorsWithType)
            {
                if (a_node[selectorWithType.Value] != null)
                {
                    // If this node is an array we need to check each entry
                    if (a_node[selectorWithType.Value].Tag == JSONNodeType.Array)
                    {
                        // Needs a local find for each entry in the array.
                        bool foundLocal = false;
                        for (int i = 0; i < a_node[selectorWithType.Value].Count; ++i)
                        {
                            if (a_node[selectorWithType.Value][i] == selectorWithType.Key)
                            {
                                foundLocal = true;
                                break;
                            }
                        }

                        if (foundLocal)
                        {
                            // Continue means it is found.
                            continue;
                        }
                    }
                    else if (a_node[selectorWithType.Value] == selectorWithType.Key)
                    {
                        // Continue means it is found.
                        continue;
                    }
                }

                // If it reaches here it has failed this selector check and is not found.
                found = false;
                break;
            }

            // If found write out this node as JSON.
            if (found)
            {
                Console.WriteLine();
                Console.WriteLine(a_node.ToString());
            }
            #endregion

            // Going through each child of this object recursively.
            // Ends when there is no more children to be searched.
            for (int i = 0; i < a_node.Count; ++i)
            {
                PrintSelectorRecursive(a_selectorsWithType, a_node[i]);
            }
        }
    }
}
