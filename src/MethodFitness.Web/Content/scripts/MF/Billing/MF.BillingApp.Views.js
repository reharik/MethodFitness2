/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 2/26/12
 * Time: 11:24 AM
 * To change this template use File | Settings | File Templates.
 */

MF.Views.PayTrainerGridView = MF.Views.View.extend({
    initialize:function(){
        this.beforeInitGrid();
        this.model = {};
        MF.vent.bind("paymentGrid:eligableRows",$.proxy(function(rows){
            this.setupElements(rows);
        },this));
        MF.mixin(this, "ajaxGridMixin");
        MF.mixin(this, "setupGridMixin");
        MF.mixin(this, "setupGridSearchMixin");
    },
    events:{
         'click .jqgrow':'handleSingleClick',
         'click .cbox':'handleSelectAllClick',
         'click #payTrainerButton':'payTrainer',
         'click .return':'retunToParent',
         'click #search':'filterByDate'
    },

    beforeInitGrid:function(){
        var that = this;
        this.options.gridId="trainerPayment";
        this.options.gridOptions={
            loadComplete : function(){
            var ids = $(this).getDataIDs();
            var paymentRows =[];
            for (var i = 0, l = ids.length; i < l; i++) {
                var rowId = ids[i];
                var rowData = $(this).getRowData(rowId);
                if (parseInt(rowData.TrainerPay) > 0) {
                    paymentRows.push({
                        id:rowId,
                        trainerPay:rowData.TrainerPay,
                        _checked:false
                    })
                } else {
                    var row = $('#' + rowId, that.el);
                    row.find("td").addClass('gridRowStrikeThrough');
                    row.find("td:first input").remove();
                }
            }
            MF.vent.trigger("paymentGrid:eligableRows",paymentRows);
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
    setupElements:function(rows){
        this.model.eligableRows = ko.mapping.fromJS(rows);
        this.model.paymentAmount = ko.observable(0);

        if($("#payTrainerButton").size()==0){
        $(this.el).find(".content-header").prepend($("#payTrainerSearchTemplate").tmpl());
        $(".title-name",this.el).append("<span class='paymentAmount' data-bind='text:paymentAmount'></span>");
        $(".content-header",this.$el).find(".search").remove();
        $("[name='EndDate']",this.$el).datepicker();
        this.model.EndDate= ko.observable( new XDate().toString("MM/dd/yyyy") );
        }

        ko.applyBindings(this.model,this.el);
    },

    retunToParent:function(){
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