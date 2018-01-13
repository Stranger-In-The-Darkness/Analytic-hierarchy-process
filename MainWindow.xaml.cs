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
using MAI;

namespace MAI
{
    public struct Expert
    {
        #region Variables
        string name;

        int criterias, alternatives;

        double[][] ratedCriterias;

        List<double[][]> ratedAlternatives;

        AHPmodel model;
        #endregion

        #region Constructors
        public Expert(string name, int criterias, int alternatives)
        {
            ratedCriterias = new double[0][];
            ratedAlternatives = new List<double[][]>();
            this.name = name;
            this.criterias = criterias;
            this.alternatives = alternatives;
            ratedCriterias = new double[criterias][];
            for(int i = 0; i<criterias; i++)
            {
                ratedCriterias[i] = new double[criterias];
                for(int i2 = 0; i2<criterias; i2++)
                {
                    ratedCriterias[i][i2] = 1;
                }
                ratedAlternatives.Add(new double[alternatives][]);
                for(int i2 = 0; i2<alternatives; i2++)
                {
                    ratedAlternatives.Last()[i2] = new double[alternatives];
                    for(int i3 = 0; i3<alternatives; i3++)
                    {
                        ratedAlternatives.Last()[i2][i3] = 1;
                    }
                }
            }
            this.model = null;
        }

        public Expert(Expert expert, string newName = "")
        {
            if (newName != "")
            {
                this.name = newName;
            }
            else
            {
                this.name = expert.name;
            }
            this.ratedCriterias = expert.ratedCriterias;
            this.criterias = expert.criterias;
            this.ratedAlternatives = new List<double[][]>();
            this.alternatives = expert.alternatives;
            for(int i = 0; i< expert.ratedAlternatives.Count; i++)
            {
                ratedAlternatives.Add(expert.ratedAlternatives[i]);
            }
            this.model = null;
        }
        #endregion

        #region Properties
        public double[][] Criterias
        {
            get
            {
                return ratedCriterias;
            }
            set
            {
                ratedCriterias = value;
            }
        }

        public List<double[][]> Alternatives
        {
            get
            {
                return ratedAlternatives;
            }
            set
            {
                ratedAlternatives = value;
            }
        }

        public AHPmodel Model
        {
            get
            {
                return model;
            }
        }
#endregion

        public void AddCriteria()
        {
            criterias++;
            var crlist = ratedCriterias.ToList();
            crlist.Add(new double[criterias]);
            ratedCriterias = crlist.ToArray();
            for (int i = 0; i < criterias; i++)
            {
                var list = ratedCriterias[i].ToList();
                if (list.Count < criterias)
                {
                    list.Add(1);
                }
                ratedCriterias[i][i] = 1;
                ratedCriterias[ratedCriterias.Length - 1][i] = 1;
                ratedCriterias[i] = list.ToArray();
            }

            for(int i = 0; i<criterias; i++)
            {
                ratedCriterias[i][i] = 1;
            }

            ratedAlternatives.Add(new double[alternatives][]);
            for(int i = 0; i<alternatives; i++)
            {
                ratedAlternatives.Last()[i] = new double[alternatives];
                for(int i2 = 0; i2<alternatives; i2++)
                {
                    ratedAlternatives.Last()[i][i2] = 1;
                }
            }
        }

        public void AddAlternative()
        {
            alternatives++;
            for (int i = 0; i < criterias; i++)
            {
                var allist = ratedAlternatives[i].ToList();
                allist.Add(new double[alternatives]);
                ratedAlternatives[i] = allist.ToArray();
                for (int i2 = 0; i2 < alternatives; i2++)
                {
                    var list = ratedAlternatives[i][i2].ToList();
                    if (list.Count < alternatives)
                    {
                        list.Add(1);
                    }
                    ratedAlternatives[i][i2][i2] = 1;
                    ratedAlternatives[i][ratedAlternatives[i].Length - 1][i2] = 1;
                    ratedAlternatives[i][i2] = list.ToArray();
                }
            }

            for(int i = 0; i<criterias; i++)
            {
                for(int j = 0; j<alternatives; j++)
                {
                    ratedAlternatives[i][j][j] = 1;
                }
            }
        }

        public void DeleteCriteria(int index)
        {
            criterias--;
            var crlist = ratedCriterias.ToList();
            crlist.RemoveAt(index);
            ratedCriterias = crlist.ToArray();
            for (int i = 0; i < crlist.Count; i++)
            {
                var list = crlist[i].ToList();
                list.RemoveAt(index);
                ratedCriterias[i] = list.ToArray();
            }

            ratedAlternatives.RemoveAt(index);
        }

