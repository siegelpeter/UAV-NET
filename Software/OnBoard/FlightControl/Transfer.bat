robocopy * bin\Debug bin\Debug\Transfer /XD .svn /XF *.config /XF *.csv /xf Usc.dll log4net.dll UsbWrapper.dll
echo "restart" >> bin\Debug\Transfer\refresh
"..\..\..\3rd Party\Software\WinSCP\Winscp.exe" /script=upload.txt /log=upload.log
pause
del /S /Q bin\Debug\Transfer