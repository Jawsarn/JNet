using System;
using Steamworks;
using JNetInternal;

public class JRPC : Attribute
{

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


    public static void RPC(string name, params object[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            var type = values[i].GetType();
            //NetworkWriter writer = new NetworkWriter();
        }
    }

    ///<summary>
    /// Advanced version of RPC where you use a stream, this can be used to improve bandwith by using e.g. half/bools as bits or other compressed info
    ///</summary>
    public static void RPCAdv(string name, JBitStream stream)
    {
        JBitStream readStream = new JBitStream(stream.Data, stream.BytesUsed);
        float val1 = readStream.ReadFloat();
        int val2 = readStream.ReadInt();
        string val3 = null;
        readStream.ReadString(out val3);
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