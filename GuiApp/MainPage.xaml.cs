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
    private readonly MainPageViewModel _viewModel;

    /// <summary>
    /// Creates an instance of the main page.
    /// </summary>
    public MainPage()
    {
        InitializeComponent();

        // Create the ViewModel and set as data context.
        _viewModel = new();
        DataContext = _viewModel;
    }

    /// <summary>
    /// Handles the 'send message' button click.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event args</param>
    private void SendMessageButtonClick(object sender, RoutedEventArgs e)
    {
        string destinationAddress = SendIpTextBox.Text;
        int destinationPort = Convert.ToInt32(SendPortTextBox.Text);
        string message = SendMessageTextBox.Text;

        _viewModel.SendChatMessage(destinationAddress, destinationPort, message);

        // Note: No exception is thrown if the destination is not reachable.
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

            _viewModel.SendImageMessage(destinationAddress, destinationPort, imagePath);
        }
        catch (Exception exception)
        {
            // Note: No exception is thrown if the destination is not reachable.
            //       But an exception is thrown if the image file is not found.
            MessageBox.Show($"An error occurred: {exception.Message}");
        }
    }
}
