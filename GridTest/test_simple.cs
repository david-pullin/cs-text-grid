using Xunit;
using Sprocket.Text.Grid;

namespace GridTest
{
    public class GridTest_Simple : test_base
    {
        #region "RowTopToBottomLeftToRight"
        [Fact]
        public void Simple_EasyFit()
        {
            Grid m = new(15, 2);

            bool result = m.Write("Hello World");

            string expected = @"
                        ---------------------------------
                        |Hello World    |WWWWWWWWWWW....|
                        |               |...............|
                        ---------------------------------";

            Assert.True(result);
            AssertGridMatches(expected, m);
        }

        #endregion

        #region "RowTopToBottonRightToLeft"
        [Fact]
        public void Simple_RTL_EasyFit()
        {
            Grid m = new(15, 2);

            GridWriteOptions opts = new();
            opts.Direction = GridWriteOptions.TextDirection.RowTopToBottonRightToLeft;

            m.Write("Hello World", opts);

            string expected = @"
                        ---------------------------------
                        |    dlroW olleH|....WWWWWWWWWWW|
                        |               |...............|
                        ---------------------------------";


            AssertGridMatches(expected, m);
        }

        #endregion

        #region "ColTopToBottomLeftToRight"



        [Fact]
        public void Simple_Col_EasyFit()
        {
            Grid m = new(2, 15);

            GridWriteOptions opts = new();
            opts.Direction = GridWriteOptions.TextDirection.ColTopToBottomLeftToRight;

            m.Write("Hello World", opts);

            string expected = @"
                        -------
                        |H |W.|
                        |e |W.|
                        |l |W.|
                        |l |W.|
                        |o |W.|
                        |  |W.|
                        |W |W.|
                        |o |W.|
                        |r |W.|
                        |l |W.|
                        |d |W.|
                        |  |..|
                        |  |..|
                        |  |..|
                        |  |..|
                        -------";

            AssertGridMatches(expected, m);
        }

        #endregion
    }
}
