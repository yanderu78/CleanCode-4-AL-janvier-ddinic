using System;
using System.Threading;

namespace CleanCode
{
    class Program
    {
        static void Main(string[] args)
        {
            Decipherer decipherer = new Decipherer();
            decipherer.TranslateEntrys(args);
        }
    }
}
