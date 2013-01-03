/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 2/26/12
 * Time: 11:24 AM
 * To change this template use File | Settings | File Templates.
 */

MF.Views.PayTrainerGridView = MF.Views.View.extend({
    initialize: function () {
        this.beforeInitGrid();
        this.model = {};
        MF.mixin(this, "ajaxGridMixin");
        MF.mixin(this, "setupGridMixin");
        MF.mixin(this, "setupGridSearchMixin");
    },
    events:{
        'click .jqgrow':'handleSingleClick',
        'click .cbox':'handleSelectAllClick',
        'click #payTrainerButton':'payTrainer',
        'click .return':'returnToParent',
        'click #search':'filterByDate'
    },

    beforeInitGrid:function(){
        var that = this;
        this.options.gridId="trainerPayment";
        this.options.gridOptions={
            loadComplete : function(){
                var ids = $(this).getDataIDs();
                var paymentRows =[];
                that.options.paymentTotal = 0;
                for (var i = 0, l = ids.length; i < l; i++) {
                    var rowId = ids[i];
                    var row;
                    var rowData = $(this).getRowData(rowId);
                    if (rowData.InArrears=="True") {
                        row = $('#' + rowId, that.el);
                        row.find("td").addClass('gridRowStrikeThrough');
                        row.find("td:first input").remove();
                    } else if(rowData.TrainerVerified=="True") {
                        row = $('#' + rowId, that.el);
                        row.find("td").addClass('gridRowGreen');
                        paymentRows.push({
                            id:rowId,
                            trainerPay:rowData.TrainerPay,
                            _checked:false
                        });
                    } else {
                        row = $('#' + rowId, that.el);
                        row.find("td").addClass('gridRowGrey');
                        paymentRows.push({
                            id:rowId,
                            trainerPay:rowData.TrainerPay,
                            _checked:false
                        });
                    }
                }
                that.model.paymentAmount = ko.observable(that.options.paymentTotal);
                that.model.eligableRows = ko.mapping.fromJS(paymentRows);
                that.setupElements();
            }}
    },
    viewLoaded:function(){
        MF.vent.bind("popup:payTrainerPopup:save",this.formSave,this);
        MF.vent.bind("popup:payTrainerPopup:cancel",this.formCancel,this);
    },
    onClose:function(){
        MF.vent.unbind("popup:payTrainerPopup:save");
        MF.vent.unbind("popup:payTrainerPopup:cancel");
        this._super("onClose",arguments);
    },
    reloadGrid: function () {
        $("#" + this.options.gridId).trigger("reloadGrid");
    },
    setupElements:function(){
        if($("#filterArea").size()==0){
            $(this.el).find(".content-header").prepend($("#payTrainerSearchTemplate").tmpl());
            $(".title-name",this.el).append("<span class='paymentAmount' data-bind='text:paymentAmount'></span>");
            $("[name='EndDate']",this.$el).datepicker();
            this.model.EndDate= ko.observable( new XDate().toString("MM/dd/yyyy") );
            $("#payTrainerButton").hide();
            $('.paymentAmount').hide();
            $(".content-header",this.$el).find(".search").remove();

            ko.applyBindings(this.model,this.el);
        }
    },

    returnToParent:function(){
        MF.WorkflowManager.returnParentView(null,true);
    },
    filterByDate:function(e){
        var obj = {"endDate":this.model.EndDate()};
        $("#" + this.options.gridId).jqGrid('setGridParam',{postData:obj});
        this.reloadGrid();
    },
    payTrainer:function(){
        var amount = this.model.paymentAmount();
        if(amount<=0){return;}
        var builder = MF.Views.popupButtonBuilder.builder("payTrainerPopup");
        builder.addButton("Ok", builder.getSaveFunc());
        builder.addCancelButton();
        var data={trainersName:this.options.TrainersName,amount:amount};
        var formOptions = {
            id: "payTrainerPopup",
            data:data,
            template:"#payTrainerTemplate",
            title:"Trainer Payment",
            buttons:builder.getButtons()
        };
        this.templatePopup = new MF.Views.TemplatedPopupView(formOptions);
        this.templatePopup.render();
        this.storeChild(this.templatePopup);
    },
    formSave:function(){
        var model = ko.mapping.toJS(this.model);
        model.eligableRows = _.filter(model.eligableRows,function(item){return item._checked;});
        model.EntityId = MF.State.get("Relationships").entityId;
        var data = JSON.stringify(model);
        var promise = MF.repository.ajaxPostModel(this.options.PayTrainerUrl,data);
        promise.done($.proxy(this.paymentCallback,this));
    },
    paymentCallback:function(result){
        if(result.Success){
            this.reloadGrid();
            window.open(result.Variable);
            this.formCancel();
        }
    },
    formCancel:function(){
        this.templatePopup.close();
    },
    handleSingleClick:function(e) {
        var checkbox = $(e.currentTarget).find(".cbox");
        if(!checkbox||checkbox.attr("disabled")){return;}

        var id = $(e.currentTarget).attr("id");
        var data = _.find(this.model.eligableRows(),function(item){return item.id() == id;});
        var itemAmount = parseFloat(data.trainerPay());

        if (checkbox.is(":checked")) {
            this.model.paymentAmount(this.model.paymentAmount() + itemAmount);
            data._checked(true);
        } else {
            this.model.paymentAmount(this.model.paymentAmount() - itemAmount);
            data._checked(false);
        }
        if(this.model.paymentAmount()>0){
            $("#payTrainerButton").show();
            $('.paymentAmount').show();
        } else {
            $("#payTrainerButton").hide();
            $('.paymentAmount').hide();
        }
    },
    handleSelectAllClick:function(e){
        if($(e.target).closest("tr").attr("role")!="rowheader"){return;}
        this.model.paymentAmount(0);
        if($(e.currentTarget).is(":checked")){
            _.each(this.model.eligableRows(),function(item){
                item._checked(true);
                this.model.paymentAmount(this.model.paymentAmount() +parseFloat(item.trainerPay()))
            },this);
        }else{
            _.each(this.model.eligableRows(),function(item){
                item._checked(false);
            },this);
        }
        if(this.model.paymentAmount()>0){
            $("#payTrainerButton").show();
        } else {
            $("#payTrainerButton").hide();
        }
    }
});

