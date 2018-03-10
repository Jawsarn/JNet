using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

///<summary>
/// Handling the network objects in the scene
///</summary>
namespace JNetInternal
{
    public class JNetSceneHandler
    {
        static Dictionary<uint, JNetIdentity> m_networkObjects = new Dictionary<uint, JNetIdentity>();

        internal static JNetIdentity FindNetIdentity(uint netID)
        {
            return m_networkObjects[netID];
        }

        internal static void ClearSceneObjects() 
        {
            // TODO check if we might just remove these OnDestroy on networkIdentity instead

            List<uint> m_toRemoveIDs = new List<uint>();
            
            // Find all sceneObjects
            foreach (var item in m_networkObjects)
            {
                if (item.Value.netID < JNetIdentity.maxNetObjectsPerPlayer)
                {
                    m_toRemoveIDs.Add(item.Value.netID);
                }
            }

            // Remove all sceneObjects
            for (int i = 0; i < m_toRemoveIDs.Count; i++)
            {
                m_networkObjects.Remove(m_toRemoveIDs[i]);
            }

            m_toRemoveIDs.Clear();
        }

        internal static void AddNetObject(JNetIdentity netID)
        {
            if (m_networkObjects.ContainsKey(netID.netID))
            {
                // TODO add warning
            }

            m_networkObjects[netID.netID] = netID;
        }
    }
}