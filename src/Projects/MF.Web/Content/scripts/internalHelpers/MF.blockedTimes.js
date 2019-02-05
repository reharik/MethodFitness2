if (typeof MF == "undefined") {
    var MF = {};
}

MF.blockedTimes= (function(){
    const provoHash = {};
    const egHash = {};
    const provoBlocked = [];
    const egBlocked = [];
    const addAppt = (appt, hash, blocked, cutOff) => {
        const startD = new XDate(appt.start);
        const endD = new XDate(appt.end);
        while(startD.diffMinutes(endD)>0) {
            const dateStr = startD.toString('M/d/yyyy h:mm ss TT');
            const spots = appt.appointmentType === 'Pair' ? 2 : 1;
            hash[dateStr] = dateStr in hash ? hash[dateStr] + spots : spots;
            if(hash[dateStr] === cutOff) {
                blocked.push(dateStr);
            }
            startD.addMinutes(15);
        }
    }
    const calculate = (appts, location) => {
        for(i=0; i<appts.length; i++) {
            addAppt(appts[i],
                appts[i].locationId === 2 ? provoHash : egHash,
                appts[i].locationId === 2 ? provoBlocked : egBlocked,
                appts[i].locationId === 2 ? 6 : 2)
        }

        return {2:provoBlocked, 1:egBlocked};
    }

    const shouldBlockAppointment = (blockedTimes, targetSlot, location) => {
        let block = false;
        if(location == null || location === 0){
            let blockedMsg = '';
            provoBlockedMsg = 'There are no appointments left at the Providence location';
            egBlockedMsg = 'There are no appointments left at the East Side location';
            const provoBlocked = blockedTimes[2].indexOf(targetSlot) > -1
            const egBlocked = blockedTimes[1].indexOf(targetSlot) > -1
            if(provoBlocked && egBlocked) {
                blockedMsg = 'There are no appointments left at either location';
                block=true;
            } else if(provoBlocked) {
                blockedMsg = provoBlockedMsg;
            } else if(egBlocked) {
                blockedMsg = egBlockedMsg;
            }
        }else if(blockedTimes[location].indexOf(targetSlot) > -1) {
            blockedMsg = location === 2 ? provoBlockedMsg : egBlockedMsg;
            block = true;
        }
        return {block, blockedMsg};
    }
    return {
        calculate,
        shouldBlockAppointment
    }
})();