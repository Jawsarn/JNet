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

    protected virtual void JNetWrite(JBitStream stream)
    {

    }

    protected virtual void JNetRead(JBitStream stream)
    {

    }
}
