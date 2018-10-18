


function main(toType,outPath,filename,fight,enum, message,packet,import)
	if(toType=="cpp") then
		createPacketCPP(enum,message,outPath,filename,packet);
	elseif (toType=="cs") then
		createPacketCS(enum,message,outPath,filename,fight,packet);
	elseif (toType=="java") then
	    createPacketJAVA(enum,message,outPath,filename,packet);
	end
end

local BaseDataType =
{
	int8 	=
			{ 
				CPP = 	{ Type = "int8" , 		Read = ">>" , 				Write = "<<", 		}, 	
				CS 	= 	{ Type = "sbyte" , 		Read = "ReadSByte" , 		Write = "Write",	},
				JAVA = 	{ Type = "byte" ,    	Read = "readByte" , 		Write = "writeByte", ArrayListType = "Byte"},
			},
	uint8	= 
			{
				CPP = 	{ Type = "uint8" ,		Read = ">>" , 				Write = "<<" ,		},	
				CS 	= 	{ Type = "byte"	,		Read = "ReadByte",			Write = "Write",	},
				JAVA 	= 	{ Type = "byte"	,		Read = "readByte",			Write = "writeByte", ArrayListType = "Byte"},
			},
	int16 	= 
			{
				CPP = 	{ Type = "int16" ,		Read = ">>" , 				Write = "<<" ,		},	
				CS 	= 	{ Type = "short",		Read = "ReadShort",			Write = "Write",	},
				JAVA = 	{ Type = "short",		Read = "readShort",			Write = "writeShort", ArrayListType = "Short"},
			},
	uint16 	= 
			{
				CPP = 	{ Type = "uint16" ,		Read = ">>" , 				Write = "<<" ,		},	
				CS 	= 	{ Type = "ushort",		Read = "ReadUShort",		Write = "Write",	},
				JAVA = 	{ Type = "short",		Read = "readShort",		Write = "writeShort", ArrayListType = "Short"},			
			},
	int32	= 
			{ 
				CPP = 	{ Type = "int" ,		Read = ">>" , 				Write = "<<" ,		},	
				CS 	= 	{ Type = "int"	,		Read = "ReadInt",			Write = "Write",	},
				JAVA = 	{ Type = "int"	,		Read = "readInt",			Write = "writeInt", ArrayListType = "Integer"},
			},
	uint32 	= 
			{ 
				CPP = 	{ Type = "uint32" ,		Read = ">>" , 				Write = "<<" ,		},	
				CS 	= 	{ Type = "uint"	,		Read = "ReadUInt",			Write = "Write",	},	
				JAVA = 	{ Type = "int"	,		Read = "readInt",			Write = "writeInt", ArrayListType = "Integer"},	
			},
	int64 	= 
			{ 
				CPP = 	{ Type = "int64" ,		Read = ">>" , 				Write = "<<" ,		},	
				CS 	= 	{ Type = "long"	,		Read = "ReadInt64",			Write = "Write",	},	
				JAVA = 	{ Type = "long"	,		Read = "readLong",			Write = "writeLong", ArrayListType = "Long"},	
			},
	uint64 	= 
			{ 
				CPP = 	{ Type = "uint64" ,		Read = ">>" , 				Write = "<<" ,		},	
				CS 	= 	{ Type = "ulong",		Read = "ReadUInt64",		Write = "Write",	},
				JAVA = 	{ Type = "long",		Read = "readLong",		Write = "writeLong", ArrayListType = "Long"},
			},
	float 	= 
			{ 
				CPP = 	{ Type = "float" ,		Read = ">>" , 				Write = "<<" ,		},	
				CS 	= 	{ Type = "float",		Read = "ReadFloat",			Write = "Write",	},	
				JAVA = 	{ Type = "float",		Read = "readFloat",			Write = "writeFloat", ArrayListType = "Float"},
			},
	double 	= 
			{ 
				CPP = 	{ Type = "double" ,		Read = ">>" , 				Write = "<<" ,		},	
				CS 	= 	{ Type = "double",		Read = "ReadDouble",		Write = "Write",	},
				JAVA = 	{ Type = "double",		Read = "readDouble",		Write = "writeDouble", ArrayListType = "Double"},		
			},
	
	bool 	= 
			{
				CPP = 	{ Type = "bool" ,		Read = ">>" , 				Write = "<<" ,		},	
				CS 	= 	{ Type = "bool"	,		Read = "ReadBool",			Write = "Write",	},
				JAVA = 	{ Type = "bool"	,		Read = "readBool",			Write = "writeBool", ArrayListType = "Boolean"},		
			},
	string  = 
			{ 
				CPP = 	{ Type = "std::string" ,		Read = ">>" , 				Write = "<<" ,		},	
				CS 	= 	{ Type = "String",		Read = "ReadString",		Write = "Write",	}, 
				JAVA = 	{ Type = "String",		Read = "readString",		Write = "writeString", ArrayListType = "String"},
			},
};

