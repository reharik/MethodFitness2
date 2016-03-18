

-- select * from session where inarrears = 1 order by changeddate desc

--   select * from client where lastname = 'Lewis'
--   select * from client where firstname = 'Jessica'



update session set sessionused =1,
 appointmentId = 25751, 
 trainerId =3
 where entityid = 26964

 select * from appointment a inner join Appointment_Client ac on a.entityid = ac.AppointmentId where a.Completed =1 
 and ac.clientid = 2400 and not exists (select 1 from session s where a.EntityId = s.AppointmentId) order by a.date desc


select * from session 
where clientid = 2400 -- and appointmenttype = 'Pair'
order by changeddate desc


/*
select * 
	from appointment a 
		inner join Appointment_Client ac on a.entityid = ac.AppointmentId 
		where a.Completed =1 
		and a.appointmentType = 'Pair'
		and (select count(entityId) from session s where a.EntityId = s.AppointmentId) < 2 order by a.date desc


		select * from session where appointmentid = 26199

		select * from appointment where entityid = 26110

		select * from session where clientid =2252 order by createddate desc
		*/


