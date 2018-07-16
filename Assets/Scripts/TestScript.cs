using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        JNetBitStream stream = new JNetBitStream(300);
        stream.WriteUShort(5);
        stream.WriteUShort(43);
        stream.WriteFloat(1337.0f);
        stream.WriteBool(true);
        stream.WriteBool(true);
        stream.WriteBool(true);
        stream.WriteBool(false);
        stream.WriteBool(true);
        stream.WriteBool(true);
        //JNetInternal.JNetPacketHandler.ReadPacket(stream.Data, (uint)stream.Data.Length, 0);
    }
	
	// Update is called once per frame
	void Update () {

	}
}
