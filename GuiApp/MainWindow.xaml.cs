/******************************************************************************
 * Filename    = MainWindow.Xaml.cs
 *
 * Author      = Ramaswamy Krishnan-Chittur
 *
 * Product     = GuiAndDistributedDemo
 * 
 * Project     = GuiApp
 *
 * Description = Interaction logic for MainWindow.xaml.
 *****************************************************************************/

using System.Windows;
using System.Windows.Controls;

namespace GuiApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    ///  Creates an instance of the main window.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        Page mainPage = new MainPage();
        MainFrame.Navigate(mainPage);
    }
}
