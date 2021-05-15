using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab8_immod
{
    class DiceRoll
    {
        private List<double> ProbList;
        private Random random = new Random();
        private int N;

        public DiceRoll(int n)
        {
            N = n;
        }
        public void GenerateProbArray()
        {
            ProbList = new List<double>();

            double sum_p = 0.0;

            for (int i = 0; i < N; i++)
            {
                double rand_p = random.NextDouble();
                ProbList.Add(rand_p);
                sum_p += rand_p;
            }

            double new_sum = 0.0;
            for (int i = 0; i < N; i++)
            {
                ProbList[i] /= sum_p;
                new_sum += ProbList[i];
            }
        }

        public int GetRolledSide()
        {
            double A = random.NextDouble();
            int k = 0;

            while (A > 0)
            {
                A -= ProbList[k];
                k++;
            }

            return k;
        }

        
    }
}
