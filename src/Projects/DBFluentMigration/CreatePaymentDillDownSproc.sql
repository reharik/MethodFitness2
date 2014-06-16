SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create PROCEDURE [dbo].[PaymentDrillDown] 
	@EntityId int
AS
BEGIN
	SET NOCOUNT ON;

SELECT        CreatedDate, FullHour,FullHourPrice,FullHourTenPack,FullHourTenPackPrice,
			HalfHour,HalfHourPrice,HalfHourTenPack,HalfHourTenPackPrice,
			Pair,PairPrice,PairTenPack,PairTenPackPrice,
			PaymentTotal, Notes
FROM            Payment 
WHERE EntityId = @EntityId 
END
