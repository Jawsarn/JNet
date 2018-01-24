using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JNetIdentity : MonoBehaviour {
    public static uint m_nextID = 1; // start at 1, 0 is error
    public ulong ownerID = 0;

    uint netID = 0;

    private void Awake()
    {
        //netID = m_nextID;
        // This is a bit problematic
        // If we do by editor etc to get ID for nonInstanciated objects, a newly loaded scene with JNetIdentities might have invalid IDs
        // e.g. 2 view objects in first scene and 6 in next, inbetween we instanciated some objects that are scene presistant
        // If we add them on Awake, it will be dependent on startorder, which is a problem
    }

    private void Start()
    {

    }
}
