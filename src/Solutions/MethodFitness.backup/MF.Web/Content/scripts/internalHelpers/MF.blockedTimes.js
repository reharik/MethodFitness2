if (typeof MF == "undefined") {
	var MF = {};
}

MF.blockedTimes = (function () {
	const locationMap = { 2: "Providence", 1: "East Greenwhich" };
	const maxAppointments = { 2: 4, 1: 2 };
	const blockedByLocationAndTime = {};
	const hash = {};
	const addAppt = (appt, hash, blocked, cutOff) => {
		const startD = new XDate(appt.start);
		const endD = new XDate(appt.end);
		while (startD.diffMinutes(endD) > 0) {
			const dateStr = startD.toString("M/d/yyyy h:mm ss TT");
			hash[dateStr] = dateStr in hash ? hash[dateStr] + 1 : 1;
			if (hash[dateStr] >= cutOff) {
				blocked[dateStr] = true;
			}
			startD.addMinutes(15);
		}
	};
	const calculate = (appts) => {
		for (i = 0; i < appts.length; i++) {
			if (!hash[appts[i].locationId]) {
				hash[appts[i].locationId] = {};
			}
			if (!blockedByLocationAndTime[appts[i].locationId]) {
				blockedByLocationAndTime[appts[i].locationId] = {};
			}
			addAppt(
				appts[i],
				hash[appts[i].locationId],
				blockedByLocationAndTime[appts[i].locationId],
				maxAppointments[appts[i].locationId]
			);
		}
		return blockedByLocationAndTime;
	};

	const shouldBlockAppointment = (
		blockedByLocationAndTime,
		targetSlot,
		location
	) => {
		let blockedMsg = "";
		if (blockedByLocationAndTime == null) {
			return;
		}

		// all location view
		if (!location || location === "0") {
			blockedMsg = Object.keys(blockedByLocationAndTime)
				.map((location) => {
					if (blockedByLocationAndTime[location][targetSlot]) {
						return (
							"There are no appointments left at the " +
							locationMap[location] +
							" location"
						);
					}
				})
				.filter(Boolean);
			blockedMsg =
				// if there is a message for every location
				blockedMsg.length === locationMap.locationMap
					? "There are no appointments left at any facility.  You may only book an offsite appointment"
					: blockedMsg;
			// specific location view
		} else if (blockedByLocationAndTime[location][targetSlot]) {
			blockedMsg =
				"There are no appointments left at the " +
				locationMap[location] +
				" location.   You may only book an offsite appointment";
		}
		return blockedMsg;
	};
	return {
		calculate,
		shouldBlockAppointment,
	};
})();
