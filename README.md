# Sprocket.Text.Grid.dll v1.0.0 API documentation

Created by [David Pullin](https://ict-man.me)

Readme Last Updated on 16.09.2021

The full API can be found at https://ict-man.me/sprocket/api/Sprocket.Text.Grid.html

Licence GPL-3.0 

<br>

# Summary

Text.Grid allow you to write text into a two dimensional grid with various options such as alignment, justification and word-wrapping.

# Examples

Namespace: Sprocket.Text.Grid


### Example writing into ASCII art

<pre><code>
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

</code></pre>

Expected output

<pre><code>
     ,@,@,@,@,@,   ,@,@,@,@,@,
   ,@!         @, ,@         !@,
 ,@!            @,@            !@,
,@!              @              !@,
,@!                             !@,
,@!   To my one and only and    !@,
 '@!      amazing friend       !@' 
  '@!     I will love you      @'
   '@!        forever        !@'
     '@!                   !@'
       '@!               !@'
         '@!           !@'
           '@!       !@'
             '@!   !@'
               '@!@'
                 @
</code></pre>
