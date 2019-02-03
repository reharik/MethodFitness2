if (typeof MF == "undefined") {
    var MF = {};
}

MF.blockedTimes= (function(){
    const timeHash = {};

    const addAppt = (appt, hash) => {
        const startD = new XDate(appt.start);
        const endD = new XDate(appt.end);
        while(startD.diffMinutes(endD)>0) {
            const dateStr = startD.toString('M/d/yyyy h(:mm) s TT');
            hash[dateStr] = dateStr in hash ? hash[dateStr] + 1 : 1;
            startD.addMinutes(15);
        }
    }
    const calculate = (appts) => {
        for(i=0; i<appts.lenght; i++) {
            addAppt(appts[i], hash)
        }
    }
    return {
        calculate:calculate
    }
})();