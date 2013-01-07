USE [MethodFitness_QA]
GO
/****** Object:  StoredProcedure [dbo].[TrainerMetric]    Script Date: 1/6/2013 5:34:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create PROCEDURE [dbo].[TrainerMetric] 
	@StartDate DateTime ,
	@EndDate DateTime ,
	@TrainerId int
AS
BEGIN
	SET NOCOUNT ON;

SELECT  Client.FirstName + ' ' + Client.LastName AS Client,
		[session].AppointmentType,
		AppointmentId


FROM [session] Left join Client ON Client.EntityId = [Session].ClientId
				left join Appointment as apt ON [Session].AppointmentId = apt.Entityid
WHERE [Session].TrainerId = 1
	AND (CAST(apt.[Date] AS date) <= CAST('2013-01-10' AS date)  AND CAST(apt.[Date] AS date) >= N'2012-01-10')
END
