/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 2/26/12
 * Time: 11:24 AM
 * To change this template use File | Settings | File Templates.
 */

MF.Views.CalendarView = MF.Views.View.extend({
    events:{
        'change [name=Location]' : 'resetCalendar',
        'click .legendLabel' : 'legendLabelClick',
        'click .legendHeader' : 'legendHeaderClick'
    },

    render:function(){
        if(this.onPreRender)this.onPreRender();
       MF.repository.ajaxGet(this.options.url, this.options.data, $.proxy(function(result){this.renderCallback(result)},this));
    },
    renderCallback:function(result){
        if(result.LoggedOut){
            window.location.replace(result.RedirectUrl);
            return;
        }
        $(this.el).html(result);
        if(extraFormOptions){
            $.extend(true, this.options, extraFormOptions);
        }

        var calContainer = this.options.calendarDef.calendarContainer;
        this.options.calendarDef.id=this.id;
        $(calContainer,this.el).asCalendar(this.options.calendarDef);
        //callback for render
        this.viewLoaded();
        //general notification of pageloaded
        MF.vent.trigger("calendar:"+this.id+":pageLoaded",this.options);
        MF.vent.bind("calendar:"+this.id+":eventDrop",this.eventDrop,this);
        MF.vent.bind("calendar:"+this.id+":eventResize",this.eventResize,this);
        MF.vent.bind("calendar:"+this.id+":dayClick",this.dayClick,this);
        MF.vent.bind("calendar:"+this.id+":eventClick",this.eventClick,this);
        MF.vent.bind("ajaxPopupFormModule:editModule:success",this.formSuccess,this);
        MF.vent.bind("ajaxPopupFormModule:editModule:cancel",this.formCancel,this);
        MF.vent.bind("popup:displayModule:edit",this.displayEdit,this);
        MF.vent.bind("ajaxPopupDisplayModule:displayModule:cancel",this.displayCancel,this);

        this.setupLegend();
    },
    onClose:function(){
        MF.vent.unbind("calendar:"+this.id+":eventDrop");
        MF.vent.unbind("calendar:"+this.id+":eventResize");
        MF.vent.unbind("calendar:"+this.id+":eventResize");
        MF.vent.unbind("calendar:"+this.id+":eventClick");
        MF.vent.unbind("ajaxPopupFormModule:editModule:success");
        MF.vent.unbind("ajaxPopupFormModule:editModule:cancel");
        MF.vent.unbind("ajaxPopupDisplayModule:displayModule:edit");
        MF.vent.unbind("ajaxPopupDisplayModule:displayModule:cancel");
    },
    setupLegend:function(){
         if(this.options.trainers.length<=0){
            $("#legend").hide();
        }
        $( "#legendTemplate" ).tmpl( this.options.trainers ).appendTo( "#legendItems" );
        $(".legendHeader").addClass("showing");
        $(".legendLabel").each(function(i,item){ $(item).addClass("showing"); });
    },
    eventDrop:function(event, dayDelta,minuteDelta,allDay,revertFunc) {
        var data = {"EntityId":event.EntityId,
            "ScheduledDate":$.fullCalendar.formatDate( event.start,"M/d/yyyy hh:mm TT"),
            "StartTime":$.fullCalendar.formatDate( event.start,"M/d/yyyy hh:mm TT"),
            "EndTime":$.fullCalendar.formatDate( event.end,"M/d/yyyy hh:mm TT")};
        MF.repository.ajaxGet(this.options.EventChangedUrl,data, $.proxy(function(result){this.changeEventCallback(result,revertFunc)},this));
    },
    eventResize:function( event, dayDelta, minuteDelta, revertFunc, jsEvent, ui, view ){
        var data = {"EntityId":event.EntityId,
            "ScheduledDate":$.fullCalendar.formatDate( event.start,"M/d/yyyy hh:mm TT"),
            "StartTime":$.fullCalendar.formatDate( event.start,"M/d/yyyy hh:mm TT"),
            "EndTime":$.fullCalendar.formatDate( event.end,"M/d/yyyy hh:mm TT")
        };
        MF.repository.ajaxGet(this.options.EventChangedUrl,data,$.proxy(function(result){this.changeEventCallback(result,revertFunc)},this));
    },
    dayClick:function(date, allDay, jsEvent, view) {
        if(new XDate(date).diffHours(new XDate())>0 && !this.options.calendarDef.CanEnterRetroactiveAppointments){
            alert("you can't add retroactively");
            return;
        }
        var data = {"ScheduledDate" : $.fullCalendar.formatDate( date,"M/d/yyyy"), "ScheduledStartTime": $.fullCalendar.formatDate( date,"hh:mm TT")};
        this.editEvent(this.options.calendarDef.AddEditUrl,data);
    },
    eventClick:function(calEvent, jsEvent, view) {
        if(calEvent.trainerId!= this.options.calendarDef.TrainerId && !this.options.calendarDef.CanSeeOthersAppointments){
            return;
        }
        this.options.calendarDef.canEdit = new XDate(calEvent.start).diffHours(new XDate())<0 || this.options.calendarDef.CanEditPastAppointments;
        var data = {"EntityId": calEvent.EntityId};
        var builder = MF.Views.popupButtonBuilder.builder("displayModule");
        builder.addButton("Delete", $.proxy(this.deleteItem,this));
        builder.addEditButton();
        builder.addButton("Copy Event",$.proxy(this.copyItem,this));
        builder.addCancelButton();

        var formOptions = {
            id: "displayModule",
            url: this.options.calendarDef.DisplayUrl,
            data:data,
            buttons:builder.getButtons()
        };
        this.ajaxPopupDisplay = new MF.Views.AjaxPopupDisplayModule(formOptions);
        this.ajaxPopupDisplay.render();
        this.storeChild(this.ajaxPopupDisplay);
        $(this.el).append(this.ajaxPopupDisplay.el);

    },
    editEvent:function(url, data){
        var formOptions = {
            id: "editModule",
            url: url,
            data:data,
            view:"AppointmentView",
            buttons: MF.Views.popupButtonBuilder.builder("editModule").standardEditButons()
        };
        this.ajaxPopupFormModule = new MF.Views.AjaxPopupFormModule(formOptions);
        this.ajaxPopupFormModule.render();
        this.storeChild(this.ajaxPopupFormModule);
        $(this.el).append(this.ajaxPopupFormModule.el);
    },

    changeEventCallback:function(result,revertFunc){
        if(!result.Success){
            alert(result.Message);
            revertFunc();
        }
    },

    copyItem:function(){
        var entityId = $("#EntityId",this.ajaxPopupDisplay.el).val();
        var data = {"EntityId":entityId,"Copy":true};
        this.editEvent(this.options.calendarDef.AddEditUrl,data);
        this.ajaxPopupDisplay.close();
    },

    deleteItem: function(){
        if (confirm("Are you sure you would like to delete this Item?")) {
            var entityId = $("#EntityId").val();
            MF.repository.ajaxGet(this.options.calendarDef.DeleteUrl,{"EntityId":entityId}, $.proxy(function(result){
                this.ajaxPopupDisplay.close();
                if(!result.Success){
                    alert(result.Message);
                }else{
                   this.reload();
                }
            },this));
        }
    },
    displayEdit:function(event){
        if(!this.options.calendarDef.canEdit){
             alert("you can't edit retroactively");
            return;
        }
        var id = $("#EntityId",this.ajaxPopupDisplay.el).val();
        this.ajaxPopupDisplay.close();
        this.editEvent(this.options.calendarDef.AddEditUrl+"/"+id);
    },


    resetCalendar:function(){
        var locId = $("[name=Location]").val();
        var ids="";
        $(".legendLabel").each(function(i,item){
            if($(item).hasClass("showing")){
                ids+= $("#trainerId",$(item).parent()).val()+",";
            }
        });
        if(ids){
            ids = ids.substr(0,ids.length-1);
        }else{
            ids="0";
        }
        this.replaceSource({url : this.options.calendarDef.Url, data:{Loc:locId, TrainerIds:ids} });
        this.reload();
    },
    reload:function(){
        $(this.options.calendarDef.calendarContainer,this.el).fullCalendar( 'refetchEvents' )
    },

    replaceSource:function(source){
        $(this.options.calendarDef.calendarContainer,this.el).fullCalendar( 'replaceEventSource', source )
    },
    legendLabelClick:function(e){
        $(e.target).toggleClass("showing");
        this.resetCalendar();
    },
    legendHeaderClick:function(e){
        if($(e.target).hasClass("showing")){
            $(".legendHeader").removeClass("showing");
            $(".legendLabel").each(function(i,item){
                $(item).removeClass("showing");
            })
        }else{
            $(".legendHeader").addClass("showing");
            $(".legendLabel").each(function(i,item){
                if(!$(item).hasClass("showing")){
                    $(item).addClass("showing");
                }
            })
        }
        this.resetCalendar();
    },

    formSuccess:function(){
        this.formCancel();
        this.reload();
    },
    formCancel:function(){
        this.ajaxPopupFormModule.close();
    },
    displayCancel:function(){
        this.ajaxPopupDisplay.close();
    }
});
MF.Views.AppointmentView = MF.Views.AjaxFormView.extend({
    events:_.extend({
        'change [name="Appointment.Length"]':'handleTimeChange',
        'change [name="sHour"]':'handleTimeChange',
        'change [name="sMinutes"]':'handleTimeChange',
        'change [name="sAMPM"]':'handleTimeChange'

    }, MF.Views.AjaxFormView.prototype.events),

    viewLoaded:function(){
        this.loadTokenizers();
    },
    handleTimeChange:function(){
        var startTime = this.getStartTime();
        var endTimeString = this.getEndTimeString(startTime);
        $("#endTime").text(endTimeString);
    },
    getEndTimeString:function(startTime){
        var min;
        switch($("[name='Appointment.Length']").val()){
            case "Hour":
                min = 60;
                break;
            case "Half Hour":
                min = 30;
                break;
            case "Hour and a Half":
                min = 90;
                break;
        }
        var endTime = startTime.addMinutes(min);
        var endHour = endTime.getHours();
            var amPm = "AM";
            if(endHour>12){
                endHour-=12;
                amPm="PM";
            }
            var min = endTime.getMinutes().toString();
            if(min == "0"){
                min="00";
            }
            return endHour+":"+min+" "+amPm;
    },
    getStartTime:function(){
        var hour = $("[name='sHour']").val();
        var min = $("[name='sMinutes']").val();
        if($("[name='sAMPM']").val()=="PM"){
            hour = new Number(hour)+12;
        }
        return new XDate().setHours(hour).setMinutes(min);
    },

    loadTokenizers: function(){
        var options = $.extend({},this.options,{el:"#clients"});

        this.tokenView = new MF.Views.TokenView(options);
        this.tokenView.render();
        this.storeChild(this.tokenView);
    }
});
MF.Views.ClientFormView = MF.Views.AjaxFormView.extend({
    events:_.extend({
        'click .payment':'payment'
    }, MF.Views.AjaxFormView.prototype.events),
    payment:function(){
        var id = $(this.el).find("#EntityId").val();
        MF.vent.trigger("route","paymentlist/"+id,true);
    }
});
MF.Views.PaymentListView = MF.Views.GridView.extend({
    addNew:function(){
        var parentId = $(this.el).find("#ParentId").val();
        MF.vent.trigger("route",this.options.addUpate+"/0/"+parentId ,true);
        //$.publish('/contentLevel/grid_'+this.id+'/AddUpdateItem', [this.options.addU+
        //
        // pdateUrl]);
    },
    editItem:function(id,itemType){
        var parentId = $(this.el).find("#ParentId").val();
        var _itemType = itemType?itemType:"";
        // fix this for all assets
        MF.vent.trigger("route",this.options.addUpate+"/"+id+"/"+parentId,true);
    }
});

