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
using System.Windows.Shapes;

namespace MAI
{
    /// <summary>
    /// Логика взаимодействия для ResultWindow.xaml
    /// </summary>
    public partial class ResultWindow : Window
    {
        double[] result = new double[0];

        List<string> alternatives = new List<string>();

        List<Expert> expertsList = new List<Expert>();

        public ResultWindow()
        {
            InitializeComponent();
        }

        public void AddAlternatives(List<string> alternatives)
        {
            this.alternatives = alternatives;
        }

        public void AddExperts(Expert[] expertsList)
        {
            this.expertsList = expertsList.ToList();
            CalculateResult();
        }

        public void CalculateResult()
        {
            
            double[][] alternatives = new double[expertsList[0].Alternatives[0].Length][];
            var experts = expertsList.Select((e) => e.Model.ModelResult.Array).ToList();
            for(int i = 0; i < alternatives.Length; i++)
            {
                alternatives[i] = new double[expertsList[0].Model.CalculatedCriteria.Array[0].Length];
                for(int i2 = 0; i2< expertsList[0].Model.CalculatedCriteria.Array[0].Length; i2++)
                {
                    alternatives[i][i2] = experts.Select((e) => e[i][i2]).Max();
                }
            }

            result = new double[alternatives.Length];

            for (int i = 0; i < expertsList.Count; i++)
            {
                for (int i2 = 0; i2 < alternatives.Length; i2++)
                {
                    double[] criterias = expertsList[i].Model.CalculatedCriteria.Array[0];

                    for (int i3 = 0; i3 < criterias.Length; i3++)
                    {
                        result[i2] += criterias[i3] * alternatives[i2][i3];
                    }
                }
            }

            DataGridTextColumn column = new DataGridTextColumn()
            {
                Header = "Alternatives"
            };
            var binding = new Binding(".Name");
            column.Binding = binding;

            ResultDataGrid.Columns.Add(column);

            DataGridTextColumn column2 = new DataGridTextColumn()
            {
                Header = "Value"
            };
            var binding2 = new Binding(".Value");
            column2.Binding = binding2;

            ResultDataGrid.Columns.Add(column2);

            var info = new { Name = this.alternatives[0], Value = result[0] };
            var list = (new[] { info }).ToList();
            for(int i = 1; i<this.alternatives.Count; i++)
            {
                list.Add(new { Name = this.alternatives[i], Value = result[i] });
            }
            ResultDataGrid.ItemsSource = list;

            resultTextBox.Text = "Оптимальний вибір це: ";
            for(int i = 0; i<list.Count; i++)
            {
                if(list[i].Value == list.Select((o) => o.Value).Max())
                {
                    resultTextBox.Text += this.alternatives[i] + "; ";
                }
            }
        }

        public void Clear()
        {
            ResultDataGrid.ItemsSource = null;
            result = new double[0];
        }
    }
}
