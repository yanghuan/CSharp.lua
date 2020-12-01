#!/bin/sh

# build solution
dotnet build /p:Configuration=Release CSharp.lua.sln
berr=$?
if [ "$berr" != 0 ]; then
	echo "errors occuring during build, please fix and retry...">&2
	exit $berr
fi
echo "build ok."
