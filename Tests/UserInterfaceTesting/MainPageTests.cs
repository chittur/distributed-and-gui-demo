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
}