function getBaseDataType_java(_type)
	if(BaseDataType[_type] == nil) then
		return;
	end
	return BaseDataType[_type].JAVA;
end
function getBaseDataType_cs(_type)
	if(BaseDataType[_type] == nil) then
		return;
	end
	return BaseDataType[_type].CS;
end
function getBaseDataType_cpp(_type)
	if(BaseDataType[_type] == nil) then
		return;
	end
	return BaseDataType[_type].CPP;
end

function createEnum_CS( enum)
	local enumdes = "";
	for enumname , data in pairs ( enum ) do
		local enumdesitem = "public enum "..enumname.."{";
		
		for k , v in pairs ( data ) do
			enumdesitem = enumdesitem.."\n\t"..v.." = "..k..",";
		end
		enumdesitem = enumdesitem.."\n}\n\n";
		enumdes = enumdes..enumdesitem;
	end
	return enumdes;
end
function createPacket_CS( message, fight)
	local messagedes = "";
	for _,content in pairs ( message ) do	
		local messagename = content.name;
		local item = "public class "..messagename.." : ISerializePacket"..(fight=="false" and "HF" or "").."\n{";
		local data = content.data;
		local des = content.des;
		local varDes = "";
		
		local newStrValue = "\t\tint _TempSize = 0; \n";
		for k , v in pairs ( data ) do			
			if v.type == 1 then
			--map
				
			elseif v.type == 2 then
			--list
				if v.key == 0 then
					varDes = varDes.."\n\tpublic List<"..v.keyname .."> "..v.name.." = new ".. "List<"..v.keyname ..">();";
					--非基础类型，那么需要回收
					newStrValue = newStrValue.."\n"..string.format(
[[
		_TempSize =  %s.Count;
		for( int i =0;i< _TempSize;++i)
		{
			var _var = %s[i];
			_var.DestroyClass();
			_var = null;
		}
]],v.name, v.name);
					newStrValue = newStrValue.."\t\t"..v.name..".Clear();\n";	
				else
					local baseType = getBaseDataType_cs(v.keyname);
					varDes = varDes.."\n\tpublic List<"..baseType.Type.."> "..v.name.." = new ".. "List<"..baseType.Type..">();";
					newStrValue = newStrValue.."\t\t"..v.name..".Clear();\n";					
				end

			else
			--general
				if v.key == 0 then
					varDes = varDes.."\n\tpublic "..v.keyname .." "..v.name.." = new "..v.keyname.."();";
					newStrValue = newStrValue.."\t\t"..v.name..".New(null);\n" ;
				else
					local baseType = getBaseDataType_cs(v.keyname);
					if v.keyname == "string" or v.keyname == "String" then
						varDes = varDes.."\n\tpublic "..baseType.Type.." "..v.name.." = String.Empty;";
						newStrValue = newStrValue.."\t\t"..v.name.." = String.Empty;\n";
					else
						varDes = varDes.."\n\tpublic "..baseType.Type.." "..v.name..";";	
                        newStrValue = newStrValue.."\t\t"..v.name.." = default("..baseType.Type..");\n";						
					end
				end
			end
		end	
		local NewStr =string.format("\n\tpublic override void New(object param)\n\t{\n %s \n\t} \n", newStrValue);
		varDes = varDes..NewStr .. "\n\n";

		local _Serialize = "\tpublic override void Serialize( WfPacket w )\n\t{"
		local _DeSerialize = "\tpublic override void DeSerialize( WfPacket r )\n\t{"
		

		local writeDesSeialize = false; 
		local writeSeialize = false;
		
		for k , v in pairs ( data ) do
			if v.type == 1 then
			--map
				if writeDesSeialize == false then
					_Serialize = _Serialize .."\n\t\tint _TempSize = 0;"
					writeDesSeialize = true;
				end
			
				
			elseif v.type == 2 then
			--list
				if writeDesSeialize == false then
					_DeSerialize = _DeSerialize .."\n\t\tint _TempSize = 0;"
					writeDesSeialize = true;
				end
				if writeSeialize == false then
					_Serialize = _Serialize .."\n\t\tint _TempSize = 0;"
					writeSeialize = true;
				end
				local typename = v.keyname;
				local serializeitem = "_var.Serialize(w);"
				local desserializeitem = "var _var = new "..v.keyname.."(); \n\t\t\t_var.DeSerialize(r);"				
				if v.key ~= 0 then
					local baseType = getBaseDataType_cs(v.keyname);
					typename = baseType.Type;
					serializeitem = "w."..baseType.Write.."(_var);"
					desserializeitem = baseType.Type.." _var = r."..baseType.Read.."();"
				end
				--序列化
				_Serialize = _Serialize.."\n"..string.format(
[[
		_TempSize = %s.Count;
		w.Write( _TempSize );
		for(int i = 0; i < _TempSize; ++i)
		{
			var _var = %s[i];
			%s
		}
]],v.name,v.name, serializeitem);
				--反序列化
				_DeSerialize = _DeSerialize.."\n"..string.format(
[[
		_TempSize =  r.ReadInt();
		for( int i =0;i< _TempSize;++i)
		{
			%s
			%s.Add(_var);
		}
]],desserializeitem,v.name);
			else
			--general
				if v.key == 0 then
					--varDes = varDes.."\n\t public "..v.keyname .." "..v.name..";";
					_Serialize = _Serialize.."\n\t\t "..v.name..".Serialize(w);"
					_DeSerialize = _DeSerialize.."\n\t\t "..v.name..".DeSerialize(r);";
				else
					local baseType = getBaseDataType_cs(v.keyname);
					_Serialize = _Serialize.."\n\t\tw."..baseType.Write.."( "..v.name..");"
					_DeSerialize = _DeSerialize.."\n\t\t "..v.name.." = r."..baseType.Read.."();";
				end
			end
		end
		_Serialize = _Serialize .."\n\t}\n";
		_DeSerialize = _DeSerialize .."\n\t}\n";
		local isfight = fight=="false" and "HF" or "";
		local _Clear = string.format(
[[
	public override void DestroyClass()
	{
		PooledClassManager%s<%s>.DeleteClass(this);
	}
]], isfight,messagename);
		varDes = varDes .. _Serialize.._DeSerialize.._Clear;		
		item = item..varDes.."}\n\n";
		messagedes = messagedes..item;
	end
	return messagedes;
