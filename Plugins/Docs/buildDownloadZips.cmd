
if exist *.zip del *.zip

echo "Building Plugin zip files"
call BuildDownloadZip.cmd ARMSim.Plugins.ARMRegister64Plugin
call BuildDownloadZip.cmd ARMSim.Plugins.EightSegmentDisplayPlugin
call BuildDownloadZip.cmd ARMSim.Plugins.EmbestBoardPlugin
call BuildDownloadZip.cmd ARMSim.Plugins.LogBase2Plugin
call BuildDownloadZip.cmd ARMSim.Plugins.TrafficLightPlugin

