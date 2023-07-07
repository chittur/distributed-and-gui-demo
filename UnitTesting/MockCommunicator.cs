/******************************************************************************
 * Filename    = MockCommunicator.cs
 *
 * Author      = Ramaswamy Krishnan-Chittur
 *
 * Product     = GuiAndDistributedDemo
 * 
 * Project     = UnitTesting
 *
 * Description = Mock communicator for unit testing.
 *****************************************************************************/

using Networking;

namespace UnitTesting
{
    /// <summary>
    /// Details of the last message received by the mock communicator.
    /// </summary>
    class MessageDetails
    {
        /// <summary>
        /// IP address of the sender.
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// Port number of the sender.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Sender ID.
        /// </summary>
        public string? SenderId { get; set; }

        /// <summary>
        /// Message sent by the sender.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Validates that the message details are equal to the parameters passed.
        /// </summary>
        /// <param name="ipAddress">IP address</param>
        /// <param name="port">Port</param>
        /// <param name="senderId">Sender ID</param>
        /// <param name="message">Message</param>
        /// <returns>Value indicating whether the last message is same as the arguments passed</returns>
        public bool IsEqual(string ipAddress, int port, string senderId, string message)
        {
            return (ipAddress == IpAddress) &&
                   (port == Port) &&
                   (senderId == SenderId) &&
                   (message == Message);
        }
    }

    /// <summary>
    /// Mock communicator for unit testing.
    /// </summary>
    public class MockCommunicator : ICommunicator
    {
        // Details of the last message received by the mock communicator.
        private readonly MessageDetails _messageDetails;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MockCommunicator()
        {
            _messageDetails = new();
            Subscribers = new Dictionary<string, IMessageListener>();
        }

        /// <summary>
        /// Subscribers to the communicator.
        /// </summary>
        public Dictionary<string, IMessageListener> Subscribers { get; }

        /// <inheritdoc />
        public int ListenPort => 100;

        /// <inheritdoc />
        public void AddSubscriber(string id, IMessageListener subscriber)
        {
            Subscribers.Add(id, subscriber);
        }

        /// <inheritdoc />
        public void RemoveSubscriber(string id)
        {
            throw new NotImplementedException("Method not required in mock class.");
        }

        /// <inheritdoc />
        public void SendMessage(string ipAddress, int port, string senderId, string message)
        {
            _messageDetails.IpAddress = ipAddress;
            _messageDetails.Port = port;
            _messageDetails.SenderId = senderId;
            _messageDetails.Message = message;
        }

        /// <summary>
        /// Validates that the last message details are equal to the parameters passed.
        /// </summary>
        /// <param name="ipAddress">IP address</param>
        /// <param name="port">Port</param>
        /// <param name="senderId">Sender ID</param>
        /// <param name="message">Message</param>
        /// <returns>Value indicating whether the last message is same as the arguments passed</returns>
        public bool IsEqualToLastMessage(string ipAddress, int port, string senderId, string message)
        {
            return (ipAddress == _messageDetails.IpAddress) &&
                   (port == _messageDetails.Port)           &&
                   (senderId == _messageDetails.SenderId)   &&
                   (message == _messageDetails.Message);
        }
    }
}