MF.Views.PaymentFormView = MF.Views.AjaxFormView.extend({
    events:_.extend({
    },MF.Views.AjaxFormView.prototype.events),
    viewLoaded:function(){
        $("#fullHour").change($.proxy(function(e){
            this.calculateTotal("FullHour","#fullHourTotal",e.target);
        },this));
        $("#halfHour").change($.proxy(function(e){
            this.calculateTotal("HalfHour","#halfHourTotal",e.target);
        },this));
        $("#fullHourTenPack").change($.proxy(function(e){
            this.calculateTotal("FullHourTenPack","#fullHourTenPackTotal",e.target);
        },this));
        $("#halfHourTenPack").change($.proxy(function(e){
            this.calculateTotal("HalfHourTenPack","#halfHourTenPackTotal",e.target);
        },this));
        $("#pair").change($.proxy(function(e){
            this.calculateTotal("Pair","#pairTotal",e.target);
        },this));

        this.calculateTotal("FullHour","#fullHourTotal","#fullHour");
        this.calculateTotal("HalfHour","#halfHourTotal","#halfHour");
        this.calculateTotal("FullHourTenPack","#fullHourTenPackTotal","#fullHourTenPack");
        this.calculateTotal("HalfHourTenPack","#halfHourTenPackTotal","#halfHourTenPack");
        this.calculateTotal("Pair","#pairTotal","#pair");

    },
    calculateTotal:function(type, totalSelector, numberSelector){
        var number = $(numberSelector).val();
        var itemTotal = (this.options.sessionRates[type] * number);
        $(totalSelector).text("$" + itemTotal);
        var total = parseInt($("#fullHourTotal").text().substring(1))
            + parseInt($("#halfHourTotal").text().substring(1))
            + parseInt($("#fullHourTenPackTotal").text().substring(1))
            + parseInt($("#halfHourTenPackTotal").text().substring(1))
            + parseInt($("#pairTotal").text().substring(1));
        $("#total").val(total);

    }
});

