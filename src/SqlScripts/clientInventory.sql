DECLARE @clientId int, @HshowsAvailable int, @HactualAvailable int, @HcompletedAppt int,@HsessionsUsed int, @HsessionsPurchased int,
@HHshowsAvailable int, @HHactualAvailable int, @HHcompletedAppt int,@HHsessionsUsed int, @HHsessionsPurchased int,
@PshowsAvailable int, @PactualAvailable int, @PcompletedAppt int, @PsessionsUsed int, @PsessionsPurchased int;

set @clientId = 2378

select @HcompletedAppt = count(entityid) from Appointment a
	inner join appointment_client ac on ac.appointmentid = a.entityid
	where a.completed = 1
	and a.isdeleted = 0
	and ac.clientid = @clientId
	and AppointmentType= 'Hour'

select @HHcompletedAppt = count(entityid) from Appointment a
	inner join appointment_client ac on ac.appointmentid = a.entityid
	where a.completed = 1
	and a.isdeleted = 0
	and ac.clientid = @clientId
	and AppointmentType= 'Half Hour'

select @PcompletedAppt = count(entityid) from Appointment a
	inner join appointment_client ac on ac.appointmentid = a.entityid
	where a.completed = 1
	and a.isdeleted = 0
	and ac.clientid = @clientId
	and AppointmentType= 'Pair'
-------------
select @HshowsAvailable = count(entityid) from Session s 
	where ClientId = @clientId 
	and AppointmentType = 'Hour'
	and SessionUsed = 0

select @HHshowsAvailable = count(entityid) from Session s 
	where ClientId = @clientId 
	and AppointmentType = 'Half Hour'
	and SessionUsed = 0

select @PshowsAvailable = count(entityid) from Session s 
	where ClientId = @clientId 
	and AppointmentType ='Pair'
	and SessionUsed = 0
--------------
select @HsessionsPurchased = sum(FullHourTenPack) * 10 + SUM(FullHour) from [Payment] where clientid = @clientId
select @HHsessionsPurchased = sum(HalfHourTenPack) * 10 + SUM(HalfHour) from [Payment] where clientid = @clientId
select @PsessionsPurchased = sum(PairTenPack) * 10 + SUM(Pair) from [Payment] where clientid = @clientId
---------------
select @HsessionsUsed = count(entityid) from Session s 
	where ClientId = @clientId 
	and AppointmentType ='Hour'
	and SessionUsed = 1

select @HHsessionsUsed = count(entityid) from Session s 
	where ClientId = @clientId 
	and AppointmentType ='Half Hour'
	and SessionUsed = 1

select @PsessionsUsed = count(entityid) from Session s 
	where ClientId = @clientId 
	and AppointmentType ='Pair'
	and SessionUsed = 1
------------------
set @HactualAvailable = @HsessionsPurchased - @HsessionsUsed
set @HHactualAvailable = @HHsessionsPurchased - @HHsessionsUsed
set @PactualAvailable = @PsessionsPurchased - @PsessionsUsed
----------
select 'Hour Sessions', @HcompletedAppt completedAppt, @HsessionsPurchased sessionsPurchased, @HsessionsUsed sessionsUsed,  @HactualAvailable actualAvailable, @HshowsAvailable showsAvailable
select 'Half Hour Sessions', @HHcompletedAppt completedAppt,@HHsessionsPurchased sessionsPurchased, @HHsessionsUsed sessionsUsed,  @HHactualAvailable actualAvailable, @HHshowsAvailable showsAvailable
select 'Pair Sessions', @PcompletedAppt completedAppt, @PsessionsPurchased sessionsPurchased, @PsessionsUsed sessionsUsed,  @PactualAvailable actualAvailable, @PshowsAvailable showsAvailable

