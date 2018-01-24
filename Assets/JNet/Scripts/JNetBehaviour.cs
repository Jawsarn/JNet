using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JNetIdentity))]
public abstract class JNetBehaviour : MonoBehaviour {

    public bool isOwner = false;

    protected virtual void JNetWrite(JBitStream stream)
    {

    }

    protected virtual void JNetRead(JBitStream stream)
    {

    }
}
