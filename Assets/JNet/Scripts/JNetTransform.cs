using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JNetTransform : JNetBehaviour {

    private void Awake()
    {
        
    }
    void Start () {
        MyFunction();
    }
	
	// Update is called once per frame
	void Update () {
        if (isOwner && JNet.isMasterClient)
        {

        }
	}

    public override void OnWrite(JNetBitStream stream)
    {
        stream.WriteVector3(transform.position);
    }

    public override void OnRead(JNetBitStream stream)
    {
        transform.position = stream.ReadVector3();
    }

    void MyFunction()
    {
        // First way
        JNet.RPC("OpenDoor", JNetTarget.All, 30.0f, 10, "wtf");

        // Second way
        JNetBitStream stream = new JNetBitStream(256);
        stream.WriteFloat(30.0f);
        stream.WriteInt(10);
        stream.WriteString("wtf");
        JNet.RPCAdv("OpenDoor2", JNetTarget.All, stream);
    }

    [JRPC]
    void OpenDoor(float val1, int val2, string val3)
    {
        // Do stuff
    }

    [JRPC]
    void OpenDoor2(JNetBitStream stream)
    {
        float val1 = stream.ReadFloat();
        int val2 = stream.ReadInt();
        string val3 = null;
        stream.ReadString(out val3);

        // Do stuff
    }
}
