using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using ScheduleFormer.ViewModels;

namespace ScheduleFormer.Views
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        public MainWindowView()
        {
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}
