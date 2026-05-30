using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace APSApp_Windows;

/// <summary>
/// The main content page displayed inside the application window.
/// Add your UI logic, event handlers, and data binding here.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
        SetSurface("Chats");
    }

    private void ShowChats_Click(object sender, RoutedEventArgs e) => SetSurface("Chats");

    private void ShowCalls_Click(object sender, RoutedEventArgs e) => SetSurface("Calls");

    private void ShowSettings_Click(object sender, RoutedEventArgs e) => SetSurface("Settings");

    private void SetSurface(string surface)
    {
        ChatsListPane.Visibility = surface == "Chats" ? Visibility.Visible : Visibility.Collapsed;
        ConversationStage.Visibility = surface == "Chats" ? Visibility.Visible : Visibility.Collapsed;

        CallsListPane.Visibility = surface == "Calls" ? Visibility.Visible : Visibility.Collapsed;
        CallsStage.Visibility = surface == "Calls" ? Visibility.Visible : Visibility.Collapsed;

        SettingsListPane.Visibility = surface == "Settings" ? Visibility.Visible : Visibility.Collapsed;
        SettingsStage.Visibility = surface == "Settings" ? Visibility.Visible : Visibility.Collapsed;

        ContextPane.Visibility = surface == "Settings" ? Visibility.Collapsed : Visibility.Visible;
        ContextTitle.Text = surface == "Calls" ? "Звонок и контакт" : "Контекст диалога";
        ContextSubtitle.Text = surface == "Calls"
            ? "Устройства, история и приватность вызова"
            : "Профиль, медиа, участники и безопасность";
    }
}
