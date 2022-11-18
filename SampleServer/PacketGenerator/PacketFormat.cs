using System;
using System.Collections.Generic;
using System.Text;

namespace PacketGenerator
{
    class PacketFormat
    {
        // {0} 패킷 이름
        // {1} 멤버 변수들

        public string packetFormat =
@"
class {0}
{{
    {1}

    public struct SkillInfo
    {{
        public int id;
        public short level;
        public float duration;

        public bool Write(Span<byte> span, ref ushort count)
        {{
            bool isSuccess = true;

            isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.id);
            count += sizeof(int);
            isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.level);
            count += sizeof(short);
            isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.duration);
            count += sizeof(float);

            return isSuccess;
        }}

        public void Read(ReadOnlySpan<byte> span, ref ushort count)
        {{
            bool isSuccess = true;
            this.id = BitConverter.ToInt32(span.Slice(count, span.Length - count));
            count += sizeof(int);
            this.level = BitConverter.ToInt16(span.Slice(count, span.Length - count));
            count += sizeof(short);
            this.duration = BitConverter.ToSingle(span.Slice(count, span.Length - count));
            count += sizeof(float);
        }}
    }}

    public List<SkillInfo> skills = new List<SkillInfo>();

    public void Read(ArraySegment<byte> segment)
    {{
        ushort count = 0;

        ReadOnlySpan<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.playerID = BitConverter.ToInt64(span.Slice(count, span.Length - count));
        count += sizeof(long);

        // string 변환
        ushort nameLen = BitConverter.ToUInt16(segment.Array, count);
        count += sizeof(ushort);
        this.name = Encoding.Unicode.GetString(span.Slice(count, nameLen));
        count += nameLen;

        // skill list
        skills.Clear();
        ushort skillLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
        count += sizeof(ushort);
        for (ushort i = 0; i < skillLen; ++i)
        {{
            SkillInfo skillInfo = new SkillInfo();
            skillInfo.Read(span, ref count);
            skills.Add(skillInfo);
        }}
    }}

    public ArraySegment<byte> Write()
    {{
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0;
        bool isSuccess = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.PlayerInfoReq);
        // playerID를 byte로 변환하여 저장
        count += sizeof(ushort);
        isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.playerID);
        // 전체 size를 byte로 변환하여 저장
        count += sizeof(long);

        // string
        ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
        isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), nameLen);
        count += sizeof(ushort);
        count += nameLen;

        // skill List
        isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)skills.Count);
        count += sizeof(ushort);
        foreach (SkillInfo skill in skills)
        {{
            isSuccess &= skill.Write(span, ref count);
        }}

        isSuccess &= BitConverter.TryWriteBytes(span, count);

        if (isSuccess == false)
            return null;

        return SendBufferHelper.Close(count);
    }}
}}
";
    }
}
