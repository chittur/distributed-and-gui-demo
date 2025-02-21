/******************************************************************************
 * Filename    = MainPageTests.cs
 *
 * Author      = Ramaswamy Krishnan-Chittur
 *
 * Product     = GuiAndDistributedDemo
 * 
 * Project     = Tests
 *
 * Description = User interface tests for the main page.
 *****************************************************************************/

using System.Drawing;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Logger = Microsoft.VisualStudio.TestTools.UnitTesting.Logging.Logger;

namespace Tests.UserInterfaceTesting;

/// <summary>
/// User interface tests for the main page.
/// </summary>
[TestClass]
public class MainPageTests
{
    private Application _app1 = null!;
    private Application _app2 = null!;
    private AutomationBase _automation = null!;

    /// <summary>
    /// Sets up the tests.
    /// </summary>
    [TestInitialize]
    public void SetUp()
    {
        _automation = new UIA3Automation();

        // Launch two instances of the application.
        _app1 = Application.Launch("GuiApp.exe");
        _app2 = Application.Launch("GuiApp.exe");
    }

    /// <summary>
    /// Cleans up the tests.
    /// </summary>
    [TestCleanup]
    public void TearDown()
    {
        _app2?.Close();
        _app1?.Close();
        _automation?.Dispose();
    }

