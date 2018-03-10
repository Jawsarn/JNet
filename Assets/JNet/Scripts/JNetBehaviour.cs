using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JNetIdentity))]
public abstract class JNetBehaviour : MonoBehaviour {

    public bool isOwner = false;

    ///<summary>
    /// Called when netID has been assigned, (is not assigned in Awake) 
    ///</summary>
    public virtual void NetAwake()
    {

    }

    public virtual void OnWrite(JNetBitStream stream)
    {

    }

    public virtual void OnRead(JNetBitStream stream)
    {

    }
}
