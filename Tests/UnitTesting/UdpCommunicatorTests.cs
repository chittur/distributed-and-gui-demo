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

using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using Moq;
using Networking;

namespace Tests.UnitTesting;

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
    public void TestAddSubscriber()
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
        Assert.AreEqual(subscribers!.Count, 1);
        Assert.IsTrue(subscribers!.ContainsKey(SubscriberId));
        Assert.AreEqual(mockListener2.Object, subscribers[SubscriberId]);
    }

    /// <summary>
    /// Tests that RemoveSubscriber removes a subscriber from the list.
    /// </summary>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public void TestRemoveSubscriber()
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
    public void TestSendMessage()
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
}
