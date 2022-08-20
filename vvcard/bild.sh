#!/bin/bash
path=C:\porject\VVCard.ru\VVCard.ru\vvcard

echo '****** Bild start ******'
cd $path
dotnet publish -a x64 --configuration Release -o $path/ReleaseBin -v 0
echo '****** Bild completed ******'
