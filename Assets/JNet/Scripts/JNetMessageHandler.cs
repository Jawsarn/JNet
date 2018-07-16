using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JNetInternal
{
    public static class JNetMessageHandler
    {
        static Dictionary<JNetConnection, List<JNetMessage>[]> m_reliableFrameMessagesToSend = new Dictionary<JNetConnection, List<JNetMessage>[]>();
        public static Dictionary<JNetConnection, List<JNetMessage>[]> reliableFrameMessagesToSend { get { return m_reliableFrameMessagesToSend; } }

        static Dictionary<JNetConnection, List<JNetMessage>[]> m_unreliableFrameMessagesToSend = new Dictionary<JNetConnection, List<JNetMessage>[]>();
        public static Dictionary<JNetConnection, List<JNetMessage>[]> unreliableFrameMessagesToSend { get { return m_unreliableFrameMessagesToSend; } }

        static List<JNetMessage> m_bufferedMessages = new List<JNetMessage>();

        public static void AddMessage(JNetMessage newMessage, JNetTarget target, uint channel = 0)
        {
            if (JNetManager.m_singleton.m_channels[channel] == JNetManager.ChannelType.Reliable)
            {
                var clientConn = JNet.GetClientConnection();
                switch (target)
                {
                    case JNetTarget.All:
                        // Add message to all connections
                        foreach (var connection in JNetConnection.m_connections)
                        {
                            m_reliableFrameMessagesToSend[connection][channel].Add(newMessage);
                        }

                        break;
                    case JNetTarget.Others:
                        // Add message to all other connections
                        foreach (var connection in JNetConnection.m_connections)
                        {
                            if (connection != clientConn)
                            {
                                m_reliableFrameMessagesToSend[connection][channel].Add(newMessage);
                            }
                        }
                        break;
                    case JNetTarget.MasterClient:
                        // Add message to only master
                        var masterConn = JNet.GetMasterClientConnection();
                        if (m_reliableFrameMessagesToSend.ContainsKey(masterConn))
                        {
                            m_reliableFrameMessagesToSend[masterConn][channel].Add(newMessage);
                        }
                        
                        break;
                    case JNetTarget.AllBuffered:
                        // Add message to all connections
                        foreach (var connection in JNetConnection.m_connections)
                        {
                            m_reliableFrameMessagesToSend[connection][channel].Add(newMessage);
                        }
                        m_bufferedMessages.Add(newMessage);
                        break;
                    case JNetTarget.OthersBuffered:
                        // Add message to all other connections
                        foreach (var connection in JNetConnection.m_connections)
                        {
                            if (connection != clientConn)
                            {
                                m_reliableFrameMessagesToSend[connection][channel].Add(newMessage);
                            }
                        }
                        m_bufferedMessages.Add(newMessage);
                        break;
                    case JNetTarget.AllViaServer:
                        break;
                    case JNetTarget.AllBufferedViaServer:
                        break;
                    default:
                        break;
                }
            }
            else //(JNetManager.m_singleton.m_channels[channel] == JNetManager.ChannelType.Unreliable)
            {
                var clientConn = JNet.GetClientConnection();
                switch (target)
                {
                    case JNetTarget.All:
                    case JNetTarget.AllBuffered:
                        // Add message to all connections
                        foreach (var connection in JNetConnection.m_connections)
                        {
                            m_unreliableFrameMessagesToSend[connection][channel].Add(newMessage);
                        }

                        break;
                    case JNetTarget.Others:
                    case JNetTarget.OthersBuffered:
                        // Add message to all other connections
                        foreach (var connection in JNetConnection.m_connections)
                        {
                            if (connection != clientConn)
                            {
                                m_unreliableFrameMessagesToSend[connection][channel].Add(newMessage);
                            }
                        }
                        break;
                    case JNetTarget.MasterClient:
                        // Add message to only master
                        var masterConn = JNet.GetMasterClientConnection();
                        if (m_unreliableFrameMessagesToSend.ContainsKey(masterConn))
                        {
                            m_unreliableFrameMessagesToSend[masterConn][channel].Add(newMessage);
                        }

                        break;
                    case JNetTarget.AllViaServer:
                        break;
                    case JNetTarget.AllBufferedViaServer:
                        break;
                    default:
                        break;
                }
            }
        }

        public static void AddConnection(JNetConnection conn)
        {
            if (m_frameMessagesToSend.ContainsKey(conn) == false)
            {
                // Create lists according to number of channels
                int numChannels = JNetManager.m_singleton.m_channels.Length;
                var channelToMsgList = new List<JNetMessage>[numChannels];

                // Create new list at each channel
                for (int i = 0; i < numChannels; i++)
                {
                    channelToMsgList[i] = new List<JNetMessage>();
                }

                m_frameMessagesToSend.Add(conn, channelToMsgList);
            }
        }

        public static void RemoveConnection(JNetConnection conn)
        {
            m_frameMessagesToSend.Remove(conn);
        }
    }
}