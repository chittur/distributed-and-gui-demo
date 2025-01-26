/******************************************************************************
 * Filename    = ImageMessenger.cs
 *
 * Author      = Ramaswamy Krishnan-Chittur
 *
 * Product     = GuiAndDistributedDemo
 * 
 * Project     = ImageMessaging
 *
 * Description = Defines an image messenger.
 *****************************************************************************/

using System.IO;
using Networking;

namespace ImageMessaging;

/// <summary>
/// Handler for image messages.
/// </summary>
/// <param name="message">received image encoded as base64 string</param>
public delegate void ImageMessageReceived(string imageAsBase64String);

/// <summary>
/// Processor for image messages.
/// </summary>
public class ImageMessenger : IMessageListener
{
    private readonly ICommunicator _communicator;

    /// <summary>
    /// The identity for this module.
    /// </summary>
    public const string Identity = "ImageMessenger";

    /// <summary>
    /// Event for handling received image messages.
    /// </summary>
    public event ImageMessageReceived? OnImageMessageReceived;

    /// <summary>
    /// Creates an instance of the image messenger.
    /// </summary>
    /// <param name="communicator">The communicator instance to use</param>
    public ImageMessenger(ICommunicator communicator)
    {
        _communicator = communicator;
        _communicator.AddSubscriber(Identity, this);
    }

    /// <summary>
    /// Sends the given image to the given ip and port, encoded as base64 string.
    /// </summary>
    /// <param name="ipAddress">IP address of the destination</param>
    /// <param name="port">Port of the destination</param>
    /// <param name="imageFilePath">Path of the image file</param>
    public void SendMessage(string ipAddress, int port, string imageFilePath)
    {
        string content = GetFileContentAsBase64(imageFilePath);
        _communicator.SendMessage(ipAddress, port, Identity, content);
    }

    /// <inheritdoc />
    public void OnMessageReceived(string message)
    {
        OnImageMessageReceived?.Invoke(message);
    }

    /// <summary>
    /// Gets the content of the given file as base64 encoded string.
    /// </summary>
    /// <param name="filename">Path to the file</param>
    /// <returns></returns>
    public static string GetFileContentAsBase64(string filename)
    {
        return Convert.ToBase64String(File.ReadAllBytes(filename));
    }
}
