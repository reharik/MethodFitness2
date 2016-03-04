

-- select * from session where inarrears = 1 order by changeddate desc



--   select * from client where lastname = 'Cheshire'
--   select * from client where firstname = 'Jessica'



--update session set sessionused =1,
 appointmentId = 25437, 
 trainerId =13
 where entityid = 27075

-- select * from appointment a inner join Appointment_Client ac on a.entityid = ac.AppointmentId where a.Completed =1 and ac.clientid = 2314 and not exists (select 1 from session s where a.EntityId = s.AppointmentId) order by a.date desc


--select * from session 
where clientid = 1 -- and appointmenttype = 'Pair'
order by changeddate desc



update session set sessionused =1,
 appointmentId = 24422, 
-- trainerpaid = 1,
 trainerId =5
 where entityid = 25553


select * 
	from appointment a 
		inner join Appointment_Client ac on a.entityid = ac.AppointmentId 
		where a.Completed =1 
		and a.appointmentType = 'Pair'
		and (select count(entityId) from session s where a.EntityId = s.AppointmentId) < 2 order by a.date desc


		select * from session where appointmentid = 24422

		select * from session where clientid =2397 order by createddate desc



