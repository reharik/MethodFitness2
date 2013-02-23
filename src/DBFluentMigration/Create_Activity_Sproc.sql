SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create PROCEDURE Activity 
	@StartDate DateTime,
	@EndDate DateTime,
	@TrainerId int,
	@ClientId int,
	@LocationId int
AS
BEGIN
	SET NOCOUNT ON;
	
SELECT t.firstname + ' ' + t.lastName as Trainer,
	c.firstname + ' ' + c.lastName as Client,
	@StartDate as StartDate,
	@EndDate as EndDate,
	a.AppointmentType,
	l.Name,
	a.[Date],
    SUM(case when s.appointmentType = 'Hour' then 1 else 0 end) Hour,
    SUM(case when s.appointmentType = 'Half Hour' then 1 else 0 end) HalfHour,
    SUM(case when s.appointmentType = 'Pair' then 1 else 0 end) Pair,

	Cast(
		SUM(case when s.appointmentType = 'Hour' then 1 else 0 end) 
		+ CAST(SUM(case when s.appointmentType = 'Half Hour' then 1 else 0 end) as decimal(10,4))/2
		+ SUM(case when s.appointmentType = 'Pair' then 1 else 0 end) as decimal(10,1)) TotalHours

From Client as c 
left outer join [session] as s on c.Entityid = s.ClientId
left join [user] as t on s.trainerid = t.entityid
left join appointment as a on s.appointmentid = a.entityId
left join location as l on l.entityId = a.LocationId

where @StartDate <= CAST(a.[date] as DATE) 
	and @EndDate >= CAST(a.[date] as DATE)
	AND (@TrainerId = 0 OR s.trainerId = @TrainerId)
	AND (@ClientId = 0 OR c.EntityId = @ClientId)
	AND (@LocationId = 0 OR a.LocationId = @LocationId)


group by c.firstname, c.lastname, t.firstname, t.lastname, a.AppointmentType, l.Name,a.[Date]
having (SUM(case when s.appointmentType = 'Hour' then 1 else 0 end) 
		+ CAST(SUM(case when s.appointmentType = 'Half Hour' then 1 else 0 end) as decimal(10,4))/2
		+ SUM(case when s.appointmentType = 'Pair' then 1 else 0 end)) > 0

order by c.LastName
END
GO
