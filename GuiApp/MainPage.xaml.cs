/******************************************************************************
 * Filename    = MainPage.Xaml.cs
 *
 * Author      = Ramaswamy Krishnan-Chittur
 *
 * Product     = GuiAndDistributedDemo
 * 
 * Project     = GuiApp
 *
 * Description = Interaction logic for MainPage.xaml.
 *****************************************************************************/

using System;
using System.Windows;
using System.Windows.Controls;
using ViewModel;

namespace GuiApp;

/// <summary>
/// Interaction logic for MainPage.xaml
/// </summary>
public partial class MainPage : Page
{
    /// <summary>
    /// Creates an instance of the main page.
    /// </summary>
    public MainPage()
    {
        InitializeComponent();

        try
        {
            // Create the ViewModel and set as data context.
            MainPageViewModel viewModel = new();
            DataContext = viewModel;
        }
        catch (Exception exception)
        {
            _ = MessageBox.Show(exception.Message);
            Application.Current.Shutdown();
        }
    }

    /// <summary>
    /// Handles the 'send message' button click.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event args</param>
    private void SendMessageButtonClick(object sender, RoutedEventArgs e)
    {
        try
        {
            string destinationAddress = SendIpTextBox.Text;
            int destinationPort = Convert.ToInt32(SendPortTextBox.Text);
            string message = SendMessageTextBox.Text;

            MainPageViewModel? viewModel = DataContext as MainPageViewModel;
            viewModel?.SendChatMessage(destinationAddress, destinationPort, message);
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message);
        }
    }

    /// <summary>
    /// Handles the 'send image' button click.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event args</param>
    private void SendImageButtonClick(object sender, RoutedEventArgs e)
    {
        try
        {
            string destinationAddress = SendIpTextBox.Text;
            int destinationPort = Convert.ToInt32(SendPortTextBox.Text);
            string imagePath = SendImageTextBox.Text;

            MainPageViewModel? viewModel = DataContext as MainPageViewModel;
            viewModel?.SendImageMessage(destinationAddress, destinationPort, imagePath);
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message);
        }
    }
}