end
function createPacketCS( enum, message, PacketPath,filename,fight,packet)
	local _file = PacketPath ..filename.. ".cs";
	os.remove( _file );
	print(_file);
	local _fileHeader = "using UnityEngine;\nusing System.Collections;\nusing System.Collections.Generic;\nusing System;\nusing System.IO;\n\n"
	WriteToFile( _fileHeader , _file );
	-- 枚举
	local enumdes = createEnum_CS(enum);
	WriteToFile( enumdes , _file ,"a");
	-- 消息
	local messagedes = createPacket_CS(message, fight);

	WriteToFile( messagedes , _file ,"a");
end

function findJavaEnumIdByName(enum, filename, messageName)
	local msgName = "em_"..messageName;
    for enumname , data in pairs ( enum ) do
        if enumname == "em"..filename then
		    for value , name in pairs ( data ) do
			    if name == msgName then
                   return value;
                end
		    end
        end
	end
    return -1;
end

function createPacket_JAVA( filename, enum, message)
    local structEnums = "";
    local structEnumCount = 0;
	local messagedes = "public final class "..filename.."{\n";
	for _,content in pairs ( message ) do	
		local messagename = content.name;

        local msgId = findJavaEnumIdByName(enum, filename, messagename);
		local isStruct = msgId == -1;
		--消息和结构体
        local item = "public final static class "..messagename.." extends BinaryMessage{\n";
        if isStruct then
            item = "public static class "..messagename.." extends BinaryMessageStruct{\n";
            item = item..string.format([[        
            public byte type()
            {
                return (byte) StructEnum.%s.value;
            }]], messagename).."\n";

            item = item ..string.format(
		    [[
			    public static %s readBy(ByteBuf buffer){
				    %s ele = new %s();
				    ele.read(buffer);
				    return ele;
			    }
		    ]],messagename, messagename, messagename);

            structEnumCount = structEnumCount + 1;
            structEnums =structEnums..messagename.."("..structEnumCount.."),\n";
        else
		    item = item .. "public static final int MsgId = "..msgId..";\n";
		    item = item.. [[        
		    @Override
		    public int getMsgId(){
			    return MsgId;
		    }
		    ]].."\n";

            item = item..string.format(
		    [[
			    public static %s readBy(ByteBuf buffer){
				    %s ele = new %s();
				    ele.read(buffer);
				    buffer.release();
				    return ele;
			    }
		    ]],messagename, messagename, messagename);
        end

		local data = content.data;
		local des = content.des;
		local varDes = "";
		
		local newStrValue = "\t\tShort _TempSize = 0; \n";

		--定义变量
		for k , v in pairs ( data ) do			
			if v.type == 1 then
			--map
				
			elseif v.type == 2 then
			--list
				if v.key == 0 then
					varDes = varDes.."\n\tpublic ArrayList<"..v.keyname .."> "..v.name.." = new ".. "ArrayList<>();";	
				else
					local baseType = getBaseDataType_java(v.keyname);
					varDes = varDes.."\n\tpublic ArrayList<"..baseType.ArrayListType.."> "..v.name.." = new ".. "ArrayList<>();";				
				end

			else
			--general
				if v.key == 0 then
					varDes = varDes.."\n\tpublic "..v.keyname .." "..v.name.." = new "..v.keyname.."();";
				else
					local baseType = getBaseDataType_java(v.keyname);
					varDes = varDes.."\n\tpublic "..baseType.Type.." "..v.name..";";						
				end
			end
		end	
		
		varDes = varDes .. "\n\n";

		local _Serialize = "\t@Override\nprotected void write(){\n"
		local _DeSerialize = "\t@Override\nprotected void read(){\n"
		

		local writeDesSeialize = false; 
		local writeSeialize = false;
		
		for k , v in pairs ( data ) do
			if v.type == 1 then
			--map
				if writeDesSeialize == false then
					_Serialize = _Serialize .."\n\t\tint _TempSize = 0;"
					writeDesSeialize = true;
				end
			
				
			elseif v.type == 2 then
			--list
				if writeDesSeialize == false then
					_DeSerialize = _DeSerialize .."\n\t\tint _TempSize = 0;"
					writeDesSeialize = true;
				end
				if writeSeialize == false then
					_Serialize = _Serialize .."\n\t\tint _TempSize = 0;"
					writeSeialize = true;
				end
				local typename = v.keyname;
				local serializeitem = "_var.write();"
				local desserializeitem = v.keyname.." _var = new "..v.keyname.."(); \n\t\t\t_var.read();"				
				if v.key ~= 0 then
					local baseType = getBaseDataType_java(v.keyname);
					typename = baseType.Type;
					serializeitem = baseType.Write.."(_var);"
					desserializeitem = typename.." _var = "..baseType.Read.."();"
				end
				--序列化
				_Serialize = _Serialize.."\n"..string.format(
				[[
						_TempSize = %s.size();
						writeShort( _TempSize );
						for(%s _var : %s)
						{
							%s
						}
				]],v.name,typename, v.name, serializeitem);
				--反序列化
				_DeSerialize = _DeSerialize.."\n"..string.format(
				[[
						_TempSize =  readShort();
						for( int i =0;i< _TempSize;++i)
						{
							%s
							%s.add(_var);
						}
				]],desserializeitem,v.name);
			else
			--general
				if v.key == 0 then
					--varDes = varDes.."\n\t public "..v.keyname .." "..v.name..";";
					_Serialize = _Serialize.."\n\t\t "..v.name..".write();"
					_DeSerialize = _DeSerialize.."\n\t\t "..v.name..".read();";
				else
					local baseType = getBaseDataType_java(v.keyname);
					_Serialize = _Serialize.."\n\t\t"..baseType.Write.."( "..v.name..");"
					_DeSerialize = _DeSerialize.."\n\t\t "..v.name.." = "..baseType.Read.."();";
				end
			end
		end
		_Serialize = _Serialize .."\n\t}\n";
		_DeSerialize = _DeSerialize .."\n\t}\n";



		varDes = varDes .. _Serialize.._DeSerialize;		
		item = item..varDes.."}\n\n";
		messagedes = messagedes..item;
        
	end

    --java的一些固有类



    messagedes = messagedes..string.format(
[[
    public enum StructEnum
    {
        %s;
        private int value;
        StructEnum(int value)
        {
            this.value = value;
        }

        public static StructEnum getStructEnum(int value)
        {
            for (StructEnum en : StructEnum.values())
            {
                if (en.value == value)
                return en;
            }
            return null;
        }
    }
]], structEnums);
    messagedes = messagedes.."\n}"
	return messagedes;
