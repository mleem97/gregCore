#!/bin/bash
# Erzeugt leere Stub-DLLs für CI-Build ohne echtes MelonLoader
# Diese enthalten nur die nötigen Type-Definitionen

dotnet new classlib -n MelonLoaderStub -f net6.0
