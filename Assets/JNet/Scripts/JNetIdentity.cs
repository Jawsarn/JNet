using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JNetIdentity : MonoBehaviour {
    public static uint m_nextID = 1; // start at 1, 0 is error
    public ulong ownerID = 0;

    uint netID = 0;
    uint m_sceneID;
    public uint sceneID { get { return m_sceneID; } } // Only used for objects that are placed in scene before run
    
    // TODO add call for JNetBehavior NetAwake when ID been assigned

    ///<summary>
    /// Should only be called by postProcess script
    ///</summary>
    public void SetSceneID(uint newID)
    {
        m_sceneID = newID;
    }
}
