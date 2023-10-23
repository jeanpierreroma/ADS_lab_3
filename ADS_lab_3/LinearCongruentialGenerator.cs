using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADS_lab_1
{
    public class LinearCongruentialGenerator
    {
        private readonly long m;
        private readonly long a;
        private readonly long c; 
        private readonly long x_start;

        public LinearCongruentialGenerator(long m, long a, long c, long x_start) 
        {
            this.m = m;
            this.a = a;
            this.c = c;
            this.x_start = x_start;
        }
        public IEnumerable<long> GenerateSequence(long length)
        {
            long xStart = x_start;

            for (long i = 0; i < length; i++)
            {
                long nextNumber = (a * xStart + c) % m;
                yield return nextNumber;
                xStart = nextNumber;
            }
        }

        public int FindPeriod(IEnumerable<long> sequence)
        {
            int period = 0;
            long firstNumber = sequence.First();

            foreach (var number in sequence)
            {
                if (number == firstNumber && period != 0)
                {
                    break;
                }
                period++;
            }

            return period;
        }
    }
}
