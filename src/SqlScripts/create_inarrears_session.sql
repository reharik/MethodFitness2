
--select * from session where inarrears  = 1

insert into [session]
	(createddate, 
	changeddate, 
	changedbyid,
	isdeleted,
	companyid, 
	cost, 
	sessionused,
	trainerpaid, 
	trainerchecknumber, 
	inarrears, 
	createdbyid, 
	trainerverified,
	
	
	appointmenttype,
	trainerid,
	appointmentid, 
	clientid
	)

	values (
	'2016-03-03',
	'2016-03-03',
	1,
	0,
	1,
	0,
	1,
	0,
	0,
	1,
	1,
	0,


	'Half Hour',
--	trainerid,
	8,
--	appointmentid,
	25847,
--	clientid,
	2400


	)