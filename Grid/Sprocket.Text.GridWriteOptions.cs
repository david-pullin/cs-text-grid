using System;
using System.Text;


namespace Sprocket.Text.Grid
{
    public class GridWriteOptions
    {

        /// <summary>
        /// Sets the direction of writing.
        /// </summary>
        public enum TextDirection
        {
            /// <summary>
            /// Default. Left to right across rows from top to buttom.
            /// </summary>
            RowTopToBottomLeftToRight = 0,

            /// <summary>
            /// Right to Left across rows from top to buttom.
            /// </summary>
            RowTopToBottonRightToLeft = 1,

            /// <summary>
            /// Write in a downwards direction on each column moving from left to right.
            /// </summary>
            ColTopToBottomLeftToRight = 2
        }

        public enum TextJustification
        {
            /// <summary>
            /// Default. No justification.
            /// </summary>
            Near = 0,

            /// <summary>
            /// Text will be centered along the row (or column if TextDirection is set to column).
            /// </summary>
            Center = 1,

            /// <summary>
            /// Text will be justified to the right if writing left to right, or to the left if writing right to left.
            /// </summary>
            Far = 2,

            /// <summary>
            /// Text will be fully justified along the row (or column if TextDirection is set to column) by inserting
            /// additional spaces between each word.
            /// </summary>
            Full = 3
        }

        /// <summary>
        /// TextAnchor can be used to set the text being anchored (aliged) to the specific edge of the grid.
        /// </summary>
        public enum TextAnchor
        {
            /// <summary>
            /// Default. No anchoring. Text will be written at the current cursor position.
            /// </summary>
            None = 0,

            /// <summary>
            /// Text will be positioned at the bottom of the grid regadless of the current cursor position.
            /// </summary>
            Bottom = 1
        }

        /// <summary>
        /// Sets the direction text is to be written.
        /// </summary>
        /// <value>Default = <see cref="TextDirection.RowTopToBottomLeftToRight"/>.</value>
        public TextDirection Direction { get; set; } = TextDirection.RowTopToBottomLeftToRight;

        /// <summary>
        /// Sets the justification to apply to the string. E.g. left, centered, right or full justification.
        /// Left and right justification is in relation to the text direction. I.e. are we writing left to right
        /// or right to left. Therefore, the terms Near and Far are used instead. When writing left to right, Near = left justified
        /// and Far = right jusitifed. When writing right to left, Near = right justified and far = left justified.
        /// </summary>
        /// <value>Default = <see cref="TextJustification.Near"/>.</value>
        public TextJustification Justification { get; set; } = TextJustification.Near;

        /// <summary>
        /// TextAnchor allows the text being written to anchor (align) against the specific edge of the grid.
        /// </summary>
        /// <value>Default = <see cref="TextAnchor.None"/>.</value>
        public TextAnchor Anchor { get; set; } = TextAnchor.None;

        /// <summary>
        /// When enabled, text will be wrapped to the next row (or column if using a different <see cref="TextDirection"/>).
        /// </summary>
        /// <value>Default = 2.  Allow text to write to new row.</value>
        public bool AllowWrapping { get; set; } = true;

        /// <summary>
        /// When enabled, words will be truncated if then cannot fit into the available space.
        /// </summary>
        /// <value>Default = true. Allow words to be truncated</value>
        public bool AllowTruncation { get; set; } = true;

        /// <summary>
        /// When enabled, if the character in the string being written matches property <see cref="NonWrappingBlankSpacingCharacter" />
        /// then a space will be plotted in the grid. 
        /// </summary>
        /// <value>Default = true. Set to true if to subsitute <see cref="NonWrappingBlankSpacingCharacter" /> as a space when writing to the grid.</value>
        public bool AllowNonWrappingBlankSpacingCharacter { get; set; } = true;

        /// <summary>
        /// By default, when multiple calls are made to write a string to the grid, 
        /// a space is automatically inserted to seperate each string. Set to false to turn off this behaviour.
        /// </summary>
        /// <value>Default = true. When enabled, multiple calls to <see cref="Grid.Write(string)"/> will insert a 
        /// space between each string.</value>
        public bool EnableWordSpacing { get; set; } = true;

        /// <summary>
        /// By default, when writing text to the grid, if a word does not fit in the available space (e.g. current word
        /// cannot fit in remaining space on the current row) the text writing process looks ahead (e.g. other rows)
        /// to find space where the whole word can be fitted.
        /// </summary>
        /// <value>Default = true</value>
        public bool EnableLookAheadForWholeWordFit { get; set; } = true;

        /// <summary>
        /// Which character should be subsituted for a space when writing text to the grid.
        /// This property is ignored if <see cref="AllowNonWrappingBlankSpacingCharacter"/> is set to false.
        /// </summary>
        /// <value>Default =  (char)0.</value>
        public char NonWrappingBlankSpacingCharacter { get; set; } = (char)0;
    }

}