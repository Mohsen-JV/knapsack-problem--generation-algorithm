using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Koleh_poshti_takamoli
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        List<article> articles;
        Random rand = new Random();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            string[] lg = listOfInd.Text.Split('\n');
            articles = new List<article>();
            foreach (var item in lg)
            {
                articles.Add(new article() { weight = int.Parse(item.Split(',')[0]), value = int.Parse(item.Split(',')[1]) });
            }
            var Pop = firstPopulation();
            int nPar = int.Parse(numOfPar.Text);
            int nGen = int.Parse(numOfGen.Text);
            var maxChild = new indv(articles);
            for (int i = 0; i < nGen; i++)
            {
                var parent = rouletweelSelection(Pop, nPar);
                Pop = rouletweelSelection(parent, nPar);
                for (int j = 0; j < nPar / 2; j++)
                {
                    if (Pop[j].fitness > maxChild.fitness) maxChild = Pop[j];
                    if (Pop[j + 1].fitness > maxChild.fitness) maxChild = Pop[j + 1];
                    var childs = xOver(Pop[j], Pop[j + 1]);
                    Pop.Add(childs.Item1);
                    if (childs.Item1.fitness > maxChild.fitness) maxChild = childs.Item1;
                    Pop.Add(childs.Item2);
                    if (childs.Item2.fitness > maxChild.fitness) maxChild = childs.Item2;
                }
            }
            watch.Stop();
            MessageBox.Show(maxChild.fitness.ToString() + '\n' + $"Execution Time: {watch.ElapsedMilliseconds} ms");
        }

        List<indv> firstPopulation()
        {
            int num = int.Parse(numOfPub.Text);
            int maxw = int.Parse(maxWeight.Text);
            var population = new List<indv>();
            while (num-- > 0)
            {
                bool[] gen = new bool[articles.Count];
                for (int i = 0; i < gen.Length; i++) gen[i] = rand.Next(0, 2) == 1 ? true : false;
                population.Add(new indv(gen, articles, maxw));
            }
            return population;
        }

        List<indv> rouletweelSelection(List<indv> pop, int num)
        {
            var sel = new List<indv>();
            int lengthRoulet = 0;
            foreach (var item in pop) lengthRoulet += item.fitness;
            while (num-- > 0)
            {
                int pointer = rand.Next(0, lengthRoulet);
                foreach (var item in pop)
                {
                    if (pointer <= item.fitness)
                    {
                        sel.Add(item);
                        break;
                    }
                    pointer -= item.fitness;
                }
            }
            return sel;
        }

        (indv, indv) xOver(indv p1, indv p2)
        {
            int point1 = rand.Next(1, p1.jen.Length);
            int point2 = rand.Next(point1, p1.jen.Length);
            bool[] ch1jen = new bool[p1.jen.Length];
            bool[] ch2jen = new bool[p1.jen.Length];
            for (int i = 0; i < p1.jen.Length; i++)
            {
                if (point1 == point2)
                {
                    ch1jen[i] = i < point1 ? p1.jen[i] : p2.jen[i];
                    ch2jen[i] = i < point1 ? p2.jen[i] : p1.jen[i];
                }
                else
                {
                    ch1jen[i] = i < point1 ? p1.jen[i] : (i < point2 ? p2.jen[i] : p1.jen[i]);
                    ch2jen[i] = i < point1 ? p2.jen[i] : (i < point2 ? p1.jen[i] : p2.jen[i]);
                }
            }
            indv ch1 = new indv(ch1jen, articles, int.Parse(maxWeight.Text));
            indv ch2 = new indv(ch2jen, articles, int.Parse(maxWeight.Text));
            if (rand.Next(0, 100) < 30) ch1 = mutation(ch1.jen);
            if (rand.Next(0, 100) < 30) ch2 = mutation(ch2.jen);
            return (ch1, ch2);
        }
        indv mutation(bool[] jen)
        {
            int i = rand.Next(jen.Length);
            jen[i] = jen[i] ? false : true;
            return new indv(jen, articles, int.Parse(maxWeight.Text));
        }
    }
    class article
    {
        public int weight;
        public int value;
    }
    class indv
    {
        public bool[] jen;
        public int fitness;
        public indv(List<article> ars)
        {
            jen = new bool[ars.Count];
            fitness = 0;
        }
        public indv(bool[] j, List<article> ars, int max)
        {
            jen = j;
            int value = 0;
            int w = 0;
            for (int i = 0; i < jen.Length; i++)
            {
                value += jen[i] ? ars[i].value : 0;
                w += jen[i] ? ars[i].weight : 0;
            }
            fitness = w <= max ? value : 0;
        }
    }
}
