using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace InventoryGui.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent(); // VIGTIGT!
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}