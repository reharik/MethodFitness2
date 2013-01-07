SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[DailyPayments]
AS
SELECT        dbo.Payment.CreatedDate, trainer.FirstName + ' ' + trainer.LastName AS Trainer, dbo.Client.FirstName + ' ' + dbo.Client.LastName AS Client, 
                         dbo.Payment.PaymentTotal
FROM            dbo.Payment INNER JOIN
                         dbo.[User] AS trainer ON dbo.Payment.CreatedById = trainer.EntityId INNER JOIN
                         dbo.Client ON dbo.Payment.ClientId = dbo.Client.EntityId

GO