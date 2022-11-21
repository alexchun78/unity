using System;
using System.IO;
using System.Xml;

namespace PacketGenerator
{
    class Program
    {
        static string _genPacket;
        static ushort _packetID = 0;
        static string _packetEnums;

        static void Main(string[] args)
        {
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreWhitespace = true,
            };

            using (XmlReader reader = XmlReader.Create("PDL.xml", settings))
            {
                reader.MoveToContent();

                while(reader.Read())
                {                    
                    if (reader.Depth == 1 && reader.NodeType == XmlNodeType.Element)
                        ParsePacket(reader);
                    //Console.WriteLine($"Name : {reader.Name}, name : {reader["name"]}");
                }

                string fileText = string.Format(PacketFormat.fileFormat, _packetEnums, _genPacket);
                File.WriteAllText("GamePacket.cs", fileText);
            }
        }

        private static void ParsePacket(XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.EndElement)
                return;

            if (reader.Name.ToLower() != "packet")
            {
                Console.WriteLine("Invalid Packet");
                return;
            }
             
            string packetName = reader["name"];
            if(string.IsNullOrEmpty(packetName))
            {
                Console.WriteLine("Packet without name");
                return;
            }

            Tuple<string, string, string> tempResult = ParseMembers(reader);
            _genPacket += string.Format(PacketFormat.packetFormat, packetName, tempResult.Item1, tempResult.Item2, tempResult.Item3);
            _packetEnums += string.Format(PacketFormat.packetEnumFormat, packetName, ++_packetID)+"\t";
        }

        // {1} 멤버 변수들
        // {2} 멤버 변수 Read
        // {3} 멤버 변수 Write
        private static Tuple<string, string, string> ParseMembers(XmlReader reader)
        {
            string packetName = reader["name"];

            string memberCode = "";
            string readCode = "";
            string writeCode = "";

            int depth = reader.Depth + 1;
            while(reader.Read())
            {
                if (reader.Depth != depth)
                    break;

                //string ttt = reader.Name;

                string memberName = reader["name"];
                if (string.IsNullOrEmpty(memberName))
                {
                    Console.WriteLine("Member without name");
                    continue;
                  //  return null;
                }

                if (string.IsNullOrEmpty(memberCode) == false)
                    memberCode += Environment.NewLine;
                if (string.IsNullOrEmpty(readCode) == false)
                    readCode += Environment.NewLine;
                if (string.IsNullOrEmpty(writeCode) == false)
                    writeCode += Environment.NewLine;

                string memberType = reader.Name.ToLower();
                switch(memberType)
                {
                    case "byte":
                    case "sbyte":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readByteFormat, memberName, memberType);
                        writeCode += string.Format(PacketFormat.writeByteFormat, memberName, memberType);
                        break;
                    case "bool":
                    case "short":
                    case "ushort":
                    case "int":
                    case "long":
                    case "float":
                    case "double":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readFormat, memberName, ToMemberType(memberType), memberType);
                        writeCode += string.Format(PacketFormat.writeFormat, memberName, memberType);
                        break;
                    case "string":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readStringFormat, memberName);
                        writeCode += string.Format(PacketFormat.writeStringFormat, memberName);
                        break;
                    case "list":
                        Tuple<string, string, string> temp = ParseList(reader);
                        memberCode += temp.Item1;
                        readCode += temp.Item2;
                        writeCode += temp.Item3;
                        break;
                    default:
                        break;

                }
            }

            memberCode = memberCode.Replace("\n", "\n\t");
            readCode = readCode.Replace("\n", "\n\t\t");
            writeCode = writeCode.Replace("\n", "\n\t\t");
            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        private static Tuple<string, string, string> ParseList(XmlReader reader)
        {
            string listName = reader["name"];
            if(string.IsNullOrEmpty(listName))
            {
                Console.WriteLine("List without name");
                return null;
            }

            Tuple<string, string, string> tempResult = ParseMembers(reader);
            string memberCode = string.Format(PacketFormat.memberListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName),
                tempResult.Item1,
                tempResult.Item2,
                tempResult.Item3
                );

            string readCode = string.Format(PacketFormat.readListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName)
                );
            string writeCode = string.Format(PacketFormat.writeListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName)
                );

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        public static string ToMemberType(string memberType)
        {
            switch (memberType)
            {
                case "bool":
                    return "ToBoolean";
                case "short":
                    return "ToInt16";
                case "ushort":
                    return "ToUInt16";
                case "int":
                    return "ToInt32";
                case "long":
                    return "ToInt64";
                case "float":
                    return "ToSingle";
                case "double":
                    return "ToDouble";
                case "byte":
                default:
                    return null;
            }
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input[0].ToString().ToUpper() + input.Substring(1);
        }
        public static string FirstCharToLower(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input[0].ToString().ToLower() + input.Substring(1);
        }
    }
}
