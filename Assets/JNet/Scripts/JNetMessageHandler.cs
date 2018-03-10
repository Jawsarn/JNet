using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JNetInternal
{
    public static class JNetMessageHandler
    {
        static Dictionary<JNetConnection, Dictionary<uint, List<JNetMessage>>> m_messagesToSend = new Dictionary<JNetConnection, Dictionary<uint, List<JNetMessage>>>();

        public static Dictionary<JNetConnection, Dictionary<uint, List<JNetMessage>>> messagesToSend { get { return m_messagesToSend; } }

        public static void AddMessage(JNetMessage newMessage, bool buffered, uint channel = 0)
        {
            // Add message to all other connected clients
            foreach (var connection in JNetConnection.m_connections)
            {
                m_messagesToSend[connection][channel].Add(newMessage);
            }
        }
    }
}