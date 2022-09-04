if (typeof MF == "undefined") {
	var MF = {};
}

MF.blockedTimes = (function () {
	const locationMap = { 2: "Providence", 1: "East Greenwhich" };
	const calculate = (blockedSlots) => {
		return (blockedSlots.Location||[]).reduce((locs, loc) => {
			locs[loc.LocationId] = loc.TimeSlots.reduce((slots, slot) => {
				slots[slot.TimeSlot] = slot.TimeSlot;
				return slots;
			}, {});
			return locs;
		}, {});
	};

	const shouldBlockAppointment = (
		blockedSlots,
		targetSlot,
		location
	) => {
		let blockedMsg = "";
		if (blockedSlots == null) {
			return;
		}
		const blockedLocs = [];
		// all location view
		if (!location || location === "0") {
			blockedMsg = Object.keys(blockedSlots)
				.map((location) => {
					if (blockedSlots[location][targetSlot]) {
						blockedLocs.push(location);
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
		} else if (blockedSlots[location][targetSlot]) {
			blockedLocs.push(location);
			blockedMsg =
				"There are no appointments left at the " +
				locationMap[location] +
				" location.   Please switch the view to book an appointment at a different site";
		}
		return {
			blockedMsg,
			blockedLocs
		}
	};
	return {
		calculate,
		shouldBlockAppointment,
	};
})();
