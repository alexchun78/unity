START ../../PacketGenerator/bin/PacketGenerator.exe ../../PacketGenerator/PDL.xml
@echo on echo d|XCOPY /Y "GenPackets.cs" "../../DummyClient/Packet"
@echo on echo d|XCOPY /Y "GenPackets.cs" "../../Client/Assets/Scripts/Packet"
@echo on echo d|XCOPY /Y "GenPackets.cs" "../../Server/Packet"
@echo on echo d|XCOPY /Y "ClientPacketManager.cs" "../../DummyClient/Packet"
@echo on echo d|XCOPY /Y "ClientPacketManager.cs" "../../Client/Assets/Scripts/Packet"
@echo on echo d|XCOPY /Y "ServerPacketManager.cs" "../../Server/Packet"