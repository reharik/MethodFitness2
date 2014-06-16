
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

alter PROCEDURE DailyPayments 
	@StartDate DateTime, 
	@EndDate DateTime 
AS
BEGIN
	SET NOCOUNT ON;

SELECT        dbo.Payment.CreatedDate, trainer.FirstName + ' ' + trainer.LastName AS Trainer, dbo.Client.FirstName + ' ' + dbo.Client.LastName AS Client, 
                         dbo.Payment.PaymentTotal
FROM            dbo.Payment INNER JOIN
                         dbo.[User] AS trainer ON dbo.Payment.CreatedById = trainer.EntityId INNER JOIN
                         dbo.Client ON dbo.Payment.ClientId = dbo.Client.EntityId
WHERE dbo.Payment.CreatedDate >= @StartDate AND dbo.Payment.CreatedDate <= @EndDate
END
GO
