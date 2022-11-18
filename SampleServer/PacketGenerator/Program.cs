using System;
using System.Xml;

namespace PacketGenerator
{
    class Program
    {
        

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

            ParseMembers(reader);
        }

        private static void ParseMembers(XmlReader reader)
        {
            string packetName = reader["name"];

            int depth = reader.Depth + 1;
            while(reader.Read())
            {
                if (reader.Depth != depth)
                    break;

                string memberName = reader["name"];
                if (string.IsNullOrEmpty(memberName))
                {
                    Console.WriteLine("Member without name");
                    return;
                }

                string memberType = reader.Name.ToLower();
                switch(memberType)
                {
                    case "long":
                        break;
                    case "string":
                        break;
                    case "int":
                        break;
                    case "short":
                        break;
                    case "float":
                        break;
                    case "byte":
                        break;
                    case "bool":
                        break;
                    case "double":
                        break;
                    case "list":
                        break;
                    default:
                        break;

                }
            }
        }
    }
}
