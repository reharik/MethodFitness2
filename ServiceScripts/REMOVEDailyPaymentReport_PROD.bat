
REM throw output away with > nul
sc query "MF.DailyPaymentReport.PROD" > nul
IF ERRORLEVEL 1060 (
    echo "Service is not installed"
) else (
    NET STOP "MF.DailyPaymentReport.PROD"
    MF.DailyPaymentReport_PROD\MF.DailyPaymentReport.exe uninstall -servicename:MF.DailyPaymentReport.PROD -displayname:MF.DailyPaymentReport.PROD
)



