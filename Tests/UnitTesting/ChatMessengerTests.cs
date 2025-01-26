/******************************************************************************
 * Filename    = ChatMessengerTests.cs
 *
 * Author      = Ramaswamy Krishnan-Chittur
 *
 * Product     = GuiAndDistributedDemo
 * 
 * Project     = Tests
 *
 * Description = Unit tests for the chat messenger.
 *****************************************************************************/

using ChatMessaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Networking;

namespace Tests.UnitTesting;

[TestClass]
public class ChatMessengerTests
{
    private Mock<ICommunicator> _mockCommunicator = null!;
    private ChatMessenger _chatMessenger = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockCommunicator = new Mock<ICommunicator>();
        _chatMessenger = new ChatMessenger(_mockCommunicator.Object);
    }

    /// <summary>
    /// Tests that the ChatMessenger constructor adds itself as a subscriber.
    /// </summary>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public void ConstructorShouldAddSubscriber()
    {
        // Assert
        _mockCommunicator.Verify(c => c.AddSubscriber(ChatMessenger.Identity, _chatMessenger), Times.Once);
    }

    /// <summary>
    /// Tests that SendMessage sends the message to the specified IP and port.
    /// </summary>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public void SendMessageShouldSendToSpecifiedIpAndPort()
    {
        // Arrange
        string ipAddress = "127.0.0.1";
        int port = 12345;
        string message = "Hello, World!";

        // Act
        _chatMessenger.SendMessage(ipAddress, port, message);

        // Assert
        _mockCommunicator.Verify(c => c.SendMessage(ipAddress, port, ChatMessenger.Identity, message), Times.Once);
    }

    /// <summary>
    /// Tests that OnMessageReceived invokes the OnChatMessageReceived event.
    /// </summary>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public void OnMessageReceivedShouldInvokeEvent()
    {
        // Arrange
        string receivedMessage = "Hello, World!";
        bool eventInvoked = false;
        _chatMessenger.OnChatMessageReceived += (message) => eventInvoked = true;

        // Act
        _chatMessenger.OnMessageReceived(receivedMessage);

        // Assert
        Assert.IsTrue(eventInvoked);
    }
}
