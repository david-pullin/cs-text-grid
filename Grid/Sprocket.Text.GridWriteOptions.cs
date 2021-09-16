using System;
using System.Text;


namespace Sprocket.Text.Grid
{
    public class GridWriteOptions
    {

        public enum TextDirection
        {
            RowTopToBottomLeftToRight = 0,

            RowTopToBottonRightToLeft = 1,

            ColTopToBottomLeftToRight = 2
        }

        public enum TextJustification
        {
            Near = 0,
            Center = 1,
            Far = 2,
            Full = 3
        }

        public enum TextAnchor
        {
            None = 0,

            Bottom = 1
        }

        public TextDirection Direction { get; set; } = TextDirection.RowTopToBottomLeftToRight;

        public TextJustification Justification { get; set; } = TextJustification.Near;

        public TextAnchor Anchor { get; set; } = TextAnchor.None;


        public bool AllowWrapping { get; set; } = true;

        public bool AllowTruncation { get; set; } = true;
        
        public bool AllowNonWrappingBlankSpacingCharacter { get; set; } = true;

        public bool EnableWordSpacing {get;set;} = true;

        public bool EnableLookAheadForWholeWordFit {get;set;} = true;
        
        public char NonWrappingBlankSpacingCharacter { get; set; } = (char)0;
    }

}