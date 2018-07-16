
public enum JNetMessageType
{
    Invalid,
    Spawn,
    //SpawnSceneObject,
    Destroy,
    Serialize,
    Rpc
    // Add from here
}

public class JNetMessage
{
    ///<summary>
    /// Initializes the message with a bitStream, writing the size and type
    ///</summary>
    public JNetMessage(JNetMessageType type, ushort msgSize)
    {
        i_msgType = (ushort)type;
        i_bitStream = new JNetBitStream(msgSize + sizeof(ushort)*2);
        i_bitStream.WriteUShort(i_msgType);
        i_bitStream.WriteUShort(msgSize);
    }

    ///<summary>
    /// Initializes the message with a stream, used for reading
    ///</summary>
    public JNetMessage(JNetMessageType type, JNetBitStream stream)
    {
        i_bitStream = stream;
    }

    // The maximum data on each message will be 65,535 bytes, which should be reasonable
    // if you want to send more data, eg large files for some reason, create a Message type with a system that slice and reconstruct the data
    public const int m_maxMessageSize = 65535;

    // Type of message
    ushort i_msgType;
    public ushort m_msgType { get { return i_msgType; } }

    public ulong m_senderID;

    // Used to read from
    JNetBitStream i_bitStream;
    public JNetBitStream m_bitStream { get { return i_bitStream; } }

}