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

        private void AddSlider(int gridPosition)
        {
            var mInfo = brightnessControl.GetBrightness();
            Slider slider = new Slider();
            slider.Minimum = mInfo.minimum;
            slider.Maximum = mInfo.maximum;
            slider.IsSnapToTickEnabled = true;
            slider.Width = 100;
            slider.Value = mInfo.current;
            slider.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            slider.ValueChanged += Slider_ValueChanged;

            TextBlock text = new TextBlock();
            text.Width = 20;
            text.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

            Binding b = new Binding();
            b.Source = slider;
            b.Path = new PropertyPath("Value", slider.Value);
            b.Mode = BindingMode.OneWay;
            b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            text.SetBinding(TextBlock.TextProperty, b);
            Root.Children.Add(slider);
            Grid.SetRow(slider, gridPosition);
            Grid.SetColumn(slider, 1);
            Root.Children.Add(text);
            Grid.SetRow(text,gridPosition);
            Grid.SetColumn(text, 0);
            Root.InvalidateVisual();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var conv = (short)e.NewValue;
            if (brightnessControl != null)
            {
                brightnessControl.SetBrightness(conv); 
            }
        }
    }
}