        public void DeleteAlternative(int index)
        {
            alternatives--;
            for (int i = 0; i < criterias; i++)
            {
                var allist = ratedAlternatives[i].ToList();
                allist.RemoveAt(index);
                ratedAlternatives[i] = allist.ToArray();
                for (int i2 = 0; i2 < allist.Count; i2++)
                {
                    var list = allist[i2].ToList();
                    list.RemoveAt(index);
                    ratedAlternatives[i][i2] = list.ToArray();
                }
            }
        }

        public void CalculateResult()
        {
            for(int i = 0; i<criterias; i++)
            {
                ratedCriterias[i][i] = 1;
                for(int i2 = 0; i2<alternatives; i2++)
                {
                    ratedAlternatives[i][i2][i2] = 1;
                }
            }
            model = new AHPmodel(criterias, alternatives);
            model.AddCriteria(ratedCriterias);
            for(int i = 0; i<criterias; i++)
            {
                model.AddCriterionRatedChoices(i, ratedAlternatives[i]);
            }
            model.CalculateModel();
        }

        public double this[int criteria1, int criteria2]
        {
            get
            {
                return this.ratedCriterias[criteria1][criteria2];
            }
            set
            {
                if(value >= 0 && value <= 9)
                {
                    this.ratedCriterias[criteria1][criteria2] = value;
                    this.ratedCriterias[criteria2][criteria1] = value != 0 ? 1.0 / value : 0.0;
                    this.criterias = ratedCriterias.Length;
                }
            }
        }

        public double this[int criteria, int alternative1, int alternative2]
        {
            get
            {
                return ratedAlternatives[criteria][alternative1][alternative2];
            }
            set
            {
                if (value >= 0 && value <= 9)
                {
                    this.ratedAlternatives[criteria][alternative1][alternative2] = value;
                    this.ratedAlternatives[criteria][alternative2][alternative1] = value != 0 ? 1.0 / value : 0.0;
                    this.alternatives = ratedAlternatives[0].Length;
                }
            }
        }
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*Мур-мур. Идея насчёт эксперта
         * Запизнуть всё это в класс Expert,
         * накидать туда на всё get, set и методы
         */
        #region Expert class
        public int criterias = 0, alternatives = 0;
        public List<int> listOfNodeNumbers = new List<int>();
        public List<string> listOfLevelsNames = new List<string>();
        public List<string> listOfCriteriasNames = new List<string>(); //Список критериев
        public List<string> listOfChoiceNames = new List<string>(); //Список альтернатив
        List<string> expertsNames = new List<string>();

        Expert model = new Expert("Model", 0, 0);

        Expert[] expertList = new Expert[0];

        bool ready = false;
        //double[][] ratedCriterias = new double[0][];
        //List<double[][]> ratedAlternatives = new List<double[][]>();
        #endregion

        //Список экспертов. 
        //Вместо прямого обращения к матрице критериев и альтернатив, использовать их же,
        //только из элементов отсюда.
        //Через ComboBox на экспертов, и его SelectedIndex
        //List<Expert> expertList = new List<Expert>();

        public static List<AHPmodel> models = new List<AHPmodel>();

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Experts
        private void ExpertsAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (expertsEnterTextBox.Text != "")
            {
                if (!expertsNames.Contains(expertsEnterTextBox.Text))
                {
                    expertsNames.Add(expertsEnterTextBox.Text);
                    expertListBox.ItemsSource = null;
                    expertListBox.ItemsSource = expertsNames;
                    expertChooseListBox.ItemsSource = null;
                    expertChooseListBox.ItemsSource = expertsNames;
                    expertsEnterTextBox.Text = "";
                }
                else
                {
                    MessageBox.Show("В моделі вже є trcgthn з таким ім'ям!");
                }
            }
            else
            {
                MessageBox.Show("Порожні ім'я експертів заборонені!");
            }
        }

