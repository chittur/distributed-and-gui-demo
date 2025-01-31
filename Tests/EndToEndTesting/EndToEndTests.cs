/******************************************************************************
 * Filename    = EndToEndTests.cs
 *
 * Author      = Ramaswamy Krishnan-Chittur
 *
 * Product     = GuiAndDistributedDemo
 * 
 * Project     = Tests
 *
 * Description = End-to-end tests for the application.
 *****************************************************************************/

using System.IO;
using ChatMessaging;
using ImageMessaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using ViewModel;

namespace Tests.EndToEndTesting;

/// <summary>
/// End-to-end tests for the application.
/// </summary>
[TestClass]
public class EndToEndTests
{
    private MainPageViewModel _viewModel = null!;

    /// <summary>
    /// Sets up the tests.
    /// </summary>
    [TestInitialize]
    public void Setup()
    {
        _viewModel = new MainPageViewModel(); // Use the actual communicator for the E2E tests, not a mock.
        Logger.LogMessage($"Communicator started in port {_viewModel.Communicator.ListenPort}");
    }

    /// <summary>
    /// Tests sending and receiving chat messages using UdpCommunicator.
    /// </summary>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public async Task TestSendAndReceiveChatMessage()
    {
        // Arrange
        string ipAddress = "127.0.0.1";
        int port = _viewModel.Communicator.ListenPort;
        string sentMessage = "Hello, World!";
        string receivedMessage = "Hello, back!";

        // Act
        _viewModel.SendChatMessage(ipAddress, port, sentMessage);
        await Task.Delay(1000); // Adding a delay to ensure the message is received
        (_viewModel.GetType().GetField("_chatMessenger", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(_viewModel) as ChatMessenger)
            ?.OnMessageReceived(receivedMessage);

        // Assert
        Assert.AreEqual(receivedMessage, _viewModel.ReceivedMessage);
    }

    /// <summary>
    /// Tests sending and receiving image messages using UdpCommunicator.
    /// </summary>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public async Task TestSendAndReceiveImageMessage()
    {
        // Arrange
        string ipAddress = "127.0.0.1";
        int port = _viewModel.Communicator.ListenPort;
        string imageFilePath = @"Resources\TestImageFile.jpg"; // Path to the test image in the resources
        byte[] imageBytes = File.ReadAllBytes(imageFilePath);
        string imageAsBase64String = Convert.ToBase64String(imageBytes);

        // Act
        _viewModel.SendImageMessage(ipAddress, port, imageFilePath);
        await Task.Delay(1000); // Adding a delay to ensure the message is received
        (_viewModel.GetType().GetField("_imageMessenger", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(_viewModel) as ImageMessenger)
            ?.OnMessageReceived(imageAsBase64String);

        // Assert
        Assert.IsNotNull(_viewModel.ReceivedImage);
    }
}
