using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab8_immod
{
    public partial class Form1 : Form
    {
        private List<double> Statistics;
        private List<double> ProbList;
        private double theory_chi = 11.070; //Для N = 5 при альфа = 0,05

        private double prob_1;
        private double prob_2;
        private double prob_3;
        private double prob_4;
        private double prob_5;
        private int N = 1;
        private Random random = new Random();

        public Form1()
        {
            InitializeComponent();
            Statistics = new List<double>();
            ProbList = new List<double>();

        }

        private void buttonResult_Click(object sender, EventArgs e)
        {
            foreach (var series in chart1.Series)
            {
                series.Points.Clear();
            }

            ReadAndCheckData();
            ModellingStatisticsCount();
            GetResults();
            CountStats();
        }
        void ReadAndCheckData()
        {
            N = (int)numericUpDownN.Value;
            double p_sum = 0.0;

            Statistics.Clear();
            ProbList.Clear();

            Statistics.AddRange(Enumerable.Repeat(0d, 5));
            prob_1 = (double)numericUpDown1.Value;
            prob_2 = (double)numericUpDown2.Value;
            prob_3 = (double)numericUpDown3.Value;
            prob_4 = (double)numericUpDown4.Value;

            p_sum = prob_1 + prob_2 + prob_3 + prob_4;
            if (p_sum > 1)
            {
                labelErr.Text = "Sum of 1-4 probability > 1";
                prob_1 /= p_sum;
                prob_2 /= p_sum;
                prob_3 /= p_sum;
                prob_4 /= p_sum;
            }

            prob_5 = 1 - p_sum;
            labelProb5.Text = prob_5.ToString();

            ProbList.Add(prob_1);
            ProbList.Add(prob_2);
            ProbList.Add(prob_3);
            ProbList.Add(prob_4);
            ProbList.Add(prob_5);
        }

        void ModellingStatisticsCount()
        {
            for (int i = 0; i < N; i++)
            {
                int k = GetEventHappenedK();
                Statistics[k - 1] += 1;
            }

            for (int j = 0; j < Statistics.Count(); j++)
            {
                Statistics[j] /= N;
            }
        }

        public int GetEventHappenedK()
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

        
        void GetResults()
        {
            for (int j = 0; j < Statistics.Count(); j++)
                chart1.Series[0].Points.AddXY(j + 1, Statistics[j]);
        }

        void CountStats()
        {
            double rme = CountRelativeMeanError() * 100;
            double rve = CountRelativeVarError() * 100;
            int rme_i = (int)rme;
            int rve_i = (int)rve;

            double chi_test_res = ChiSquaredTest();
            bool result = chi_test_res > theory_chi;

            labelAverage.Text = CountMean().ToString() + "(" + rme_i.ToString() + "%)";
            labelVariance.Text = CountVar().ToString() + "(" + rve_i.ToString() + "%)";           
            labelChi2.Text = chi_test_res.ToString() + " > " + theory_chi.ToString();          
            labelRes.Text = result.ToString();
        }

        double CountMean()
        {
            double mean = 0.0;

            for (int x = 0; x < ProbList.Count; x++)
                mean += ProbList[x] * (x + 1);

            return mean;
        }

        double CountVar()
        {
            double mean_square = CountMean();
            mean_square *= mean_square;

            double var = 0.0;

            for (int x = 0; x < ProbList.Count; x++)
                var += ProbList[x] * (x + 1) * (x + 1);

            var -= mean_square;

            return var;
        }   
      
        double CountEmpiricExpectation()
        {
            double mean = 0.0;
            for (int x = 0; x < Statistics.Count; x++)
                mean += Statistics[x] * (x + 1);

            return mean;
        }

        double CountEmpiricVar()
        {
            double emp_mean_square = CountEmpiricExpectation();
            emp_mean_square *= emp_mean_square;

            double var = 0.0;

            for (int x = 0; x < Statistics.Count; x++)
                var += Statistics[x] * (x + 1) * (x + 1);

            var -= emp_mean_square;

            return var;
        }

        double CountAbsoluteMeanError()
        {
            return Math.Abs(CountEmpiricExpectation() - CountMean());
        }

        double CountAbsoluteVarError()
        {
            return Math.Abs(CountEmpiricVar() - CountVar());
        }

        double CountRelativeMeanError()
        {
            return CountAbsoluteMeanError() / Math.Abs(CountMean());
        }

        double CountRelativeVarError()
        {
            return CountAbsoluteVarError() / Math.Abs(CountVar());
        }

        double ChiSquaredTest()
        {
            double xi_square = 0;

            for (int x = 0; x < Statistics.Count; x++)
                xi_square += ((Statistics[x] - ProbList[x]) * (Statistics[x] - ProbList[x])) / ProbList[x];

            return xi_square *= N;
        }        
    }
}
