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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DDCMonitorManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BrightnessControl brightnessControl;
        public MainWindow()
        {
            InitializeComponent();
            Window window = Window.GetWindow(this);
            var wih = new WindowInteropHelper(window);
            IntPtr hWnd = wih.Handle;
            brightnessControl = new BrightnessControl(hWnd);
            InitializeSliders(brightnessControl.GetMonitors());
        }

        private void InitializeSliders(uint count)
        {
            for (int i = 0; i<count; ++i)
            {
                AddSlider(i);
            }
        }

        private void AddSlider(int monitorNumber)
        {
            var mInfo = brightnessControl.GetBrightnessCapabilities(monitorNumber);
            Slider slider = new Slider();
            if (mInfo.current == -1)
            {
                slider.IsEnabled = false;
            }
            slider.Name = "M"+monitorNumber;
            slider.Minimum = mInfo.minimum;
            slider.Maximum = mInfo.maximum;
            slider.IsSnapToTickEnabled = true;
            slider.Width = 200;
            slider.Value = mInfo.current;
            slider.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            slider.ValueChanged += Slider_ValueChanged;

            TextBlock text = new TextBlock();
            text.Width = 50;
            text.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

            Binding b = new Binding();
            b.Source = slider;
            b.Path = new PropertyPath("Value", slider.Value);
            b.Mode = BindingMode.OneWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            text.SetBinding(TextBlock.TextProperty, b);
            Root.Children.Add(slider);
            Grid.SetRow(slider, monitorNumber);
            Grid.SetColumn(slider, 1);
            Root.Children.Add(text);
            Grid.SetRow(text,monitorNumber);
            Grid.SetColumn(text, 0);
            Root.InvalidateVisual();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            int monitorNumber = Int32.Parse(slider.Name.Substring(1));
            var conv = (short)e.NewValue;
            if (brightnessControl != null)
            {
                brightnessControl.SetBrightness(conv,monitorNumber); 
            }
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(this);
            settingsWindow.Show();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
