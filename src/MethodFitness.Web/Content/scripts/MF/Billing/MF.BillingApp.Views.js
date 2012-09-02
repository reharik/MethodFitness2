/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 2/26/12
 * Time: 11:24 AM
 * To change this template use File | Settings | File Templates.
 */

MF.Views.PayTrainerGridView = MF.Views.GridView.extend({
     events:_.extend({
         'click .jqgrow':'handleSingleClick',
         'click .cbox':'handleSelectAllClick',
         'click #payTrainerButton':'payTrainer',
         'click .return':'retunToParent',
         "click #search":"filterByDate"

    }, MF.Views.GridView.prototype.events),
    beforeInitGrid:function(){
        var $gridContainer = this.options.$gridContainer;
        this.options.gridOptions={loadComplete : function(){
            var ids = $gridContainer.jqGrid('getDataIDs');
            for (var i = 0, l = ids.length; i < l; i++) {
                var rowid = ids[i];
                var rowData =$gridContainer.jqGrid('getRowData',ids[i]);
                if (parseInt($(rowData.TrainerPay).text())<=0) {
                    var row = $('#' + rowid, this.el);
                   row.find("td").addClass('gridRowStrikeThrough');
                    row.find("td:first input").remove();
                }
            }
        }}
    },
    viewLoaded:function(){
        $(this.el).find(".content-header > .search").hide();
        $(this.el).find(".content-header").prepend("<button id='payTrainerButton' class='dollar_sign' ></button>");
        $(this.el).find(".content-header").prepend($("#payTrainerSearchTemplate").tmpl());
        $(this.el).find(".content-header #end_date").val(new XDate().toString("MM/dd/yyyy"));
        $(this.el).find(".content-header > .title-name").append("<span class='paymentAmount'></span>");
        $(this.el).find(".paymentAmount").data().total ={amount:0,items:[]};
        MF.vent.bind("popup:payTrainerPopup:save",this.formSave,this);
        MF.vent.bind("popup:payTrainerPopup:cancel",this.formCancel,this);
    },
    onClose:function(){
         MF.vent.unbind("popup:payTrainerPopup:save");
         MF.vent.unbind("popup:payTrainerPopup:cancel");
        if(this.options.notificationArea){
            MF.vent.unbind(this.options.notificationArea.areaName()+":"+this.id+":success",this.paymentSuccess,this);
        }
        this._super("onClose",arguments);
    },
    retunToParent:function(){
        MF.WorkflowManager.returnParentView(null,true);
    },
    filterByDate:function(e){
        var date = $(this.el).find($(".content-header #end_date")).val();
        var obj = {"endDate":date};
       this.options.$gridContainer.jqGrid('setGridParam',{postData:obj});
        this.reloadGrid();
    },
    payTrainer:function(){
        var amount = $(this.el).find(".paymentAmount").data().total.amount;
        if(amount<=0){return;}
        jQuery.ajaxSettings.traditional = true;

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
        var total = $(this.el).find(".paymentAmount").data().total;
        var arr = [];
        arr.push({"name":"EntityId", "value":this.options.EntityId});
        arr.push({"name":"PaymentDetailsDto.amount", "value":total.amount});
        $.each(total.items,function(i,item){
            arr.push({"name":"PaymentDetailsDto.items["+i+"].id","value":item.id});
            arr.push({"name":"PaymentDetailsDto.items["+i+"].amount","value":item.amount});
         });
        
        var data = $.param(arr);
        MF.repository.ajaxPost(this.options.PayTrainerUrl,data).done($.proxy(this.paymentCallback,this));
    },
    paymentCallback:function(result){
        this.options.notificationArea = new cc.NotificationArea(this.cid,"#errorMessagesGrid","#errorMessagesForm", MF.vent);
        this.options.notificationArea.render(this.$el);
        MF.vent.bind(this.options.notificationArea.areaName()+":"+this.id+":success",this.paymentSuccess,this);
        this.formCancel();
        MF.notificationService.addArea(this.options.notificationArea);
        MF.notificationService.resetArea(this.options.notificationArea.areaName());
        MF.notificationService.processResult(result,this.options.notificationArea.areaName(),this.id);
    },
    paymentSuccess:function(result){
        this.reloadGrid();
        $(this.el).find(".paymentAmount").data().total ={amount:0,items:[]};
        $(this.el).find(".paymentAmount").text(0);
        window.open(result.Variable);
    },
    formCancel:function(){
        this.templatePopup.close();
    },
    handleSingleClick:function(e) {
        var checkbox = $(e.currentTarget).find(".cbox");
        if(!checkbox||checkbox.attr("disabled")){return;}

        var id = $(e.currentTarget).attr("id");
        var data = this.options.$gridContainer.jqGrid('getRowData', id);
        var $span = $(this.el).find(".paymentAmount");
        var itemAmount = parseFloat($(data.TrainerPay).text());

        if (checkbox.is(":checked")) {
            $span.data().total.amount = $span.data().total.amount + itemAmount;
            $span.data().total.items.push({id:id,amount:itemAmount});
        } else {
            $span.data().total.amount = $span.data().total.amount - itemAmount;
            $span.data().total.items = $.map($span.data().total.items, function (a) {
                return (a.id != id ? a : null);
            });
        }
        $span.text($span.data().total.amount);
    },
    handleSelectAllClick:function(e){
        if($(e.target).closest("tr").attr("role")!="rowheader"){return;}
        var $span = $(this.el).find(".paymentAmount");
        $span.data().total ={amount:0,items:[]};
        if($(e.currentTarget).is(":checked")){
            var ids = cc.gridHelper.getCheckedBoxes(this.options.$gridContainer);
            $.each(ids,function(i,id){
                var data = this.options.$gridContainer.jqGrid('getRowData', id);
                var itemAmount = parseFloat($(data.TrainerPay).text());
                if(itemAmount>0){
                    $span.data().total.amount = $span.data().total.amount + itemAmount;
                    $span.data().total.items.push({id:id,amount:itemAmount});
                }
            });
        }
        $span.text($span.data().total.amount);
    }
});

MF.Views.TrainerPaymentListGridView = MF.Views.GridView.extend({
    events:_.extend({
         'click .return':'retunToParent'
    }, MF.Views.GridView.prototype.events),
    retunToParent:function(){
        MF.WorkflowManager.returnParentView(null,true);
    },
    displayItem:function(id){
        var parentId = this.$el.find("#EntityId").val();
        window.open("/Billing/PayTrainer/TrainerReceipt/"+id+"?ParentId="+parentId);
        return false;
    },
    onClose:function(){
        MF.vent.unbind("AddUpdateItem");
        MF.vent.unbind("DisplayItem");
        this._super("onClose",arguments);
    }

});