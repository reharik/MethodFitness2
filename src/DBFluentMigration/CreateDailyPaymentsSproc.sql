SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create PROCEDURE DailyPayments 
	@Date DateTime 
AS
BEGIN
	SET NOCOUNT ON;

SELECT        dbo.Payment.CreatedDate, trainer.FirstName + ' ' + trainer.LastName AS Trainer, dbo.Client.FirstName + ' ' + dbo.Client.LastName AS Client, 
                         dbo.Payment.PaymentTotal
FROM            dbo.Payment INNER JOIN
                         dbo.[User] AS trainer ON dbo.Payment.CreatedById = trainer.EntityId INNER JOIN
                         dbo.Client ON dbo.Payment.ClientId = dbo.Client.EntityId
WHERE @Date = CAST('1800-01-01' as Date) AND dbo.Payment.CreatedDate = dbo.Payment.CreatedDate
	  OR 
	  CAST(dbo.Payment.CreatedDate AS date) = @Date
END
GO
