set FightDir=../Packets/cs_fight_packet/
set HotFixDir=../Packets/cs_hotfix_packet/

PacketTool -outpath=%HotFixDir% -filename=Packet_Login -type=cs -fight=false
PacketTool -outpath=%FightDir% -filename=Packet_Battle -type=cs -fight=true
pause