#!/bin/sh
srcp=$1
dstp="./out"

if [ -z "$srcp" ]; then
	echo "no source directory for conversion supplied, using fibonacci src as default...">&2
	srcp=./test/fibonacci/src
fi

if ! [ -d "$dstp" ]; then mkdir "$dstp"; fi
dotnet ./CSharp.lua.Launcher/bin/Release/net5.0/CSharp.lua.Launcher.dll -s "$srcp" -d "$dstp/."
converterr=$?
if [ $converterr != 0 ]; then
        echo "errors occurred during convert">&2
        exit 2
fi
echo "convert ok to $dstp.">&2
echo "run ./runtest.sh to attempt to interpret output.">&2
exit 0

