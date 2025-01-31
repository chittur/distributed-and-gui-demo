/******************************************************************************
 * Filename    = CommunicatorFactoryTests.cs
 *
 * Author      = Ramaswamy Krishnan-Chittur
 *
 * Product     = GuiAndDistributedDemo
 * 
 * Project     = Tests
 *
 * Description = Unit tests for the communicator factory.
 *****************************************************************************/

using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using Networking;

namespace Tests.UnitTesting;

/// <summary>
/// Unit tests for the communicator factory.
/// </summary>
[TestClass]
public class CommunicatorFactoryTests
{
    /// <summary>
    /// Tests that CreateCommunicator returns a new instance of ICommunicator.
    /// </summary>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public void TestCreateCommunicatorShouldReturnNewInstance()
    {
        // Act
        ICommunicator communicator = CommunicatorFactory.CreateCommunicator();

        // Assert
        Assert.IsNotNull(communicator);
        Assert.IsInstanceOfType(communicator, typeof(ICommunicator));

        // Access the public ListenPort property directly
        int listenPort = ((UdpCommunicator)communicator).ListenPort;
        Logger.LogMessage($"ListenPort: {listenPort}");
    }

    /// <summary>
    /// Tests that CreateCommunicator logs the starting port number.
    /// </summary>
    [TestMethod]
    [Owner("Ramaswamy Krishnan-Chittur")]
    public void TestCreateCommunicatorShouldLogStartingPort()
    {
        // Arrange
        using var listener = new CustomTraceListener();
        Trace.Listeners.Add(listener);

        // Act
        ICommunicator communicator = CommunicatorFactory.CreateCommunicator();

        // Debugging information
        var messagesCopy = listener.Messages.ToList(); // Create a copy of the collection
        foreach (string message in messagesCopy)
        {
            Logger.LogMessage(message);
        }

        // Assert
        Assert.IsTrue(listener.Messages.Count > 0, "No messages were logged.");
        Assert.IsTrue(listener.Messages.Any(message => message.Contains("Starting Udp Communicator in port")), "Expected log message not found.");
    }
}

/// <summary>
/// Custom TraceListener to capture trace output.
/// </summary>
internal class CustomTraceListener : TraceListener
{
    public List<string> Messages { get; } = [];

    public override void Write(string? message)
    {
        if (message != null)
        {
            Messages.Add(message);
        }
    }

    public override void WriteLine(string? message)
    {
        if (message != null)
        {
            Messages.Add(message);
        }
    }

    public override void TraceEvent(TraceEventCache? eventCache, string source, TraceEventType eventType, int id, string? message)
    {
        if (message != null)
        {
            Messages.Add(message);
        }
    }

    public override void TraceEvent(TraceEventCache? eventCache, string source, TraceEventType eventType, int id, string? format, params object?[]? args)
    {
        if (format != null)
        {
            string message = string.Format(format, args!);
            Messages.Add(message);
        }
    }
}