end
function createPacketJAVA( enum, message, PacketPath,filename,packet)
	local _file = PacketPath ..filename.. ".java";
	os.remove( _file );
	print(_file);
	local _fileHeader = "package message;\nimport java.util.ArrayList;\nimport message.BinaryMessage;\nimport io.netty.buffer.ByteBuf;\nimport message.BinaryMessageStruct;\n\n"
	WriteToFile( _fileHeader , _file );

	-- 消息(将枚举和消息体放到了一起)
	local messagedes = createPacket_JAVA( filename, enum, message);

	WriteToFile( messagedes , _file ,"a");
end

function createEnum_CPP( enum)
	local enumdes = "";
	for enumname , data in pairs ( enum ) do
		local enumdesitem = "enum "..enumname.."{";
		
		for k , v in pairs ( data ) do
			enumdesitem = enumdesitem.."\n\t"..v.." = "..k..",";
		end
		enumdesitem = enumdesitem.."\n};\n\n";
		enumdes = enumdes..enumdesitem;
	end
	return enumdes;
end

function createPacket_CPP_H( message)
	local messagedes = "";
	
	for _,content in pairs ( message ) do	
		local messagename = content.name;
		messagedes = messagedes .."\n class "..messagename..";";
	end
	messagedes = messagedes .."\n\n"
	
	for _,content in pairs ( message ) do	
		local messagename = content.name;	
		local item = "class "..messagename.." : public ISerializePacket\n{";
		local data = content.data;
		local varDes = "";
		
		local setProperty = false;
		varDes = varDes .."\npublic:";
		for k , v in pairs ( data ) do			
			if v.type == 1 then
			--map
				
			elseif v.type == 2 then
			--list
				if v.key == 0 then
					varDes = varDes.."\n\tstd::vector<"..v.keyname .."> "..v.name..";";
				else
					local baseType = getBaseDataType_cpp(v.keyname);
					varDes = varDes.."\n\tstd::vector<"..baseType.Type.."> "..v.name..";";
				end
			else
			--general
				if v.key == 0 then
					varDes = varDes.."\n\t"..v.keyname .." "..v.name..";";
				else
					local baseType = getBaseDataType_cpp(v.keyname);
					varDes = varDes.."\n\t"..baseType.Type.." "..v.name..";";
				end
			end
		end	

		if setProperty then
			varDes = varDes .."\npublic:";
			for k , v in pairs ( data ) do			
				if v.type == 1 then
				--map
					
				elseif v.type == 2 then
				--list

				else
				--general
					if v.key == 0 then
						varDes = varDes.."\n\tinline void set_"..v.name.."(const "..v.keyname.."& value) {"..v.name.." = value;}";
						varDes = varDes.."\n\tinline "..v.keyname.." get_"..v.name.."() const { return "..v.name..";}";	
					else
						local baseType = getBaseDataType_cpp(v.keyname);
						varDes = varDes.."\n\tinline void set_"..v.name.."(const "..baseType.Type.."& value) {"..v.name.." = value;}";
						varDes = varDes.."\n\tinline "..baseType.Type.." get_"..v.name.."() const { return "..v.name..";}";					
					end
				end
			end	
		end
		
		varDes = varDes .."\npublic:";
		local _Serialize = "\n\tbool Serialize( wf::BaseStream& os ) const;"
		local _DeSerialize = "\n\tbool DeSerialize( wf::BaseStream& is );"		
		varDes = varDes .. _Serialize.._DeSerialize;		
		item = item..varDes.."\n};\n\n";
		messagedes = messagedes..item;
	end
	return messagedes;
