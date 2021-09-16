using Xunit;
using Sprocket.Text.Grid;

namespace GridTest
{
    public class GridTest_Advanced : test_base
    {
        [Fact]
        public void Heart()
        {
            Grid m = Grid.FromContent(@"
                |     ,@,@,@,@,@,   ,@,@,@,@,@,     |
                |   ,@!         @, ,@         !@,   |
                | ,@!            @,@            !@, |
                |,@!              @              !@,|
                |,@!                             !@,|
                |,@!  -------------------------  !@,|
                | '@! ------------------------- !@' |
                |  '@!  ---------------------   @'  |
                |   '@! --------------------- !@'   |
                |     '@!  ---------------- !@'     |
                |       '@!   ----------  !@'       |
                |         '@!           !@'         |
                |           '@!       !@'           |
                |             '@!   !@'             |
                |               '@!@'               |
                |                 @                 |
            ", '-');

            GridWriteOptions opts = new();
            opts.Justification = GridWriteOptions.TextJustification.Center;

            m.Write("To my one and only and amazing friend\nI will love you forever", opts);

            string expected = @"
                -------------------------------------------------------------------------
                |     ,@,@,@,@,@,   ,@,@,@,@,@,     |WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                |   ,@!         @, ,@         !@,   |WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                | ,@!            @,@            !@, |WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                |,@!              @              !@,|WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                |,@!                             !@,|WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                |,@!   To my one and only and    !@,|WWWWW.WWWWWWWWWWWWWWWWWWWWWWW.WWWWW|
                | '@!      amazing friend       !@' |WWWWW.WWWWWWWWWWWWWWWWWWWWWWW.WWWWW|
                |  '@!     I will love you      @'  |WWWWWWW.WWWWWWWWWWWWWWWWWWW.WWWWWWW|
                |   '@!        forever        !@'   |WWWWWWW.WWWWWWWWWWWWWWWWWWW.WWWWWWW|
                |     '@!                   !@'     |WWWWWWWWWW................WWWWWWWWW|
                |       '@!               !@'       |WWWWWWWWWWWWW..........WWWWWWWWWWWW|
                |         '@!           !@'         |WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                |           '@!       !@'           |WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                |             '@!   !@'             |WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                |               '@!@'               |WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                |                 @                 |WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                -------------------------------------------------------------------------
            ";

            AssertGridMatches(expected, m);

        }

        [Fact]
        public void Fill()
        {
            Grid m = Grid.FromContent(@"
                |              -              |
                |             ---             |
                |        --  -----  --        |
                |         -----------         |
                |     --   ---------   --     |
                | --------  ------   -------- |
                |  -------------------------  |
                |-----------------------------|
                |   -----------------------   |
                |      -----------------      |
                |       ---------------       |
                |      -----------------      |
                |              -              |
                |              -              |
                |              -              |
            ", '-');

            GridWriteOptions opts = new();
            opts.EnableWordSpacing = false;
            opts.EnableLookAheadForWholeWordFit = false;

            while(m.Write("Hello", opts));

            string expected = @"
                    -------------------------------------------------------------
                    |              H              |WWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                    |             ell             |WWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                    |        oH  elloH  el        |WWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                    |         loHelloHell         |WWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                    |     oH   elloHello   He     |WWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                    | lloHello  HelloH   elloHell |WWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                    |  oHelloHelloHelloHelloHell  |WWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                    |oHelloHelloHelloHelloHelloHel|WWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                    |   loHelloHelloHelloHelloH   |WWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                    |      elloHelloHelloHel      |WWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                    |       loHelloHelloHel       |WWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                    |      loHelloHelloHello      |WWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                    |              H              |WWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                    |              e              |WWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                    |              l              |WWWWWWWWWWWWWWWWWWWWWWWWWWWWW|
                    -------------------------------------------------------------            
            ";

            AssertGridMatches(expected, m);
        }


    }
}