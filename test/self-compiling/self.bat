set dir=../../CSharp.lua/bin/Debug/
set systemdll=System.Xml;System.Runtime;System.Threading.Tasks;System.Text.Encoding;System.IO
"%dir%CSharp.lua.exe" -s ../../CSharp.lua/ -d selfout -l %systemdll%;%dir%Microsoft.CodeAnalysis.CSharp.dll;%dir%Microsoft.CodeAnalysis.dll;%dir%System.Collections.Immutable.dll;%dir%System.Reflection.Metadata.dll