@echo off
set /p ver="Version: "

dotnet pack ../src/triggerit/triggerit.csproj -o ../artifacts -p:PackageVersion=%ver%
dotnet pack ../src/triggerit.xamarin.forms/triggerit.xamarin.forms.csproj -o ../artifacts -p:PackageVersion=%ver%