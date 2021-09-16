using Xunit;
using Sprocket.Text.Grid;

namespace GridTest
{
    public class GridTest_Wrapping : test_base
    {

        #region "Default - RowTopToBottomLeftToRight"

        [Fact]
        public void DefaultsFittingPerfectly()
        {
            Grid m = new(5, 2);

            bool result = m.Write("Hello World");

            string expected = @"
                        -------------
                        |Hello|WWWWW|
                        |World|WWWWW|
                        -------------";

            Assert.True(result);
            AssertGridMatches(expected, m);
        }

        [Fact]
        public void DefaultsFittingWordWrapped()
        {
            Grid m = new(5, 2);

            m.Write("Hi You");

            string expected = @"
                        -------------
                        |Hi   |WWW..|
                        |You  |WWW..|
                        -------------";

            AssertGridMatches(expected, m);
        }


        [Fact]
        public void DefaultsOutOfSpaceWordTruncated()
        {
            Grid m = new(5, 2);

            bool result = m.Write("Good Looking");

            // we ran out of space, by default truncation is allowed
            string expected = @"
                        -------------
                        |Good |WWWWW|
                        |Looki|WWWWW|
                        -------------";

            Assert.False(result);
            AssertGridMatches(expected, m);
        }

        [Fact]
        public void DefaultsOutOfSpaceWordWrapped()
        {
            Grid m = new(5, 3);

            m.Write("Good Looking");

            // we ran out of space, by default truncation is allowed
            string expected = @"
                        -------------
                        |Good |WWWWW|
                        |Looki|WWWWW|
                        |ng   |WW...|
                        -------------";

            AssertGridMatches(expected, m);
        }



        [Fact]
        public void DefaultsFittingAroundMapA()
        {
            Grid m = Grid.FromMap(@"
                    |.........|
                    |.*****...|
                    |........*|
                    |.........|
                    ");


            m.Write("Looking Good With You");

            // keeps whole word 'Good' together as there is space on line 3
            string expected = @"
                    ---------------------
                    |Looking  |WWWWWWWW.|
                    |         |.*****...|
                    |Good     |WWWWW...*|
                    |With You |WWWWWWWW.|
                    ---------------------";
            AssertGridMatches(expected, m);
        }

        [Fact]
        public void DefaultsFittingAroundMapB()
        {
            Grid m = Grid.FromMap(@"
                    |.........|
                    |.*****...|
                    |........*|
                    |.........|
                    ");


            m.Write("Looking Good You");

            // keeps whole word 'Good' together as cannot find space on line two
            // "You" cannot fit after Good as there would then be no space between the "u" and the "*" in the map

            string expected = @"
                    ---------------------
                    |Looking  |WWWWWWWW.|
                    |         |.*****...|
                    |Good     |WWWWW...*|
                    |You      |WWW......|
                    ---------------------";

            AssertGridMatches(expected, m);
        }
        #endregion

        #region "Default - RowTopToBottonRightToLeft"

        [Fact]
        public void Defaults_RTL_FittingPerfectly()
        {
            Grid m = new(5, 2);

            GridWriteOptions opts = new();
            opts.Direction = GridWriteOptions.TextDirection.RowTopToBottonRightToLeft;

            m.Write("Hello World", opts);

            string expected = @"
                        -------------
                        |olleH|WWWWW|
                        |dlroW|WWWWW|
                        -------------";


            AssertGridMatches(expected, m);
        }


        [Fact]
        public void Defaults_RTL_FittingWordWrapped()
        {
            Grid m = new(5, 2);

            GridWriteOptions opts = new();
            opts.Direction = GridWriteOptions.TextDirection.RowTopToBottonRightToLeft;

            m.Write("Hi You", opts);

            string expected = @"
                        -------------
                        |   iH|..WWW|
                        |  uoY|..WWW|
                        -------------";

            AssertGridMatches(expected, m);
        }


        [Fact]
        public void Defaults_RTL_OutOfSpaceWordTruncated()
        {
            Grid m = new(5, 2);

            GridWriteOptions opts = new();
            opts.Direction = GridWriteOptions.TextDirection.RowTopToBottonRightToLeft;

            m.Write("Good Looking", opts);

            // we ran out of space, by default truncation is allowed
            string expected = @"
                        -------------
                        | dooG|WWWWW|
                        |ikooL|WWWWW|
                        -------------";

            AssertGridMatches(expected, m);
        }

        [Fact]
        public void Defaults_RTL_OutOfSpaceWordWrapped()
        {
            Grid m = new(5, 3);

            GridWriteOptions opts = new();
            opts.Direction = GridWriteOptions.TextDirection.RowTopToBottonRightToLeft;

            m.Write("Good Looking", opts);

            // we ran out of space, by default truncation is allowed
            string expected = @"
                        -------------
                        | dooG|WWWWW|
                        |ikooL|WWWWW|
                        |   gn|...WW|
                        -------------";

            AssertGridMatches(expected, m);
        }

        [Fact]
        public void Defaults_RTL_FittingAroundMapA()
        {
            Grid m = Grid.FromMap(@"
                    |.........|
                    |.*****...|
                    |........*|
                    |.........|
                    ");


            GridWriteOptions opts = new();
            opts.Direction = GridWriteOptions.TextDirection.RowTopToBottonRightToLeft;

            m.Write("Looking Good With You", opts);

            // keeps whole word 'Good' together as there is space on line 3
            string expected = @"
                    ---------------------
                    |  gnikooL|.WWWWWWWW|
                    |         |.*****...|
                    |   dooG  |..WWWWW.*|
                    | uoY htiW|.WWWWWWWW|
                    ---------------------";
            AssertGridMatches(expected, m);
        }

        #endregion

        #region "NoneWrappingBlankCharacter"
        [Fact]
        public void NonSpacingDefaultCharacter()
        {
            Grid m = new(8, 2);

            // By default, char 0 will be written as a space but not used as a break between words
            // In this grid, the default settings, allow the word (which is "Hello\0World") to be truncated
            // and continued (wrapped) on the next line
            string s = string.Format("Hello{0}World", (char)0);

            bool result = m.Write(s);

            string expected = @"
                        -------------------
                        |Hello Wo|WWWWWWWW|
                        |rld     |WWW.....|
                        -------------------";

            Assert.True(result);
            AssertGridMatches(expected, m);
        }
        #endregion
    }
}