        private void ExpertsDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (expertListBox.SelectedIndex != -1)
            {
                expertsNames.RemoveAt(expertListBox.SelectedIndex);

                model.DeleteAlternative(expertListBox.SelectedIndex);
                expertListBox.ItemsSource = null;
                expertListBox.ItemsSource = expertsNames;
            }
            else
            {
                MessageBox.Show("Спочатку виберіть експерта!");
            }
        }

        private void ExpertsEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (alternativesListBox.SelectedIndex != -1)
            {
                expertsEnterTextBox.Text = expertsNames[expertListBox.SelectedIndex];
                expertsNames.RemoveAt(expertListBox.SelectedIndex);
                expertListBox.ItemsSource = null;
                expertListBox.ItemsSource = expertsNames;
            }
            else
            {
                MessageBox.Show("Спочатку виберіть експерта!");
            }
        }
        #endregion

        #region Criterias
        public void FillCriterias()
        {
            criteriasDataGrid.Columns.Clear();
            int count = 1;
            criteriasСomboBox1.ItemsSource = listOfCriteriasNames.Select((s) => s = $"K{count++} - {s}").ToList();
            criteriasСomboBox1.SelectedIndex = 0;
            count = 1;
            criteriasСomboBox2.ItemsSource = listOfCriteriasNames.Select((s) => s = $"K{count++} - {s}").ToList();
            criteriasСomboBox2.SelectedIndex = 0;
            count = 1;
            alternativeCriteriasComboBox.ItemsSource = listOfCriteriasNames.Select((s) => s = $"K{count++} - {s}").ToList();
            criteriasСomboBox2.SelectedIndex = 0;

            for (int i = 0; i < expertList[0].Criterias.Length; i++)
            {
                var col = new DataGridTextColumn
                {
                    Header = $"K{i + 1}"
                };
                var binding = new Binding($"[{i}]");
                col.Binding = binding;
                criteriasDataGrid.Columns.Add(col);
            }
            criteriasDataGrid.ItemsSource = expertList[expertChooseListBox.SelectedIndex].Criterias;

            PrioritiesSelector selector = new PrioritiesSelector();
            selector.ComputePriorities(new DotNetMatrix.GeneralMatrix(expertList[expertChooseListBox.SelectedIndex].Criterias));
            coeficientyIndexTextBlock.Text = $"Індекс узгодженності: {(selector.ConsistencyRatio.Equals(double.NaN) ? 0 : selector.ConsistencyRatio).ToString()}";
        }

        private void ChangeCriterias()
        {
            criteriasDataGrid.ItemsSource = null;
            criteriasDataGrid.ItemsSource = expertList[expertChooseListBox.SelectedIndex].Criterias;

            PrioritiesSelector selector = new PrioritiesSelector();
            selector.ComputePriorities(new DotNetMatrix.GeneralMatrix(expertList[expertChooseListBox.SelectedIndex].Criterias));
            coeficientyIndexTextBlock.Text = $"Індекс узгодженності: {(selector.ConsistencyRatio.Equals(double.NaN) ? 0 : selector.ConsistencyRatio).ToString()}";
        }

        private void CriteriasRateListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(criteriasСomboBox1.SelectedIndex != -1 && criteriasСomboBox2.SelectedIndex != -1)
            {
                if (criteriasСomboBox1.SelectedIndex != criteriasСomboBox2.SelectedIndex)
                {
                    Expert exp = expertList[expertChooseListBox.SelectedIndex];
                    exp[criteriasСomboBox1.SelectedIndex, criteriasСomboBox2.SelectedIndex] = criteriasRateListBox.SelectedIndex;
                    expertList[expertChooseListBox.SelectedIndex] = exp;
                    ChangeCriterias();
                }
            }
        }

        private void CriteriasEnterTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (criteriasEnterTextBox.IsFocused && e.Key == Key.Enter)
            {
                if(criteriasEnterTextBox.Text != "")
                {
                    if (!listOfCriteriasNames.Contains(criteriasEnterTextBox.Text))
                    {
                        listOfCriteriasNames.Add(criteriasEnterTextBox.Text);
                        criteriasListBox.ItemsSource = null;
                        criteriasListBox.ItemsSource = listOfCriteriasNames;
                        criteriasEnterTextBox.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("В моделі вже є критерій з таким ім'ям!");
                    }
                }
                else
                {
                    MessageBox.Show("Порожні критерії заборонені!");
                }
            }
        }

        private void CriteriasAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (criterias < 20)
            {
                if (criteriasEnterTextBox.Text != "")
                {
                    if (!listOfCriteriasNames.Contains(criteriasEnterTextBox.Text))
                    {
                        listOfCriteriasNames.Add(criteriasEnterTextBox.Text);
                        criterias++;
                        criteriasListBox.ItemsSource = null;
                        criteriasListBox.ItemsSource = listOfCriteriasNames;
                        model.AddCriteria();
                        if (expertList.Count() != 0)
                        {
                            for (int i = 0; i < expertList.Count(); i++)
                            {
                                Expert expert = expertList[i];
                                expert.AddCriteria();
                                expertList[i] = expert;
                            }
                        }
                        criteriasEnterTextBox.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("В моделі вже є критерій з таким ім'ям!");
                    }
                }
                else
                {
                    MessageBox.Show("Порожні критерії заборонені!");
                }
            }
            else
            {
                MessageBox.Show("Більше 20 критеріїв не підтримуються!");
            }
        }

        private void CriteriasDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if(criteriasListBox.SelectedIndex != -1)
            {
                listOfCriteriasNames.RemoveAt(criteriasListBox.SelectedIndex);

                model.DeleteCriteria(criteriasListBox.SelectedIndex);
                if(expertList.Count() != 0)
                {
                    for(int i = 0; i < expertList.Count(); i++)
                    {
                        Expert expert = expertList[i];
                        expert.DeleteCriteria(criteriasListBox.SelectedIndex);
                        expertList[i] = expert;
                    }
                }

                //alternativeCriteriasComboBox.ItemsSource = listOfCriteriasNames.Select((s) => s = $"K{count++} - {s}").ToList();
                criteriasСomboBox2.SelectedIndex = 0;

                criteriasListBox.ItemsSource = null;
                criteriasListBox.ItemsSource = listOfCriteriasNames;
            }
            else
            {
                MessageBox.Show("Спочатку виберіть критерій!");
            }
        }

        private void CriteriasEditButton_Click(object sender, RoutedEventArgs e)
        {
            if(criteriasListBox.SelectedIndex != -1)
            {
                criteriasEnterTextBox.Text = listOfCriteriasNames[criteriasListBox.SelectedIndex];
                listOfCriteriasNames.RemoveAt(criteriasListBox.SelectedIndex);
                model.DeleteCriteria(criteriasListBox.SelectedIndex);
                criteriasListBox.ItemsSource = null;
                criteriasListBox.ItemsSource = listOfCriteriasNames;
            }
            else
            {
                MessageBox.Show("Спочатку виберіть критерій!");
            }
        }

        private void CriteriasReadyButton_Click(object sender, RoutedEventArgs e)
        {
            FillCriterias();
        }
        #endregion

        #region Alternatives
        public void FillAlternatives()
        {
            alternativesDataGrid.Columns.Clear();
            int count = 1;
            AlternativeComboBox1.ItemsSource = listOfChoiceNames.Select((s) => s = $"A{count++} - {s}").ToList();
            AlternativeComboBox1.SelectedIndex = 0;
            count = 1;
            AlternativeComboBox2.ItemsSource = listOfChoiceNames.Select((s) => s = $"A{count++} - {s}").ToList();
            AlternativeComboBox2.SelectedIndex = 0;

            for (int i = 0; i < model.Alternatives[0][0].Length; i++)
            {
                var col = new DataGridTextColumn()
                {
                    Header = $"A{i + 1}"
                };
                var binding = new Binding($"[{i}]");
                col.Binding = binding;
                alternativesDataGrid.Columns.Add(col);
            }
            alternativesDataGrid.ItemsSource = expertList[expertChooseListBox.SelectedIndex].Alternatives[0];

            alternativeCriteriasComboBox.SelectedIndex = 0;
        }

        private void ChangeAlternatives()
        {
            alternativesDataGrid.ItemsSource = null;
            alternativesDataGrid.ItemsSource = expertList[expertChooseListBox.SelectedIndex].Alternatives[alternativeCriteriasComboBox.SelectedIndex];
        }

        private void AlternativesListBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (alternativesEnterTextBox.IsFocused && e.Key == Key.Enter)
            {
                if (alternativesEnterTextBox.Text != "")
                {
                    if (!listOfChoiceNames.Contains(alternativesEnterTextBox.Text))
                    {
                        listOfChoiceNames.Add(alternativesEnterTextBox.Text);
                        alternativesListBox.ItemsSource = null;
                        alternativesListBox.ItemsSource = listOfChoiceNames;
                        alternativesEnterTextBox.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("В моделі вже є альтернатива з таким ім'ям!");
                    }
                }
                else
                {
                    MessageBox.Show("Порожні альтернативи заборонені!");
                }
            }
            ChangeAlternatives();
        }

        private void AlternativesAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (alternatives < 20)
            {
                if (alternativesEnterTextBox.Text != "")
                {
                    if (!listOfChoiceNames.Contains(alternativesEnterTextBox.Text))
                    {
                        listOfChoiceNames.Add(alternativesEnterTextBox.Text);
                        alternatives++;
                        alternativesListBox.ItemsSource = null;
                        alternativesListBox.ItemsSource = listOfChoiceNames;
                        alternativesEnterTextBox.Text = "";

                        model.AddAlternative();
                        alternativesEnterTextBox.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("В моделі вже є альтернатива з таким ім'ям!");
                    }
                }
                else
                {
                    MessageBox.Show("Порожні альтернативи заборонені!");
                }
            }
            else
            {
                MessageBox.Show("Більше 20 альтернатив не підтримуються!");
            }
        }

        private void AlternativesDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (alternativesListBox.SelectedIndex != -1)
            {
                listOfChoiceNames.RemoveAt(alternativesListBox.SelectedIndex);
                alternatives--;

                model.DeleteAlternative(alternativesListBox.SelectedIndex);
                alternativesListBox.ItemsSource = null;
                alternativesListBox.ItemsSource = listOfChoiceNames;
            }
            else
            {
                MessageBox.Show("Спочатку виберіть альтернативу!");
            }
        }

        private void AlternativesEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (alternativesListBox.SelectedIndex != -1)
            {
                alternativesEnterTextBox.Text = listOfChoiceNames[alternativesListBox.SelectedIndex];
                listOfChoiceNames.RemoveAt(alternativesListBox.SelectedIndex);
                model.DeleteAlternative(alternativesListBox.SelectedIndex);
                alternativesListBox.ItemsSource = null;
                alternativesListBox.ItemsSource = listOfChoiceNames;
            }
            else
            {
                MessageBox.Show("Спочатку виберіть альтернативу!");
            }
        }

        private void AlternativesReadyButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AlternativeCriteriasComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeAlternatives();
        }

        private void AlternativesRateListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AlternativeComboBox1.SelectedIndex != -1 && AlternativeComboBox2.SelectedIndex != -1)
            {
                if (AlternativeComboBox1.SelectedIndex != AlternativeComboBox2.SelectedIndex)
                {
                    Expert exp = expertList[expertChooseListBox.SelectedIndex];
                    exp[alternativeCriteriasComboBox.SelectedIndex, AlternativeComboBox1.SelectedIndex, AlternativeComboBox2.SelectedIndex] = alternativesRateListBox.SelectedIndex;
                    expertList[expertChooseListBox.SelectedIndex] = exp;
                    ChangeAlternatives();
                }
            }
        }
        #endregion

        private void ReadyButton_Click(object sender, RoutedEventArgs e)
        {
            CriteriasGroupBox.IsEnabled = true;
            CriteriasMatrixGroupBox.IsEnabled = true;
            AlternativeGroupBox.IsEnabled = true;
            AlternativeMatrixGroupBox.IsEnabled = true;

            dataGroupBoxStakPanel.IsEnabled = false;
            readyButton.IsEnabled = false;
            changeButton.IsEnabled = true;


            var m = expertList.ToList();
            m.Clear();
            expertList = m.ToArray();
            for (int i = 0; i < expertsNames.Count; i++)
            {
                var l = expertList.ToList();
                l.Add(new Expert(expertsNames[i], model.Criterias.Length, model.Alternatives[0].Length));
                expertList = l.ToArray();
            }

            expertChooseListBox.SelectedIndex = 0;

            FillCriterias();
            FillAlternatives();

            ready = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ResultWindow window = new ResultWindow();
            bool ready = true;
            for(int i = 0; i<expertList.Count(); i++)
            {
                try
                {
                    Expert expert = expertList[i];
                    expert.CalculateResult();
                    expertList[i] = expert;
                }
                catch (Exception ex)
                {
                    ready = false;
                    MessageBox.Show(ex.Message);
                }
            }
            if (ready)
            {
                window.AddAlternatives(listOfChoiceNames);
                window.AddExperts(expertList);
                window.Show();
                return;
            }
            window.Clear();
        }

        private void ExpertChooseListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ready)
            {
                criteriasDataGrid.ItemsSource = null;
                criteriasDataGrid.ItemsSource = expertList[expertChooseListBox.SelectedIndex].Criterias;
                alternativesDataGrid.ItemsSource = null;
                alternativesDataGrid.ItemsSource = expertList[expertChooseListBox.SelectedIndex].Alternatives[alternativeCriteriasComboBox.SelectedIndex];
                PrioritiesSelector selector = new PrioritiesSelector();
                selector.ComputePriorities(new DotNetMatrix.GeneralMatrix(expertList[expertChooseListBox.SelectedIndex].Criterias));
                coeficientyIndexTextBlock.Text = "Індекс узгодженності: " + selector.ConsistencyRatio.ToString();
            }
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            CriteriasGroupBox.IsEnabled = false;
            CriteriasMatrixGroupBox.IsEnabled = false;
            AlternativeGroupBox.IsEnabled = false;
            AlternativeMatrixGroupBox.IsEnabled = false;

            dataGroupBoxStakPanel.IsEnabled = true;
            readyButton.IsEnabled = true;
            changeButton.IsEnabled = false;

            ready = false;

            FillCriterias();
            FillAlternatives();
        }
    }
}
