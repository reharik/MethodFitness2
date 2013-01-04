/****** Object:  View [dbo].[DailyPayments]    Script Date: 1/3/2013 5:16:28 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[DailyPayments]
AS
SELECT        dbo.Payment.CreatedDate, trainer.FirstName, trainer.LastName, dbo.Client.FirstName AS Expr1, dbo.Client.LastName AS Expr2, dbo.Payment.PaymentTotal
FROM            dbo.Payment INNER JOIN
                         dbo.[User] AS trainer ON dbo.Payment.CreatedById = trainer.EntityId INNER JOIN
                         dbo.Client ON dbo.Payment.ClientId = dbo.Client.EntityId

GO