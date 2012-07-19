using System;
using System.Windows;
using System.Windows.Controls;
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
        }

        private void input_TextChanged(object sender, TextChangedEventArgs e)
        {
            string specification = input.Text;
            output.Text = NUnitTestCodeGenerator.GenerateCode(specification);
        }

        private void input_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                input.Text += Environment.NewLine + "    - ";
                input.CaretIndex = input.Text.Length;
                e.Handled = true;
            }
        }

        private void copyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(output.Text);
            output.Focus();
            output.Select(0, output.Text.Length);
        }
    }
}
