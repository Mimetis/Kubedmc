using System;

namespace KubeDmc
{
    public static class ConsoleExt
    {
        /// <summary>
        /// Delete a line. 
        /// If cursorLeft is set, delete the right part of the line. 
        /// If cursorTop is set, delete the line at selected row
        /// </summary>
        public static void DeleteLine(int? cursorTop = null, int? cursorLeft = null, bool replaceCursor = false)
        {
            // Remember cursor position before cleaning
            (int _top, int _left) = (Console.CursorTop, Console.CursorLeft);

            Console.CursorTop = cursorTop ?? _top;
            Console.CursorLeft = cursorLeft ?? 0;

            Console.Write(new string(' ', Console.BufferWidth - Console.CursorLeft));

            // Replace cursor position
            if (replaceCursor)
                (Console.CursorTop, Console.CursorLeft) = (_top, _left);
        }

        /// <summary>
        /// Delete the right part of the current line
        /// </summary>
        public static void DeleteEndOfLine()
        {
            DeleteLine(Console.CursorTop, Console.CursorLeft, true);
        }



    }
}
