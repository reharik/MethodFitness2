
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
            events: {
                url:calendarDefinition.Url,
                data: { Loc: $("[name=Location]").val() }
            },
            dayClick: function(date, allDay, jsEvent, view){ MF.vent.trigger("calendar:"+calendarDefinition.id+":dayClick", date, allDay, jsEvent, view);},
            eventClick: function(calEvent, jsEvent, view){ MF.vent.trigger("calendar:"+calendarDefinition.id+":eventClick", calEvent, jsEvent, view);},
            eventDrop: function(event, dayDelta,minuteDelta,allDay,revertFunc){ MF.vent.trigger("calendar:"+calendarDefinition.id+":eventDrop", event, dayDelta,minuteDelta,allDay,revertFunc);},
            eventResize: function(event, dayDelta, minuteDelta, revertFunc, jsEvent, ui, view ){MF.vent.trigger("calendar:"+calendarDefinition.id+":eventResize", event, dayDelta, minuteDelta, revertFunc, jsEvent, ui, view );}
        };

        var calendarOptions = $.extend(calendarDefaultOptions, userOptions || {});
        var calendar = this;
        calendar.fullCalendar(calendarOptions);
        return calendar;
    }
})(jQuery);
