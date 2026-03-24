using System.Windows;
using Jaezoo.Client.Services;
using Jaezoo.Client.Wpf.ViewModels;

namespace Jaezoo.Client.Wpf.Views;

public partial class MainWindow : Window
{
    public MainWindow(ApiClient api, Session session)
    {
        InitializeComponent();
        var vm = new MainViewModel(api, session, Dispatcher);
        DataContext = vm;
        Loaded += async (_, __) => await vm.RefreshAsync();
    }
}
