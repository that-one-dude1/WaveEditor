using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Text.Json;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace WaveEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string comboSelection;
        private int enemyNum;
        private float sliderNum = 0f;
        private string currentWave;
        private bool isNewLine = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboSelection = e.AddedItems[0].ToString().Split(" ")[1];
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sliderNum = (float)Math.Truncate(e.NewValue * 10) / 10;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(numBox.Text, out _))
                enemyNum = int.Parse(numBox.Text);
        }

        private void waveListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            if ((comboSelection != null && enemyNum != 0 && sliderNum != 0) || (comboSelection == "Delay" && sliderNum != 0))
            {
                string type;
                var boxItems = waveListBox.Items;

                if (comboSelection != "Delay")
                {
                    type = comboSelection.Substring(0, 1).ToUpper();
                    currentWave = currentWave + " > " + type + enemyNum + "@" + sliderNum;
                }
                else
                {
                    type = "+";
                    currentWave = currentWave + " > " + type + sliderNum;
                }

                currentWave = currentWave.TrimStart(' ', '>', ' ');

                if (boxItems.Count == 0 || isNewLine)
                {
                    boxItems.Add(currentWave);
                    isNewLine = false;
                }
                else
                {
                    boxItems[^1] = currentWave;
                }
            }
        }

        private void Del_Button_Click(object sender, RoutedEventArgs e)
        {
            var boxItems = waveListBox.Items;
            List<string> itemList;
            string itemString = "";

            if (boxItems.Count != 0 && isNewLine == false)
            {
                itemList = boxItems[^1].ToString().Split(" > ").ToList();
                itemList.RemoveAt(itemList.Count - 1);

                foreach (var item in itemList)
                {
                    itemString += item + " > ";
                }

                itemString = itemString.TrimEnd(' ', '>', ' ');
                currentWave = itemString;
                boxItems[^1] = itemString;
            }
        }

        private void end_Click(object sender, RoutedEventArgs e)
        {
            if (currentWave != "")
            {
                isNewLine = true;
                currentWave = "";
            }
        }

        private void load_Click(object sender, RoutedEventArgs e)
        {
            string json = File.ReadAllText("waveList");
            List<string> waveList = JsonSerializer.Deserialize<List<string>>(json);
            waveListBox.Items.Clear();
            currentWave = "";

            foreach (var item in waveList)
            {
                waveListBox.Items.Add(item);
            }

            isNewLine = true;
        }

        private void out_Click(object sender, RoutedEventArgs e)
        {
            string json = JsonSerializer.Serialize(waveListBox.Items, jsonOptions);
            File.WriteAllText("waveList", json);
            waveListBox.Items.Clear();
            currentWave = "";
        }
    }

    public class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool booleanValue = (bool)value;
            return !booleanValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool booleanValue = (bool)value;
            return !booleanValue;
        }
    }
}