MF.Views.TrainerFormView = MF.Views.AjaxFormView.extend({
     events:_.extend({
        'click #trainerPayments' : 'trainerPayments',
         'click #payTrainer' : 'payTrainer'
    }, MF.Views.AjaxFormView.prototype.events),
    viewLoaded:function(){
        this.loadPlugins();
        this.loadTokenizers();
    },
    loadTokenizers: function(){
        var clientOptions = $.extend({el:"#clients", id:"clientToken"},this.options.clientOptions);
        var userRoleOptions = $.extend({el:"#userRoles"},this.options.userRolesOptions);
        this.clientsView = new MF.Views.TrainerEditableTokenView(clientOptions);
        this.clientsView.render();
        this.storeChild(this.clientsView);

        this.userRolesView = new MF.Views.TokenView(userRoleOptions);
        this.userRolesView.render();
        this.storeChild(this.userRolesView);
    },
    loadPlugins:function(){
        $('#color',"#detailArea").miniColors();
    },
    trainerPayments:function(){
        var id = $(this.el).find("#EntityId").val();
        MF.vent.trigger("route","trainerpaymentlist/"+id,true);
    },
    payTrainer:function(){
        var id = $(this.el).find("#EntityId").val();
        MF.vent.trigger("route","paytrainerlist/"+id,true);
    }
});

