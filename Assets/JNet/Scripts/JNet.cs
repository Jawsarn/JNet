using System;
using Steamworks;
using JNetInternal;
using UnityEngine;

public class JRPC : Attribute
{

}


///<summary>
/// How the RPC will execute between clients.
///</summary>
// Mirroring Photons version
public enum JNetTarget
{
    All,
    Others,
    MasterClient,
    AllBuffered,
    OthersBuffered,
    AllViaServer,
    AllBufferedViaServer
}

// This class is used as a facade mostly
public class JNet
{
    public enum LobbyType
    {
        Private,        // only way to join the lobby is to invite to someone else
        FriendsOnly,    // shows for friends or invitees, but not in lobby list
        Public,         // visible for friends and in lobby list
        Invisible,		// returned by search, but not visible to other friends
    }

    public static bool isMasterClient = false;

    public static ulong GetCurrentHostID()
    {
        return JNetManager.m_singleton.GetCurrentHostID();
    }

    public static ulong GetClientID()
    {
        return JNetManager.m_singleton.GetCurrentHostID();
    }

    public static uint GetClientIndex()
    {
        return JNetManager.m_singleton.GetCurrentClientIndex();
    }

    public static void RPC(string name, JNetTarget target, params object[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            var type = values[i].GetType();
            //NetworkWriter writer = new NetworkWriter();
            // TODO continue
        }
    }

    ///<summary>
    /// Advanced version of RPC where you use a stream, this can be used to improve bandwith by using e.g. half/bools as bits or other compressed info
    ///</summary>
    public static void RPCAdv(string name, JNetTarget target, JNetBitStream stream)
    {
        JNetBitStream readStream = new JNetBitStream(stream.Data, stream.BytesUsed);
        float val1 = readStream.ReadFloat();
        int val2 = readStream.ReadInt();
        string val3 = null;
        readStream.ReadString(out val3);
        // TODO continue
    }

    public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion orientation)
    {
        GameObject prefabObj = JNetManager.m_singleton.GetPrefab(prefab);
        return InstantiateAndSpawn(prefabObj, position, orientation);
    }

    public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion orientation)
    {
        GameObject prefabObj = JNetManager.m_singleton.GetPrefab(prefabName);
        return InstantiateAndSpawn(prefabObj, position, orientation); ;
    }

    private static GameObject InstantiateAndSpawn(GameObject prefab, Vector3 position, Quaternion orientation)
    {
        if (prefab != null)
        {
            GameObject newObject = GameObject.Instantiate(prefab, position, orientation);
            JNetIdentity netID = newObject.GetComponent<JNetIdentity>();
            netID.SetOwnerIndex(GetClientIndex());
            netID.GenerateNextID();

            ushort prefabIndex = JNetManager.m_singleton.GetPrefabIndex(prefab);

            // Create stream to write data
            JNetMessage newMessage = new JNetMessage(JNetMessageType.Spawn, sizeof(uint) + sizeof(float) * 3 + sizeof(float) * 4);
            newMessage.m_bitStream.WriteUShort(prefabIndex);
            newMessage.m_bitStream.WriteUInt(netID.netID);
            newMessage.m_bitStream.WriteVector3(position);
            newMessage.m_bitStream.WriteQuaternion(orientation);
            newMessage.m_senderID = GetClientID(); // Not sure if needed?

            JNetMessageHandler.AddMessage(newMessage, false); // Don't buffer, since we want to send an updated position

            return newObject;
        }
        else
        {
            return null;
        }
    }

    public static void StartHosting(LobbyType lobbyType)
    {
        ELobbyType stype = ELobbyType.k_ELobbyTypeFriendsOnly;
        switch (lobbyType)
        {
            case LobbyType.Private:
                stype = ELobbyType.k_ELobbyTypePrivate;
                break;
            case LobbyType.FriendsOnly:
                stype = ELobbyType.k_ELobbyTypeFriendsOnly;
                break;
            case LobbyType.Public:
                stype = ELobbyType.k_ELobbyTypePublic;
                break;
            case LobbyType.Invisible:
                stype = ELobbyType.k_ELobbyTypeInvisible;
                break;
            default:
                break;
        }

        JNetManager.m_singleton.CreateLobby(stype);
    }

    public static void JoinUsersLobby(ulong userID)
    {

    }

    public static void JoinLobby(ulong lobbyID)
    {
        JNetManager.m_singleton.JoinLobby((CSteamID)lobbyID);
    }

    public static void JoinRandomLobby()
    {
        JNetManager.m_singleton.JoinRandomLobby();
    }

    public static void RegisterMessageCallback(JNetMessageType type, OnMessageReceivedFunction callbackFunc)
    {
        JNetPacketHandler.RegisterMessageCallback(type, callbackFunc);
    }

    public static void UnregisterMessageCallback(JNetMessageType type)
    {
        JNetPacketHandler.UnregisterMessageCallback(type);
    }
}