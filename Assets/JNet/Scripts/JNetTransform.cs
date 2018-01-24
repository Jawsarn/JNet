using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JNetTransform : JNetBehaviour {

	void Start () {
        MyFunction();
    }
	
	// Update is called once per frame
	void Update () {
        if (isOwner && JNet.isMasterClient)
        {

        }
	}

    protected override void JNetWrite(JBitStream stream)
    {

    }

    protected override void JNetRead(JBitStream stream)
    {
        Vector3 newPos;
        stream.ReadVector3(out newPos);
    }

    void MyFunction()
    {
        // First way
        JNet.RPC("OpenDoor", 30.0f, 10, "wtf");

        // Second way
        JBitStream stream = new JBitStream(256);
        stream.WriteFloat(30.0f);
        stream.WriteInt(10);
        stream.WriteString("wtf");
        JNet.RPCAdv("OpenDoor2", stream);
    }

    [JRPC]
    void OpenDoor(float val1, int val2, string val3)
    {
        // Do stuff
    }

    [JRPC]
    void OpenDoor2(JBitStream stream)
    {
        float val1 = stream.ReadFloat();
        int val2 = stream.ReadInt();
        string val3 = null;
        stream.ReadString(out val3);

        // Do stuff
    }
}
