using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnMessageReceivedFunction(JNetMessage netMessage);

namespace JNetInternal
{
    // Responsible for reading packets and delivering them to networkIDs
    public static class JNetPacketHandler
    {
        static Dictionary<ushort, OnMessageReceivedFunction> m_msgFunctionDict = new Dictionary<ushort, OnMessageReceivedFunction>();


        // Looking at UDP packet size
        // https://stackoverflow.com/questions/14993000/the-most-reliable-and-efficient-udp-packet-size
        // 508 or 1492

        public static void ReadPacket(byte[] buffer, uint fullMsgSize, ulong senderID)
        {
            // TODO check what we do here
            if (fullMsgSize < 4)
            {
                return;
            }

            JBitStream fullStream = new JBitStream(buffer, (int)fullMsgSize);

            // While there is data left to read
            while (fullStream.BytesUsed < fullMsgSize)
            {
                // Read data of message and copy to new Message
                ushort msgSize = fullStream.ReadUShort(); // The maximum data on each message wil be 65,535 bytes, which should be reasonable, if you want to send more, create system that slice data
                ushort msgType = fullStream.ReadUShort();
                byte[] msgBuffer = new byte[msgSize];
                fullStream.ReadByteArray(msgBuffer);

                // Check if we have function for this message
                if (m_msgFunctionDict.ContainsKey(msgType))
                {
                    // Create message
                    JNetMessage newMessage = new JNetMessage();
                    newMessage.m_msgType = msgType;
                    newMessage.m_bitStream = new JBitStream(msgBuffer);
                    newMessage.m_senderID = senderID;

                    // Call function handling this message
                    m_msgFunctionDict[msgType](newMessage);
                }
                else
                {
                    // TODO add error message
                }
            }
        }

        public static void RegisterMessageCallback(JNetMessageType type, OnMessageReceivedFunction callbackFunc)
        {
            m_msgFunctionDict.Add((ushort)type, callbackFunc);
        }

        public static void UnregisterMessageCallback(JNetMessageType type)
        {
            m_msgFunctionDict.Remove((ushort)type);
        }

        public static void SendPackets()
        {

        }
    }
}