MF.Views.TrainerSessionVerificationView = MF.Views.View.extend({
    initialize:function(){
        this.beforeInitGrid();
        this.model = {};
        MF.mixin(this, "ajaxGridMixin");
        MF.mixin(this, "setupGridMixin");
        MF.mixin(this, "setupGridSearchMixin");
        this.notification = new CC.NotificationService();

    },
    events:{
        'click #acceptSessionsButton':'acceptSessions',
        'click #alertAdminButton':'alertAdmin',
        'click #search':'filterByDate'
    },
    successSelector:"#messageContainer",
    errorSelector:"#messageContainer",
    beforeInitGrid:function(){
        var that = this;
        this.options.gridId="trainerPayment";
        this.options.gridOptions={
            multiselect:false,
            loadComplete : function(){
                that.options.paymentTotal = 0;
                var ids = $(this).getDataIDs();
                var paymentRows =[];
                for (var i = 0, l = ids.length; i < l; i++) {
                    var rowId = ids[i];
                    var rowData = $(this).getRowData(rowId);
                    if (rowData.InArrears=="True") {
                        var row = $('#' + rowId, that.el);
                        row.find("td").addClass('gridRowStrikeThrough');
                        row.find("td:first input").remove();
                    } else {
                        paymentRows.push({
                            id:rowId,
                            trainerPay:rowData.TrainerPay
                        });
                        that.options.paymentTotal += parseFloat(rowData.TrainerPay);
                    }
                }

                that.model.paymentAmount = ko.observable(that.options.paymentTotal);
                that.model.eligableRows = ko.mapping.fromJS(paymentRows);
                that.setupElements();
            }}
    },
    viewLoaded:function(){
        MF.vent.bind("popup:trainerAlertAdminPopup:save",this.formSave,this);
        MF.vent.bind("popup:trainerAlertAdminPopup:cancel",this.formCancel,this);
    },
    onClose:function(){
        MF.vent.unbind("popup:trainerAlertAdminPopup:save");
        MF.vent.unbind("popup:trainerAlertAdminPopup:cancel");
        this._super("onClose",arguments);
    },
    setupElements:function(){
        if($("#filterArea").size()==0){
//            $(this.el).find(".content-header").append($("#payTrainerSearchTemplate").tmpl());
            $(this.el).find(".content-header").prepend('<a href="#" id="acceptSessionsButton"><img src="/content/images/thumbs_up.jpg" title="Accept Sessions" class="thumbsImage" /></a><a href="#" id="alertAdminButton"><img src="/content/images/thumbs_down.jpg" title="Email Admin of a problem" class="thumbsImage thumbsLeft" /></a>' );
//            $("#filterArea").empty();
            $(".title-name",this.el).append("<span class='paymentAmount' data-bind='text:paymentAmount'></span>");
            $("[name='EndDate']",this.$el).datepicker();
            this.model.EndDate= ko.observable( new XDate().toString("MM/dd/yyyy") );

            ko.applyBindings(this.model,this.el);
        }
    },
    filterByDate:function(e){
        var obj = {"endDate":this.model.EndDate()};
        $("#" + this.options.gridId).jqGrid('setGridParam',{postData:obj});
        this.reloadGrid();
    },
    reloadGrid: function () {
        $("#" + this.options.gridId).trigger("reloadGrid");
    },
    acceptSessions:function(){
        var model = ko.mapping.toJS(this.model);
        model.EntityIds = _.pluck(model.eligableRows,"id");
        var data = JSON.stringify(model);
        var promise = MF.repository.ajaxPostModel(this.options.AcceptSessionsUrl,data);
        promise.done($.proxy(this.acceptSessionsCallback,this));
    },
    acceptSessionsCallback:function(_result){
        var result = typeof _result =="string" ? JSON.parse(_result) : _result;
        if(!result.Success){
            if(result.Message && !$.noty.getByViewIdAndElementId(this.cid)){
                $(this.errorSelector).noty({type: "error", text: result.Message, viewId:this.cid});
            }
            if(result.Errors && !$.noty.getByViewIdAndElementId(this.cid)){
                _.each(result.Errors,function(item){
                    $(this.errorSelector).noty({type: "error", text:item.ErrorMessage, viewId:this.cid});
                })
            }
        }else{
            if(result.Message){
                var note = $(this.successSelector).noty({type: "success", text:result.Message, viewId:this.cid});
                note.setAnimationSpeed(1000);
                note.setTimeout(3000);
                $.noty.closeAllErrorsByViewId(this.cid);
            }
            this.model.paymentAmount(0);
            this.reloadGrid();
            MF.vent.trigger("form:"+this.id+":success",result);
        }
    },
    alertAdmin:function(){
        this.model.From = ko.observable(this.options.From);
        this.model.To = ko.observable(this.options.To);
        this.model.Subject = ko.observable(this.options.Subject);
        this.model.Body = ko.observable(this.options.Body);
        var builder = MF.Views.popupButtonBuilder.builder("trainerAlertAdminPopup");
        builder.addButton("Send", builder.getSaveFunc());
        builder.addCancelButton();
        var data=this.model;
        var formOptions = {
            id: "trainerAlertAdminPopup",
            data:data,
            template:"#emailTemplate",
            title:"Alert Admin",
            buttons:builder.getButtons()
        };
        this.templatePopup = new MF.Views.KOPopupView(formOptions);
        this.templatePopup.render();
        this.storeChild(this.templatePopup);
    },
    formSave:function(){
        var model = ko.mapping.toJS(this.model);
        var data = JSON.stringify(model);
        var promise = MF.repository.ajaxPostModel(this.options.AlertAdminEmailUrl,data);
        promise.done($.proxy(this.emailCallback,this));
    },
    emailCallback:function(_result){
        var result = typeof _result =="string" ? JSON.parse(_result) : _result;
        this.notification.render(_result.Success ?$("#messageContainer").get(0) : $("#puMessageContainer",this.templatePopup.el).get(0));
        if(!this.notification.handleResult(result,this.cid)){
            return;
        }
        MF.vent.trigger("form:"+this.id+":success",result);
       this.formCancel();
    },
    formCancel:function(){
        this.templatePopup.close();
    }
});

