using Xunit;
using System.Reflection;
using Sprocket.Text.Grid;

namespace GridTest
{
    public class GridTest_Private_Padding : test_base
    {
        [Fact]
        public void Padding_Rows()
        {

            Grid m = Grid.FromMap(@"
                    |.........|
                    |.*****...|
                    |........*|
                    |.........|
                    ");

            MethodInfo? dynMethod = m.GetType().GetTypeInfo().GetDeclaredMethod("AddTemporaryPaddingCharactersToMapForPaddingDirection");
            Assert.NotNull(dynMethod);

            if (dynMethod is not null)
            {
                dynMethod.Invoke(m, new object[] { 0 });        // 0 = private enum PaddingDirection.Rows

                string expected = @"
                    ---------------------
                    |         |.........|
                    |         |w*****w..|
                    |         |.......w*|
                    |         |.........|
                    ---------------------";

                AssertGridMatches(expected, m);
            }
        }
    
        [Fact]
        public void Padding_Cols()
        {

            Grid m = Grid.FromMap(@"
                    |.........|
                    |.*****...|
                    |........*|
                    |.........|
                    ");

            MethodInfo? dynMethod = m.GetType().GetTypeInfo().GetDeclaredMethod("AddTemporaryPaddingCharactersToMapForPaddingDirection");
            Assert.NotNull(dynMethod);

            if (dynMethod is not null)
            {
                dynMethod.Invoke(m, new object[] { 1 });        // 1 = private enum PaddingDirection.Cols

                string expected = @"
                    ---------------------
                    |         |.wwwww...|
                    |         |.*****..w|
                    |         |.wwwww..*|
                    |         |........w|
                    ---------------------";

                AssertGridMatches(expected, m);
            }
        }

        [Fact]
        public void Padding_RowsAndCols()
        {

            Grid m = Grid.FromMap(@"
                    |.........|
                    |.*****...|
                    |........*|
                    |.........|
                    ");

            MethodInfo? dynMethod = m.GetType().GetTypeInfo().GetDeclaredMethod("AddTemporaryPaddingCharactersToMapForPaddingDirection");
            Assert.NotNull(dynMethod);

            if (dynMethod is not null)
            {
                dynMethod.Invoke(m, new object[] { 2 });        // 1 = private enum PaddingDirection.RowsAndCols

                string expected = @"
                    ---------------------
                    |         |.wwwww...|
                    |         |w*****w.w|
                    |         |.wwwww.w*|
                    |         |........w|
                    ---------------------";

                AssertGridMatches(expected, m);
            }
        }

    }
}