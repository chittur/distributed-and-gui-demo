/******************************************************************************
 * Filename    = IntegrationTests.cs
 *
 * Author      = Ramaswamy Krishnan-Chittur
 *
 * Product     = GuiAndDistributedDemo
 * 
 * Project     = UnitTesting
 *
 * Description = Integration tests for ViewModel and the data models (messengers).
 *****************************************************************************/

using System.IO;
using System.Windows.Media.Imaging;
using ChatMessaging;
using ImageMessaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using ViewModel;

namespace UnitTesting
{
    [TestClass]
    public class IntegrationTests
    {
        // For integration test, we should let the components live across test cases as
        // opposed to unit tests where they may be spawned and discarded per test case.
        MockCommunicator? _mockCommunicator;
        MainPageViewModel? _viewModel;

        /// <summary>
        /// Sets up the test.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            _mockCommunicator = new();
            _viewModel = new(_mockCommunicator);
        }

        /// <summary>
        /// Validates that the messengers subscribe with the communicator passed to it.
        /// </summary>
        [TestMethod]
        public void TestSubscribers()
        {
            Logger.LogMessage($"Validate that the messengers subscribe with the communicator passed to it.");
            Assert.AreEqual(_mockCommunicator?.Subscribers.Count, 2);
            Assert.IsNotNull(_mockCommunicator?.Subscribers[ChatMessenger.Identity]);
            Assert.IsNotNull(_mockCommunicator?.Subscribers[ImageMessenger.Identity]);
        }

        /// <summary>
        /// Validates message sending functionality of the messengers.
        /// </summary>
        [TestMethod]
        public void TestSendMessage()
        {
            // Send a message to the chat messenger and validate that it is received by the communicator.
            string ipAddress = "127.0.0.1";
            int port = 500;
            string chatMessage = "Hello World";
            _viewModel?.SendChatMessage(ipAddress, port, chatMessage);
            Logger.LogMessage($"Validate that the message sent from the ViewModel is received in the Communicator.");
            Assert.IsTrue(_mockCommunicator?.IsEqualToLastMessage(ipAddress, port, ChatMessenger.Identity, chatMessage));

            // Send an image message to the image messenger and validate that it is received by the communicator.
            string imageFilename = @"Resources\TestImageFile.jpg";
            Assert.IsTrue(File.Exists(imageFilename));
            _viewModel?.SendImageMessage(ipAddress, port, imageFilename);
            Logger.LogMessage($"Validate that the image message sent from the ViewModel is received in the Communicator.");
            Assert.IsTrue(_mockCommunicator?.IsEqualToLastMessage(ipAddress, port, ImageMessenger.Identity, ImageMessenger.GetFileContentAsBase64(imageFilename)));

            // Send another image message to the image messenger and validate that it is received by the communicator.
            string anotherImageFilename = @"Resources\SecondTestImageFile.jpg";
            Assert.IsTrue(File.Exists(anotherImageFilename));
            _viewModel?.SendImageMessage(ipAddress, port, anotherImageFilename);
            Logger.LogMessage($"Validate that the second image message sent from the ViewModel is received in the Communicator.");
            Assert.IsTrue(_mockCommunicator?.IsEqualToLastMessage(ipAddress, port, ImageMessenger.Identity, ImageMessenger.GetFileContentAsBase64(anotherImageFilename)));

            // Send another chat message to the chat messenger and validate that it is received by the communicator.
            string anotherChatMessage = "Another Hello World Message";
            _viewModel?.SendChatMessage(ipAddress, port, anotherChatMessage);
            Logger.LogMessage($"Validate that the second chat message sent from the ViewModel is received in the Communicator.");
            Assert.IsTrue(_mockCommunicator?.IsEqualToLastMessage(ipAddress, port, ChatMessenger.Identity, anotherChatMessage));
        }

        /// <summary>
        /// Validates that the received message is set in the view model.
        /// </summary>
        [TestMethod]
        public void TestReceiveMessage()
        {
            // Send a message from the communicator and validate it is received by the ViewModel.
            string chatMessage = "Testing received message";
            _mockCommunicator?.Subscribers[ChatMessenger.Identity].OnMessageReceived(chatMessage);
            Logger.LogMessage($"Validate that the message sent from the Communicator is received in the ViewModel.");
            Assert.AreEqual(_viewModel?.ReceivedMessage, chatMessage);

            // Send another message from the communicator and validate it is received by the ViewModel.
            string anotherChatMessage = "Testing a new message";
            _mockCommunicator?.Subscribers[ChatMessenger.Identity].OnMessageReceived(anotherChatMessage);
            Logger.LogMessage($"Validate that the second message sent from the Communicator is received in the ViewModel.");
            Assert.AreEqual(_viewModel?.ReceivedMessage, anotherChatMessage);

            // Send an image from the communicator and validate it is received by the ViewModel.
            string imageFilename = @"Resources\TestImageFile.jpg";
            string content = ImageMessenger.GetFileContentAsBase64(imageFilename);
            _mockCommunicator?.Subscribers[ImageMessenger.Identity].OnMessageReceived(content);
            BitmapImage image1 = new(new Uri(imageFilename, UriKind.RelativeOrAbsolute));
            Logger.LogMessage($"Validate that the image sent from the Communicator is received in the ViewModel.");
            Assert.IsTrue(AreEqual(image1, _viewModel?.ReceivedImage));
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
}
