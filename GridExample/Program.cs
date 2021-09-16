using System;
using static System.Console;

namespace Sprocket.Text.Grid
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("Sprocket.Text.Grid Examples - Select which example to execute");
            WriteLine("A] With word spacing             B] Without word spacing");
            WriteLine("C] Full Justification            D] Write in shape");
            WriteLine("E] Writing in columns            F] Fill in shape (by rows)");
            WriteLine("G] Fill in shape (by cols)       H] Set cusror X/Y");
            WriteLine("I] Simple From Map");

            char key = Console.ReadKey().KeyChar;

            Console.WriteLine();

            switch (key)
            {
                case 'A':
                case 'a':
                    WriteInBox(spacingAroundExistingContent: true);
                    break;

                case 'B':
                case 'b':
                    WriteInBox(spacingAroundExistingContent: false);
                    break;

                case 'C':
                case 'c':
                    WriteInBox(textJustification: GridWriteOptions.TextJustification.Full);
                    break;

                case 'D':
                case 'd':
                    WriteInShape();
                    break;

                case 'E':
                case 'e':
                    WriteColumn();
                    break;

                case 'F':
                case 'f':
                    FillInShape(GridWriteOptions.TextDirection.RowTopToBottomLeftToRight);
                    break;

                case 'G':
                case 'g':
                    FillInShape(GridWriteOptions.TextDirection.ColTopToBottomLeftToRight);
                    break;

                case 'H':
                case 'h':
                    SetCursorPosition();
                    break;

                case 'I':
                case 'i':
                    SimpleFromMap();
                    break;
            }

        }

        static void WriteInBox(bool spacingAroundExistingContent = true,
        GridWriteOptions.TextJustification textJustification = GridWriteOptions.TextJustification.Near)
        {

            GridWriteOptions opts = new();
            opts.EnableWordSpacing = spacingAroundExistingContent;
            opts.Justification = textJustification;

            Grid g = Grid.FromContent(@"
                |/-----------------------\|
                ||                       ||
                ||                       ||
                |\-----------------------/|
            ");

            Console.WriteLine("Content before write:");
            Console.WriteLine(g.Content());

            g.Write("Hello World, Welcome to the show.", opts);

            Console.WriteLine("Content after write:");
            Console.WriteLine(g.Content());

        }

        static void WriteInShape()
        {

            Grid g = Grid.FromContent(@"
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

            g.Write("To my one and only and amazing friend\nI will love you forever", opts);

            Console.WriteLine(g.Content());
        }

        static void WriteColumn()
        {
            Grid g = new(10, 8);

            GridWriteOptions opts = new();
            opts.Direction = GridWriteOptions.TextDirection.ColTopToBottomLeftToRight;
            opts.AllowTruncation = true;

            g.Write("It was the best of times, it was the worst of times", opts);

            Console.WriteLine(g.Content());
        }

        static void FillInShape(GridWriteOptions.TextDirection textDirection)
        {
            Grid g = Grid.FromContent(@"
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
            opts.Direction = textDirection;

            while (g.Write("Hello", opts)) ;

            Console.WriteLine(g.Content());
        }

        static void SetCursorPosition()
        {
            Grid g = new Grid(10, 10);

            g.CursorX = 5;
            g.CursorY = 3;

            g.Write("Hello World");

            Console.WriteLine(g.Content());

        }

        static void SimpleFromMap()
        {
            Grid g = Grid.FromMap(@"
                    |.........|
                    |.*****...|
                    |........*|
                    |.........|
                ");

            g.Write("Hello World");

            Console.WriteLine(g.Content());

        }
    }
}
