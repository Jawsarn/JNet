
public enum JNetMessageType
{
    Invalid,
    Spawn,
    Destroy,
    Serialize,
    Rpc
    // Add from here
}

public class JNetMessage
{
    // The maximum data on each message will be 65,535 bytes, which should be reasonable
    // if you want to send more data, eg large files for some reason, create a Message type with a system that slice and reconstruct the data
    public const int m_maxMessageSize = 65535;

    // Type of message
    public ushort m_msgType;

    public ulong m_senderID;

    // Used to read from
    public JBitStream m_bitStream;

}