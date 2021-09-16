using Xunit;
using Sprocket.Text.Grid;
using System;

namespace GridTest
{
    public class GridTest_NewLine : test_base
    {
        [Theory]
        [InlineData("\n")]
        [InlineData("\r")]
        [InlineData("\n\r")]
        [InlineData("\r\n")]
        public void SingleLF(string separator)
        {
            Grid m = new(15, 3);

            m.Write("Hello" + separator + "World");

            string expected = @"
                        ---------------------------------
                        |Hello          |WWWWW..........|
                        |World          |WWWWW..........|
                        |               |...............|
                        ---------------------------------";

            AssertGridMatches(expected, m);
        }

        [Fact]
        public void DoubleLF()
        {
            Grid m = new(15, 3);

            m.Write("Hello\n\n World");

            //NB: The space before "World" is ignored (truncated) as the output goes against the first column on the next row
            string expected = @"
                        ---------------------------------
                        |Hello          |WWWWW..........|
                        |               |...............|
                        |World          |WWWWW..........|
                        ---------------------------------";

            AssertGridMatches(expected, m);
        }


    }
}