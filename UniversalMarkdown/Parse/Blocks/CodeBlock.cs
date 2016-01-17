﻿// Copyright (c) 2016 Quinn Damerell
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalMarkdown.Helpers;

namespace UniversalMarkdown.Parse.Elements
{
    public class CodeBlock : MarkdownBlock
    {
        /// <summary>
        /// The source code to display.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Initializes a new code block.
        /// </summary>
        public CodeBlock() : base(MarkdownBlockType.Code)
        {
        }

        /// <summary>
        /// Parses a code block.
        /// </summary>
        /// <param name="markdown"> The markdown text. </param>
        /// <param name="start"> The location of the first character in the block. </param>
        /// <param name="maxEnd"> The location to stop parsing. </param>
        /// <param name="actualEnd"> Set to the end of the block when the return value is non-null. </param>
        /// <returns> A parsed code block, or <c>null</c> if this is not a code block. </returns>
        internal static CodeBlock Parse(string markdown, int start, int maxEnd, out int actualEnd)
        {
            int startOfLine = start;
            StringBuilder code = null;
            while (startOfLine < maxEnd)
            {
                // Add every line that starts with a tab character or at least 4 spaces.
                int pos = startOfLine;
                if (pos < maxEnd && markdown[pos] == '\t')
                    pos++;
                else
                {
                    int spaceCount = 0;
                    while (pos < maxEnd && spaceCount < 4)
                    {
                        if (markdown[pos] == ' ')
                            spaceCount++;
                        else if (markdown[pos] == '\t')
                            spaceCount += 4;
                        else
                            break;
                        pos++;
                    }
                    if (spaceCount < 4)
                    {
                        // We found a line that doesn't start with a tab or 4 spaces.
                        // But don't end the code block until we find a non-blank line.
                        int pos2 = pos;
                        bool foundNonSpaceChar = false;
                        while (pos2 < maxEnd)
                        {
                            char c = markdown[pos2];
                            if (c == '\n')
                                break;
                            else if (!Common.IsWhiteSpace(c))
                            {
                                foundNonSpaceChar = true;
                                break;
                            }
                            pos2++;
                        }
                        if (foundNonSpaceChar)
                            break;
                    }
                }

                // Find the end of the line.
                int endOfLine = Common.FindNextSingleNewLine(markdown, pos, maxEnd, out startOfLine);

                // Separate each line of the code text.
                if (code == null)
                    code = new StringBuilder();
                else
                    code.AppendLine();

                // Append the code text, excluding the first tab/4 spaces, and convert tab characters into spaces.
                string lineText = markdown.Substring(pos, endOfLine - pos);
                int startOfLinePos = code.Length;
                for (int i = 0; i < lineText.Length; i++)
                {
                    char c = lineText[i];
                    if (c == '\t')
                        code.Append(' ', 4 - ((code.Length - startOfLinePos) % 4));
                    else
                        code.Append(c);
                }
            }

            if (code == null)
            {
                // Not a valid code block.
                actualEnd = start;
                return null;
            }

            // Blank lines should be trimmed from the start and end.
            actualEnd = startOfLine;
            return new CodeBlock() { Text = code.ToString().Trim('\r', '\n') };
        }

        /// <summary>
        /// Converts the object into it's textual representation.
        /// </summary>
        /// <returns> The textual representation of this object. </returns>
        public override string ToString()
        {
            return Text;
        }
    }
}
