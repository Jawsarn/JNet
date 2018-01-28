using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// All network objects using JNet should have this.
///</summary>
public class JNetIdentity : MonoBehaviour {
    public static uint m_nextID = 1; // start at 1, 0 is error
    public ulong ownerID = 0;

    uint m_netID = 0;
    public uint netID { get { return m_netID; } }
    uint m_sceneID;
    public uint sceneID { get { return m_sceneID; } } // Only used for objects that are placed in scene before run

    JNetBehaviour[] m_JNetBehaviours;

    private void Awake()
    {
        UpdateBehaviours();
    }

    ///<summary>
    /// Should only be called by postProcess script
    ///</summary>
    public void SetSceneID(uint newID)
    {
        m_sceneID = newID;
    }

    ///<summary>
    /// Should only be called by Spawn messagest
    ///</summary>
    public void SetNetID(uint newID)
    {
        m_netID = newID;

        // TODO add call for JNetBehavior NetAwake when ID been assigned

    }

    void UpdateBehaviours()
    {
        m_JNetBehaviours = GetComponents<JNetBehaviour>(); // TODO consider adding a behavior in runtime, problematic with serialization
    }
}
