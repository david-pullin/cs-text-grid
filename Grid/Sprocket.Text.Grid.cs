using System;
using System.Text;

/*
*   Sprocket.Text.Grid represents a 2 dimensional grid where text can be written to with word wrapping, truncation, justification 
*   as you would find in a text editor.
*
*   The main method to write to the Grid is Write() were wrapping, truncation options are controlled by passing an instance
*   of Sprocket.Text.GridWriteOptions
*
*   See GridWriteOptions.EnableWordSpacing (default=true) on why text written may be prefixed by a space.
*
*/

namespace Sprocket.Text.Grid
{
    /// <summary>
    /// Represents a 2 dimensional grid with methods to write text. Writing text includes options for justification,
    /// alignment, direction (orientation) and word wrapping.
    /// For an example of how this class can be used to write text into ASCII art 
    /// see the example in <see cref="FromContent"/>
    /// </summary>
    public class Grid
    {
        /// <summary>
        /// Valid values for the map character array
        /// </summary>
        private enum MapCharacters
        {

            /// <summary>Is use/used</summary>
            WrittenTo = 'W',

            /// <summary>Free and allowed to be written to</summary>
            Free = '.',

            /// <summary>Special internal character used to pass around existing text to ensure spacing</summary>
            TempPadding = 'w'
        }

        /// <summary>
        /// Internal flag to assist with positioning the cursor (current X,Y and Index position) when the
        /// first Write takes place.  This is to assist with RTL direction where simple default starting point of 0,0 
        /// will not suffice as we need to start from the right most column.
        /// </summary>
        private bool _firstWriteSetCursorPositionDone = false;

        /// <summary>
        /// The X co-ordinate of the current cursor position where the next call to Write will commence from.
        /// X and Y co-ordinates start at the "Top/Left" of our 2 dimensional grid if plotted visually.
        /// </summary>
        /// <value>0 to number of columns - 1.</value>
        public int CursorX
        {
            get
            {
                return this._cursorX;
            }
            set
            {
                if (value >= 0 && value < this.Width)
                {
                    this._cursorX = value;

                    if (!_firstWriteSetCursorPositionDone)
                    {
                        _firstWriteSetCursorPositionDone = true;
                    }

                    this._cursorIdx = this.CalcIndexFromXY(this._cursorX, this._cursorY);
                }
            }
        }

        /// <summary>
        /// The Y co-ordinate of the current cursor position where the next call to Write will commence from.
        /// X and Y co-ordinates start at the "Top/Left" of our 2 dimensional grid if plotted visually.
        /// </summary>
        /// <value>0 to number of rows - 1.</value>
        public int CursorY
        {
            get
            {
                return this._cursorY;
            }
            set
            {
                if (value >= 0 && value < this.Height)
                {
                    this._cursorY = value;

                    if (!_firstWriteSetCursorPositionDone)
                    {
                        _firstWriteSetCursorPositionDone = true;
                    }

                    this._cursorIdx = this.CalcIndexFromXY(this._cursorX, this._cursorY);
                }
            }
        }


        /// <summary>
        /// Internal results used to determine the result of the Write operation
        /// </summary>
        private enum WriteAtCursorResult
        {
            /// <summary>Text did not completely fit and was truncated (if allowed).</summary>
            NotFittedTextTruncated = 0,

            /// <summary>Text did fit and cursor position is now at the next character ready for the next Write.</summary>
            FittedAndCursorMovedOn = 1,

            /// <summary>Text did fit but it came right up to the end of the map.  As a result the cusor position was not moved on (ready for the next Write).</summary>
            FittedAndEndOfMap = 2
        }

        /// <summary>
        /// Array of characters representing the Grid's map.  Value values of each element should match entries in the <see cref="MapCharacters"/> enum.
        /// </summary>
        private char[] _map;

        /// <summary>
        /// Array of characters representing the Grid's content.
        /// </summary>
        private char[] _content;

        /// <summary>
        /// The width, number of columns, in the grid.
        /// </summary>
        /// <value>Number of columns/characters per row.</value>
        public int Width { get; init; }

        /// <summary>
        /// The height, number of rows, in the grid.
        /// </summary>
        /// <value>Number of rows/characters per column.</value>
        public int Height { get; init; }


        /// <summary>
        /// The total number of cells (characters) in the grid.  The Length is always Width * Height.
        /// </summary>
        /// <value>Number of cells in the grid.</value>
        public int Length { get; init; }

        /// <summary>
        /// Current X cursor position.  Values are from 0 to number of columns - 1 (Length - 1).
        /// </summary>
        private int _cursorX = 0;

        /// <summary>
        /// Current Y cursor position.  Values are from 0 to number rows - 1 (Height - 1).
        /// </summary>
        private int _cursorY = 0;

        /// <summary>
        /// Current index position in the <see cref="_map"/> and <see cref="_content"/> arrays
        /// that represents to position as specified by <see cref="_cursorX"/> and <see cref="_cursorY"/>.
        /// See also <see cref="CalcIndexFromXY"/>.
        /// </summary>
        private int _cursorIdx = 0;

        /// <summary>
        /// This enum is used when additing the temporary padding characters.  Where the
        /// characters are to be added depends on the main writing direction (i.e. is the Writing direction Rows or Columns)
        /// </summary>
        private enum PaddingDirection
        {
            Rows = 0,
            Cols = 1,
            RowsAndCols = 2
        }


        #region "Constructors"

        /// <summary>
        /// Initializes a new instance of <see cref="Grid"/>.
        /// </summary>
        /// <param name="width">Number of columns for the grid.</param>
        /// <param name="height">Number of rows for the grid.</param>
        public Grid(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.Length = width * height;
            this._map = new string((char)MapCharacters.Free, this.Length).ToCharArray();
            this._content = new string(' ', this.Length).ToCharArray();
        }

        /// <summary>
        /// Creates a grid based upon a "map".  The map is a string, with optional whitespace and carriage returns.
        /// A '|' character is used to signify the left and right bounds of the grid.
        /// A '.' or ' ' (space) character signifies an free empty cell.
        /// Any other character will be set in the grid's map as a cell that is not to be written to.
        /// </summary>
        /// <param name="map">A string containing a "map" to based this new grid upon</param>
        /// <exception cref="Exception">Thrown if the widths of the rows in the map are unequal.</exception>
        /// <returns>An instance of <see cref="Grid"/>.</returns>
        /// <example>
        /// <code>
        /// Grid m = Grid.FromMap(@"
        ///         |.........|
        ///         |.*****...|
        ///         |........*|
        ///         |.........|
        ///     ");
        /// g.Write("Hello World");
        /// Console.WriteLine(g.Content());
        /// </code>
        /// In this example, the * character in the map indicates that the cell should not be written to.
        /// The word 'hello' fits on the first row.  The word 'world' would be truncated on the first row
        /// and cannot fit on the second row. There is, however, room on the third row.
        /// <code>
        ///    Hello
        /// 
        ///    World
        /// </code>
        /// </example>
        public static Grid FromMap(string map)
        {
            string[] lines = Grid.SuperTrim(map, "~/~").Split("~/~");

            int lineCount = 0;
            int mapWidth = 0;
            StringBuilder mapCharacters = new(map.Length);

            foreach (string line in lines)
            {
                string l = line;

                if (l.Length > 0)
                {
                    // remove any leading and trailing | from line
                    if (l.StartsWith('|'))
                    {
                        l = l.Substring(1) ?? "";
                        if (0 == l.Length)
                        {
                            continue;
                        }
                    }

                    if (l.EndsWith('|'))
                    {
                        l = l.Substring(0, l.Length - 1);
                        if (0 == l.Length)
                        {
                            continue;
                        }
                    }

                    // the first list determines the width of our map
                    if (0 == lineCount)
                    {
                        mapWidth = l.Length;
                    }
                    else if (l.Length != mapWidth)
                    {
                        throw new Exception("Unequal Widths in Map.  All rows must have the same width");
                    }

                    mapCharacters.Append(l.Replace(' ', (char)Grid.MapCharacters.Free));
                    lineCount++;
                }
            }

            Grid g = new(mapWidth, lineCount);
            g._map = mapCharacters.ToString().ToCharArray();
            g._content = new string(' ', mapWidth * lineCount).ToCharArray();

            return g;
        }

