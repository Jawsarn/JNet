using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// All network objects using JNet should have this.
///</summary>
public class JNetIdentity : MonoBehaviour {

    ///<summary>
    /// The maximum amount of network objects per scene or player.
    /// Change this if you need more, but will limit the maximum amount of players.
    ///</summary>
    public const uint maxNetObjectsPerPlayer = 10000;
    
    public static uint m_currentOffsettID = 0;
    public ulong ownerID = 0;

    public uint m_ownerIndex = 0;
    public uint ownerIndex { get { return m_ownerIndex; } }

    uint m_netID = 0;
    public uint netID { get { return m_netID; } }

    JNetBehaviour[] m_JNetBehaviours;

    private void Awake()
    {
        UpdateBehaviours();
    }

    ///<summary>
    /// Should only be called by Spawn message
    ///</summary>
    public void SetNetID(uint newID)
    {
        m_netID = newID;

        // TODO add call for JNetBehavior NetAwake when ID been assigned

    }

    internal void GenerateNextID()
    {
        m_netID = ownerIndex*maxNetObjectsPerPlayer + m_currentOffsettID++;
    }

    ///<summary>
    /// Should only be called by Spawn message
    ///</summary>
    public void SetOwnerIndex(uint newIndex)
    {
        m_ownerIndex = newIndex;
    }

    void UpdateBehaviours()
    {
        m_JNetBehaviours = GetComponents<JNetBehaviour>(); // TODO consider adding a behavior in runtime, problematic with serialization
    }

    internal void Deserialize(JNetBitStream m_bitStream)
    {
        foreach (var behaviour in m_JNetBehaviours)
        {
            behaviour.OnRead(m_bitStream);
        }
    }

}
