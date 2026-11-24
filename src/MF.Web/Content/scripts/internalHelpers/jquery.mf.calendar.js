
(function($) {
    $.fn.asCalendar = function(calendarDefinition, userOptions) {
        var calendarDefaultOptions = {
            header: {
                left: 'prev,next today',
                center: 'title',
                right: 'month,agendaWeek,agendaDay'
            },
            defaultView: 'agendaWeek',
            editable:true,
            theme:true,
            firstDay:1,
            allDaySlot:false,
            allDayDefault:false,
            slotMinutes:15,
            minTime:4,
            maxTime:22,

            events: function(start, end, callback) {
                MF.repository.ajaxGet(calendarDefinition.Url,
                    { Loc: $("[name=Location]").val(),
                        start, end})
                    .then((response)=> callback(response) );
            },
            dayClick: function(date, allDay, jsEvent, view){ MF.vent.trigger("calendar:"+calendarDefinition.id+":dayClick", date, allDay, jsEvent, view);},
            eventClick: function(calEvent, jsEvent, view){ MF.vent.trigger("calendar:"+calendarDefinition.id+":eventClick", calEvent, jsEvent, view);},
            eventDrop: function(event, dayDelta,minuteDelta,allDay,revertFunc){
                MF.vent.trigger("calendar:"+calendarDefinition.id+":eventDrop", event, dayDelta,minuteDelta,allDay,revertFunc);},
            eventResize: function(event, dayDelta, minuteDelta, revertFunc, jsEvent, ui, view ){MF.vent.trigger("calendar:"+calendarDefinition.id+":eventResize", event, dayDelta, minuteDelta, revertFunc, jsEvent, ui, view );}
        };

        // A phone shows one day at a time, with the week a tap away; seven columns do not fit.
        if (MF.isPhone()) {
            calendarDefaultOptions.defaultView = 'agendaDay';
            calendarDefaultOptions.header.right = 'agendaDay,agendaWeek';
        }

        var calendarOptions = $.extend(calendarDefaultOptions, userOptions || {});
        var calendar = this;
        calendar.fullCalendar(calendarOptions);
        return calendar;
    }
})(jQuery);
