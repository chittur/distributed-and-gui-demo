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

using System.Collections.Concurrent;
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
        List<string> messagesCopy = listener.Messages; // Create a copy of the collection
        foreach (string message in messagesCopy)
        {
            Logger.LogMessage(message);
        }

        // Assert
        Assert.IsTrue(messagesCopy.Count > 0, "No messages were logged.");
        Assert.IsTrue(messagesCopy.Any(message => message.Contains("Starting Udp Communicator in port")), "Expected log message not found.");
    }
}

/// <summary>
/// Custom TraceListener to capture trace output.
/// </summary>
internal class CustomTraceListener : TraceListener
{
    private ConcurrentBag<string> _messages = [];

    /// <summary>
    /// Gets the list of messages.
    /// </summary>
    public List<string> Messages => _messages.ToList();

    /// <inheritdoc/>
    public override void Write(string? message)
    {
        if (message != null)
        {
            _messages.Add(message);
        }
    }

    /// <inheritdoc/>
    public override void WriteLine(string? message)
    {
        if (message != null)
        {
            _messages.Add(message);
        }
    }

    /// <inheritdoc/>
    public override void TraceEvent(TraceEventCache? eventCache, string source, TraceEventType eventType, int id, string? message)
    {
        if (message != null)
        {
            _messages.Add(message);
        }
    }

    /// <inheritdoc/>
    public override void TraceEvent(TraceEventCache? eventCache, string source, TraceEventType eventType, int id, string? format, params object?[]? args)
    {
        if (format != null)
        {
            string message = string.Format(format, args!);
            _messages.Add(message);
        }
    }
}
