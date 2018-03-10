using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JNetInternal
{
    ///<summary>
    /// Handles internal messages of spawning, destroying, serializing etc. Register called from JNetManager.
    ///</summary>
    public class JNetInternalMessageLogic
    {
        public void RegisterCallbacks()
        {
            JNetPacketHandler.RegisterMessageCallback(JNetMessageType.Spawn, OnSpawnMessage);
            JNetPacketHandler.RegisterMessageCallback(JNetMessageType.Destroy, OnDestroyMessage);
            JNetPacketHandler.RegisterMessageCallback(JNetMessageType.Serialize, OnSerializeMessage);
            JNetPacketHandler.RegisterMessageCallback(JNetMessageType.Rpc, OnRpcMessage);
        }

        void OnSpawnMessage(JNetMessage msg)
        {
            if (msg.m_senderID != JNet.GetCurrentHostID())
            {
                // Read prefab, pos, rot and authority
                uint netID = msg.m_bitStream.ReadUInt();
                ushort prefabID = msg.m_bitStream.ReadUShort();
                Vector3 spawnPos = msg.m_bitStream.ReadVector3();
                Quaternion spawnRot = msg.m_bitStream.ReadQuaternion();
                bool senderAuthority = msg.m_bitStream.ReadBool();

                // Spawn object
                GameObject newObject = JNetManager.m_singleton.InstanciateFromPrefab(prefabID, spawnPos, spawnRot);
                if (newObject != null)
                {
                    JNetIdentity identity = newObject.GetComponent<JNetIdentity>();
                    if (senderAuthority)
                    {
                        identity.ownerID = msg.m_senderID;
                    }
                    else
                    {
                        identity.ownerID = JNet.GetClientID();
                    }
                }
                else
                {
                    // TODO add error msg
                }
            }
            else
            {
                // TODO add error message
            }
        }

        void OnDestroyMessage(JNetMessage msg)
        {
            if (JNet.isMasterClient)
            {
                // Destroy and tell everyone else
            }
            else if (msg.m_senderID == JNet.GetCurrentHostID())
            {

            }
            else
            {
                // TODO add error message
            }
        }

        void OnSerializeMessage(JNetMessage msg)
        {
            uint netID = msg.m_bitStream.ReadUInt();

            JNetIdentity netIdentity = JNetSceneHandler.FindNetIdentity(netID);

            if (netIdentity != null)
            {
                netIdentity.Deserialize(msg.m_bitStream);
            }
        }

        void OnRpcMessage(JNetMessage msg)
        {

        }
    }
}