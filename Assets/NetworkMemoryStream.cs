using System;
using System.IO;

public class NetworkMemoryStream
{
    private MemoryStream m_memoryStream;
    public BinaryWriter Writer;
    public BinaryReader Reader;
    public NetworkMemoryStream()
    {
        m_memoryStream = new MemoryStream();
        Writer = new BinaryWriter(m_memoryStream);
        Reader = new BinaryReader(m_memoryStream);
    }
    public void Reset()
    {
        m_memoryStream.SetLength(0);
    }
    public void Set(ArraySegment<byte> messageSegment)
    {
        Reset();
        m_memoryStream.Write(messageSegment);
        m_memoryStream.Position = 0;
    }
}