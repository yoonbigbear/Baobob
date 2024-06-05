// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace MyGame.Sample
{

using global::System;
using global::System.Collections.Generic;
using global::Google.FlatBuffers;

public struct Packet : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_24_3_25(); }
  public static Packet GetRootAsPacket(ByteBuffer _bb) { return GetRootAsPacket(_bb, new Packet()); }
  public static Packet GetRootAsPacket(ByteBuffer _bb, Packet obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public Packet __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public string Message { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetMessageBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
  public ArraySegment<byte>? GetMessageBytes() { return __p.__vector_as_arraysegment(4); }
#endif
  public byte[] GetMessageArray() { return __p.__vector_as_array<byte>(4); }

  public static Offset<MyGame.Sample.Packet> CreatePacket(FlatBufferBuilder builder,
      StringOffset messageOffset = default(StringOffset)) {
    builder.StartTable(1);
    Packet.AddMessage(builder, messageOffset);
    return Packet.EndPacket(builder);
  }

  public static void StartPacket(FlatBufferBuilder builder) { builder.StartTable(1); }
  public static void AddMessage(FlatBufferBuilder builder, StringOffset messageOffset) { builder.AddOffset(0, messageOffset.Value, 0); }
  public static Offset<MyGame.Sample.Packet> EndPacket(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<MyGame.Sample.Packet>(o);
  }
}


static public class PacketVerify
{
  static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
  {
    return verifier.VerifyTableStart(tablePos)
      && verifier.VerifyString(tablePos, 4 /*Message*/, false)
      && verifier.VerifyTableEnd(tablePos);
  }
}

}
