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
using Networking;
using ViewModel;

namespace Tests.EndToEndTesting;

[TestClass]
public class EndToEndTests
{
    private MainPageViewModel _viewModel = null!;
    private ICommunicator _communicator = null!;

    [TestInitialize]
    public void Setup()
    {
        // Use the actual CommunicatorFactory to create a UdpCommunicator
        _communicator = CommunicatorFactory.CreateCommunicator();
        _viewModel = new MainPageViewModel(_communicator);
        Logger.LogMessage($"Communicator started in port {_communicator.ListenPort}");
    }

    /// <summary>
    /// Tests sending and receiving chat messages using UdpCommunicator.
    /// </summary>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public void TestSendAndReceiveChatMessage()
    {
        // Arrange
        string ipAddress = "127.0.0.1";
        int port = _communicator.ListenPort;
        string sentMessage = "Hello, World!";
        string receivedMessage = "Hello, back!";

        // Act
        _viewModel.SendChatMessage(ipAddress, port, sentMessage);
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
    public void TestSendAndReceiveImageMessage()
    {
        // Arrange
        string ipAddress = "127.0.0.1";
        int port = _communicator.ListenPort;
        string imageFilePath = @"Resources\TestImageFile.jpg"; // Path to the test image in the resources
        byte[] imageBytes = File.ReadAllBytes(imageFilePath);
        string imageAsBase64String = Convert.ToBase64String(imageBytes);

        // Act
        _viewModel.SendImageMessage(ipAddress, port, imageFilePath);
        (_viewModel.GetType().GetField("_imageMessenger", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(_viewModel) as ImageMessenger)
            ?.OnMessageReceived(imageAsBase64String);

        // Assert
        Assert.IsNotNull(_viewModel.ReceivedImage);
    }
}
