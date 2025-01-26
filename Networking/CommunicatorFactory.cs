/******************************************************************************
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
        int port = UdpCommunicator.GetRandomAvailablePort();
        Trace.TraceInformation($"Starting communicator in port {port}");
        return new UdpCommunicator(port);
    }
}