end
function createPacket_CPP( message)
	local messagedes = "";
		
	for _,content in pairs ( message ) do	
		local messagename = content.name;
		local data = content.data;
		local _Serialize = "\n\tbool "..messagename.."::Serialize( wf::BaseStream& os ) const\n\t{"
		local _DeSerialize = "\n\tbool "..messagename.."::DeSerialize( wf::BaseStream& is )\n\t{"
		
		local writeDesSeialize = false; 
		
		for k , v in pairs ( data ) do
			if v.type == 1 then
			--map
				if writeDesSeialize == false then
					_Serialize = _Serialize .."\n\t\tint _TempSize = 0;"
					writeDesSeialize = true;
				end
			
				
			elseif v.type == 2 then
			--list
				if writeDesSeialize == false then
					_DeSerialize = _DeSerialize .."\n\t\tint _TempSize = 0;"
					writeDesSeialize = true;
				end
				
				local typename = v.keyname;
				local serializeitem = "obj.Serialize(os); if (!os.IsOK()) return false;"
				local desserializeitem = v.keyname .." obj; obj.DeSerialize(is); if (!is.IsOK()) return false;"				
				if v.key ~= 0 then
					local baseType = getBaseDataType_cpp(v.keyname);
					typename = baseType.Type;
					serializeitem = "os "..baseType.Write.." obj; if (!os.IsOK()) return false;"
					desserializeitem = baseType.Type.." obj; is "..baseType.Read.." obj; if (!is.IsOK()) return false;"
				end
				--序列化
				_Serialize = _Serialize.."\n"..string.format(
[[
		os << (uint32)%s.size();
		for( auto& obj : %s )
		{
			%s
		}]],v.name,v.name,serializeitem);
				--反序列化
				_DeSerialize = _DeSerialize.."\n"..string.format(
[[
		_TempSize =  is.read_uint32();
		for( int i =0;i< _TempSize;++i)
		{
			%s
			%s.push_back(obj); 
		}]],desserializeitem,v.name);
			else
			--general
				if v.key == 0 then
					_Serialize = _Serialize.."\n\t\t "..v.name..".Serialize(os); if (!os.IsOK()) return false;"
					_DeSerialize = _DeSerialize.."\n\t\t "..v.name..".DeSerialize(is); if (!is.IsOK()) return false;";
				else
					local baseType = getBaseDataType_cpp(v.keyname);
					_Serialize = _Serialize.."\n\t\tos "..baseType.Write.." "..v.name.."; if (!os.IsOK()) return false;"
					_DeSerialize = _DeSerialize.."\n\t\tis "..baseType.Read.." "..v.name.."; if (!is.IsOK()) return false;"
				end
			end
		end
		_Serialize = _Serialize .."\n\t\treturn os.IsOK();";
		_Serialize = _Serialize .."\n\t}\n";
		_DeSerialize = _DeSerialize .."\n\t\treturn is.IsOK();";
		_DeSerialize = _DeSerialize .."\n\t}\n";
		
		item = _Serialize.._DeSerialize.."\n";	
		messagedes = messagedes..item;
	end
	return messagedes;
