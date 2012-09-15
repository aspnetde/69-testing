using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace SpecificationTextToTestConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            input.Focus();
        }

        private void input_TextChanged(object sender, TextChangedEventArgs e)
        {
            string specification = input.Text;
            richOutput.Document = NUnitTestCodeGenerator.GenerateCode(specification);
        }

        private void input_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int remainingCharactersToEnd = input.Text.Length - input.CaretIndex;

                input.Text = input.Text.Insert(input.CaretIndex, Environment.NewLine + "    - ");
                input.CaretIndex = input.Text.Length - remainingCharactersToEnd;
                e.Handled = true;
            }
        }

        private void copyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            string text = new TextRange(richOutput.Document.ContentStart, richOutput.Document.ContentEnd).Text.Trim();
            Clipboard.SetText(text);
            richOutput.Focus();
            richOutput.SelectAll();
        }
    }
}
