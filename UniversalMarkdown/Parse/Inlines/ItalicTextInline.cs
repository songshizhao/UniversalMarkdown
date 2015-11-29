﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalMarkdown.Helpers;

namespace UniversalMarkdown.Parse.Elements
{
    class ItalicTextElement : MarkdownInline
    {
        public ItalicTextElement()
            : base(MarkdownInlineType.Italic)
        { }

        internal override int Parse(ref string markdown, int startingPos, int endingPos)
        {
            int italicStart = Common.IndexOf(ref markdown, '*', startingPos, endingPos);
            // These should always be =
            if(italicStart != startingPos)
            {
                DebuggingReporter.ReportCriticalError("italic parse didn't find * in at the starting pos");
            }
            italicStart++;

            // Find the ending
            int italicEnd = Common.IndexOf(ref markdown, '*', italicStart, endingPos, true);
            if (italicEnd + 1 != endingPos)
            {
                DebuggingReporter.ReportCriticalError("italic parse didn't find * in at the end pos");
            }

            // Make sure there is something to parse, and not just dead space
            if (italicEnd > italicStart)
            {
                // Parse any children of this bold element
                ParseInlineChildren(ref markdown, italicStart, italicEnd);
            }

            // Return the point after the *
            return italicEnd + 1;
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
            // Test for italic
            // Note this will find a bold tag also, but due to this check (tempPos < currentClosesPos) it
            // will not enter.
            int italicStartingPos = Common.IndexOf(ref markdown, '*', startingPos, endingPos);
            if (italicStartingPos != -1 && italicStartingPos < currentNextElementStart && markdown.Length > italicStartingPos + 1)
            {
                // We might have one, try to find the ending that is in the current endingPos
                // We need to loop and keep looking for * not followed by another * (bold)
                int italicEndPos = Common.IndexOf(ref markdown, '*', italicStartingPos + 1, endingPos);
                while (italicEndPos != -1 && italicEndPos + 1 < markdown.Length && markdown[italicEndPos + 1] == '*')
                {
                    italicEndPos = Common.IndexOf(ref markdown, '*', italicEndPos + 2, endingPos);
                }

                // If we found it and it is the next closest ending pos use it!
                if (italicEndPos != -1)
                {
                    currentNextElementStart = italicStartingPos;
                    elementEndingPos = italicEndPos + 1;
                    return true;
                }
            }
            return false;
        }
    }
}