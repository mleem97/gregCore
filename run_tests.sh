#!/bin/bash
export DOTNET_ROLL_FORWARD=Major
dotnet test tests/gregCore.Tests.csproj
