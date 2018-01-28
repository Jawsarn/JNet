using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JNetInternal
{
    public class JNetInternalMessageLogic
    {

        //Invalid,
        //Spawn,
        //Destroy,
        //Serialize,
        //Rpc

        public void RegisterCallbacks()
        {
            JNetPacketHandler.RegisterMessageCallback(JNetMessageType.Spawn, OnSpawnMessage);
            JNetPacketHandler.RegisterMessageCallback(JNetMessageType.SpawnSceneObject, OnSpawnSceneObjectMessage);
            JNetPacketHandler.RegisterMessageCallback(JNetMessageType.Destroy, OnDestroyMessage);
            JNetPacketHandler.RegisterMessageCallback(JNetMessageType.Serialize, OnSerializeMessage);
            JNetPacketHandler.RegisterMessageCallback(JNetMessageType.Rpc, OnRpcMessage);
        }

        void OnSpawnMessage(JNetMessage msg)
        {
            if (JNet.isMasterClient)
            {
                // Spawn and tell everyone else
                
            }
            else if (msg.m_senderID == JNet.GetCurrentHostID())
            {
                // Read prefab, pos, rot and authority
                uint netID = msg.m_bitStream.ReadUInt();
                ushort prefabID = msg.m_bitStream.ReadUShort();
                Vector3 spawnPos;
                msg.m_bitStream.ReadVector3(out spawnPos);
                Quaternion spawnRot;
                msg.m_bitStream.ReadQuaternion(out spawnRot);
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

        void OnSpawnSceneObjectMessage(JNetMessage msg)
        {
            if (JNet.isMasterClient)
            {
                // Spawn and tell everyone else?
            }
            else if (msg.m_senderID == JNet.GetCurrentHostID())
            {
                // Read netID and sceneID
                uint netID = msg.m_bitStream.ReadUInt();
                uint sceneID = msg.m_bitStream.ReadUInt(); // change to short?

                // Find sceneObject
                GameObject sceneObj = JNetSceneHandler.FindSceneObjectWithID(sceneID);
                if (sceneObj != null)
                {
                    
                }
                else
                {
                    // TODO error msg
                }
            }
            else
            {
                // TODO error msg
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

        }

        void OnRpcMessage(JNetMessage msg)
        {

        }
    }
}