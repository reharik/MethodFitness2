DECLARE @clientId int;
DECLARE @sessionType varchar(50);
DECLARE @apptType varchar(50);
DECLARE @showsAvailable int;
DECLARE @actualAvailable int;
DECLARE @completedAppt int;
DECLARE @sessionsUsed int;
DECLARE @sessionsPurchased int

set @clientId = 2430
set @sessionType = 'Hour'
set @apptType = 'FullHour'
set @showsAvailable = 0
set @actualAvailable = 0
set @completedAppt = 0
set @sessionsUsed = 0
set @sessionsPurchased = 0

select @completedAppt = count(entityid) from Appointment a
	inner join appointment_client ac on ac.appointmentid = a.entityid
	where a.completed = 1
	and ac.clientid = @clientId
	and AppointmentType= @sessionType

select @showsAvailable = count(entityid) from Session s 
	where ClientId = @clientId 
	and AppointmentType =@sessionType
	and SessionUsed = 0

select @sessionsPurchased = sum(FullHourTenPack) * 10 + SUM(FullHour) from [Payment] where clientid = @clientId

select @sessionsUsed = count(entityid) from Session s 
	where ClientId = @clientId 
	and AppointmentType =@sessionType
	and SessionUsed = 1

set @actualAvailable = @sessionsPurchased - @sessionsUsed

select @completedAppt completedAppt, @sessionsUsed sessionsUsed,  @actualAvailable actualAvailable, @showsAvailable showsAvailable

