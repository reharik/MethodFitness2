GO
/****** Object:  StoredProcedure [dbo].[Activity]    Script Date: 12/13/2014 12:38:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[Activity] 
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
    SUM(case when a.appointmentType = 'Hour' then 1 else 0 end) Hour,
    SUM(case when a.appointmentType = 'Half Hour' then 1 else 0 end) HalfHour,
    SUM(case when a.appointmentType = 'Pair' then 1 else 0 end) Pair,
	Cast(
		SUM(case when a.appointmentType = 'Hour' then 1 else 0 end) 
		+ CAST(SUM(case when a.appointmentType = 'Half Hour' then 1 else 0 end) as decimal(10,4))/2
		+ SUM(case when a.appointmentType = 'Pair' then 1 else 0 end) as decimal(10,1)) TotalHours
FROM Appointment a 
	left join Appointment_client ac on a.EntityId = ac.appointmentId
	left join client c on ac.clientid = c. entityid
	left join [user] t on a.trainerid = t.entityid
	left join location l on a.locationid = l.entityid
where @StartDate <= CAST(a.[date] as DATE) 
	and @EndDate >= CAST(a.[date] as DATE)
	AND (@TrainerId = 0 OR a.trainerId = @TrainerId)
	AND (@ClientId = 0 OR c.EntityId = @ClientId)
	AND (@LocationId = 0 OR a.LocationId = @LocationId)
group by c.firstname, c.lastname, t.firstname, t.lastname, a.AppointmentType, l.Name,a.[Date]
having (SUM(case when a.appointmentType = 'Hour' then 1 else 0 end) 
		+ CAST(SUM(case when a.appointmentType = 'Half Hour' then 1 else 0 end) as decimal(10,4))/2
		+ SUM(case when a.appointmentType = 'Pair' then 1 else 0 end)) > 0
order by a.[date], t.lastname, c.LastName 
END