end

function createPacketCPP( enum, message, PacketPath,filename,packet)
	local _file_h = PacketPath ..filename.. ".pb.h";
	local _file_cpp = PacketPath ..filename.. ".pb.cc";
	os.remove( _file_h );
	os.remove( _file_cpp );
	print(filename);
	local _fileHeader = "#pragma once\n#include \"WfSerializePacket.h\"\n\n"
	WriteToFile( _fileHeader , _file_h );
	
	local _filecppHeader = "#include \""..filename.. ".pb.h\"\n\n"; 
	WriteToFile( _filecppHeader , _file_cpp );
	
	local namespace = "";
	if #packet >0 then
		for k , v in pairs ( packet ) do
			namespace = namespace.."namespace "..v.."\n{";
		end
		WriteToFile( namespace , _file_h ,"a");
		WriteToFile( namespace , _file_cpp ,"a");
	end
	
	-- 枚举
	local enumdes = createEnum_CPP(enum);
	WriteToFile( enumdes , _file_h ,"a");
	-- 消息
	local messagedes = createPacket_CPP_H(message);
	WriteToFile( messagedes , _file_h ,"a");
	--消息定义
	local messagedes_pp = createPacket_CPP(message);
	WriteToFile( messagedes_pp , _file_cpp ,"a");	
	
	if #packet >0 then
		namespace = "";
		for k , v in pairs ( packet ) do
			namespace = namespace.."}\n";
		end
		WriteToFile( namespace , _file_h ,"a");
		WriteToFile( namespace , _file_cpp ,"a");
	end
