/**
 * Created by .
 * User: Harik
 * Date: 11/22/11
 * Time: 8:16 PM
 * To change this template use File | Settings | File Templates.
 */

mf.AppointmentCalendarController = mf.Controller.extend({
    events:_.extend({
    }, mf.Controller.prototype.events),
    initialize:function(){
        $.extend(this,this.defaults());
        this.options = $.extend({},mf.crudControllerDefaults, this.options);
        $.clearPubSub();
        this.registerSubscriptions();
        var displayOptions={
            el:"#masterArea",
            id:"appointment"
        };
        var options = $.extend({}, this.options,displayOptions);
        this.views.calendarView = new mf.CalendarView(options);
    },
    registerSubscriptions:function(){
        $.subscribe('/contentLevel/calendar_appointment/pageLoaded', $.proxy(this.loadEvents,this), this.cid);
        $.subscribe('/contentLevel/form_editModule/pageLoaded', $.proxy(this.loadTokenizers,this), this.cid);

        $.subscribe('/contentLevel/calendar_appointment/dayClick', $.proxy(this.dayClick,this), this.cid);
        $.subscribe('/contentLevel/calendar_appointment/eventClick', $.proxy(this.eventClick,this), this.cid);
        // from form
        $.subscribe('/contentLevel/form_editModule/success', $.proxy(this.formSuccess,this), this.cid);
        $.subscribe('/contentLevel/ajaxPopupDisplayModule_editModule/cancel', $.proxy(this.formCancel,this), this.cid);
        // from display
        $.subscribe('/contentLevel/ajaxPopupDisplayModule_displayModule/cancel', $.proxy(this.displayCancel,this), this.cid);
        $.subscribe('/contentLevel/popup_displayModule/edit', $.proxy(this.displayEdit,this), this.cid);
    },
    loadEvents:function(){
        $("[name=Location]").change($.proxy(function(){
            var locId = $("[name=Location]").val();
            this.views.calendarView.replaceSource({url : this.options.Url, data:{Loc:locId} });// function(){return { Loc: $("[name=Location]").val() }}})
         this.views.calendarView.reload();
        },this));
    },

    loadTokenizers: function(formOptions){
        var options = $.extend({},formOptions,{el:"#clients"});
        this.views.roles = new mf.TokenView(options);
    },
    dayClick:function(date, allDay, jsEvent, view) {
        if(new XDate(date).diffHours(new XDate())>0 && !this.options.CanEnterRetroactiveAppointments){
            alert("you can't add retroactively");
            return;
        }
        var data = {"ScheduledDate" : $.fullCalendar.formatDate( date,"M/d/yyyy"), "ScheduledStartTime": $.fullCalendar.formatDate( date,"hh:mm TT")};
        this.editEvent(this.options.AddEditUrl,data);
    },

    editEvent:function(url, data){
        $("#masterArea").after("<div id='dialogHolder'/>");
        var moduleOptions = {
            id:"editModule",
            el:"#dialogHolder",
            url: url,
            data:data,
            buttons: mf.popupButtonBuilder.builder("editModule").standardEditButons()
        };
        this.modules.popupForm = new mf.AjaxPopupFormModule(moduleOptions);
    },

    eventClick:function(calEvent, jsEvent, view) {
        if(calEvent.trainerId!= this.options.TrainerId && !this.options.CanSeeOthersAppointments){
            return;
        }
        this.options.canEdit = new XDate(calEvent.start).diffHours(new XDate())<0 || this.options.CanEditPastAppointments;
        var data = {"EntityId": calEvent.EntityId};
        var builder = mf.popupButtonBuilder.builder("displayModule");
        builder.addButton("Delete", $.proxy(this.deleteItem,this));
        builder.addEditButton();
        builder.addButton("Copy Event",$.proxy(this.copyItem,this));
        builder.addCancelButton();
       $("#masterArea").after("<div id='dialogHolder'/>");
        var moduleOptions = {
            id:"displayModule",
            el:"#dialogHolder",
            url: this.options.DisplayUrl,
            data:data,
            buttons:builder.getButtons()
        };
        this.modules.popupDisplay = new mf.AjaxPopupDisplayModule(moduleOptions);
    },

    copyItem:function(){
        var entityId = $("[name$='EntityId']").val();
        var data = {"EntityId":entityId,"Copy":true};
        this.editEvent(this.options.AddEditUrl,data);
    },

    deleteItem: function(){
        if (confirm("Are you sure you would like to delete this Item?")) {
        var entityId = $("#EntityId").val();
        mf.repository.ajaxGet(this.options.DeleteUrl,{"EntityId":entityId}, $.proxy(function(result){
            this.modules.popupDisplay.destroy();
                if(!result.Success){
                    alert(result.Message);
                }else{
                   this.views.calendarView.reload();
                }
            },this));
        }

    },
    //from form
    formSuccess:function(){
        this.formCancel();
        this.views.calendarView.reload();

    },
    formCancel: function(){
        this.modules.popupForm.destroy();
    },

    //from display
    displayCancel:function(){
        this.modules.popupDisplay.destroy();
    },

    displayEdit:function(event){
        if(!this.options.canEdit){
             alert("you can't edit retroactively");
            return;
        }
        var url = $("#AddUpdateUrl",this.modules.popupDisplay.el).val();
        this.modules.popupDisplay.destroy();
        this.editEvent(url);
    }

});