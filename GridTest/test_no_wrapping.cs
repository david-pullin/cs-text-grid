using Xunit;
using Sprocket.Text.Grid;

namespace GridTest
{
    
    /// <summary>
    /// Series of tests for when word/wrapping is not allowed
    /// </summary>
    public class GridTest_No_Wrapping : test_base
    {
        [Fact]
        public void No_Wrapping_With_Truncation_ExactWholeWord()
        {
            Grid m = new(5, 2);

            GridWriteOptions opts = new();
            opts.AllowWrapping = false;

            bool result = m.Write("Hello World", opts);

            string expected = @"
                        -------------
                        |Hello|WWWWW|
                        |     |.....|
                        -------------";

            Assert.False(result);               // Will be false, as not all characters is the string were written
            AssertGridMatches(expected, m);

        }

        [Fact]
        public void No_Wrapping_With_Truncation_Truncated_Mid_Word()
        {
            Grid m = new(7, 2);

            GridWriteOptions opts = new();
            opts.AllowWrapping = false;

            bool result = m.Write("Hello World", opts);

            string expected = @"
                        -----------------
                        |Hello W|WWWWWWW|
                        |       |.......|
                        -----------------";

            Assert.False(result);               // Will be false, as not all characters is the string were written
            AssertGridMatches(expected, m);
        }

        [Fact]
        public void No_Wrapping_Truncation_Not_Allowed()
        {
            Grid m = new(7, 2);

            GridWriteOptions opts = new();
            opts.AllowWrapping = false;
            opts.AllowTruncation = false;

            bool result = m.Write("Hello World", opts);

            string expected = @"
                        -----------------
                        |Hello  |WWWWWW.|
                        |       |.......|
                        -----------------";

            Assert.False(result);               // Will be false, as not all characters is the string were written
            AssertGridMatches(expected, m);
        }

        [Fact]
        public void No_Wrapping_Truncation_Not_Allowed_All_Fitted_Around_Map()
        {
            Grid m = Grid.FromMap(@"
                    |......***........|
                    |.................|
                    ");

            GridWriteOptions opts = new();
            opts.AllowWrapping = false;
            opts.AllowTruncation = false;

            bool result = m.Write("Hello World", opts);

            string expected = @"
                        -------------------------------------
                        |Hello     World  |WWWWW.***.WWWWW..|
                        |                 |.................|
                        -------------------------------------";

            Assert.True(result);               // Will be true, as all characters is the string were written
            AssertGridMatches(expected, m);
        }

        [Fact]
        public void No_Wrapping_Truncation_Not_Allowed_Not_Fitted_Around_Map()
        {
            Grid m = Grid.FromMap(@"
                    |......***........|
                    |.................|
                    ");

            GridWriteOptions opts = new();
            opts.AllowWrapping = false;
            opts.AllowTruncation = false;

            bool result = m.Write("Hello World Again", opts);

            string expected = @"
                        -------------------------------------
                        |Hello     World  |WWWWW.***.WWWWWW.|
                        |                 |.................|
                        -------------------------------------";

            Assert.False(result);               // Will be false, as not all characters is the string were written
            AssertGridMatches(expected, m);
        }

        [Fact]
        public void No_Wrapping_With_Truncation_Truncated_Around_Map()
        {
            Grid m = Grid.FromMap(@"
                    |...**....**......|
                    |.................|
                    ");

            GridWriteOptions opts = new();
            opts.AllowWrapping = false;

            bool result = m.Write("Hello World", opts);

            string expected = @"
                        -------------------------------------
                        |He    ll    o Wor|WW.**.WW.**.WWWWW|
                        |                 |.................|
                        -------------------------------------";

            Assert.False(result);               // Will be false, as not all characters is the string were written
            AssertGridMatches(expected, m);
        }

        [Fact]
        public void No_Wrapping_With_Truncation_Fits_On_Rows()
        {
            Grid m = Grid.FromMap(@"
                    |...**....**........|
                    |...................|
                    ");

            GridWriteOptions opts = new();
            opts.AllowWrapping = false;

            bool result = m.Write("Hello World", opts);

            string expected = @"
                        -----------------------------------------
                        |He    ll    o World|WW.**.WW.**.WWWWWWW|
                        |                   |...................|
                        -----------------------------------------";

            Assert.True(result);
            AssertGridMatches(expected, m);
        }
    }
}