/******************************************************************************
 * Filename    = UdpCommunicatorTests.cs
 *
 * Author      = Ramaswamy Krishnan-Chittur
 *
 * Product     = GuiAndDistributedDemo
 * 
 * Project     = Tests
 *
 * Description = Unit tests for the UDP communicator.
 *****************************************************************************/

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using Moq;
using Networking;

namespace Tests.UnitTesting;

/// <summary>
/// Unit tests for the UDP communicator.
/// </summary>
[TestClass]
public class UdpCommunicatorTests
{
    private Mock<IMessageListener> _mockListener = null!;
    private UdpCommunicator _communicator = null!;

    /// <summary>
    /// Sets up the tests.
    /// </summary>
    [TestInitialize]
    public void Setup()
    {
        _mockListener = new Mock<IMessageListener>();
        _communicator = new UdpCommunicator();
        Logger.LogMessage($"Setup: Udp communicator listening on port {_communicator.ListenPort}");
    }

    /// <summary>
    /// Tests that AddSubscriber adds a subscriber to the list.
    /// </summary>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public void TestAddSubscriberBasic()
    {
        // Arrange
        string subscriberId = "testSubscriber";

        // Act
        _communicator.AddSubscriber(subscriberId, _mockListener.Object);

        // Assert
        System.Reflection.FieldInfo? subscribersField = typeof(UdpCommunicator).GetField("_subscribers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var subscribers = (Dictionary<string, IMessageListener>?)subscribersField?.GetValue(_communicator);
        Assert.IsNotNull(subscribers);
        Assert.IsTrue(subscribers!.ContainsKey(subscriberId));
    }

    /// <summary>
    /// Tests that AddSubscriber overwrites an existing subscriber with the same id.
    /// </summary>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public void TestAddSubscriberOverwritesForSameId()
    {
        // Arrange
        Mock<IMessageListener> mockListener1 = new();
        Mock<IMessageListener> mockListener2 = new();
        UdpCommunicator udpCommunicator = new UdpCommunicator();
        Logger.LogMessage($"Udp communicator listening on port {udpCommunicator.ListenPort}");
        const string SubscriberId = "TestNewSubscriberId";

        // Act
        udpCommunicator.AddSubscriber(SubscriberId, mockListener1.Object);
        udpCommunicator.AddSubscriber(SubscriberId, mockListener2.Object);

        // Assert
        System.Reflection.FieldInfo? subscribersField = typeof(UdpCommunicator).GetField("_subscribers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var subscribers = (Dictionary<string, IMessageListener>?)subscribersField?.GetValue(udpCommunicator);
        Assert.IsNotNull(subscribers);
        Assert.AreEqual(1, subscribers!.Count);
        Assert.IsTrue(subscribers!.ContainsKey(SubscriberId));
        Assert.AreEqual(mockListener2.Object, subscribers[SubscriberId]);
    }

    /// <summary>
    /// Tests that RemoveSubscriber removes a subscriber from the list.
    /// </summary>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public void TestRemoveSubscriberBasic()
    {
        // Arrange
        string subscriberId = "testSubscriber";
        _communicator.AddSubscriber(subscriberId, _mockListener.Object);

        // Act
        _communicator.RemoveSubscriber(subscriberId);

        // Assert
        System.Reflection.FieldInfo? subscribersField = typeof(UdpCommunicator).GetField("_subscribers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var subscribers = (Dictionary<string, IMessageListener>?)subscribersField?.GetValue(_communicator);
        Assert.IsNotNull(subscribers);
        Assert.IsFalse(subscribers!.ContainsKey(subscriberId));
    }

    /// <summary>
    /// Tests that SendMessage sends a UDP message.
    /// </summary>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public void TestSendMessageBasic()
    {
        // Arrange
        string ipAddress = "127.0.0.1";
        int port = 12345;
        string senderId = "testSender";
        string message = "Hello, World!";
        var udpClient = new UdpClient(port);
        var endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);

        // Act
        _communicator.SendMessage(ipAddress, port, senderId, message);

        // Assert
        byte[] receivedBytes = udpClient.Receive(ref endPoint);
        string receivedMessage = Encoding.ASCII.GetString(receivedBytes);
        Assert.AreEqual($"{senderId}:{message}", receivedMessage);
    }

    /// <summary>
    /// Tests that valid messages are received even when they are sent after corrupt messages.
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public async Task TestCorruptMessagesFollowedByValidMessages()
    {
        // Arrange
        Mock<IMessageListener> mockListener = new();
        UdpCommunicator udpCommunicator = new UdpCommunicator();
        Logger.LogMessage($"Udp communicator listening on port {udpCommunicator.ListenPort}");
        const string SubscriberId = "TestNewSubscriberId";
        udpCommunicator.AddSubscriber(SubscriberId, mockListener.Object);

        // Test messages: corrupt as well as valid
        string message = "Hello, World!";
        string anotherMessage = "Another message!";
        string corruptMessage = message; // No colon in the message to separate a subscriber id from the message.
        string invalidSubscriberMessage = $"InvalidSubscriberId:{message}"; // Invalid subscriber id.

        // Act
        string ipAddress = "127.0.0.1";
        int port = udpCommunicator.ListenPort;
        SendMessage(ipAddress, port, corruptMessage); // Send a corrupt message.
        SendMessage(ipAddress, port, invalidSubscriberMessage); // Send a message with an invalid subscriber id.
        SendMessage(ipAddress, port, $"{SubscriberId}:{message}"); // Send a valid message.
        SendMessage(ipAddress, port, $"{SubscriberId}:{anotherMessage}"); // Send another valid message.

        // Assert
        await Task.Delay(1000); // Adding a delay to ensure the message is received
        mockListener.Verify(listener => listener.OnMessageReceived(message), Times.Once); // Verify that the valid message was received.
        mockListener.Verify(listener => listener.OnMessageReceived(anotherMessage), Times.Once); // Verify that the other valid message was received.
    }

    /// <summary>
    /// Tests that a corrupt subscriber is handled.
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public async Task TestCorruptSubscriberIsHandled()
    {
        // Arrange
        UdpCommunicator udpCommunicator = new UdpCommunicator();
        Logger.LogMessage($"Udp communicator listening on port {udpCommunicator.ListenPort}");

        // Setup a proper subscriber
        Mock<IMessageListener> properListener = new();
        const string SubscriberId = "ProperSubscriberId";
        udpCommunicator.AddSubscriber(SubscriberId, properListener.Object);

        // Setup a corrupt subscriber
        Mock<IMessageListener> corruptListener = new();
        corruptListener.Setup(listener => listener.OnMessageReceived(It.IsAny<string>()))
                    .Throws(new Exception("Simulated exception")); // Set up the OnMessageReceived method to throw.
        const string CorruptSubscriberId = "CorruptSubscriberId";
        udpCommunicator.AddSubscriber(CorruptSubscriberId, corruptListener.Object);

        // Act
        string ipAddress = "127.0.0.1";
        int port = udpCommunicator.ListenPort;
        string message = "Hello, World!";
        _communicator.SendMessage(ipAddress, port, CorruptSubscriberId, "Some message"); // Send a message to the corrupt subscriber.
        _communicator.SendMessage(ipAddress, port, SubscriberId, message); // Send a message to the valid subscriber.

        // Assert
        await Task.Delay(1000); // Adding a delay to ensure the message is received
        properListener.Verify(listener => listener.OnMessageReceived(message), Times.Once); // Verify that the valid subscriber received the message.
    }

    /// <summary>
    /// Sends a message to the specified IP address and port.
    /// </summary>
    /// <param name="ipAddress">IP address of the destination</param>
    /// <param name="port">Port of the destination</param>
    /// <param name="message">The message to be sent</param>
    private static void SendMessage(string ipAddress, int port, string message)
    {
        Socket socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPAddress broadcastAddress = IPAddress.Parse(ipAddress);
        byte[] sendBuffer = Encoding.ASCII.GetBytes($"{message}");
        IPEndPoint endPoint = new(broadcastAddress, port);
        int bytesSent = socket.SendTo(sendBuffer, endPoint);
        Debug.Assert(bytesSent == sendBuffer.Length);
    }
}
