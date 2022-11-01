using System;
namespace neolink.Helpers
{
    public static class Numbers
    {

        public static string GetRandomSequence(int min, int max, int size)
        {
            string sequence = "";

            for(int i=0; i<size; i++)
            {
                sequence += GetNextRandom(min, max);
            }
            return sequence;
        }

        public static double GetNextRandom(int num)
        {
            Random rand = new Random();

            return rand.Next(0, num);
        }

        public static int GetNextRandom(int min, int max)
        {
            Random rand = new Random();

            return (int)rand.Next(min, max);
        }
    }
}

