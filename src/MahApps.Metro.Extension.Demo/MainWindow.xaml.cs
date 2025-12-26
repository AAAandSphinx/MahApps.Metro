using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Extension.Demo.Models;
#nullable disable
namespace MahApps.Metro.Extension.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
    public class MainWindowViewModel : ViewModelBase
    {
        public PropertyGridModelTest PropertyGridModel { get; set; }
        public MainWindowViewModel()
        {
            PropertyGridModel = new PropertyGridModelTest();
        }




    }
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected virtual void Raise([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}