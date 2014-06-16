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

select t.firstname + ' ' + t.lastName as Trainer,
	c.firstname + ' ' + c.lastName as Client,
    count(shour.clientid) as Hour,
    count(shalfhour.clientid) as HalfHour,
    count(sPair.clientid) as Pair,
	(count(shour.clientid) + count(shalfhour.clientid)/2 + count(sPair.clientid)) /
	(DATEDIFF(DAY, @StartDate, @EndDate)/7) as HoursPerWeek

From [session] as s 
left join appointment as a on s.appointmentid = a.entityId
left join Client as c on s.clientid = c.entityid 
left join [user] as t on s.trainerid = t.entityid
left outer join [session] as sHour  on c.Entityid = shour.ClientId and shour.appointmentType = 'Hour'
left outer join [session] as sHalfHour on c.Entityid = sHalfHour.ClientId and sHalfHour.appointmentType = 'HalfHour'
left outer join [session] as sPair on c.Entityid = sPair.ClientId and sPair.appointmentType = 'Pair'
where t.entityid = @TrainerId and a.[date] >= @StartDate and a.[date] <= @EndDate
group by c.firstname, c.lastname, t.firstname, t.lastname
order by c.LastName
END
