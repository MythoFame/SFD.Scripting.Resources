_default:
    @just --list

generate-script:
    dotnet build SFD.Scripting.Resources.csproj -t:GenerateScript

generate-src-db:
    dotnet build SFD.Scripting.Resources.Generator/ -t:GenerateSourceDb
