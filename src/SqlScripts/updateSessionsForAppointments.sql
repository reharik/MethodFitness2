

-- select * from session where inarrears = 1 order by changeddate desc

--   select * from client where lastname = 'Lewis'
--   select * from client where firstname = 'Jessica'

/*
select * from Session where EntityId =27186
select * from Session where AppointmentId =25841
select * from Appointment a inner join Appointment_Client ac on a.EntityId = ac.AppointmentId where EntityId =25841
*/


update session set sessionused =1,
 appointmentId = 25330, 
-- trainerpaid = 1,
 trainerId =8
 where entityid = 27501

 select * from appointment a inner join Appointment_Client ac on a.entityid = ac.AppointmentId where a.Completed =1 
 and ac.clientid = 2464 and not exists (select 1 from session s where a.EntityId = s.AppointmentId) order by a.date desc


select * from session 
where clientid = 2464 -- and appointmenttype = 'Pair'
order by changeddate desc

-- select * from TrainerPaymentSessionItem where appointmentid = 25330

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