        /// <summary>
        /// Creates a grid based upon contents of a string.  the string can contain optional whitespace and carriage returns.
        /// A '|' character is used to signify the left and right bounds of the grid.
        /// </summary>
        /// <param name="content">A string containing a "map" to based this new grid upon</param>
        /// <param name="writeAreaMarker">Optional.  By default spaces denote cells where text is allow to be written to.
        /// These will be converted to spaces when the grid is populated and used by the grid's internal
        /// map to indicate a writable cell.
        /// </param>
        /// <returns>An instance of <see cref="Grid"/>.</returns>
        /// <exception cref="Exception">Thrown if the widths of the rows in the contents are unequal.</exception>
        /// <example>
        /// The example below will create a grid with 2 rows and 14 columns with contents as displayed:
        /// <code>
        /// Grid m = Grid.FromContent(@"
        ///         |Hello         |
        ///         |World         |
        ///         ");
        /// </code>
        /// </example>
        /// <example>
        /// The example below uses a specified writeAreaMarker character
        /// <code>
        /// Grid m = Grid.FromContent(@"
        ///             |Hello         |
        ///             |      $$$$$$$ |
        ///             ", '$');
        /// m.Write("World");
        /// Console.WriteLine(m.Content());
        /// </code>
        /// Expected output:
        /// <code>
        /// Hello
        ///       World
        /// </code>
        /// This example sets the content to a ASCII shaped heart and which cells can be written to.
        /// <code>
        /// Grid g = Grid.FromContent(@"
        ///        |     ,@,@,@,@,@,   ,@,@,@,@,@,     |
        ///        |   ,@!         @, ,@         !@,   |
        ///        | ,@!            @,@            !@, |
        ///        |,@!              @              !@,|
        ///        |,@!                             !@,|
        ///        |,@!  -------------------------  !@,|
        ///        | '@! ------------------------- !@' |
        ///        |  '@!  ---------------------   @'  |
        ///        |   '@! --------------------- !@'   |
        ///        |     '@!  ---------------- !@'     |
        ///        |       '@!   ----------  !@'       |
        ///        |         '@!           !@'         |
        ///        |           '@!       !@'           |
        ///        |             '@!   !@'             |
        ///        |               '@!@'               |
        ///        |                 @                 |
        ///     ", '-');
        ///GridWriteOptions opts = new();
        ///opts.Justification = GridWriteOptions.TextJustification.Center;
        ///g.Write("To my one and only and amazing friend\nI will love you forever", opts);
        ///Console.WriteLine(g.Content());
        /// </code>
        /// Expected Output
        /// <code>
        ///      ,@,@,@,@,@,   ,@,@,@,@,@,
        ///    ,@!         @, ,@         !@,
        ///  ,@!            @,@            !@,
        /// ,@!              @              !@,
        /// ,@!                             !@,
        /// ,@!   To my one and only and    !@,
        ///  '@!      amazing friend       !@'
        ///   '@!     I will love you      @'
        ///    '@!        forever        !@'
        ///      '@!                   !@'
        ///        '@!               !@'
        ///          '@!           !@'
        ///            '@!       !@'
        ///              '@!   !@'
        ///                '@!@'
        ///                  @
        /// </code>
        /// </example>
        public static Grid FromContent(string content, char writeAreaMarker = ' ')
        {
            string[] lines = Grid.SuperTrim(content, "~/~").Split("~/~");

            int lineCount = 0;
            int mapWidth = 0;
            StringBuilder contentCharacters = new(content.Length);

            foreach (string line in lines)
            {
                string l = line;

                if (l.Length > 0)
                {
                    // remove any leading and trailing | from line
                    if (l.StartsWith('|'))
                    {
                        l = l.Substring(1) ?? "";
                        if (0 == l.Length)
                        {
                            continue;
                        }
                    }

                    if (l.EndsWith('|'))
                    {
                        l = l.Substring(0, l.Length - 1);
                        if (0 == l.Length)
                        {
                            continue;
                        }
                    }

                    // the first list determines the width of our map
                    if (0 == lineCount)
                    {
                        mapWidth = l.Length;
                    }
                    else if (l.Length != mapWidth)
                    {
                        throw new Exception("Unequal Widths in Content.  All rows must have the same width");
                    }

                    contentCharacters.Append(l);
                    lineCount++;
                }
            }

            Grid g = new(mapWidth, lineCount);
            g._content = contentCharacters.ToString().ToCharArray();


            // populate map
            StringBuilder mapCharacters = new(content.Length);
            foreach (char c in g._content)
            {
                if (c == writeAreaMarker)
                {
                    mapCharacters.Append((char)Grid.MapCharacters.Free);
                }
                else
                {
                    mapCharacters.Append((char)Grid.MapCharacters.WrittenTo);
                }
            }
            g._map = mapCharacters.ToString().ToCharArray();

            if (writeAreaMarker != ' ')
            {
                g._content = contentCharacters.ToString().Replace(writeAreaMarker, ' ').ToCharArray();
            }

            return g;
        }


        #endregion

        #region "Public Methods"

        #region "Plot..."

        /// <summary>
        /// Sets a cell's content (single character) at the specified co-ordinates.
        /// This method does not enforce any spacing checks, special character processes, alignments, etc.
        /// It does not affect the current writing cursor position and will overwrite any existing cell's content.
        /// </summary>
        /// <param name="x">Column position from 0 to Width-1.</param>
        /// <param name="y">Row position from 0 to Height-1.</param>
        /// <param name="c">Character to plot</param>
        /// <exception cref="IndexOutOfRangeException">Thrown if x or y are out of bounds.</exception>
        public void RawPlot(int x, int y, char c)
        {
            if (x < 0 || x >= this.Width)
            {
                throw new IndexOutOfRangeException($"Value of {nameof(x)} ({x}) out of range.");
            }

            if (y < 0 || y >= this.Height)
            {
                throw new IndexOutOfRangeException($"Value of {nameof(y)} ({y}) out of range.");
            }

            int idx = CalcIndexFromXY(x, y);
            this._content[idx] = c;
            this._map[idx] = (char)MapCharacters.WrittenTo;
        }
        #endregion

        #region "Write..."

        /// <summary>
        /// Writes a string into the grid's content at the current cursor position.
        /// The starting write position is 0,0 (top-left).
        /// See <see cref="GridWriteOptions"/> for the default writing rules/options.
        /// </summary>
        /// <param name="stringToWrite">String to write.</param>
        /// <returns>Will return true if all the characters were written.  If false is returned then 0 or more characters
        /// may have been written but there was no more space in the grid.)
        /// </returns>
        public bool Write(string stringToWrite)
        {
            return Write(stringToWrite, new GridWriteOptions());
        }

        /// <summary>
        /// Writes a string into the grid's content at the current cursor position.
        /// The starting write position is 0,0 (top-left) unless <see cref="GridWriteOptions"/> specify RTL direction
        /// or <see cref="GridWriteOptions.Anchor"/> is set.
        /// </summary>
        /// <param name="stringToWrite">String to write.</param>
        /// <param name="opts">Write options. See <see cref="GridWriteOptions"/>.</param>
        /// <returns>Will return true if all the characters were written.  If false is returned then 0 or more characters
        /// may have been written but there was no more space in the grid.)
        /// </returns>
        public bool Write(string stringToWrite, GridWriteOptions opts)
        {
            switch (opts.Anchor)
            {
                case GridWriteOptions.TextAnchor.None:
                    return this.DoWrite(stringToWrite, opts);

                case GridWriteOptions.TextAnchor.Bottom:

                    // starting form the bottom row and working upwards
                    // keep plotting to a sub-section copy (Extract method) of that
                    // region to see if it fits.  If not, try the row above.
                    int rowCount = 0;
                    while (rowCount < this.Height)
                    {
                        rowCount++;

                        Grid subMap = this.Extract(0, this.Height - rowCount, this.Width, rowCount);

                        if (subMap.DoWrite(stringToWrite, opts))
                        {
                            this.InsertGrid(0, this.Height - rowCount, subMap, true);
                            if (!_firstWriteSetCursorPositionDone)
                            {
                                // prevent the next write changing the cursor position
                                this._firstWriteSetCursorPositionDone = true;
                            }
                            return true;
                        }
                    }

                    return false;

                default:
                    throw new NotImplementedException($"{nameof(opts.Anchor)} of {opts.Anchor} not implemented in WriteText");

            }
        }
        #endregion

