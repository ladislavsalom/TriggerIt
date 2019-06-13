@echo off
set /p ver="Version: "

dotnet pack ../src/triggerit/TriggerIt.csproj -o ../artifacts -p:PackageVersion=%ver%
dotnet pack ../src/triggerit.xamarin.forms/TriggerIt.Xamarin.Forms.csproj -o ../artifacts -p:PackageVersion=%ver%