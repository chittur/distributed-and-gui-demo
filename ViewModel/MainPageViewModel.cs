/******************************************************************************
 * Filename    = MainPageViewModel.cs
 *
 * Author      = Ramaswamy Krishnan-Chittur
 *
 * Product     = GuiAndDistributedDemo
 * 
 * Project     = ViewModel
 *
 * Description = Defines the main page viewmodel.
 *****************************************************************************/

using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ChatMessaging;
using ImageMessaging;
using Networking;

namespace ViewModel;

/// <summary>
/// ViewModel for the main page.
/// </summary>
public class MainPageViewModel : INotifyPropertyChanged
{
    private readonly ChatMessenger _chatMessenger;   // Messenger used to send and receive chat messages.
    private readonly ImageMessenger _imageMessenger; // Messenger used to send and receive image messages.

    /// <summary>
    /// Communicator used to send and receive messages.
    /// </summary>
    internal ICommunicator Communicator { get; private set; } = null!;

    /// <summary>
    /// Creates an instance of the main page viewmodel.
    /// </summary>
    /// <param name="communicator">Optional instance of communicator</param>
    public MainPageViewModel(ICommunicator? communicator = null)
    {
        Communicator = communicator ?? CommunicatorFactory.CreateCommunicator();

        // Update the port that the communicator is listening on.
        ReceivePort = Communicator.ListenPort.ToString();
        OnPropertyChanged(nameof(ReceivePort));

        // Create an instance of the chat messenger and signup for callback.
        _chatMessenger = new(Communicator);
        _chatMessenger.OnChatMessageReceived += delegate (string message)
        {
            // UI element update needs to happen on the UI thread, and this callback is
            // likely run on a worker thread. However we do not need to explicitly
            // dispatch to the UI thread here because OnPropertyChanged event is
            // automatically marshaled to the UI thread.
            ReceivedMessage = message;
            OnPropertyChanged(nameof(ReceivedMessage));
        };

        // Create an instance of the image messenger and signup for callback.
        _imageMessenger = new(Communicator);
        _imageMessenger.OnImageMessageReceived += delegate (string imageAsBase64String)
        {
            // Like mentioned above, OnPropertyChanged event is automatically marshaled
            // to the UI thread. However, the bitmap image has thread affinity and needs
            // to be created on the same thread as it is displayed on (UI thread). Hence
            // let us explicitly dispatch the handling of this message to the UI thread.
            Dispatcher.Invoke((string image) =>
            {
                ReceivedImage = GetImageSourceFromBase64String(image);
                OnPropertyChanged(nameof(ReceivedImage));
            },
            imageAsBase64String);
        };
    }

    /// <summary>
    /// Sends the given message to the given ip and port.
    /// </summary>
    /// <param name="ipAddress">IP address of the destination</param>
    /// <param name="port">Port of the destination</param>
    /// <param name="message">Message to be sent</param>
    public void SendChatMessage(string ipAddress, int port, string message)
    {
        _chatMessenger.SendMessage(ipAddress, port, message);
    }

    /// <summary>
    /// Sends the given image to the given ip and port, encoded as base64 string.
    /// </summary>
    /// <param name="ipAddress">IP address of the destination</param>
    /// <param name="port">Port of the destination</param>
    /// <param name="message">Path of the image file</param>
    public void SendImageMessage(string ipAddress, int port, string message)
    {
        _imageMessenger.SendMessage(ipAddress, port, message);
    }

    /// <summary>
    /// Gets the port for receiving messages.
    /// </summary>
    public string? ReceivePort { get; private set; }

    /// <summary>
    /// Gets the received message.
    /// </summary>
    public string? ReceivedMessage { get; private set; }

    /// <summary>
    /// Gets the received image.
    /// </summary>
    public BitmapImage? ReceivedImage { get; private set; }

    /// <summary>
    /// Property changed event raised when a property is changed on a component.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Handles the property changed event raised on a component.
    /// </summary>
    /// <param name="property">The name of the property</param>
    private void OnPropertyChanged(string property)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }

    /// <summary>
    /// Gets the main thread dispatcher in the real mode.
    /// For unit test mode, it gets the current thread's dispatcher.
    /// </summary>
    private static Dispatcher Dispatcher => Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

    /// <summary>
    /// Gets the image from its base64 encoded string.
    /// </summary>
    /// <param name="imageAsBase64String">Base64 encoded string version of the image</param>
    /// <returns>The image</returns>
    private static BitmapImage GetImageSourceFromBase64String(string imageAsBase64String)
    {
        // Convert base64 string to byte[].
        byte[] imageBytes = Convert.FromBase64String(imageAsBase64String);

        // Convert byte[] to Bitmap.
        using MemoryStream memoryStream = new(imageBytes, 0, imageBytes.Length);
        BitmapImage image = new();
        image.BeginInit();
        image.StreamSource = memoryStream;
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.EndInit();
        Trace.TraceInformation($"Format of the decoded image is: {image.Format}");
        return image;
    }
}
