/****** Object:  StoredProcedure [dbo].[TrainerMetric]    Script Date: 1/26/2013 1:40:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[TrainerMetric] 
	@StartDate DateTime ,
	@EndDate DateTime ,
	@TrainerId int
AS
BEGIN
	SET NOCOUNT ON;

select 
	t.firstname + ' ' + t.lastName as Trainer,
	c.firstname + ' ' + c.lastName as Client,

    SUM(case when s.appointmentType = 'Hour' then 1 else 0 end) Hour,
    SUM(case when s.appointmentType = 'HalfHour' then 1 else 0 end) HalfHour,
    SUM(case when s.appointmentType = 'Pair' then 1 else 0 end) Pair,

	cast((SUM(case when s.appointmentType = 'Hour' then 1 else 0 end) 
		+ SUM(case when s.appointmentType = 'HalfHour' then 1 else 0 end)/2
		+ SUM(case when s.appointmentType = 'Pair' then 1 else 0 end)) as numeric(10,2)) TotalHours,

	cast((DATEDIFF(DAY, @StartDate, @EndDate)/7) as numeric(10,2))NumberOfWeeks,
	
	cast(
	(cast(SUM(case when s.appointmentType = 'Hour' then 1 else 0 end) 
		+ SUM(case when s.appointmentType = 'HalfHour' then 1 else 0 end)/2
		+ SUM(case when s.appointmentType = 'Pair' then 1 else 0 end) as numeric(10,2)) /
    cast((DATEDIFF(DAY, @StartDate, @EndDate)/7) as numeric(10,2))) as numeric(10,2))
	 as HoursPerWeek

From Client as c 
left outer join [session] as s on c.Entityid = s.ClientId
left join [user] as t on s.trainerid = t.entityid
left join appointment as a on s.appointmentid = a.entityId
where t.entityid = @TrainerId  and a.[date] >=  @StartDate and a.[date] <= @EndDate
group by c.firstname, c.lastname, t.firstname, t.lastname
order by c.LastName

END