MF.Views.TrainerEditableTokenView = MF.Views.EditableTokenView.extend({
     events:_.extend({
        'click .tokenEditor' : 'tokenEditor'
    }, MF.Views.EditableTokenView.prototype.events),
    internalTokenMarkup: function(item) {
        var cssClass = "class='selectedItem'";
        return "<p><a " + cssClass + ">" + item.name+" ( "+item.percentage + " )</a><a href='javascript:void(0);' class='tokenEditor' >&nbsp;-- Edit</a><input id='itemId' type='hidden' value='"+item.id+"' </p>";
    },
    render:function(){
        MF.vent.bind("popup:templatePopup:save",this.tokenSave,this);
        MF.vent.bind("popup:templatePopup:cancel",this.tokenCancel,this);
    },
    onClose:function(){
        MF.vent.unbind("popup:templatePopup:save");
        MF.vent.unbind("popup:templatePopup:cancel");
    },
    afterTokenSelectedFunction:function(item) {
        if(!$(this.options.inputSelector,this.el).data("selectedItems"))$(this.options.inputSelector,this.el).data("selectedItems",[]);
        $(this.options.inputSelector,this.el).data("selectedItems").push(item);
    },
    deleteToken:function(hidden_input,token_data) {
        var data = $(this.options.inputSelector,this.el).data("selectedItems");
        var idx=0;
        $.each(data,function(i,item){
            if(item.id == hidden_input.id){
                idx=i;
            }
        });
        data.splice(idx,1);
    },
    tokenEditor:function(e){
        this.options.currentlyEditing = $(e.target).prev("a");
        var id = $(e.target).next("input#itemId").val();
        var data = $(this.options.inputSelector,this.el).data("selectedItems");
        var dataItem;
        $.each(data,function(i,item){
            if(item.id == id) dataItem = item;
        });
        var buttons = this.options.buttons?this.options.buttons:MF.Views.popupButtonBuilder.builder("templatePopup").standardEditButons();
        var popupOptions = {
            id:"templatePopup",
            buttons: buttons,
            data:dataItem,
            template:"#percentageTemplate"
        };
        this.templatedPopupView = new MF.Views.TemplatedPopupView(popupOptions);
        this.templatedPopupView.render();
        this.storeChild(this.templatedPopupView);

    },
    tokenSave:function(){
        var id = $("#editingId").val();
        var data = $(this.options.inputSelector,this.options.el).data("selectedItems");
        var dataItem;
        $.each(data,function(i,item){
            if(item.id == id) dataItem = item;
        });
        dataItem.percentage = $("#newTrainerPercentage").val();
        var anchor = $(this.options.currentlyEditing).text();
        var newText = anchor.substr(0,anchor.indexOf('(')) +"( "+$("#newTrainerPercentage").val()+" ) ";
        $(this.options.currentlyEditing).text(newText);
//        MF.vent.unbind("popup:templatePopup:save");
        this.templatedPopupView.close();
    },
    tokenCancel:function(){
        this.templatedPopupView.close();
    }

});

MF.Views.TrainerGridView = MF.Views.GridView.extend({
    viewLoaded:function(){
        MF.vent.bind("Redirect",this.showPayGrid,this);
    },
    showPayGrid:function(id){
        MF.vent.trigger("route","paytrainerlist/"+id,true);
    }
});