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

/// <summary>
/// Unit tests for the chat messenger.
/// </summary>
[TestClass]
public class ChatMessengerTests
{
    private Mock<ICommunicator> _mockCommunicator = null!;
    private ChatMessenger _chatMessenger = null!;

    /// <summary>
    /// Sets up the tests.
    /// </summary>
    [TestInitialize]
    public void Setup()
    {
        _mockCommunicator = new();
        _chatMessenger = new(_mockCommunicator.Object);
    }

    /// <summary>
    /// Tests that the ChatMessenger constructor adds itself as a subscriber.
    /// </summary>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public void TestConstructorShouldAddSubscriber()
    {
        // Assert
        _mockCommunicator.Verify(c => c.AddSubscriber(ChatMessenger.Identity, _chatMessenger), Times.Once);
    }

    /// <summary>
    /// Tests that SendMessage sends the message to the specified IP and port.
    /// </summary>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public void TestSendMessageShouldSendToSpecifiedIpAndPort()
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
    public void TestOnMessageReceivedShouldInvokeEvent()
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

    /// <summary>
    /// Tests that OnMessageReceived does not invoke OnChatMessageReceived with no subscribers.
    /// </summary>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public void TestOnMessageReceivedShouldNotInvokeEventWithNoSubscribers()
    {
        // Arrange
        Mock<ICommunicator> mockCommunicator = new();
        ChatMessenger chatMessenger = new(mockCommunicator.Object);
        string receivedMessage = "Hello, World!";

        // Act
        chatMessenger.OnMessageReceived(receivedMessage);

        // Assert
        // If no exception is thrown, the test will pass.
    }
}
