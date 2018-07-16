using Steamworks;
using System;
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

        static int maxPacketSize = 508; // only unrealiable
        // Looking at UDP packet size
        // https://stackoverflow.com/questions/14993000/the-most-reliable-and-efficient-udp-packet-size
        // 508 or 1492
        // 1200 is biggest for steam p2p unrealiable

        public static void ProcessIncommingPackets()
        {
            uint msgSize;
            uint msgCount;
            byte[] buffer;
            CSteamID senderID;
            for (int channel = 0; channel < JNetManager.m_singleton.m_channels.Length; channel++)
            {
                msgCount = 0;
                while (SteamNetworking.IsP2PPacketAvailable(out msgSize) && msgCount < JNetManager.m_singleton.m_maxPacketPerChannelPerUpdate)
                {
                    Debug.Log("receiving something");
                    msgCount++;
                    buffer = new byte[msgSize];
                    if (SteamNetworking.ReadP2PPacket(buffer, 0, out msgSize, out senderID, channel))
                    {
                        // Read packet
                        ReadPacket(buffer, msgSize, senderID.m_SteamID);
                    }
                }
            }
        }

        static void ReadPacket(byte[] buffer, uint fullMsgSize, ulong senderID)
        {
            // TODO check what we do here
            if (fullMsgSize < 4)
            {
                return;
            }

            JNetBitStream fullStream = new JNetBitStream(buffer, (int)fullMsgSize);

            // While there is data left to read
            while (fullStream.BytesUsed < fullMsgSize)
            {
                // Read data of message and copy to new Message
                ushort msgType = fullStream.ReadUShort();
                ushort msgSize = fullStream.ReadUShort(); // The maximum data on each message wil be 65,535 bytes, which should be reasonable, if you want to send more, create system that slice data
                byte[] msgBuffer = new byte[msgSize];
                fullStream.ReadByteArray(msgBuffer);

                // Check if we have function for this message
                if (m_msgFunctionDict.ContainsKey(msgType))
                {
                    // Create message
                    JNetMessage newMessage = new JNetMessage((JNetMessageType)msgType, new JNetBitStream(msgBuffer));
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

        public static void ProcessOutgoingPackets()
        {
            // TODO make this array and clear each frame instead 
            List<List<JNetMessage>> unreliableMessages = new List<List<JNetMessage>>();

            // Fix frame messages
            var frameMessages = JNetMessageHandler.frameMessagesToSend;
            var channels = JNetManager.m_singleton.m_channels;
            int channelsLen = channels.Length;

            // Can we send to target?

            foreach (var connToChannelList in frameMessages)
            {
                for (int i = 0; i < channelsLen; i++)
                {
                    if (channels[i] == JNetManager.ChannelType.Reliable)
                    {
                        var messages = connToChannelList.Value[i];
                        foreach (var message in messages)
                        {
                            switch (message.m_target)
                            {
                                case JNetTarget.All:
                                    break;
                                case JNetTarget.Others:
                                    break;
                                case JNetTarget.MasterClient:
                                    break;
                                case JNetTarget.AllBuffered:
                                    break;
                                case JNetTarget.OthersBuffered:
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
                    else //if (channels[i] == JNetManager.ChannelType.Unreliable) 
                    {
                        var messages = connToChannelList.Value[i];
                        foreach (var message in messages)
                        {
                            switch (message.m_target)
                            {
                                case JNetTarget.All:
                                case JNetTarget.AllBuffered:
                                    break;
                                case JNetTarget.MasterClient:
                                    break;
                                case JNetTarget.Others:
                                case JNetTarget.OthersBuffered:
                                    break;
                                case JNetTarget.AllViaServer:
                                case JNetTarget.AllBufferedViaServer:
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                }
            }



            // Start sending messages
            var outgoingMessages = JNetMessageHandler.unreliableMessagesToSend;
            var channels = JNetManager.m_singleton.m_channels;













            foreach (var senderToMessages in outgoingMessages)
            {
                // TODO consider using the count of message dict, pro con?
                for (uint channel = 0; channel < senderToMessages.Value.Count; channel++)
                {
                    // TODO consider one size always
                    // Check how large packet we need
                    int messagesToUseCount = 0;
                    int currSize = 0;
                    foreach (var item in senderToMessages.Value[channel])
                    {
                        currSize += item.m_bitStream.BytesUsed;
                        if (currSize > maxPacketSize)
                        {
                            break;
                        }
                        messagesToUseCount++;
                    }

                    // Create message containig data
                    byte[] dataToSend = new byte[currSize]; // TODO consider making this one size and have it only once allocated
                    foreach (var item in senderToMessages.Value[channel])
                    {
                        if (messagesToUseCount == 0)
                        {
                            break;
                        }

                        int offset = 0;
                        for (int i = 0; i < messagesToUseCount; i++)
                        {
                            int amount = item.m_bitStream.BytesUsed;
                            Buffer.BlockCopy(item.m_bitStream.Data, 0, dataToSend, offset, amount);
                            offset += amount;
                        }
                        messagesToUseCount--;
                    }
                    if(SteamNetworking.SendP2PPacket((CSteamID)senderToMessages.Key.clientID, dataToSend, 0, channels[channel], (int)channel))
                    {
                        // Remove if successfully send
                        senderToMessages.Value[channel].RemoveRange(0, messagesToUseCount);
                    }
                    else
                    {
                        // TODO consider breaking
                        continue;
                    }
                }
            }
        }
    }
}