end

function Trim( s )
	if( s ) then
		return (string.gsub( s , "^%s+(.-)%s+$" , "%1" ));
	end
end
function WriteToFile( writeString , filePath , model )
	model = model or "w";
	local file = io.open( filePath , model );
	if( file )then
		file:write( writeString );
		file:close();
	else
		print( string.format( "Error! %s ioopen failed" , filePath ) );
	end
end
-- 序列化tablle表--將表轉化成string
function serialize(obj)
    local lua = ""
    local t = type(obj)
    if t == "number" then
        lua = lua .. obj
    elseif t == "boolean" then
        lua = lua .. tostring(obj)
    elseif t == "string" then
        lua = lua .. string.format("%q", obj)
    elseif t == "table" then
        lua = lua .. "{\n"
    for k, v in pairs(obj) do
        lua = lua .. "[" .. serialize(k) .. "]=" .. serialize(v) .. ",\n"
    end
    local metatable = getmetatable(obj)
        if metatable ~= nil and type(metatable.__index) == "table" then
        for k, v in pairs(metatable.__index) do
            lua = lua .. "[" .. serialize(k) .. "]=" .. serialize(v) .. ",\n"
        end
    end
        lua = lua .. "}"
    elseif t == "nil" then
        return nil
    else
        return "-nil-" 
        --error("can not serialize a " .. t .. " type.")
    end
    return lua
end
