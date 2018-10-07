
REM throw output away with > nul
sc query "MF.WeeklyManagerReport.PROD" > nul
IF ERRORLEVEL 1060 (
    echo "Service is not installed"
) else (
   NET STOP "MF.WeeklyManagerReport.PROD"
   MF.WeeklyManagerReport_PROD\MF.WeeklyManagerReport.exe uninstall -servicename:MF.WeeklyManagerReport.PROD -displayname:MF.WeeklyManagerReport.PROD
)
