using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
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
            OnSerialize = new ECommand(OnSerializeExec);
        }


        public ICommand OnSerialize { get; set; }

        private void OnSerializeExec()
        {
            _ = PropertyGridModel.PictureACTS; 
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
    public class ECommand : ICommand
    {
        private readonly Action act;

        public event EventHandler CanExecuteChanged;
        public ECommand(Action act)
        {
            CanExecuteChanged?.Invoke(this,default);
            this.act = act;
        }
        public bool CanExecute(object parameter)
        { 
            return true;
        }

        public void Execute(object parameter)
        {
            act?.Invoke();
        }
    }
}