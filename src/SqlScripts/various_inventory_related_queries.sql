


select * from session s where inarrears = 1 
 and exists  (select 1 from session where clientid = s.clientid and appointmenttype = s.appointmenttype and sessionused = 0)
 order by changeddate desc




select * from appointment a where a.Completed =1 and not exists (select 1 from session s where a.EntityId = s.AppointmentId) order by a.date desc

select * from appointment a inner join Appointment_Client ac on a.entityid = ac.AppointmentId where a.Completed =1 and ac.clientid = 2400 and not exists (select 1 from session s where a.EntityId = s.AppointmentId) order by a.date desc

select * from appointment a inner join Appointment_Client ac on a.entityid = ac.AppointmentId where a.Completed =1 and not exists (select 1 from session s where a.EntityId = s.AppointmentId) order by a.date desc

select * from appointment a inner join TrainerPaymentSessionItem tpsi on a.entityid = tpsi.AppointmentId where a.Completed =1 and not exists (select 1 from session s where a.EntityId = s.AppointmentId) order by clientid

select * from TrainerPaymentSessionItem where appointmentId =25325

select * from session s  where clientid = 2400 and appointmentid = 25325

select * from Appointment_Client where appointmentid = 25421

select * from session where clientid = 2400

select * from client where entityid =2400