/******************************************************************************
 * Filename    = ImageMessengerUnitTests.cs
 *
 * Author      = Ramaswamy Krishnan-Chittur
 *
 * Product     = GuiAndDistributedDemo
 * 
 * Project     = Tests
 *
 * Description = Unit tests for the image messenger.
 *****************************************************************************/

using System.IO;
using ImageMessaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Networking;

namespace Tests.UnitTesting;

[TestClass]
public class ImageMessengerTests
{
    private Mock<ICommunicator> _mockCommunicator = null!;
    private ImageMessenger _imageMessenger = null!;
    const string ImageFilePath = @"Resources\TestImageFile.jpg";

    [TestInitialize]
    public void Setup()
    {
        _mockCommunicator = new Mock<ICommunicator>();
        _imageMessenger = new ImageMessenger(_mockCommunicator.Object);
    }

    /// <summary>
    /// Tests that the ImageMessenger constructor adds itself as a subscriber.
    /// </summary>
    [TestMethod]
    public void ConstructorShouldAddSubscriber()
    {
        // Assert
        _mockCommunicator.Verify(c => c.AddSubscriber(ImageMessenger.Identity, _imageMessenger), Times.Once);
    }

    /// <summary>
    /// Tests that SendMessage sends the image as a base64 encoded string.
    /// </summary>
    [TestMethod]
    public void SendMessageShouldSendBase64EncodedImage()
    {
        // Arrange
        string ipAddress = "127.0.0.1";
        int port = 12345;
        string imageFilePath = ImageFilePath;
        string base64Content = Convert.ToBase64String(File.ReadAllBytes(imageFilePath));

        // Act
        _imageMessenger.SendMessage(ipAddress, port, imageFilePath);

        // Assert
        _mockCommunicator.Verify(c => c.SendMessage(ipAddress, port, ImageMessenger.Identity, base64Content), Times.Once);
    }

    /// <summary>
    /// Tests that OnMessageReceived invokes the OnImageMessageReceived event.
    /// </summary>
    [TestMethod]
    public void OnMessageReceivedShouldInvokeEvent()
    {
        // Arrange
        string base64Message = "base64ImageString";
        bool eventInvoked = false;
        _imageMessenger.OnImageMessageReceived += (imageAsBase64String) => eventInvoked = true;

        // Act
        _imageMessenger.OnMessageReceived(base64Message);

        // Assert
        Assert.IsTrue(eventInvoked);
    }

    /// <summary>
    /// Tests that GetFileContentAsBase64 returns the correct base64 encoded string.
    /// </summary>
    [TestMethod]
    public void GetFileContentAsBase64ShouldReturnBase64String()
    {
        // Arrange
        string filePath = ImageFilePath;
        string expectedBase64Content = Convert.ToBase64String(File.ReadAllBytes(filePath));

        // Act
        string actualBase64Content = ImageMessenger.GetFileContentAsBase64(filePath);

        // Assert
        Assert.AreEqual(expectedBase64Content, actualBase64Content);
    }
}
