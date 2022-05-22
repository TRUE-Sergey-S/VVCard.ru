#!/bin/bash

echo "Try start vvcard server"
cd /home/user/VVCard.ru/vvcard
dotnet run --project vvcard--urls=https://0.0.0.0:5006/
