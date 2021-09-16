using Xunit;
using Sprocket.Text.Grid;

namespace GridTest
{
    public class GridTest_Anchor : test_base
    {
        [Fact]
        public void Anchor_Bottom_FitsOnRow()
        {
            Grid m = Grid.FromMap(@"
                    |..............|
                    |..............|
                    |*...........**|
                    |.............*|
                    ");

            GridWriteOptions opts = new();
            opts.Anchor = GridWriteOptions.TextAnchor.Bottom;

            bool result = m.Write("Hello World", opts);
            m.RawPlot(m.CursorX, m.CursorY, 'X');       // Highlight where cursor's position is now

            string expected = @"
                        -------------------------------
                        |              |..............|
                        |              |..............|
                        |              |*...........**|
                        |Hello WorldX  |WWWWWWWWWWWW.*|
                        -------------------------------";

            Assert.True(result);
            AssertGridMatches(expected, m);
        }

        [Fact]
        public void Anchor_Bottom_NeedsTwoRows()
        {
            Grid m = Grid.FromMap(@"
                    |..............|
                    |..............|
                    |*...........**|
                    |......********|
                    ");

            GridWriteOptions opts = new();
            opts.Anchor = GridWriteOptions.TextAnchor.Bottom;

            bool result = m.Write("Hello World", opts);
            m.RawPlot(m.CursorX, m.CursorY, 'X');       // Highlight where cursor's position is now

            string expected = @"
                        -------------------------------
                        |              |..............|
                        |              |..............|
                        |  Hello       |*.WWWWWW....**|
                        |WorldX        |WWWWWW********|
                        -------------------------------";

            Assert.True(result);
            AssertGridMatches(expected, m);
        }

        [Fact]
        public void Anchor_Bottom_NeedsTwoRows_WithJustification()
        {
            Grid m = Grid.FromMap(@"
                    |..............|
                    |..............|
                    |*...........**|
                    |..........****|
                    ");

            GridWriteOptions opts = new();
            opts.Anchor = GridWriteOptions.TextAnchor.Bottom;
            opts.Justification = GridWriteOptions.TextJustification.Far;

            bool result = m.Write("Hello World", opts);
            m.RawPlot(m.CursorX, m.CursorY, 'X');       // Highlight where cursor's position is now

            string expected = @"
                        -------------------------------
                        |              |..............|
                        |              |..............|
                        |      Hello   |*.WWWWWWWWW.**|
                        |    WorldX    |WWWWWWWWWW****|
                        -------------------------------";

            Assert.True(result);
            AssertGridMatches(expected, m);
        }

    }
}