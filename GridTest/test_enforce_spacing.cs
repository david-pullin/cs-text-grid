using Xunit;
using Sprocket.Text.Grid;

namespace GridTest
{
    public class GridTest_WordSpacing : test_base
    {
        [Fact]
        public void EnableWordSpacing_DefaultBehaviour()
        {

            //Mock up a Grid with existing content
            Grid m = Grid.FromContent(@"
                    |Hello         |
                    ");

            // NB: First available free map character is after the 'o'
            // however, due to the default of EnableWordSpacing (true)
            // the 'W' in "World" is written a character later to ensure there is a space
            // between existing content and what is being written now
            bool result = m.Write("World");

            string expected = @"
                        -------------------------------
                        |Hello World   |WWWWW.WWWWW...|
                        -------------------------------";

            Assert.True(result);
            AssertGridMatches(expected, m);
        }

        [Fact]
        public void EnableWordSpacing_Disabled()
        {

            //Mock up a Grid with existing content
            Grid m = Grid.FromContent(@"
                    |Hello         |
                    ");

            GridWriteOptions opts = new();
            opts.EnableWordSpacing = false;

            // "World" is writted directly after the 'o' in "Hello"
            bool result = m.Write("World", opts);

            string expected = @"
                        -------------------------------
                        |HelloWorld    |WWWWWWWWWW....|
                        -------------------------------";

            Assert.True(result);
            AssertGridMatches(expected, m);
        }
    }
}