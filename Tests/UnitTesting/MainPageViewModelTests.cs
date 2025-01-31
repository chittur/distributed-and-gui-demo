/******************************************************************************
 * Filename    = MainPageViewModelTests.cs
 *
 * Author      = Ramaswamy Krishnan-Chittur
 *
 * Product     = GuiAndDistributedDemo
 * 
 * Project     = Tests
 *
 * Description = Unit tests for the main page view model.
 *****************************************************************************/

using System.IO;
using System.Reflection;
using ChatMessaging;
using ImageMessaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Networking;
using ViewModel;

namespace Tests.UnitTesting;

/// <summary>
/// Unit tests for the main page view model.
/// </summary>
[TestClass]
public class MainPageViewModelTests
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
    /// Tests that the MainPageViewModel constructor initializes the ReceivePort property.
    /// </summary>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public void TestConstructorShouldInitializeReceivePort()
    {
        // Arrange
        string expectedPort = _mockCommunicator.Object.ListenPort.ToString();

        // Act
        string actualPort = _viewModel.ReceivePort!;

        // Assert
        Assert.AreEqual(expectedPort, actualPort);
    }

    /// <summary>
    /// Tests that SendChatMessage sends the message to the specified IP and port.
    /// </summary>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public void TestSendChatMessageShouldSendToSpecifiedIpAndPort()
    {
        // Arrange
        string ipAddress = "127.0.0.1";
        int port = 12345;
        string message = "Hello, World!";

        // Act
        _viewModel.SendChatMessage(ipAddress, port, message);

        // Assert
        _mockCommunicator.Verify(c => c.SendMessage(ipAddress, port, ChatMessenger.Identity, message), Times.Once);
    }

    /// <summary>
    /// Tests that SendImageMessage sends the image to the specified IP and port.
    /// </summary>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public void TestSendImageMessageShouldSendToSpecifiedIpAndPort()
    {
        // Arrange
        string ipAddress = "127.0.0.1";
        int port = 12345;
        string imageFilePath = Path.GetTempFileName();
        File.WriteAllBytes(imageFilePath, [1, 2, 3, 4]);

        // Act
        _viewModel.SendImageMessage(ipAddress, port, imageFilePath);

        // Assert
        _mockCommunicator.Verify(c => c.SendMessage(ipAddress, port, ImageMessenger.Identity, It.IsAny<string>()), Times.Once);

        // Clean up
        File.Delete(imageFilePath);
    }

    /// <summary>
    /// Tests that OnChatMessageReceived updates the ReceivedMessage property.
    /// </summary>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public void TestOnChatMessageReceivedShouldUpdateReceivedMessage()
    {
        // Arrange
        string receivedMessage = "Hello, World!";
        bool propertyChangedInvoked = false;
        _viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(MainPageViewModel.ReceivedMessage))
            {
                propertyChangedInvoked = true;
            }
        };

        // Act
        (_viewModel.GetType().GetField("_chatMessenger", BindingFlags.NonPublic | BindingFlags.Instance)
        ?.GetValue(_viewModel) as ChatMessenger)
        ?.OnMessageReceived(receivedMessage);

        // Assert
        Assert.AreEqual(receivedMessage, _viewModel.ReceivedMessage);
        Assert.IsTrue(propertyChangedInvoked);
    }

    /// <summary>
    /// Tests that OnImageMessageReceived updates the ReceivedImage property.
    /// </summary>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public void TestOnImageMessageReceivedShouldUpdateReceivedImage()
    {
        // Arrange
        string imageFilePath = @"Resources\TestImageFile.jpg"; // Path to the test image in the resources
        byte[] imageBytes = File.ReadAllBytes(imageFilePath);
        string imageAsBase64String = Convert.ToBase64String(imageBytes);
        bool propertyChangedInvoked = false;
        _viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(MainPageViewModel.ReceivedImage))
            {
                propertyChangedInvoked = true;
            }
        };

        // Act
        FieldInfo? imageMessengerField = _viewModel.GetType().GetField("_imageMessenger", BindingFlags.NonPublic | BindingFlags.Instance);
        if (imageMessengerField != null)
        {
            var imageMessenger = imageMessengerField.GetValue(_viewModel) as ImageMessenger;
            imageMessenger?.OnMessageReceived(imageAsBase64String);
        }

        // Assert
        Assert.IsNotNull(_viewModel.ReceivedImage);
        Assert.IsTrue(propertyChangedInvoked);
    }
}