        #region "Extract/Insert"
        /// <summary>
        /// Extracts a portion of the grid into a new grid object.
        /// </summary>
        /// <param name="x">X position from 0 to number of columns-1.  X/Y co-ordinates start at 0,0 on the Grid's top left corner. </param>
        /// <param name="y">Y position from 0 to number of rows-1.</param>
        /// <param name="width">Number of columns to extact.</param>
        /// <param name="height">Number of rows to extract.</param>
        /// <returns>New instance of a Grid object</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown the area to be extracted is out of bounds.</exception>
        public Grid Extract(int x, int y, int width, int height)
        {
            if (x < 0 || x >= this.Width)
            {
                throw new IndexOutOfRangeException($"Value of {nameof(x)} ({x}) out of range.");
            }
            else if (width < 0 || x + width > this.Width)
            {
                throw new IndexOutOfRangeException($"Value of {nameof(width)} ({width}) out of range or not enough characters from offset.");
            }
            else if (y < 0 || y >= this.Height)
            {
                throw new IndexOutOfRangeException($"Value of {nameof(y)} ({y}) out of range.");
            }
            else if (height < 0 || y + height > this.Height)
            {
                throw new IndexOutOfRangeException($"Value of {nameof(height)} ({height}) out of range or not enough characters from offset.");
            }

            Grid newMap = new(width, height);

            int sourceRow = y;
            int destMapIndex = 0;
            for (int sourceRowCount = 0; sourceRowCount < height; sourceRowCount++)
            {
                int sourceMapIndex = this.CalcIndexFromXY(x, sourceRow + sourceRowCount);

                for (int sourceColCount = 0; sourceColCount < width; sourceColCount++)
                {
                    newMap._map[destMapIndex] = this._map[sourceMapIndex];
                    newMap._content[destMapIndex] = this._content[sourceMapIndex];
                    sourceMapIndex++;
                    destMapIndex++;
                }
            }

            return newMap;
        }

        /// <summary>
        /// Inserts the contents of an external grid into the current grid.
        /// </summary>
        /// <param name="x">X position from 0 to number of columns-1.  X/Y co-ordinates start at 0,0 on the Grid's top left corner. </param>
        /// <param name="y">Y position from 0 to number of rows-1.</param>
        /// <param name="sourceGrid">The Grid to insert into the current Grid.</param>
        /// <exception cref="IndexOutOfRangeException">Thrown the area to be inserted is out of bounds or there is not enough space in the current Grid.</exception>
        public void Insert(int x, int y, Grid sourceGrid)
        {
            this.InsertGrid(x, y, sourceGrid, false);
        }
        #endregion

        #region "Get..."
        /// <summary>
        /// Returns a single row from the grid.
        /// </summary>
        /// <param name="rowIndex">Which row to extract. Row numbers start from 0.</param>
        /// <returns>String containing the grid content for that row.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown the row index is invalid.</exception>
        public string GetRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= this.Height)
            {
                throw new IndexOutOfRangeException($"{nameof(rowIndex)} of {rowIndex} out of range");
            }

            int mapIndex = CalcIndexFromXY(0, rowIndex);
            StringBuilder s = new(this.Width);
            for (int cnt = 0; cnt < this.Width; cnt++)
            {
                s.Append(this._content[mapIndex]);
                mapIndex++;
            }

