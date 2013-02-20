/****** Object:  StoredProcedure [dbo].[DailyPayments]    Script Date: 2/13/2013 9:03:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[DailyPayments] 
	@StartDate DateTime, 
	@EndDate DateTime 
AS
BEGIN
	SET NOCOUNT ON;

SELECT P.CreatedDate,
 t.FirstName + ' ' + t.LastName AS Trainer, 
 c.FirstName + ' ' + c.LastName AS Client, 
 p.PaymentTotal, 
 p.EntityId,
 p.FullHour,
 p.FullHourPrice,
 p.FullHourTenPack,
 p.FullHourTenPackPrice,
  p.HalfHour,
 p.HalfHourPrice,
 p.HalfHourTenPack,
 p.HalfHourTenPackPrice,
  p.Pair,
 p.PairPrice,
 p.PairTenPack,
 p.PairTenPackPrice
 
FROM Payment as p INNER JOIN
 [User] AS t ON p.CreatedById = t.EntityId INNER JOIN
 Client as c ON p.ClientId = c.EntityId
WHERE p.CreatedDate >= @StartDate AND p.CreatedDate <= @EndDate
END
