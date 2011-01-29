using System;

namespace Extinction
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ExtGame game = new ExtGame())
            {
                game.Run();
            }
        }
    }
#endif
}

