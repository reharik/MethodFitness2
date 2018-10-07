
REM throw output away with > nul
sc query "MF.SessionManagement.PROD" > nul
IF ERRORLEVEL 1060 (
    echo "Service is not installed"
) else (
    NET STOP "MF.SessionManagement.PROD"
    MF.SessionManagement_PROD\MF.SessionManagement.exe uninstall -servicename:MF.SessionManagement.PROD -displayname:MF.SessionManagement.PROD
)