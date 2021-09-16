using Xunit;
using Sprocket.Text.Grid;

namespace GridTest
{
    public class GridTest_Justifaction : test_base
    {
        #region "Default - RowTopToBottomLeftToRight"

        [Fact]
        public void NearJustification_AllDefault()
        {
            Grid m = new(20, 2);

            m.Write("Hello World");

            string expected = @"
                -------------------------------------------
                |Hello World         |WWWWWWWWWWW.........|
                |                    |....................|
                -------------------------------------------
                ";

            AssertGridMatches(expected, m);
        }

        [Fact]
        public void NearJustification_AllDefault_AroundMap()
        {
            Grid m = Grid.FromMap(@"
                    |.........|
                    |.*****...|
                    |........*|
                    |.........|
                    ");

            m.Write("Full Me A B More");

            string expected = @"
                    ---------------------
                    |Full Me A|WWWWWWWWW|
                    |       B |.*****.WW|
                    |More     |WWWW....*|
                    |         |.........|
                    ---------------------
                    ";

            AssertGridMatches(expected, m);
        }

        [Fact]
        public void FarJustification_Rows()
        {
            Grid m = new(20, 2);

            GridWriteOptions opts = new();
            opts.Justification = GridWriteOptions.TextJustification.Far;
            m.Write("Hello World", opts);

            string expected = @"
                    -------------------------------------------
                    |         Hello World|WWWWWWWWWWWWWWWWWWWW|
                    |                    |....................|
                    -------------------------------------------
                ";

            AssertGridMatches(expected, m);
        }

        [Fact]
        public void FarJustification_Rows_AroundMap()
        {
            Grid m = Grid.FromMap(@"
                    |.........|
                    |.*****...|
                    |........*|
                    |.........|
                    ");

            GridWriteOptions opts = new();
            opts.Justification = GridWriteOptions.TextJustification.Far;
            m.Write("Full Me A B More Text", opts);

            string expected = @"
                    ---------------------
                    |Full Me A|WWWWWWWWW|
                    |        B|.*****.WW|
                    |   More  |WWWWWWW.*|
                    |     Text|WWWWWWWWW|
                    ---------------------
                    ";

            AssertGridMatches(expected, m);
        }

        [Fact]
        public void CenterJustification_Rows()
        {
            Grid m = new(20, 2);

            GridWriteOptions opts = new();
            opts.Justification = GridWriteOptions.TextJustification.Center;
            m.Write("Hello World Thats Spanning Over", opts);

            string expected = @"
                    -------------------------------------------
                    | Hello World Thats  |WWWWWWWWWWWWWWWWWWWW|
                    |   Spanning Over    |WWWWWWWWWWWWWWWWWWWW|
                    -------------------------------------------";

            AssertGridMatches(expected, m);
        }

        [Fact]
        public void CenterJustification_Rows_AroundMap()
        {
            Grid m = Grid.FromMap(@"
                    |..............|
                    |.*****........|
                    |***..........*|
                    |..............|
                    ");

            GridWriteOptions opts = new();
            opts.Justification = GridWriteOptions.TextJustification.Center;
            m.Write("Full Me A B More Text Lastly", opts);

            string expected = @"
                    -------------------------------
                    | Full Me A B  |WWWWWWWWWWWWWW|
                    |        More  |.*****.WWWWWWW|
                    |      Text    |***.WWWWWWWW.*|
                    |    Lastly    |WWWWWWWWWWWWWW|
                    -------------------------------
                    ";

            AssertGridMatches(expected, m);
        }

        [Fact]
        public void FullJustification_Rows()
        {
            Grid m = new(20, 2);

            GridWriteOptions opts = new();
            opts.Justification = GridWriteOptions.TextJustification.Full;
            m.Write("Hello World Thats Spanning Over", opts);

            string expected = @"
                        -------------------------------------------
                        |Hello  World   Thats|WWWWWWWWWWWWWWWWWWWW|
                        |Spanning        Over|WWWWWWWWWWWWWWWWWWWW|
                        -------------------------------------------";

            AssertGridMatches(expected, m);
        }


        [Fact]
        public void FullJustification_Rows_AroundMap()
        {
            Grid m = Grid.FromMap(@"
                    |..............|
                    |.*****........|
                    |***..........*|
                    |***...........|
                    |..............|
                    ");

            GridWriteOptions opts = new();
            opts.Justification = GridWriteOptions.TextJustification.Full;
            m.Write("Full Me More Too You A Text Low Text Lastly", opts);

            string expected = @"
                    -------------------------------
                    |Full  Me  More|WWWWWWWWWWWWWW|
                    |       Too You|.*****.WWWWWWW|
                    |    A   Text  |***.WWWWWWWW.*|
                    |    Low   Text|***.WWWWWWWWWW|
                    |Lastly        |WWWWWW........|
                    -------------------------------
                    ";

            AssertGridMatches(expected, m);
        }

        #endregion



        #region "Default - RowTopToBottonRightToLeft"

        [Fact]
        public void NearJustification_RTL()
        {
            Grid m = new(20, 2);

            GridWriteOptions opts = new();
            opts.Direction = GridWriteOptions.TextDirection.RowTopToBottonRightToLeft;

            m.Write("Hello World", opts);

            string expected = @"
                -------------------------------------------
                |         dlroW olleH|.........WWWWWWWWWWW|
                |                    |....................|
                -------------------------------------------
                ";

            AssertGridMatches(expected, m);
        }

        [Fact]
        public void NearJustification_RTL_AroundMap()
        {
            Grid m = Grid.FromMap(@"
                    |.........|
                    |.*****...|
                    |........*|
                    |.........|
                    ");

            GridWriteOptions opts = new();
            opts.Direction = GridWriteOptions.TextDirection.RowTopToBottonRightToLeft;

            m.Write("Full Me A B More", opts);

            string expected = @"
                    ---------------------
                    |A eM lluF|WWWWWWWWW|
                    |        B|.*****.WW|
                    |   eroM  |...WWWW.*|
                    |         |.........|
                    ---------------------
                    ";

            AssertGridMatches(expected, m);
        }

        [Fact]
        public void FarJustification_RTL()
        {
            Grid m = new(20, 2);

            GridWriteOptions opts = new();
            opts.Justification = GridWriteOptions.TextJustification.Far;
            opts.Direction = GridWriteOptions.TextDirection.RowTopToBottonRightToLeft;
            m.Write("Hello World", opts);

            string expected = @"
                    -------------------------------------------
                    |dlroW olleH         |WWWWWWWWWWWWWWWWWWWW|
                    |                    |....................|
                    -------------------------------------------
                ";

            AssertGridMatches(expected, m);
        }

        [Fact]
        public void FarJustification_RTL_Rows_AroundMap()
        {
            Grid m = Grid.FromMap(@"
                    |.........|
                    |.*****...|
                    |........*|
                    |.........|
                    ");

            GridWriteOptions opts = new();
            opts.Justification = GridWriteOptions.TextJustification.Far;
            opts.Direction = GridWriteOptions.TextDirection.RowTopToBottonRightToLeft;
            m.Write("Full Me A B More Text", opts);

            string expected = @"
                    ---------------------
                    |A eM lluF|WWWWWWWWW|
                    |       B |.*****.WW|
                    |eroM     |WWWWWWW.*|
                    |txeT     |WWWWWWWWW|
                    ---------------------
                    ";

            AssertGridMatches(expected, m);
        }

        [Fact]
        public void CenterJustification_RTL_Rows()
        {
            Grid m = new(20, 2);

            GridWriteOptions opts = new();
            opts.Justification = GridWriteOptions.TextJustification.Center;
            opts.Direction = GridWriteOptions.TextDirection.RowTopToBottonRightToLeft;
            m.Write("Hello World Thats Spanning Over", opts);

            string expected = @"
                    -------------------------------------------
                    |  stahT dlroW olleH |WWWWWWWWWWWWWWWWWWWW|
                    |    revO gninnapS   |WWWWWWWWWWWWWWWWWWWW|
                    -------------------------------------------";

            AssertGridMatches(expected, m);
        }

        [Fact]
        public void CenterJustification_RTL_AroundMap()
        {
            Grid m = Grid.FromMap(@"
                    |..............|
                    |.*****........|
                    |***..........*|
                    |..............|
                    ");

            GridWriteOptions opts = new();
            opts.Justification = GridWriteOptions.TextJustification.Center;
            opts.Direction = GridWriteOptions.TextDirection.RowTopToBottonRightToLeft;
            m.Write("Full Me A B More Text Lastly", opts);

            string expected = @"
                    -------------------------------
                    |  B A eM lluF |WWWWWWWWWWWWWW|
                    |         eroM |.*****.WWWWWWW|
                    |      txeT    |***.WWWWWWWW.*|
                    |    yltsaL    |WWWWWWWWWWWWWW|
                    -------------------------------
                    ";

            AssertGridMatches(expected, m);
        }

        [Fact]
        public void FullJustification_RTL()
        {
            Grid m = new(20, 2);

            GridWriteOptions opts = new();
            opts.Justification = GridWriteOptions.TextJustification.Full;
            opts.Direction = GridWriteOptions.TextDirection.RowTopToBottonRightToLeft;
            m.Write("Hello World Thats Spanning Over", opts);

            string expected = @"
                        -------------------------------------------
                        |stahT   dlroW  olleH|WWWWWWWWWWWWWWWWWWWW|
                        |revO        gninnapS|WWWWWWWWWWWWWWWWWWWW|
                        -------------------------------------------";

            AssertGridMatches(expected, m);
        }


        [Fact]
        public void FullJustification_RTL_Rows_AroundMap()
        {
            Grid m = Grid.FromMap(@"
                    |..............|
                    |.*****........|
                    |***..........*|
                    |***...........|
                    |..............|
                    ");

            GridWriteOptions opts = new();
            opts.Justification = GridWriteOptions.TextJustification.Full;
            opts.Direction = GridWriteOptions.TextDirection.RowTopToBottonRightToLeft;
            m.Write("Full Me More Too You A Text Low Text Lastly", opts);

            string expected = @"
                    -------------------------------
                    |eroM  eM  lluF|WWWWWWWWWWWWWW|
                    |       uoY ooT|.*****.WWWWWWW|
                    |    txeT   A  |***.WWWWWWWW.*|
                    |    txeT   woL|***.WWWWWWWWWW|
                    |        yltsaL|........WWWWWW|
                    -------------------------------
                    ";

            AssertGridMatches(expected, m);
        }

        #endregion

        #region "ColTopToBottomLeftToRight"


        [Fact]
        public void NearJustification_Col()
        {
            Grid m = new(5, 20);

            GridWriteOptions opts = new();
            opts.Direction = GridWriteOptions.TextDirection.ColTopToBottomLeftToRight;

            m.Write("Hello World", opts);

            string expected = @"
                -------------
                |H    |W....|
                |e    |W....|
                |l    |W....|
                |l    |W....|
                |o    |W....|
                |     |W....|
                |W    |W....|
                |o    |W....|
                |r    |W....|
                |l    |W....|
                |d    |W....|
                |     |.....|
                |     |.....|
                |     |.....|
                |     |.....|
                |     |.....|
                |     |.....|
                |     |.....|
                |     |.....|
                |     |.....|
                -------------
                ";

            AssertGridMatches(expected, m);
        }

        [Fact]
        public void NearJustification_Col_AroundMap()
        {
            Grid m = Grid.FromMap(@"
                    |..*...|
                    |.*....|
                    |.*....|
                    |.*....|
                    |..*...|
                    |......|
                    ");

            GridWriteOptions opts = new();
            opts.Direction = GridWriteOptions.TextDirection.ColTopToBottomLeftToRight;

            m.Write("Full Me A B More", opts);

            string expected = @"
                    ---------------
                    |F  MM |W.*WW.|
                    |u  eo |W*.WW.|
                    |l   r |W*.WW.|
                    |l  Ae |W*.WW.|
                    |      |W.*W..|
                    |   B  |...W..|
                    ---------------
                    ";

            AssertGridMatches(expected, m);
        }


        [Fact]
        public void FarJustification_Col()
        {
            Grid m = new(5, 20);

            GridWriteOptions opts = new();
            opts.Direction = GridWriteOptions.TextDirection.ColTopToBottomLeftToRight;
            opts.Justification = GridWriteOptions.TextJustification.Far;

            m.Write("Hello World", opts);

            string expected = @"
                -------------
                |     |W....|
                |     |W....|
                |     |W....|
                |     |W....|
                |     |W....|
                |     |W....|
                |     |W....|
                |     |W....|
                |     |W....|
                |H    |W....|
                |e    |W....|
                |l    |W....|
                |l    |W....|
                |o    |W....|
                |     |W....|
                |W    |W....|
                |o    |W....|
                |r    |W....|
                |l    |W....|
                |d    |W....|
                -------------
                ";

            AssertGridMatches(expected, m);
        }

        [Fact]
        public void FarJustification_Col_AroundMap()
        {
            Grid m = Grid.FromMap(@"
                    |..*...|
                    |.*....|
                    |.*....|
                    |.*....|
                    |..*...|
                    |......|
                    ");

            GridWriteOptions opts = new();
            opts.Direction = GridWriteOptions.TextDirection.ColTopToBottomLeftToRight;
            opts.Justification = GridWriteOptions.TextJustification.Far;

            m.Write("Full Me A B More", opts);

            string expected = @"
                    ---------------
                    |   M  |W.*WW.|
                    |   e  |W*.WW.|
                    |F   M |W*.WW.|
                    |u  Ao |W*.WW.|
                    |l   r |W.*WW.|
                    |l  Be |W..WW.|
                    ---------------
                    ";

            AssertGridMatches(expected, m);
        }

        [Fact]
        public void CenterJustification_Col()
        {
            Grid m = new(5, 20);

            GridWriteOptions opts = new();
            opts.Direction = GridWriteOptions.TextDirection.ColTopToBottomLeftToRight;
            opts.Justification = GridWriteOptions.TextJustification.Center;

            m.Write("Hello World", opts);

            string expected = @"
                -------------
                |     |W....|
                |     |W....|
                |     |W....|
                |     |W....|
                |H    |W....|
                |e    |W....|
                |l    |W....|
                |l    |W....|
                |o    |W....|
                |     |W....|
                |W    |W....|
                |o    |W....|
                |r    |W....|
                |l    |W....|
                |d    |W....|
                |     |W....|
                |     |W....|
                |     |W....|
                |     |W....|
                |     |W....|
                -------------
                ";

            AssertGridMatches(expected, m);
        }


        #endregion

    }

}

