#!/bin/bash
set -euo pipefail
dotnet run "./tools/Build.cs" -- $@