MF.Views.TrainerSessionView = MF.Views.View.extend({
    initialize:function(){
        this.beforeInitGrid();
        this.model = {};
        MF.mixin(this, "ajaxGridMixin");
        MF.mixin(this, "setupGridMixin");
        MF.mixin(this, "setupGridSearchMixin");
    },
    beforeInitGrid:function(){
        var that = this;
        this.options.gridId="trainerPayment";
        this.options.gridOptions={
            multiselect:false,
            loadComplete : function(){
                that.options.paymentTotal = 0;
                var ids = $(this).getDataIDs();
                for (var i = 0, l = ids.length; i < l; i++) {
                    var rowId = ids[i];
                    var rowData = $(this).getRowData(rowId);
                    if (rowData.InArrears=="True") {
                        var row = $('#' + rowId, that.el);
                        row.find("td").addClass('gridRowStrikeThrough');
                        row.find("td:first input").remove();
                    }
                    that.options.paymentTotal += parseFloat(rowData.TrainerPay);
                }

                that.model.paymentAmount = ko.observable(that.options.paymentTotal);
                that.setupElements();
            }}
    },

    reloadGrid: function () {
        $("#" + this.options.gridId).trigger("reloadGrid");
    },
    setupElements:function(){
        $(".title-name",this.el).append("<span class='paymentAmount' data-bind='text:paymentAmount'></span>");
        ko.applyBindings(this.model,this.el);
    }
});

MF.Views.TrainerPaymentListGridView = MF.Views.View.extend({
     initialize:function(){
        this.options.gridOptions ={multiselect:false};
        MF.mixin(this, "ajaxGridMixin");
        MF.mixin(this, "setupGridMixin");
        MF.mixin(this, "defaultGridEventsMixin");
        MF.mixin(this, "setupGridSearchMixin");
        this.options.gridId = "trainerPaymentsList";
    },
    events:{
         'click .return':'retunToParent'
    },
    retunToParent:function(){
        MF.WorkflowManager.returnParentView(null,true);
    },
    displayItem:function(id){
        var parentId = this.$el.find("#EntityId").val();
        window.open("/Billing/PayTrainer/TrainerReceipt/"+id+"?ParentId="+parentId);
        return false;
    },
    viewLoaded:function(){
        this.setupBindings();
    },
    onClose:function(){
        this.unbindBindings();
    }
});