    /// <summary>
    /// Tests that a message box is shown when attempting to send an image with incorrect details.
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public async Task TestAttemptToSendWithIncorrectImageDetails()
    {
        // Arrange

        // Find the receive port of the second instance of the application.
        Window window = _app1.GetMainWindow(_automation);

        // Act

        // From the second instance of the application, enter a message and the destination
        // port of the first application, and click the Send Message button.
        const string Message = "Hello World!";
        TextBox imageTextBox = window.FindFirstDescendant(cf => cf.ByAutomationId("SendImageTextBox")).AsTextBox();
        imageTextBox.Enter(Message);
        TextBox sendPortTextBox = window.FindFirstDescendant(cf => cf.ByAutomationId("SendPortTextBox")).AsTextBox();
        sendPortTextBox.Enter("0"); // Invalid destination port.
        Button sendImageButton = window.FindFirstDescendant(cf => cf.ByAutomationId("SendImageButton")).AsButton();
        sendImageButton.Invoke();
        Logger.LogMessage($"Window SendPortTextBox:0, SendMessageTextBox: {Message}");

        // Wait for a short while for the message box to show.
        Logger.LogMessage("Waiting for a short while for the message box to show.");
        await Task.Delay(1000);

        // Assert

        // Check if the message box was shown.
        Window? messageBox = window.ModalWindows.FirstOrDefault();
        Assert.IsNotNull(messageBox, "MessageBox did not appear.");

        // Verify the MessageBox content
        Label messageBoxText = messageBox.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Text)).AsLabel();
        Assert.IsTrue(messageBoxText.Text.Contains("An error occurred"), "MessageBox text is incorrect.");

        // Close the MessageBox
        Button okButton = messageBox.FindFirstDescendant(cf => cf.ByAutomationId("2")).AsButton();
        okButton.Invoke();
    }

    /// <summary>
    /// Tests that messages can be sent and received between two instances of the application.
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public async Task TestSendAndReceiveMessageViaUI()
    {
        // Arrange

        // Find the receive port of the first instance of the application.
        Window window1 = _app1.GetMainWindow(_automation);
        TextBox receivePortTextBox1 = window1.FindFirstDescendant(cf => cf.ByAutomationId("ReceivePortTextBox")).AsTextBox();
        int port1 = int.Parse(receivePortTextBox1.Text);
        Logger.LogMessage($"Window1 ReceivePortTextBox: {port1}");

        // Find the receive port of the second instance of the application.
        Window window2 = _app2.GetMainWindow(_automation);
        TextBox receivePortTextBox2 = window2.FindFirstDescendant(cf => cf.ByAutomationId("ReceivePortTextBox")).AsTextBox();
        int port2 = int.Parse(receivePortTextBox2.Text);
        Logger.LogMessage($"Window2 ReceivePortTextBox: {port2}");

        // Act

        // From the second instance of the application, enter a message and the destination
        // port of the first application, and click the Send Message button.
        const string Message = "Hello World!";
        TextBox messageTextBox2 = window2.FindFirstDescendant(cf => cf.ByAutomationId("SendMessageTextBox")).AsTextBox();
        messageTextBox2.Enter(Message);
        TextBox sendPortTextBox2 = window2.FindFirstDescendant(cf => cf.ByAutomationId("SendPortTextBox")).AsTextBox();
        sendPortTextBox2.Enter($"{port1}");
        Button sendMessageButton2 = window2.FindFirstDescendant(cf => cf.ByAutomationId("SendMessageButton")).AsButton();
        sendMessageButton2.Invoke();
        Logger.LogMessage($"Window2 SendPortTextBox:{port1}, SendMessageTextBox: {Message}");

        // Wait for a short while for the message to be received.
        Logger.LogMessage("Waiting for a short while for the message to be received");
        await Task.Delay(1000);

        // Assert

        // Check if the message was received by the first instance of the application.
        TextBox receiveMessageTextBox1 = window1.FindFirstDescendant(cf => cf.ByAutomationId("ReceiveMessageTextBox")).AsTextBox();
        Logger.LogMessage($"Window1 ReceiveMessageTextBox: {receiveMessageTextBox1.Text}");
        Assert.AreEqual(Message, receiveMessageTextBox1.Text);
    }

    /// <summary>
    /// Tests that images can be sent and received between two instances of the application.
    /// </summary>
    /// <returns>Task</returns>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public async Task TestSendAndReceiveImageViaUI()
    {
        // Arrange

        // Find the receive port of the first instance of the application.
        Window window1 = _app1.GetMainWindow(_automation);
        TextBox receivePortTextBox1 = window1.FindFirstDescendant(cf => cf.ByAutomationId("ReceivePortTextBox")).AsTextBox();
        int port1 = int.Parse(receivePortTextBox1.Text);
        Logger.LogMessage($"Window1 ReceivePortTextBox: {port1}");

        // Find the receive port of the second instance of the application.
        Window window2 = _app2.GetMainWindow(_automation);
        TextBox receivePortTextBox2 = window2.FindFirstDescendant(cf => cf.ByAutomationId("ReceivePortTextBox")).AsTextBox();
        int port2 = int.Parse(receivePortTextBox2.Text);
        Logger.LogMessage($"Window2 ReceivePortTextBox: {port2}");

        // Act

        // From the second instance of the application, enter an image file and the destination
        // port of the first application, and click the Send Image button.
        const string ImageFilename = @"Resources\TestImageFile.jpg";
        TextBox imageFileTextBox2 = window2.FindFirstDescendant(cf => cf.ByAutomationId("SendImageTextBox")).AsTextBox();
        imageFileTextBox2.Enter(ImageFilename);
        TextBox sendPortTextBox2 = window2.FindFirstDescendant(cf => cf.ByAutomationId("SendPortTextBox")).AsTextBox();
        sendPortTextBox2.Enter($"{port1}");
        Button sendImageButton2 = window2.FindFirstDescendant(cf => cf.ByAutomationId("SendImageButton")).AsButton();
        sendImageButton2.Invoke();
        Logger.LogMessage($"Window2 SendPortTextBox:{port1}, SendImageTextBox: {ImageFilename}");

        // Wait for a short while for the image to be received.
        Logger.LogMessage("Waiting for a short while for the image to be received");
        await Task.Delay(1000);

        // Assert

        // Check if the image was received by the first instance of the application.
        AutomationElement imageControl = window1.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Image));
        Bitmap image1 = imageControl.Capture();
        // resize the original image to the size of the image control, and compare to the received image.
        Bitmap orignal = new Bitmap(ImageFilename);
        Bitmap resized = ResizeBitmap(orignal, image1.Width, image1.Height);
        CompareImages(resized, image1);
    }

    /// <summary>
    /// Resizes a bitmap to the specified width and height.
    /// </summary>
    /// <param name="bitmap">Given bitmap</param>
    /// <param name="width">Width to resize to</param>
    /// <param name="height">Height to resize to</param>
    /// <returns>Resized image.</returns>
    private static Bitmap ResizeBitmap(Bitmap bitmap, int width, int height)
    {
        Bitmap resizedBitmap = new Bitmap(width, height);
        using (Graphics g = Graphics.FromImage(resizedBitmap))
        {
            g.DrawImage(bitmap, 0, 0, width, height);
        }

        return resizedBitmap;
    }

    /// <summary>
    /// Compares two bitmaps for equality.
    /// </summary>
    /// <param name="bitmap1">First image</param>
    /// <param name="bitmap2">Second image</param>
    /// <returns>A value indicating whether the images are similar.</returns>
    private static bool CompareImages(Bitmap bitmap1, Bitmap bitmap2)
    {
        if (bitmap1.Width != bitmap2.Width || bitmap1.Height != bitmap2.Height)
        {
            return false;
        }

        for (int y = 0; y < bitmap1.Height; y++)
        {
            for (int x = 0; x < bitmap1.Width; x++)
            {
                if (bitmap1.GetPixel(x, y) != bitmap2.GetPixel(x, y))
                {
                    return false;
                }
            }
        }

        return true;
    }
}
