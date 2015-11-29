﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalMarkdown.Helpers;

namespace UniversalMarkdown.Parse.Elements
{
    class BoldTextElement : MarkdownInline
    {
        public BoldTextElement()
            : base(MarkdownInlineType.Bold)
        { }

        internal override int Parse(ref string markdown, int startingPos, int endingPos)
        {
            int boldStart = Common.IndexOf(ref markdown, "**", startingPos, endingPos);
            // These should always be =
            if(boldStart != startingPos)
            {
                DebuggingReporter.ReportCriticalError("bold parse didn't find ** in at the starting pos");
            }
            boldStart += 2;

            // Find the ending
            int boldEnding = Common.IndexOf(ref markdown, "**", boldStart, endingPos, true);
            if (boldEnding + 2 != endingPos)
            {
                DebuggingReporter.ReportCriticalError("bold parse didn't find ** in at the end pos");
            }

            // Make sure there is something to parse, and not just dead space
            if (boldEnding > boldStart)
            {
                // Parse any children of this bold element
                ParseInlineChildren(ref markdown, boldStart, boldEnding);
            }

            // Return the point after the **
            return boldEnding + 2;
        }

        /// <summary>
        /// Attempts to find a element in the range given. If an element is found we must check if the starting is less than currentNextElementStart,
        /// and if so update that value to be the start and update the elementEndPos to be the end of the element. These two vales will be passed back to us
        /// when we are asked to parse. We then return true or false to indicate if we are the new candidate. 
        /// </summary>
        /// <param name="markdown">mark down to parse</param>
        /// <param name="currentPos">the starting point to search</param>
        /// <param name="maxEndingPos">the ending point to search</param>
        /// <param name="elementStartingPos">the current starting element, if this element is < we will update this to be our starting pos</param>
        /// <param name="elementEndingPos">The ending pos of this element if it is interesting.</param>
        /// <returns>true if we are the next element candidate, false otherwise.</returns>
        public static bool FindNextClosest(ref string markdown, int startingPos, int endingPos, ref int currentNextElementStart, ref int elementEndingPos)
        {
            // Test for bold
            int boldStartingPos = Common.IndexOf(ref markdown, "**", startingPos, endingPos);
            if (boldStartingPos != -1 && boldStartingPos < currentNextElementStart && markdown.Length > boldStartingPos + 2)
            {
                // We might have one, try to find the ending that is in the current endingPos
                int boldEndingPos = Common.IndexOf(ref markdown, "**", boldStartingPos + 2, endingPos);

                // If we found it and it is the next closest ending pos use it!
                if (boldEndingPos != -1)
                {
                    currentNextElementStart = boldStartingPos;
                    elementEndingPos = boldEndingPos + 2;
                    return true;
                }
            }
            return false;
        }
    }
}