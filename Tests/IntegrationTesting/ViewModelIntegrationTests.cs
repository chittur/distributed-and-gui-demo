/******************************************************************************
 * Filename    = ViewModelIntegrationTests.cs
 *
 * Author      = Ramaswamy Krishnan-Chittur
 *
 * Product     = GuiAndDistributedDemo
 * 
 * Project     = Tests
 *
 * Description = Integration tests for the view model.
 *****************************************************************************/

using System.IO;
using System.Windows.Media.Imaging;
using ChatMessaging;
using ImageMessaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Networking;
using ViewModel;

namespace Tests.IntegrationTesting;

/// <summary>
/// Integration tests for the view model.
/// </summary>
[TestClass]
public class ViewModelIntegrationTests
{
    private Mock<ICommunicator> _mockCommunicator = null!;
    private MainPageViewModel _viewModel = null!;

    /// <summary>
    /// Sets up the tests.
    /// </summary>
    [TestInitialize]
    public void Setup()
    {
        _mockCommunicator = new Mock<ICommunicator>();
        _viewModel = new MainPageViewModel(_mockCommunicator.Object);
    }

    /// <summary>
    /// Tests that the messengers subscribe with the communicator passed to it.
    /// </summary>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public void TestSubscribersBasic()
    {
        // Arrange
        _mockCommunicator = new Mock<ICommunicator>();

        // Act
        _viewModel = new MainPageViewModel(_mockCommunicator.Object);

        // Assert
        _mockCommunicator.Verify(c => c.AddSubscriber(ChatMessenger.Identity, It.IsAny<IMessageListener>()), Times.Once);
        _mockCommunicator.Verify(c => c.AddSubscriber(ImageMessenger.Identity, It.IsAny<IMessageListener>()), Times.Once);
    }


    /// <summary>
    /// Tests message sending functionality of the messengers.
    /// </summary>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public void TestSendMessageBasic()
    {
        // Arrange
        string ipAddress = "127.0.0.1";
        int port = 500;
        string chatMessage = "Hello World";
        string imageFilename = @"Resources\TestImageFile.jpg";
        string anotherImageFilename = @"Resources\SecondTestImageFile.jpg";
        string anotherChatMessage = "Another Hello World Message";

        // Act
        _viewModel.SendChatMessage(ipAddress, port, chatMessage);
        _viewModel.SendImageMessage(ipAddress, port, imageFilename);
        _viewModel.SendImageMessage(ipAddress, port, anotherImageFilename);
        _viewModel.SendChatMessage(ipAddress, port, anotherChatMessage);

        // Assert
        _mockCommunicator.Verify(c => c.SendMessage(ipAddress, port, ChatMessenger.Identity, chatMessage), Times.Once);
        _mockCommunicator.Verify(c => c.SendMessage(ipAddress, port, ImageMessenger.Identity, It.IsAny<string>()), Times.Exactly(2));
        _mockCommunicator.Verify(c => c.SendMessage(ipAddress, port, ChatMessenger.Identity, anotherChatMessage), Times.Once);
    }

    /// <summary>
    /// Tests that the received message is set in the view model.
    /// </summary>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public void TestReceiveMessageBasic()
    {
        // Arrange
        string chatMessage = "Testing received message";
        string anotherChatMessage = "Testing a new message";
        string imageFilename = @"Resources\TestImageFile.jpg";
        string content = ImageMessenger.GetFileContentAsBase64(imageFilename);
        BitmapImage image1 = new(new Uri(imageFilename, UriKind.RelativeOrAbsolute));

        // Act
        var chatMessenger = _viewModel.GetType().GetField("_chatMessenger", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(_viewModel) as ChatMessenger;
        chatMessenger?.OnMessageReceived(chatMessage);

        // Assert
        Assert.AreEqual(chatMessage, _viewModel.ReceivedMessage);

        // Act
        chatMessenger?.OnMessageReceived(anotherChatMessage);

        // Assert
        Assert.AreEqual(anotherChatMessage, _viewModel.ReceivedMessage);

        // Act
        var imageMessenger = _viewModel.GetType().GetField("_imageMessenger", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(_viewModel) as ImageMessenger;
        imageMessenger?.OnMessageReceived(content);

        // Assert
        Assert.IsTrue(AreEqual(image1, _viewModel.ReceivedImage));
    }


    /// <summary>
    /// Compares two images for equality.
    /// </summary>
    /// <param name="image1">First image</param>
    /// <param name="image2">Second image</param>
    /// <returns>Gets a value indicating whether the images are same</returns>
    private static bool AreEqual(BitmapImage? image1, BitmapImage? image2)
    {
        if (image1 == null || image2 == null)
        {
            return false;
        }

        return ToBytes(image1).SequenceEqual(ToBytes(image2));
    }

    /// <summary>
    /// Converts a bitmap image to a byte array.
    /// </summary>
    /// <param name="image">Image to be converted</param>
    /// <returns>Byte array equivalent of the bitmap image</returns>
    private static byte[] ToBytes(BitmapImage image)
    {
        BmpBitmapEncoder encoder = new();
        encoder.Frames.Add(BitmapFrame.Create(image));
        using MemoryStream memoryStream = new();
        encoder.Save(memoryStream);
        byte[] data = memoryStream.ToArray();
        return data;
    }
}
