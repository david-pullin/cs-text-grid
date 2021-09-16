using Xunit;
using System;
using System.Text;
using Sprocket.Text.Grid;

namespace GridTest
{
    public class test_base
    {
        private static readonly string TestLineTerminator = "~/~";

        public static void AssertGridMatches(string expected, Grid m)
        {
            Assert.Equal(RemovePadding(expected), m.Dump(TestLineTerminator));
        }


        private static string RemovePadding(string s)
        {
            string[] lines = s.Trim().Split(Environment.NewLine);

            StringBuilder res = new(s.Length);

            foreach (string l in lines)
            {
                res.Append(l.Trim());
                res.Append(TestLineTerminator);
            }

            return res.ToString();
        }


    }
}