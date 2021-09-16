using Xunit;
using Sprocket.Text.Grid;

namespace GridTest
{
    public class GridTest_Cursor : test_base
    {
        [Fact]
        public void SetCursorPosition()
        {
            Grid m = new Grid(10, 10);

            m.CursorX = 5;
            m.CursorY = 3;

            bool result = m.Write("Hello World");

            string expected = @"
                        -----------------------
                        |          |..........|
                        |          |..........|
                        |          |..........|
                        |     Hello|.....WWWWW|
                        |World     |WWWWW.....|
                        |          |..........|
                        |          |..........|
                        |          |..........|
                        |          |..........|
                        |          |..........|
                        -----------------------";

            // Assert.True(result);
            AssertGridMatches(expected, m);


        }

    }
}
