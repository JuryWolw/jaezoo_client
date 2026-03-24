using Jaezoo.Client.ViewModels;
using Jaezoo.Client.Services;
using Jaezoo.Client;
using System.Windows;

namespace Jaezoo.Client.Wpf.Views;

public partial class AuthWindow : Window
{
    private readonly Session _session = new();
    private readonly ApiClient _api;

    public AuthWindow()
    {
        InitializeComponent();
        _api = new ApiClient(_session);
        var vm = new AuthViewModel(_api);
        vm.LoggedIn += OnLoggedIn;
        DataContext = vm;
    }

    private void OnLoggedIn()
    {
        var mw = new MainWindow(_api, _session);
        mw.Show();
        Close();
    }

    private void LPass_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is AuthViewModel vm) vm.LPassword = LPass.Password;
    }
    private void RPass_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is AuthViewModel vm) vm.RPassword = RPass.Password;
    }
    private void RConf_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is AuthViewModel vm) vm.RConfirm = RConf.Password;
    }
}