            return s.ToString();
        }
        #endregion

        #region "Conversions"

        /// <summary>
        /// Return's the grid's internal Map as an array of strings.
        /// </summary>
        /// <returns>Array of strings.</returns>
        public string[] MapToStringArray()
        {
            int charIdx = 0;
            int resIdx = 0;
            string[] res = new string[this.Height];
            for (int row = 0; row < this.Height; row++, charIdx += this.Width)
            {
                StringBuilder sb = new(this.Width);

                ArraySegment<char> seg = new(this._map, charIdx, this.Width);
                foreach (char c in seg)
                {
                    sb.Append(c);
                }

                res[resIdx++] = sb.ToString();
            }

            return res;
        }

        /// <summary>
        /// Return's the grid's content as an array of string.
        /// </summary>
        /// <returns>Array of strings.</returns>
        public string[] ToStringArray()
        {
            int charIdx = 0;
            int resIdx = 0;
            string[] res = new string[this.Height];
            for (int row = 0; row < this.Height; row++, charIdx += this.Width)
            {
                StringBuilder sb = new(this.Width);

                ArraySegment<char> seg = new(this._content, charIdx, this.Width);
                foreach (char c in seg)
                {
                    sb.Append(c);
                }

                res[resIdx++] = sb.ToString();
            }

            return res;
        }

        /// <summary>
        /// Return's the grid's content as single string.
        /// </summary>
        /// <returns>String containing the grid's content.</returns>
        public override string ToString()
        {
            return _content.ToString() ?? "";
        }

        #endregion // Conversion

        #region "Dump"
        /// <summary>
        /// Returns a single string containing the grids content and map, surrounded by - and | characters, with each row in the 
        /// grid separated by Environment.NewLine.
        /// </summary>
        /// <returns>Formatting string containing contents of the grid's content.</returns>
        public string Dump()
        {
            return Dump(Environment.NewLine);
        }

        /// <summary>
        /// Returns a single string containing the grids content and map, surrounded by - and | characters, with each row in the 
        /// grid separated by the specified character.
        /// </summary>
        /// <param name="rowSeparator">Which character(s) to use to separate each row in the grid.</param>
        /// <returns>Formatted string containing contents of the grid's content.</returns>
        public string Dump(string rowSeparator)
        {
            StringBuilder res = new((Width + 5) * Height);

            string[] content = this.ToStringArray();
            string[] map = this.MapToStringArray();

            res.Append('-', (Width * 2) + 3);
            res.Append(rowSeparator);

            for (int i = 0; i < Height; i++)
            {
                res.Append('|');
                res.Append(content[i]);
                res.Append('|');
                res.Append(map[i]);
                res.Append('|');
                res.Append(rowSeparator);

            }

            res.Append('-', (Width * 2) + 3);
            res.Append(rowSeparator);

            return res.ToString();

        }

        #endregion //Dump

        #region "Content"

        /// <summary>
        /// Returns a single string containing the grids content with each row in the 
        /// grid separated by Environment.NewLine.
        /// </summary>
        /// <returns>String containing the grid's content as a string string using NewLine characters</returns>
        public string Content()
        {
            return Content(Environment.NewLine);
        }

        /// <summary>
        /// Returns a single string containing the grids content with each row in the 
        /// grid separated by the specified string.
        /// </summary>
        /// <param name="rowSeparator">String to mark the end of each row.</param>
        /// <returns>String containing the grid's content as a string string using the specified separation character</returns>
        public string Content(string rowSeparator)
        {
            StringBuilder res = new((Width + 2) * Height);

            for (int i = 0; i < Height; i++)
            {
                res.Append(this.GetRow(i));
                res.Append(rowSeparator);
            }

            return res.ToString();
        }

        #endregion

        #endregion //Public

        #region "Private Methods"

        /// <summary>
        /// Private method that inserts the contents from grid into the current grid.
        /// </summary>
        /// <param name="x">X position from 0 to number of columns-1.  X/Y co-ordinates start at 0,0 on the Grid's top left corner. </param>
        /// <param name="y">Y position from 0 to number of rows-1.</param>
        /// <param name="sourceGrid">The Grid to insert into the current Grid.</param>
        /// <param name="updateCursor">If true, then the current Grid's cursor will be move to after the last character in the current Grid.</param>
        /// <exception cref="IndexOutOfRangeException">Thrown the area to be inserted is out of bounds or there is not enough space in the current Grid.</exception>
        private void InsertGrid(int x, int y, Grid sourceGrid, bool updateCursor)
        {
            int sourceWidth = sourceGrid.Width;
            int sourceHeight = sourceGrid.Height;

            if (x < 0 || x >= this.Width)
            {
                throw new IndexOutOfRangeException($"Value of {nameof(x)} ({x}) out of range.");
            }
            else if (x + sourceWidth > this.Width)
            {
                throw new IndexOutOfRangeException($"Source Map's width of {sourceWidth} too big to fit at {nameof(x)} offset of {x}.");
            }
            else if (y < 0 || y >= this.Height)
            {
                throw new IndexOutOfRangeException($"Value of {nameof(y)} ({y}) out of range.");
            }
            else if (y + sourceHeight > this.Height)
            {
                throw new IndexOutOfRangeException($"Source Map's height of {sourceHeight} too big to fit at {nameof(y)} offset of {y}.");
            }

            int destRow = y;
            int sourceMapIndex = 0;
            for (int destRowCount = 0; destRowCount < sourceGrid.Height; destRowCount++)
            {
                int destMapIndex = this.CalcIndexFromXY(x, destRow + destRowCount);

                for (int destColCount = 0; destColCount < sourceGrid.Width; destColCount++)
                {
                    this._map[destMapIndex] = sourceGrid._map[sourceMapIndex];
                    this._content[destMapIndex] = sourceGrid._content[sourceMapIndex];
                    sourceMapIndex++;
                    destMapIndex++;
                }
            }

            if (updateCursor)
            {
                this._cursorX = x + sourceGrid._cursorX;
                this._cursorY = y + sourceGrid._cursorY;
                this._cursorIdx = CalcIndexFromXY(this._cursorX, this._cursorY);
            }

        }


        /// Writes a string into the grid's content at the current cursor position.
        /// This method is private and is used by public <see cref="Write"/> method.
        /// The starting write position is 0,0 (top-left) unless <see cref="GridWriteOptions"/> specify RTL direction
        /// or <see cref="GridWriteOptions.Anchor"/> is set.
        /// </summary>
        /// <param name="stringToWrite">String to write.</param>
        /// <param name="opts">Write options. See <see cref="GridWriteOptions"/></param>
        /// <returns>Will return true if all the characters were written.  If false is returned then 0 or more characters
        /// may have been written but there was no more space in the grid.)
        private bool DoWrite(string stringToWrite, GridWriteOptions opts)
        {

            if (stringToWrite.Length > 0)
            {
                bool paddingAdded;

                if (opts.EnableWordSpacing)
                {
                    paddingAdded = AddTemporaryPaddingCharactersToMap(opts);
                }
                else
                {
                    paddingAdded = false;
                }

                try
                {
                    // replace \r\n or \n\r with just a single \n so works with single or double characters to represent end of line
                    // and then replace any single \r left to a \n
                    stringToWrite = stringToWrite.Replace("\n\r", "\n");
                    stringToWrite = stringToWrite.Replace("\r\n", "\n");
                    stringToWrite = stringToWrite.Replace("\r", "\n");

                    if (!this._firstWriteSetCursorPositionDone)
                    {
                        SetCursorPositionBasedOnWriteOptions(opts);
                        this._firstWriteSetCursorPositionDone = true;
                    }

                    bool breakPending = false;
                    int breakCheckCurrentWritingDirectionPosition = 0;

                    int stringIdx = 0;
                    int charactersLeftToWrite = stringToWrite.Length;
                    while (charactersLeftToWrite > 0)
                    {
                        // skip leading spaces, if at start of the row/column (depending on writing direction)
                        if (CalcIfAtStartOnWritingDirectionRowOrCol(opts))
                        {
                            while (charactersLeftToWrite > 0 && stringToWrite.Substring(stringIdx, 1) == " ")
                            {
                                charactersLeftToWrite--;
                                if (charactersLeftToWrite == 0)
                                {
                                    // everything fitted, return true
                                    return true;
                                }
                                stringIdx++;
                            }
                        }

                        int spaceCount = 0;

                        spaceCount = CountSpaceAtCurrentCursorPosForWritingDirection(opts);

                        // if no space, then move to next available space
                        while (0 == spaceCount)
                        {
                            if (!MoveCursorToMapMatch(opts, MapCharacters.Free))
                            {
                                // no more room, string not fully fitted
                                return false;
                            }

                            spaceCount = CountSpaceAtCurrentCursorPosForWritingDirection(opts);
                        }

                        // Get text from our string from the currentIndex until the end of string or next CR or LF
                        string stringTillEndOrNewLine;
                        int poss_CRorLF_Index = stringToWrite.IndexOf("\n", stringIdx);
                        if (poss_CRorLF_Index < 0)
                        {
                            stringTillEndOrNewLine = stringToWrite.Substring(stringIdx);
                        }
                        else if (poss_CRorLF_Index == stringIdx)
                        {
                            if (!MoveCursorToStartOfNextCrossWritingDirection(opts))
                            {
                                //no more rows or cols to move to, therefore output not fitted
                                return false;
                            }

                            // skip over the CR or LF in the string
                            stringIdx++;
                            charactersLeftToWrite--;

                            // nothing to write in this pass of the lookp
                            stringTillEndOrNewLine = string.Empty;
                        }
                        else
                        {
                            stringTillEndOrNewLine = stringToWrite.Substring(stringIdx, poss_CRorLF_Index - stringIdx);
                            breakPending = true;
                            breakCheckCurrentWritingDirectionPosition = this.GetMainDirectionPosition(opts);
                        }

                        if (stringTillEndOrNewLine.Length > 0)
                        {
                            if (spaceCount < charactersLeftToWrite)
                            {
                                if (opts.AllowWrapping)
                                {
                                    bool spaceFurtherAhead = false;
                                    int spaceFurtherFoundAtMapIndex = 0;

                                    string bestFit = GetBestFitWords(opts, spaceCount, stringTillEndOrNewLine, this._cursorIdx, ref spaceFurtherAhead, ref spaceFurtherFoundAtMapIndex);

                                    if (bestFit.Length == 0)
                                    {
                                        // could not be fitted at current position 
                                        if (spaceFurtherAhead)
                                        {
                                            // can be fitted (all or part) later on
                                            this._cursorIdx = spaceFurtherFoundAtMapIndex;
                                            CalcXYFromIndex(this._cursorIdx, ref this._cursorX, ref this._cursorY);
                                            continue;
                                        }

                                        // cannot be fitted. return back to caller.
                                        return false;
                                    }

                                    charactersLeftToWrite -= bestFit.Length;
                                    stringIdx += bestFit.Length;

                                    string justified = PadStringForJustification(opts, bestFit, spaceCount);

                                    WriteAtCursorResult writeResult = WriteAtCursor(opts, justified);

                                    if (writeResult == WriteAtCursorResult.NotFittedTextTruncated)
                                    {
                                        // end of space in map
                                        return false;
                                    }
                                    else if (writeResult == WriteAtCursorResult.FittedAndEndOfMap)
                                    {
                                        if (charactersLeftToWrite == 0)
                                        {
                                            // nothing left to write
                                            return true;
                                        }
                                        else
                                        {
                                            // reached end of map without writing all characters 
                                            // we are in the (while (charactersLeftToWrite > 0)) loop
                                            // where we would have more to write from the next iteratrion
                                            return false;
                                        }
                                    }

                                    // did the call to GetBestFitWordsWhenWrappingIsAllowed detect space furher ahead to prevent a word from being truncated?
                                    if (spaceFurtherAhead)
                                    {
                                        // if so then move the cursor to the position returned by GetBestFitWordsWhenWrappingIsAllowed
                                        this._cursorIdx = spaceFurtherFoundAtMapIndex;
                                        CalcXYFromIndex(this._cursorIdx, ref this._cursorX, ref this._cursorY);
                                    }
                                }
                                else if (opts.AllowTruncation)
                                {

                                    WriteAtCursorResult writeResult = WriteAtCursor(opts, stringToWrite.Substring(stringIdx, spaceCount));

                                    charactersLeftToWrite -= spaceCount;
                                    stringIdx += spaceCount;

                                    if (writeResult == WriteAtCursorResult.NotFittedTextTruncated)
                                    {
                                        // end of space in map
                                        return false;
                                    }
                                    else if (writeResult == WriteAtCursorResult.FittedAndEndOfMap)
                                    {
                                        if (charactersLeftToWrite == 0)
                                        {
                                            // nothing left to write
                                            return true;
                                        }
                                        else
                                        {
                                            // reached end of map without writing all characters
                                            return false;
                                        }
                                    }

                                    // Everything written?
                                    if (charactersLeftToWrite <= 0)
                                    {
                                        return true;
                                    }

                                    // Wrapping is not allowed, as such if the cursor was moved 
                                    // to the next row (or col if writing by columns) then we need to stop
                                    if (CalcIfAtStartOnWritingDirectionRowOrCol(opts))
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    // If here, both wrapping and truncation are not allowed
                                    // So, we are only looking to fit whole words on the main row/col (depending of main writing direction)
                                    bool spaceFurtherAhead = false;
                                    int spaceFurtherFoundAtMapIndex = 0;
                                    string bestFit = GetBestFitWords(opts, spaceCount, stringTillEndOrNewLine, this._cursorIdx, ref spaceFurtherAhead, ref spaceFurtherFoundAtMapIndex);

                                    if (bestFit == "" && !spaceFurtherAhead)
                                    {
                                        // nothing can be fitted here, and no further space ahead
                                        return false;
                                    }

                                    WriteAtCursor(opts, bestFit);

                                    charactersLeftToWrite -= bestFit.Length;
                                    stringIdx += bestFit.Length;



                                    // can't fit string/remainder of based on settings.  return back to caller.
                                    // return false;
                                }
                            }
                            else
                            {
                                string justified = PadStringForJustification(opts, stringTillEndOrNewLine, spaceCount);
                                stringIdx += stringTillEndOrNewLine.Length;
                                charactersLeftToWrite -= stringTillEndOrNewLine.Length;

                                WriteAtCursorResult writeResult = WriteAtCursor(opts, justified);

                                if (writeResult == WriteAtCursorResult.NotFittedTextTruncated)
                                {
                                    // end of space in map
                                    return false;
                                }
                                else if (writeResult == WriteAtCursorResult.FittedAndEndOfMap)
                                {
                                    if (charactersLeftToWrite == 0)
                                    {
                                        // reached end of map and all characters written
                                        return true;

                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }

                            }

                            // stringTillEndOrNewLine ends with \n
                            if (breakPending)
                            {
                                // Only move the next line (or col depending on main writing direction) if the cursor was not already
                                // wrapped (e.g. due to full justification or exact match).
                                if (breakCheckCurrentWritingDirectionPosition == this.GetMainDirectionPosition(opts))
                                {
                                    breakPending = false;

                                    if (!MoveCursorToStartOfNextCrossWritingDirection(opts))
                                    {
                                        //no more rows or cols to move to, therefore output not fitted
                                        return false;
                                    }

                                    // skip over the CR or LF in the string
                                    stringIdx++;
                                    charactersLeftToWrite--;
                                }

                            }
                        } // stringTillEndOrNewLine.Length > 0
                    } // while (charactersLeftToWrite > 0)

                    // everything fitted, return true
                    return true;

                }
                finally
                {

                    if (paddingAdded)
                    {
                        RemovePaddingCharactersFromMap();
                    }
                }
            }

            // nothing to output, therefore it fitted!!!
            return true;
        }

        /// <summary>
        /// Adds <see cref="MapCharacters.TempPadding"> characters into <see cref="_map"/>. Calls Writing text to prevent 
        /// cells in the map from being written to. This allows us to ensure that text being written maintains a space
        /// with existing grid content.
        /// </summary>
        /// <param name="opts">Used to determine where the temporary "padding" is to be added as we may be writing across rows or down columns</param>
        /// <returns>true if any temporary characters were written to the map.</returns>
        private bool AddTemporaryPaddingCharactersToMap(GridWriteOptions opts)
        {
            switch (opts.Direction)
            {
                case GridWriteOptions.TextDirection.RowTopToBottomLeftToRight:
                case GridWriteOptions.TextDirection.RowTopToBottonRightToLeft:

                    return AddTemporaryPaddingCharactersToMapForPaddingDirection(PaddingDirection.Rows);

                case GridWriteOptions.TextDirection.ColTopToBottomLeftToRight:

                    return AddTemporaryPaddingCharactersToMapForPaddingDirection(PaddingDirection.Cols);

                default:
                    throw new NotImplementedException($"{nameof(opts.Direction)} of {opts.Direction} not implemented in SetCursorPositionBasedOnWriteOptions");

            }
        }

        /// <summary>
        /// Support method for <see cref="AddTemporaryPaddingCharactersToMap"/>.  
        /// Adds <see cref="MapCharacters.TempPadding"> characters into <see cref="_map"/>.
        /// </summary>
        /// <param name="direction">Direction to insert padding characters around existing content.</param>
        /// <returns></returns>
        private bool AddTemporaryPaddingCharactersToMapForPaddingDirection(PaddingDirection direction)
        {
            // Waring: GridTesting has a test to call this method by reflection.  This is so we can test this method
            //         to aid development whist keeping this method private.  The test has an hard-coded
            //         string containing this methods name.
            bool anySet = false;
            int x = 0;
            int y = 0;
            int idx = 0;
            int idxLeft = -1;
            int idxRight = 1;
            int idxAbove = -this.Width;
            int idxBelow = this.Width;

            int lastColIdx = this.Width - 1;
            int lastRowIdx = this.Height - 1;

            foreach (char c in this._map)
            {
                if (c != (char)MapCharacters.Free && c != (char)MapCharacters.TempPadding)
                {
                    if (direction == PaddingDirection.Rows || direction == PaddingDirection.RowsAndCols)
                    {
                        // check char to left (if within range)
                        if (x > 0 && this._map[idxLeft] == (char)MapCharacters.Free)
                        {
                            this._map[idxLeft] = (char)MapCharacters.TempPadding;

                            if (!anySet)
                            {
                                anySet = true;
                            }
                        }

                        // check char to right (if within range)
                        if (x < lastColIdx && this._map[idxRight] == (char)MapCharacters.Free)
                        {

                            this._map[idxRight] = (char)MapCharacters.TempPadding;
                            if (!anySet)
                            {
                                anySet = true;
                            }
                        }
                    }

                    if (direction == PaddingDirection.Cols || direction == PaddingDirection.RowsAndCols)
                    {
                        // check char above (if within range)
                        if (y > 0 && this._map[idxAbove] == (char)MapCharacters.Free)
                        {
                            this._map[idxAbove] = (char)MapCharacters.TempPadding;

                            if (!anySet)
                            {
                                anySet = true;
                            }
                        }

                        // check char below (if within range)
                        if (y < lastRowIdx && this._map[idxBelow] == (char)MapCharacters.Free)
                        {

                            this._map[idxBelow] = (char)MapCharacters.TempPadding;
                            if (!anySet)
                            {
                                anySet = true;
                            }
                        }
                    }
                }

                idx++;
                idxLeft++;
                idxRight++;
                idxAbove++;
                idxBelow++;
                x++;
                if (x > lastColIdx)
                {
                    x = 0;
                    y++;
                }
            }
            return anySet;

        }

        /// <summary>
        /// Replaces all <see cref="MapCharacters.TempPadding"/> characters in the map with <see cref="MapCharacters.Free"> characters.
        /// Thus reversing any changes made by <see cref="AddTemporaryPaddingCharactersToMap(GridWriteOptions)"/>.
        /// </summary>
        private void RemovePaddingCharactersFromMap()
        {
            for (int i = 0; i < this._map.Length; i++)
            {
                if (this._map[i] == (char)MapCharacters.TempPadding)
                {
                    this._map[i] = (char)MapCharacters.Free;
                }
            }
        }

        /// <summary>
        /// For the given string (part of) and the available number of contiguous characters return a string containing the text to write.
        /// If the string to be written is longer than contiguous characters, it will look ahead in the map to see if there is a space
        /// to fit that word or not.  Wether or not, the look ahead looks at the next row (or column depending on the main writing
        /// direction) will depend if wrapping is allowed.
        /// </summary>
        /// <param name="opts">Write options. See <see cref="GridWriteOptions"/>.
        /// Used to determine the main direction of writing and wrapping options.</param>
        /// <param name="contiguousSpace">The number of contiguous characters.</param>
        /// <param name="fromString">The string to be written.</param>
        /// <param name="atCursorIdx">Cursor index in the internal content being written to.</param>
        /// <param name="moreToCome">Set by reference.  If the number of contiguous characters are not enough to 
        /// write <see cref="fromString"/> and there is space further head in the map, this will be set to true.</param>
        /// <param name="moreAtMapIndex">Set by reference.  If <see cref="moreToCome"/> is true this will be set to 
        /// the index position within the map where writing can continue at.</param>
        /// <returns></returns>
        private string GetBestFitWords(GridWriteOptions opts, int contiguousSpace, string fromString, int atCursorIdx, ref bool moreToCome, ref int moreAtMapIndex)
        {
            moreToCome = false;
            moreAtMapIndex = 0;



            if (fromString.Length <= contiguousSpace)
            {
                return fromString;
            }

            if (!opts.EnableLookAheadForWholeWordFit)
            {
                return fromString.Substring(0, contiguousSpace);
            }




            // get whole words and build up string for available space
            string[] words = fromString.Split(' ');

            int numWords = words.Length;
            int wordIdx = 0;
            int spaceLeft = contiguousSpace;

            StringBuilder canFitWords = new(contiguousSpace);

            while (wordIdx < numWords)
            {
                int thisWordLength = words[wordIdx].Length;

                if (canFitWords.Length != 0)
                {
                    canFitWords.Append(' ');
                    spaceLeft--;
                }

                if (canFitWords.Length + thisWordLength <= contiguousSpace)
                {
                    //we can fit this word within the available space
                    canFitWords.Append(words[wordIdx]);
                    spaceLeft -= thisWordLength;
                    if (spaceLeft <= 0)
                    {
                        return canFitWords.ToString();
                    }
                }
                else
                {
                    // cannot fit word, but will there be space further ahead?
                    int lookAheadFromIndex = atCursorIdx;
                    int lookAheadX = 0;
                    int lookAheadY = 0;
                    CalcXYFromIndex(lookAheadFromIndex, ref lookAheadX, ref lookAheadY);

                    // calc cursor position after this block of contiguous characters
                    bool ableToMove;
                    if (opts.AllowWrapping)
                    {
                        ableToMove = CalcMove(opts, contiguousSpace, ref lookAheadFromIndex, ref lookAheadX, ref lookAheadY);
                    }
                    else
                    {
                        ableToMove = CalcMoveOnMainWritingDirectionOnly(opts, contiguousSpace, ref lookAheadFromIndex, ref lookAheadX, ref lookAheadY);
                    }

                    if (ableToMove)
                    {
                        // now look for a space big enough for the currentWord
                        int? spaceFoundAtIdx = LookAheadForSpace(opts, thisWordLength, lookAheadFromIndex, lookAheadX, lookAheadY);
                        if (spaceFoundAtIdx is not null)
                        {
                            // there will be space later on.  So just return what we have now
                            moreToCome = true;
                            moreAtMapIndex = (int)spaceFoundAtIdx;
                            return canFitWords.ToString();
                        }
                    }

                    // if here then current word will not fit further ahead

                    if (opts.AllowTruncation)
                    {
                        // add characters till space filled
                        foreach (char thisWordChar in words[wordIdx])
                        {
                            canFitWords.Append(thisWordChar);
                            spaceLeft--;
                            if (spaceLeft == 0)
                            {
                                break;
                            }
                        }

                        return canFitWords.ToString();
                    }
                    else
                    {
                        // truncation not taking place so just return what we have
                        return canFitWords.ToString();
                    }
                }

                wordIdx++;
            }

            return canFitWords.ToString();
        }

        /// <summary>
        /// Pads string with spaces based on justification options
        /// </summary>
        /// <param name="opts">Write options. See <see cref="GridWriteOptions"/>.</param>
        /// <param name="s">String to padd.</param>
        /// <param name="toFitSpace">How many characters are there to pad to.</param>
        /// <returns>String to be written with, if required, additional spaces.</returns>
        private string PadStringForJustification(GridWriteOptions opts, string s, int toFitSpace)
        {
            switch (opts.Justification)
            {
                case GridWriteOptions.TextJustification.Near:
                    return s.TrimStart();

                case GridWriteOptions.TextJustification.Far:

                    //Left to Right, Far Justification = Right Align - Add Leading Spaces
                    s = s.TrimEnd();
                    int numPaddToAdd = toFitSpace - s.Length;
                    if (numPaddToAdd > 0)
                    {
                        String padd = new String(' ', toFitSpace - s.Length);
                        return padd + s;
                    }
                    else
                    {
                        return s;
                    }


                case GridWriteOptions.TextJustification.Center:

                    s = s.Trim();
                    int spaceLeft = toFitSpace - s.Length;
                    if (spaceLeft > 0)
                    {

                        int charsAtStart = spaceLeft / 2;
                        if (charsAtStart > 0)
                        {
                            String paddStart = new String(' ', charsAtStart);
                            s = paddStart + s;
                            spaceLeft -= charsAtStart;
                        }

                        if (spaceLeft > 0)
                        {
                            String paddEnd = new String(' ', spaceLeft);
                            s = s + paddEnd;
                        }

                        return s;
                    }
                    else
                    {
                        return s;
                    }

                case GridWriteOptions.TextJustification.Full:

                    s = s.Trim();
                    int spaceLeftFull = toFitSpace - s.Length;

                    // any spaceleft and any spaces to add additional spaces to ?
                    if (spaceLeftFull > 0 && s.IndexOf(' ') >= 0)
                    {
                        int fromIndex = s.Length - 1;

                        while (spaceLeftFull > 0)
                        {
                            int idx = s.LastIndexOf(' ', fromIndex);
                            if (idx >= 0)
                            {
                                s = s.Insert(idx, " ");
                                fromIndex = idx - 1;
                                if (fromIndex < 0)
                                {
                                    fromIndex = s.Length - 1;

                                }
                                spaceLeftFull--;
                            }
                            else
                            {
                                //back to end again
                                fromIndex = s.Length - 1;
                            }
                        }
                    }

                    return s;

                default:
                    throw new NotImplementedException($"{nameof(opts.Justification)} of {opts.Justification} not implemented in PadStringForJustification");
            }
        }

        /// <summary>
        /// Write characters into the grid content and marks its map as being in use (<see cref="MapCharacters.WrittenTo"/>).
        /// Characters are written to the grid's <see cref="CursorX"/>> and <see cref="CursorY"/> location.
        /// </summary>
        /// <param name="opts">Write options. See <see cref="GridWriteOptions"/>.</param>
        /// <param name="s">String to write.</param>
        /// <returns>Result indicating if all the string could be inserted and if the cursor was moved on to the next X/Y position.</returns>
        private WriteAtCursorResult WriteAtCursor(GridWriteOptions opts, string s)
        {
            int charactersWrittenCount = 0;
            foreach (char c in s)
            {
                if (c == opts.NonWrappingBlankSpacingCharacter && opts.AllowNonWrappingBlankSpacingCharacter)
                {
                    this._content[this._cursorIdx] = ' ';
                }
                else
                {
                    this._content[this._cursorIdx] = c;
                }
                this._map[this._cursorIdx] = (char)MapCharacters.WrittenTo;

                charactersWrittenCount++;

                if (!this.MoveCursor(opts, 1))
                {
                    // did we fit perfectly at the end of map?
                    if (charactersWrittenCount == s.Length)
                    {
                        return WriteAtCursorResult.FittedAndEndOfMap;
                    }
                    else
                    {
                        return WriteAtCursorResult.NotFittedTextTruncated;
                    }
                }
            }

            return WriteAtCursorResult.FittedAndCursorMovedOn;
        }


        /// <summary>
        /// Called when the first write is performed.  This is to ensure that the cursor position is set in an appropiate location
        /// to accomodate RTL.
        /// </summary>
        /// <param name="opts">Write options. See <see cref="GridWriteOptions"/>.</param>
        private void SetCursorPositionBasedOnWriteOptions(GridWriteOptions opts)
        {
            switch (opts.Direction)
            {
                case GridWriteOptions.TextDirection.RowTopToBottomLeftToRight:
                case GridWriteOptions.TextDirection.ColTopToBottomLeftToRight:

                    _cursorX = _cursorY = _cursorIdx = 0;
                    break;

                case GridWriteOptions.TextDirection.RowTopToBottonRightToLeft:
                    _cursorY = -0;
                    _cursorX = _cursorIdx = this.Width - 1;
                    break;

                default:
                    throw new NotImplementedException($"{nameof(opts.Direction)} of {opts.Direction} not implemented in SetCursorPositionBasedOnWriteOptions");

            }
        }


        /// <summary>
        /// Moves the maps' cursor position from its current position until if finds the specified character in the grid's map.
        /// If the character at the map's current cursor position matches the character to find then the 
        /// current cursor position remains unchanged.
        /// </summary>
        /// <param name="opts">Write options. See <see cref="GridWriteOptions"/>.</param>
        /// <param name="characterToFind">Which character to find.</param>
        /// <returns></returns>
        private bool MoveCursorToMapMatch(GridWriteOptions opts, MapCharacters characterToFind)
        {
            while (this._map[_cursorIdx] != (char)characterToFind)
            {
                if (IsAtEndOfMap())
                {
                    return false;
                }

                MoveCursor(opts, 1);
            }

            return true;
        }

        /// <summary>
        /// Checks if the grid's cursor it on the last cell.
        /// </summary>
        /// <returns>True it at the last cell.  False if not.</returns>
        private bool IsAtEndOfMap()
        {
            return (this._cursorIdx == this._map.Length - 1);
        }

        /// <summary>
        /// Calculates the position of the cursor if moved on by the number of specified characters.
        /// Moves will be on the main writing direction, wrapping if required.  E.g. if at the end
        /// of the row, it will wrap to the next row.
        /// </summary>
        /// <param name="opts">Write options. See <see cref="GridWriteOptions"/>. 
        /// Used to determine the direction of cursor movement. I.e. we are moving by rows or columns and in which direction.</param>
        /// <param name="numberOfCharacters">Number of characters to move.</param>
        /// <param name="cursorIdx">Set by reference. If true is returned this will be set to array index of where the cursor will be.
        /// If false is returned any current value will be untouched.</param>
        /// <param name="cursorX">Set by reference. If true is returned this will be set to position of where the X cursor will be.
        /// If false is returned any current value will be untouched.</param>
        /// <param name="cursorY">Set by reference. If true is returned this will be set to position of where the X cursor will be.
        /// If false is returned any current value will be untouched.</param>
        /// <returns>True if cursor can be moved the specified number of characters.  False if not.</returns>
        private bool CalcMove(GridWriteOptions opts, int numberOfCharacters, ref int cursorIdx, ref int cursorX, ref int cursorY)
        {
            switch (opts.Direction)
            {
                case GridWriteOptions.TextDirection.RowTopToBottomLeftToRight:
                    int newIdx = cursorIdx + numberOfCharacters;

                    // too far ?
                    if (newIdx >= this._map.Length || newIdx < 0)
                    {
                        return false;
                    }

                    cursorIdx = newIdx;
                    CalcXYFromIndex(newIdx, ref cursorX, ref cursorY);

                    return true;

                case GridWriteOptions.TextDirection.RowTopToBottonRightToLeft:

                    while (numberOfCharacters > 0)
                    {
                        cursorX--;
                        cursorIdx--;

                        // end of this line, move dowe to the right hand side of the next
                        if (cursorX < 0)
                        {
                            if (cursorY >= this.Height - 1)
                            {
                                // no more moves
                                cursorX++;
                                cursorIdx++;
                                return false;
                            }

                            cursorX = this.Width - 1;
                            cursorY++;
                            cursorIdx = CalcIndexFromXY(cursorX, cursorY);
                        }

                        numberOfCharacters--;
                    }

                    return true;

                case GridWriteOptions.TextDirection.ColTopToBottomLeftToRight:

                    while (numberOfCharacters > 0)
                    {
                        cursorY++;
                        cursorIdx += this.Width;

                        // end of this column, move to top of next
                        if (cursorY == this.Height)
                        {
                            if (cursorX >= this.Width - 1)
                            {
                                // no more moves
                                cursorY--;
                                cursorIdx -= this.Width;
                                return false;
                            }

                            cursorX++;
                            cursorY = 0;
                            cursorIdx = CalcIndexFromXY(cursorX, cursorY);
                        }

                        numberOfCharacters--;
                    }

                    return true;

                default:
                    throw new NotImplementedException($"{nameof(opts.Direction)} of {opts.Direction} not implemented in CalcMoveCursor");
            }
        }

        /// <summary>
        /// Calculates the position of the cursor if moved on the main writing direction. 
        /// Moves will only be calculated on the main writing direction.  E.g. across on row, but will not wrap to the next row.
        /// </summary>
        /// <param name="opts">Write options. See <see cref="GridWriteOptions"/>. 
        /// Used to determine the direction of cursor movement. I.e. we are moving by rows or columns and in which direction.</param>
        /// <param name="numberOfCharacters">Number of characters to move.</param>
        /// <param name="cursorIdx">Set by reference. If true is returned this will be set to array index of where the cursor will be.
        /// If false is returned any current value will be unchanged.</param>
        /// <param name="cursorX">Set by reference. If true is returned this will be set to the position of where the X cursor will be.
        /// If false is returned any current value will be unchanged.</param>
        /// <param name="cursorY">Set by reference. If true is returned this will be set to the position of where the Y cursor will be.
        /// If false is returned any current value will be unchanged.</param>
        /// <returns>True if the cursor could be moved as required, false if not.</returns>
        private bool CalcMoveOnMainWritingDirectionOnly(GridWriteOptions opts, int numberOfCharacters, ref int cursorIdx, ref int cursorX, ref int cursorY)
        {
            switch (opts.Direction)
            {
                case GridWriteOptions.TextDirection.RowTopToBottomLeftToRight:
                    for (int cnt = 0; cnt < numberOfCharacters; cnt++)
                    {
                        cursorX++;
                        if (cursorX == this.Width)
                        {
                            // no more moves in the main writing direction
                            cursorX--;
                            return false;
                        }
                        else
                        {
                            cursorIdx++;
                        }
                    }

                    return true;


                case GridWriteOptions.TextDirection.RowTopToBottonRightToLeft:
                    for (int cnt = 0; cnt < numberOfCharacters; cnt++)
                    {
                        cursorX--;
                        if (cursorX < 0)
                        {
                            // no more moves on the main writing direction
                            cursorX++;
                            return false;
                        }
                        else
                        {
                            cursorIdx--;
                        }
                    }

                    return true;

                case GridWriteOptions.TextDirection.ColTopToBottomLeftToRight:
                    for (int cnt = 0; cnt < numberOfCharacters; cnt++)
                    {
                        cursorY++;
                        if (cursorY >= this.Height)
                        {
                            // no more moves on the main writing direction
                            cursorY--;
                            return false;
                        }
                        else
                        {
                            cursorIdx += this.Width;
                        }
                    }

                    return true;


            }

            throw new NotImplementedException($"{nameof(opts.Direction)} of {opts.Direction} not implemented in CalcMoveOnMainWritingDirectionOnly");
        }


        /// <summary>
        /// Moves the grid's cursor by the specified number of characters.
        /// </summary>
        /// <param name="opts">Write options. See <see cref="GridWriteOptions"/>. 
        /// Used to determine the direction of cursor movement. I.e. we are moving by rows or columns.</param>
        /// <param name="numberOfCharacters">Number of characters to move.</param>
        /// <returns>True if moved. False if there is not enough characters to move that far (current position remains unchanged).</returns>
        private bool MoveCursor(GridWriteOptions opts, int numberOfCharacters)
        {
            return CalcMove(opts, numberOfCharacters, ref this._cursorIdx, ref this._cursorX, ref this._cursorY);
        }

        /// <summary>
        /// Moves the cursor to beginning of the next cross row or column depending on the direction text is being written.
        /// E.g. move to the start or the next row if we a writing by rows.
        /// </summary>
        /// <param name="opts">Write options. See <see cref="GridWriteOptions"/>. 
        /// Used to determine the direction we are writing.</param>
        /// <returns>True if able to move the cursor false if not.</returns>
        private bool MoveCursorToStartOfNextCrossWritingDirection(GridWriteOptions opts)
        {
            switch (opts.Direction)
            {
                case GridWriteOptions.TextDirection.RowTopToBottomLeftToRight:
                    if (this._cursorY == this.Height - 1)
                    {
                        return false;
                    }

                    this._cursorX = 0;
                    this._cursorY++;
                    this._cursorIdx = CalcIndexFromXY(this._cursorX, this._cursorY);
                    return true;


                default:
                    throw new NotImplementedException($"{nameof(opts.Direction)} of {opts.Direction} not implemented in MoveCursorToStartOfNextCrossWritingDirection");
            }
        }

        /// <summary>
        /// From the specified cursor position, look ahead to see if there are enough contiguous free space characters in the grid's map.
        /// </summary>
        /// <param name="opts">Write options. See <see cref="GridWriteOptions"/>. 
        /// Used to determine the direction we are writing.</param>
        /// <param name="wordLengthRequired">How many contiguous free space characters are we looking for.</param>
        /// <param name="cursorIndex">Map array index to start at.</param>
        /// <param name="cursorX">The X position to start at.</param>
        /// <param name="cursorY">The Y position to start at.</param>
        /// <returns>null if not enough contiguous character found or the index position in the grid's map (from 0).</returns>
        private int? LookAheadForSpace(GridWriteOptions opts, int wordLengthRequired, int cursorIndex, int cursorX, int cursorY)
        {
            int spaceAvailable = 0;
            int? indexAtSpace = null;

            while (cursorIndex < this._map.Length)
            {
                if (this._map[cursorIndex] == (char)MapCharacters.Free)
                {
                    spaceAvailable++;
                    if (indexAtSpace is null)
                    {
                        indexAtSpace = cursorIndex;
                    }

                    if (spaceAvailable == wordLengthRequired)
                    {
                        return indexAtSpace;
                    }
                }
                else
                {
                    spaceAvailable = 0;
                    indexAtSpace = null;
                }

                // if false is return then there more characters on the main writing direction
                // E.g. when writing across in rows false will be returned if we cannot move the next character on that row
                if (!CalcMoveOnMainWritingDirectionOnly(opts, 1, ref cursorIndex, ref cursorX, ref cursorY))
                {
                    if (opts.AllowWrapping)
                    {
                        //As we know we are at the end, this will move the cursor to the start
                        //of next row (or column if writing in columns)
                        spaceAvailable = 0;
                        indexAtSpace = null;

                        if (!CalcMove(opts, 1, ref cursorIndex, ref cursorX, ref cursorY))
                        {
                            // was unable to move on at all = at then end of the map
                            return null;        // not enough freespace for 'wordLengthRequired'
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            // a big enough space was not found
            return null;
        }

        /// <summary>
        /// From and including the grid's current cursor position count the number of free space characters in the grid's map.
        /// </summary>
        /// <param name="opts">Write options. See <see cref="GridWriteOptions"/>. 
        /// Used to determine the direction we are writing.</param>
        /// <returns>0 or number of characters from the current grid's X/Y position that contains free space characters.</returns>
        private int CountSpaceAtCurrentCursorPosForWritingDirection(GridWriteOptions opts)
        {
            int pos = this._cursorIdx;
            int cX = this._cursorX;
            int cY = this._cursorY;

            int count = 0;

            char c = this._map[pos];

            while ((char)MapCharacters.Free == c)
            {
                count++;

                if (!CalcMoveOnMainWritingDirectionOnly(opts, 1, ref pos, ref cX, ref cY))
                {
                    // false is returned if unable to move cursor any more across/down on the main writing direction
                    break;
                }

                c = this._map[pos];
            }

            return count;

        }


        /// <summary>
        /// Checks if the grid's current cursor position is at the start of the current row/column
        /// i.e. are we are the start or a row when writing across in rows.
        /// </summary>
        /// <param name="opts">Write options. See <see cref="GridWriteOptions"/>. 
        /// Used to determine the direction we are writing.</param>
        /// <returns>True if at the start, false if not.</returns>
        private bool CalcIfAtStartOnWritingDirectionRowOrCol(GridWriteOptions opts)
        {
            switch (opts.Direction)
            {
                case GridWriteOptions.TextDirection.RowTopToBottomLeftToRight:
                    return (this._cursorX == 0);

                case GridWriteOptions.TextDirection.RowTopToBottonRightToLeft:
                    return (this._cursorX == this.Width - 1);

                case GridWriteOptions.TextDirection.ColTopToBottomLeftToRight:
                    return (this._cursorY == 0);
            }

            throw new NotImplementedException($"{nameof(opts.Direction)} of {opts.Direction} not implemented in CalcIfAtStartOnWritingDirectionRowOrCol");
        }

        /// <summary>
        /// For the given array index, calculate the cursors X and Y position.  
        /// No checks are made here to determine if the index is valid and within the grid's bounds.
        /// </summary>
        /// <param name="index">The grid's array index starting from 0.</param>
        /// <param name="X">Will be set by reference to the calculated position of the X cursor.</param>
        /// <param name="Y">Will be set by reference to the calculated position of the Y cursor.</param>
        private void CalcXYFromIndex(int index, ref int X, ref int Y)
        {
            X = (index % this.Width);
            Y = (index - X) / this.Width;
        }

        /// <summary>
        /// For the given cursor X and Y positions calculated the internal map and content array's index.
        /// No checks are made here to determine if the cursor positions are valid and within the grid's bounds.
        /// </summary>
        /// <param name="X">The Cursor X position to use.</param>
        /// <param name="Y">The Cursor Y position to use.</param>
        /// <returns>The position in the contents and map arrays index (from 0) that for the given X,Y position.</returns>
        private int CalcIndexFromXY(int X, int Y)
        {
            return (this.Width * Y) + X;
        }

        /// <summary>
        /// Returns the position of the main writing direction.
        /// If writing by rows, the the current row number will be returnbed.
        /// If writing by columns then the current column number will be returned.
        /// </summary>
        /// <param name="opts"></param>
        /// <returns>Current row or column number depending if writing across in rows or down in columns.</returns>
        private int GetMainDirectionPosition(GridWriteOptions opts)
        {
            switch (opts.Direction)
            {
                case GridWriteOptions.TextDirection.RowTopToBottomLeftToRight:
                case GridWriteOptions.TextDirection.RowTopToBottonRightToLeft:
                    return this._cursorY;           // row number


                case GridWriteOptions.TextDirection.ColTopToBottomLeftToRight:
                    return this._cursorX;           // column number


                default:
                    throw new NotImplementedException($"{nameof(opts.Direction)} of {opts.Direction} not implemented in GetMainDirectionPosition");

            }
        }

        /// <summary>
        /// Removes leading and trailing spaces from the string.  If the string contains any new line characters (\n)
        /// it will remove leading and training spaces from each sub-string.
        /// </summary>
        /// <param name="s">String to trim.</param>
        /// <param name="LineTerminator">Terminator to add to the returned string.  This will be added to the 
        /// end of the string and also each sub-string if the original string contained any new line characters (\n)</param>
        /// <returns>Trimmed string.</returns>
        private static string SuperTrim(string s, string LineTerminator)
        {
            string[] lines = s.Trim().Replace("\r", "").Split('\n');

            StringBuilder res = new(s.Length);

            foreach (string l in lines)
            {
                res.Append(l.Trim());
                res.Append(LineTerminator);
            }

            return res.ToString();
        }


        #endregion
    }

}