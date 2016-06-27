/****** Object:  StoredProcedure [dbo].[SessionReconciliation]    Script Date: 3/18/2016 6:03:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

alter PROCEDURE [dbo].SessionReconciliation 
AS
BEGIN
	SET NOCOUNT ON;
	
	begin transaction
	
	if OBJECT_ID('tempdb..#temp') is not null 
	drop table #temp
select distinct 
a.entityid as appointmentid,
ac.ClientId,
a.TrainerId,
s.InArrears,
s.SessionUsed,
a.AppointmentType
into #temp
from
	appointment_Client ac
	inner join Appointment a on ac.AppointmentID=a.EntityId
	left outer join Session s on ac.ClientID=s.ClientID and a.AppointmentType=s.AppointmentType and s.SessionUsed = 0
where a.EndTime<sysdatetime() and a.Completed=0

insert into Session (
IsDeleted,
CompanyId,
AppointmentType,
SessionUsed,
TrainerPaid,
TrainerCheckNumber,
InArrears,
ClientID,
appointmentId,
trainerId,
TrainerVerified,
createdDate,
createdById,
changedDate,
ChangedById
)
select distinct 
0,
1,
AppointmentType,
1,
0,
0,
1,
ClientID,
appointmentid,
trainerID,
0,
sysdatetime(),
17,
sysdatetime(),
17
from #temp where SessionUsed is null

update Session set 
	SessionUsed=1,
	appointmentid = t.appointmentid,
	trainerId = t.trainerId,
	changedDate = sysdatetime(),
	changedById = 17
from  #temp t 
 inner join Session s on
 s.AppointmentType=t.AppointmentType 
	and s.SessionUsed=0 
	and s.entityid = (
		select top 1 s1.entityid from session s1
		where s1.ClientID=s.ClientID 
		and s1.AppointmentType=s.AppointmentType 
		and s1.SessionUsed=0 )
where s.ClientID=t.ClientID 
	
update Appointment set Completed=1 where Completed=0 AND EndTime<SYSDATETIME() 

--select * from session where appointmentId in (select appointmentid from #temp)

--rollback transaction
commit transaction

END