/******************************************************************************
 * Filename    = ChatMessengerUnitTests.cs
 *
 * Author      = Ramaswamy Krishnan-Chittur
 *
 * Product     = GuiAndDistributedDemo
 * 
 * Project     = UnitTesting
 *
 * Description = Unit tests for the chat messenger.
 *****************************************************************************/

using ChatMessaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
namespace UnitTesting
{
    /// <summary>
    /// Unit tests for the chat messenger.
    /// </summary>
    [TestClass]
    public class ChatMessengerUnitTests
    {
        /// <summary>
        /// Validates that the chat messenger subscribes with the communicator passed to it.
        /// </summary>
        [TestMethod]
        public void TestSubscriber()
        {
            MockCommunicator mockCommunicator = new();
            ChatMessenger _ = new(mockCommunicator);

            Logger.LogMessage($"Validate that the chat messenger subscribes with the communicator passed to it.");
            Assert.AreEqual(mockCommunicator.Subscribers.Count, 1);
            Assert.IsNotNull(mockCommunicator.Subscribers[ChatMessenger.Identity]);
        }

        /// <summary>
        /// Validates message sending functionality of the chat messenger.
        /// </summary>
        [TestMethod]
        public void TestSendMessage()
        {
            MockCommunicator mockCommunicator = new();
            ChatMessenger messenger = new(mockCommunicator);

            // Send a message to the chat messenger and validate that it is received by the communicator.
            Logger.LogMessage($"Send a message to the chat messenger and validate that it is received by the communicator.");
            string ipAddress = "127.0.0.1";
            int port = 500;
            string chatMessage = "Hello World";
            messenger.SendMessage(ipAddress, port, chatMessage);
            Assert.IsTrue(mockCommunicator.IsEqualToLastMessage(ipAddress, port, ChatMessenger.Identity, chatMessage));

            // Send another message to the chat messenger and validate that it is received by the communicator.
            Logger.LogMessage($"Send another message to the chat messenger and validate that it is received by the communicator.");
            string anotherChatMessage = "Another Hello World Message";
            messenger.SendMessage(ipAddress, port, anotherChatMessage);
            Assert.IsTrue(mockCommunicator.IsEqualToLastMessage(ipAddress, port, ChatMessenger.Identity, anotherChatMessage));
        }

        /// <summary>
        /// Validates that the chat messenger processes received message, and notifies clients.
        /// </summary>
        [TestMethod]
        public void TestOnMessageReceived()
        {
            MockCommunicator mockCommunicator = new();
            ChatMessenger messenger = new(mockCommunicator);

            Logger.LogMessage($"Sign up with the messenger for notifiation on message received.");
            string? message = null;
            messenger.OnChatMessageReceived += delegate (string chatMessage)
            {
                message = chatMessage;
            };

            Logger.LogMessage($"Validate that the messenger notifies clients upon receiving a message.");
            string testMessage = "Hello World!";
            messenger.OnMessageReceived(testMessage);
            Assert.AreEqual(message, testMessage);
        }
    }
}
