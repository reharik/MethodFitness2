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
         $.when(MF.loadTemplateAndModel(this))
         .done($.proxy(this.renderCallback,this));
//       MF.repository.ajaxGet(this.options.url, this.options.data).done($.proxy(this.renderCallback,this));
    },
    renderCallback:function(){
        this.model = this.rawModel;
        this.model.id=this.id;
        $("#calendar",this.el).asCalendar(this.model.CalendarDefinition);
        //callback for render
        this.viewLoaded();
        //general notification of pageloaded
        MF.vent.trigger("calendar:"+this.id+":pageLoaded",this.options);
        this.calendarBindings();
    },
    calendarBindings:function(){
        MF.vent.trigger("calendar:"+this.id+":pageLoaded",this.options);
        MF.vent.bind("calendar:"+this.id+":eventDrop",this.eventDrop,this);
        MF.vent.bind("calendar:"+this.id+":eventResize",this.eventResize,this);
        MF.vent.bind("calendar:"+this.id+":dayClick",this.dayClick,this);
        MF.vent.bind("calendar:"+this.id+":eventClick",this.eventClick,this);
        MF.vent.bind("ajaxPopupFormModule:editModule:success",this.formSuccess,this);
        MF.vent.bind("ajaxPopupFormModule:editModule:cancel",this.formCancel,this);
        MF.vent.bind("ajaxPopupDisplayModule:displayModule:cancel",this.displayCancel,this);
        MF.vent.bind("popup:displayModule:edit",this.displayEdit,this);

        this.setupLegend();
    },
    onClose:function(){
        MF.vent.unbind("calendar:"+this.id+":eventDrop");
        MF.vent.unbind("calendar:"+this.id+":eventResize");
        MF.vent.unbind("calendar:"+this.id+":eventClick");
        MF.vent.unbind("calendar:"+this.id+":dayClick");
        MF.vent.unbind("ajaxPopupFormModule:editModule:success");
        MF.vent.unbind("ajaxPopupFormModule:editModule:cancel");
        MF.vent.unbind("ajaxPopupDisplayModule:displayModule:cancel");
        MF.vent.unbind("popup:displayModule:edit");
    },
    setupLegend:function(){
         if(this.model.Trainers.length<=0){
            $("#legend").hide();
        }
        $( "#legendTemplate" ).tmpl( this.model.Trainers ).appendTo( "#legendItems" );
        $(".legendHeader").addClass("showing");
        $(".legendLabel").each(function(i,item){ $(item).addClass("showing"); });
    },
    eventDrop:function(event, dayDelta,minuteDelta,allDay,revertFunc) {
        var data = {"EntityId":event.EntityId,
            "ScheduledDate":$.fullCalendar.formatDate( event.start,"M/d/yyyy hh:mm TT"),
            "StartTime":$.fullCalendar.formatDate( event.start,"M/d/yyyy hh:mm TT"),
            "EndTime":$.fullCalendar.formatDate( event.end,"M/d/yyyy hh:mm TT")};
        MF.repository.ajaxGet(this.options.EventChangedUrl,data).done($.proxy(this.changeEventCallback,this));
    },
    eventResize:function( event, dayDelta, minuteDelta, revertFunc, jsEvent, ui, view ){
        var data = {"EntityId":event.EntityId,
            "ScheduledDate":$.fullCalendar.formatDate( event.start,"M/d/yyyy hh:mm TT"),
            "StartTime":$.fullCalendar.formatDate( event.start,"M/d/yyyy hh:mm TT"),
            "EndTime":$.fullCalendar.formatDate( event.end,"M/d/yyyy hh:mm TT")
        };
        MF.repository.ajaxGet(this.options.EventChangedUrl,data).done($.proxy(this.changeEventCallback,this));
    },
    dayClick:function(date, allDay, jsEvent, view) {
        if(new XDate(date).diffHours(new XDate())>0 && !this.model.CalendarDefinition.CanEnterRetroactiveAppointments){
            alert("That period is closed");
            return;
        }
        var data = {"ScheduledDate" : $.fullCalendar.formatDate( date,"M/d/yyyy"), "ScheduledStartTime": $.fullCalendar.formatDate( date,"hh:mm TT")};
        this.editEvent(this.model.CalendarDefinition.AddEditUrl,data);
    },
    eventClick:function(calEvent, jsEvent, view) {
        if(calEvent.trainerId!= this.model.CalendarDefinition.TrainerId && !this.model.CalendarDefinition.CanSeeOthersAppointments){
            return;
        }
        this.model.CalendarDefinition.canEdit = new XDate(calEvent.start).diffHours(new XDate())<0 || this.model.CalendarDefinition.CanEditPastAppointments;
        var data = {"EntityId": calEvent.EntityId};
        var builder = MF.Views.popupButtonBuilder.builder("displayModule");
        builder.addButton("Delete", $.proxy(this.deleteItem,this));
        builder.addEditButton();
        builder.addButton("Copy Event",$.proxy(this.copyItem,this));
        builder.addCancelButton();

        var formOptions = {
            id: "displayModule",
            route: this.model.DisplayRoute,
            templateUrl: this.model.DisplayUrl+"_Template?Popup=true",
            view: this.options.subViewName?"Display" + this.options.subViewName:"",
            AddUpdateUrl: this.model.AddUpdateUrl,
            url: this.model.CalendarDefinition.DisplayUrl,
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
            route: this.model.AddUpdateRoute,
            url: url,
            templateUrl: url+"_Template?Popup=true",
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
        this.editEvent(this.model.CalendarDefinition.AddEditUrl,data);
        this.ajaxPopupDisplay.close();
    },

    deleteItem: function(){
        if (confirm("Are you sure you would like to delete this Item?")) {
            var entityId = $("#EntityId").val();
            MF.repository.ajaxGet(this.model.CalendarDefinition.DeleteUrl,{"EntityId":entityId}).done($.proxy(function(result){
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
        if(!this.model.CalendarDefinition.canEdit){
             alert("you can't edit retroactively");
            return;
        }
        var id = $("#EntityId",this.ajaxPopupDisplay.el).val();
        this.ajaxPopupDisplay.close();
        this.editEvent(this.model.CalendarDefinition.AddEditUrl+"/"+id);
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
        this.replaceSource({url : this.model.CalendarDefinition.Url, data:{Loc:locId, TrainerIds:ids} });
        this.reload();
    },
    reload:function(){
        $(this.model.CalendarDefinition.calendarContainer,this.el).fullCalendar( 'refetchEvents' )
    },

    replaceSource:function(source){
        $(this.model.CalendarDefinition.calendarContainer,this.el).fullCalendar( 'replaceEventSource', source )
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
MF.Views.AppointmentView = MF.Views.View.extend({
    initialize: function(){
        MF.mixin(this, "formMixin");
        MF.mixin(this, "ajaxFormMixin");
        MF.mixin(this, "modelAndElementsMixin");
    },
    events:{
        'change [name="AppointmentType"]':'handleTimeChange',
        'click #save' : 'saveItem',
        'click #cancel' : 'cancel'
    },
    viewLoaded:function(){
        var startDate = new XDate(new Date("1/5/1972 "+this.model.StartTime()));
        this.setEndTime(startDate);
        MF.vent.bind("StartTime:timeBox:close",this.handleTimeChange,this);
    },
    onClose:function(){
        MF.vent.unbind("StartTime:timeBox:close",this.handleTimeChange,this);
    },
    renderElements:function(){
        var collection = this.elementsViewmodel.collection;
        var startTimeElement = collection["StartTime"];
        startTimeElement.timeDefaults.stepMinute = 15;
        for(var item in collection){
            collection[item].render();
        }
    },
    handleTimeChange:function(valArray) {
        var scroller = $('[name="StartTime"]').scroller('getInst');
        if (!scroller) {return;}
        var date = $('[name="Date"]').val();
        var timeValues;
        timeValues = scroller.values;
        var startTime = new XDate(date).setHours(timeValues[0] + (parseInt(timeValues[2])*12)).setMinutes(timeValues[1]);
        this.setEndTime(startTime);
    },
    setEndTime:function(startTime){
        var aptMin;
        switch($("[name='AppointmentType']").val()){
            case "Hour":
            case "Pair":
                aptMin = 60;
                break;
            case "Half Hour":
                aptMin = 30;
                break;
        }
        var endTime = startTime.addMinutes(aptMin);
        var endHour = endTime.getHours();
            var amPm = "AM";
            if(endHour>12){
                endHour-=12;
                amPm="PM";
            }
            var endMin = endTime.getMinutes().toString();
            if(endMin.length == 1){
                endMin="0"+endMin;
            }
            $("#endTime").text(endHour+":"+endMin+" "+amPm);
    }

});
MF.Views.ClientFormView = MF.Views.View.extend({
     events:_.extend({
        'click .client_payment':'payment',
        'click .delete':'deleteItem',
        'click #save' : 'saveItem',
        'click #cancel' : 'cancel'
    }),
    initialize:function(){
        MF.mixin(this, "formMixin");
        MF.mixin(this, "ajaxFormMixin");
        MF.mixin(this, "modelAndElementsMixin");
    },
    viewLoaded:function(){
        this._setupBindings();
    },
     _setupBindings:function(){
         MF.vent.bind("delete:"+this.id+":success",this.deleteSuccess,this);
    },
    _unbindBindings:function(){
        MF.vent.unbind("delete:"+this.id+":success",this.deleteSuccess,this);
    },
    payment:function(){
        var id =this.model.EntityId();
        MF.vent.trigger("route",MF.generateRoute("paymentlist",id),true);
    },
    deleteItem:function(){
        if (confirm("Are you sure you would like to delete this Item?")) {
            var id =this.model.EntityId();
            MF.repository.ajaxPost(this.model._deleteUrl, {'EntityId':id},$.proxy(this.deleteCallback,this));
        }
    },
    deleteCallback:function(_result){
        var result = typeof _result =="string" ? JSON.parse(_result) : _result;
        if(!CC.notification.handleResult(result,this.cid)){
            return;
        }
        MF.vent.trigger("delete:"+this.id+":success",result);
    },
    deleteSuccess:function(result){
        MF.WorkflowManager.returnParentView(result,true);
    },
    onClose:function(){
        this._unbindBindings();
        this._super("onClose",arguments);
    }
});
MF.Views.PaymentListView = MF.Views.View.extend({
    initialize:function(){
        MF.mixin(this, "ajaxGridMixin");
        MF.mixin(this, "setupGridMixin");
        MF.mixin(this, "defaultGridEventsMixin");
        MF.mixin(this, "setupGridSearchMixin");
    },
    addNew:function(){
        var parentId = this.options.ParentId;
        MF.vent.trigger("route",MF.generateRoute(this.options.addUpdate,0,parentId) ,true);
    },
    editItem:function(id){
        var parentId = this.options.ParentId;
        MF.vent.trigger("route",MF.generateRoute(this.options.addUpdate,id,parentId),true);
    },
    viewLoaded:function(){
        this.setupBindings();
    },
    onClose:function(){
        this.unbindBindings();
    }
});

MF.Views.PaymentFormView = MF.Views.View.extend({
    initialize: function(){
        MF.mixin(this, "formMixin");
        MF.mixin(this, "ajaxFormMixin");
        MF.mixin(this, "modelAndElementsMixin");
    },
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
         $("#pairTenPack").change($.proxy(function(e){
            this.calculateTotal("PairTenPack","#pairTenPackTotal",e.target);
        },this));

        this.calculateTotal("FullHour","#fullHourTotal","#fullHour");
        this.calculateTotal("HalfHour","#halfHourTotal","#halfHour");
        this.calculateTotal("FullHourTenPack","#fullHourTenPackTotal","#fullHourTenPack");
        this.calculateTotal("HalfHourTenPack","#halfHourTenPackTotal","#halfHourTenPack");
        this.calculateTotal("Pair","#pairTotal","#pair");
        this.calculateTotal("PairTenPack","#pairTenPackTotal","#pairTenPack");

    },
    calculateTotal:function(type, totalSelector, numberSelector){
        var number = $(numberSelector).val();
        var itemTotal = (this.model._sessionRateDto[type]() * number);
        $(totalSelector).text("$" + itemTotal);
        var total = parseInt($("#fullHourTotal").text().substring(1))
            + parseInt($("#halfHourTotal").text().substring(1))
            + parseInt($("#fullHourTenPackTotal").text().substring(1))
            + parseInt($("#halfHourTenPackTotal").text().substring(1))
            + parseInt($("#pairTotal").text().substring(1))
            + parseInt($("#pairTenPackTotal").text().substring(1));
        $("#total").val(total);
    }
});

MF.Views.TrainerFormView = MF.Views.AjaxFormView.extend({
     events:_.extend({
        'click #trainerPayments' : 'trainerPayments',
         'click #payTrainer' : 'payTrainer'
    }, MF.Views.AjaxFormView.prototype.events),
    viewLoaded:function(){
        this.$el.find(this.options.crudFormSelector).data().crudForm.setBeforeSubmitFuncs(this.clientRateBeforeSubmit);
        this.loadPlugins();
        this.loadTokenizers();
    },
    clientRateBeforeSubmit:function(arr){
        var items =$("[name='ClientsInput']").data('selectedItems');
        if(items&&$(items).size()>0){
            $.each(items, function(i,item){
            arr.push({"name":"SelectedClients["+i+"].id","value":item.id});
            arr.push({"name":"SelectedClients["+i+"].name","value":item.name});
            arr.push({"name":"SelectedClients["+i+"].percentage","value":item.percentage});
            })
        }
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
    },
    onClose:function(){
        MF.vent.unbind("Redirect");
        this._super("onClose",arguments);
    }
});

MF.Views.ClientGridView = MF.Views.View.extend({
    initialize:function(){
        MF.mixin(this, "ajaxGridMixin");
        MF.mixin(this, "setupGridMixin");
        MF.mixin(this, "defaultGridEventsMixin");
        MF.mixin(this, "setupGridSearchMixin");
    },
    viewLoaded:function(){
         MF.vent.bind(this.options.gridId+":Redirect",this.showPayGrid,this);
        this.setupBindings();
    },
    onClose:function(){
        MF.vent.unbind(this.options.gridId+":Redirect",this.showPayGrid,this);
        this.unbindBindings();
    },
    showPayGrid:function(id){
        MF.vent.trigger("route",MF.generateRoute("paymentlist",id),true);
    }
});
