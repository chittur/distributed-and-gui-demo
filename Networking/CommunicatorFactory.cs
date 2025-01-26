﻿/******************************************************************************
 * Filename    = CommunicatorFactory.cs
 *
 * Author      = Ramaswamy Krishnan-Chittur
 *
 * Product     = GuiAndDistributedDemo
 * 
 * Project     = Networking
 *
 * Description = Factory for creating a communicator.
 *****************************************************************************/

using System.Diagnostics;
using System.Net.Sockets;
using System.Net;

namespace Networking;

/// <summary>
/// Factory for creating a communicator.
/// </summary>
public static class CommunicatorFactory
{
    /// <summary>
    /// Creates a communicator.
    /// </summary>
    /// <returns>A new communicator instance</returns>
    public static ICommunicator CreateCommunicator()
    {
        // Create a random port number between 1000 and 65000.
        // Please note that this can throw if the port is already in use.
        int port = UdpCommunicator.GetRandomAvailablePort();
        Trace.TraceInformation($"Starting communicator in port {port}");
        return new UdpCommunicator(port);
    }
}
