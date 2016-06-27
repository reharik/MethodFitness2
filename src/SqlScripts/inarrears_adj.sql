

-- select * from session where InArrears = 1 order by changeddate desc


update session set SessionUsed = 1, 
AppointmentId = 26274, 
trainerid = 8
where entityid = 27346

delete session 
where entityid = 27355

select EntityId, AppointmentType, SessionUsed, InArrears, AppointmentId, TrainerId, ClientId from session 
where clientid = 2451
order by changeddate desc





