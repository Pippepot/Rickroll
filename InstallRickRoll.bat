cd C:\Windows\Microsoft.NET\Framework\v4.0.30319 
InstallUtil.exe %~dp0RickRoll.exe
sc config RicksService start=auto
net start AstleyLovesYou
