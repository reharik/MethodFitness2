/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 2/26/12
 * Time: 11:24 AM
 * To change this template use File | Settings | File Templates.
 */

MF.Views.TrainerPaymentGridView = MF.Views.GridView.extend({
     events:_.extend({
        'change .cbox':'checkboxClick'

    }, MF.Views.GridView.prototype.events),
    onPreRender:function(){
        this.options.gridOptions={loadComplete : function(){
            var ids = $("#gridContainer").jqGrid('getDataIDs');
            for (var i = 0, l = ids.length; i < l; i++) {
                var rowid = ids[i];
                var rowData =$("#gridContainer").jqGrid('getRowData',ids[i]);
                if (parseInt($(rowData.TrainerPay).text())<=0) {
                    var row = $('#' + rowid, this.el);
                    row.addClass('gridRowStrikeThrough');
                   // row.find("td").addClass('gridRowStrikeThrough');
                    row.find("td:first input").hide();
                }
            }
        }}
    },
    viewLoaded:function(){
        $(this.el).find(".content-header").append("<button id='payTrainerButton' ></button>");
        $(this.el).find(".content-header > .title-name").append("<span class='paymentAmount'></span>");
        $(this.el).find(".paymentAmount").data().total ={amount:0,items:[]};

        MF.vent.bind("popup:payTrainerPopup:save",this.formSuccess,this);
        MF.vent.bind("popup:payTrainerPopup:cancel",this.formCancel,this);
        $("#payTrainerButton").click($.proxy(this.payTrainer, this));
    },
    payTrainer:function(){
        jQuery.ajaxSettings.traditional = true;
        var builder = MF.Views.popupButtonBuilder.builder("payTrainerPopup");
        builder.addButton("Ok", builder.getSaveFunc());
        builder.addCancelButton();
        var amount = $(this.el).find(".paymentAmount").data().total.amount;
        var data={trainersName:this.options.TrainersName,amount:amount};
        var formOptions = {
            id: "payTrainerPopup",
            data:data,
            template:"#payTrainerTemplate",
            buttons:builder.getButtons()
        };
        this.templatePopup = new MF.Views.TemplatedPopupView(formOptions);
        this.templatePopup.render();
        this.storeChild(this.templatePopup);
    },
    formSuccess:function(){
        var total = $(this.el).find(".paymentAmount").data().total;
        var arr = [];
        arr.push({"name":"EntityId", "value":this.options.EntityId});
        arr.push({"name":"PaymentDetailsDto.amount", "value":total.amount});
        $.each(total.items,function(i,item){
            arr.push({"name":"PaymentDetailsDto.items["+i+"].id","value":item.id});
            arr.push({"name":"PaymentDetailsDto.items["+i+"].amount","value":item.amount});
         });
        
        var data = $.param(arr);
        MF.repository.ajaxPost(this.options.PayTrainerUrl,data,$.proxy(this.paymentSuccess,this));
    },
    paymentSuccess:function(result){
        var notification = cc.utilities.messageHandling.notificationResult();
        notification.setErrorContainer('#errorMessagesGrid');
        notification.result(result);
        if(result.Success){
            window.open(result.Variable);
        this.reload();
        }
        this.formCancel();
    },
    formCancel:function(){
        this.templatePopup.close();
    },
    checkboxClick:function(e) {
        if($(e.target).closest("tr").attr("role")=="rowheader"){
            this.handleSelectAllClick(e);
        }else{
            this.handleSingleClick(e);
        }
    },
    handleSingleClick:function(e) {
        var id = $(e.target).closest("tr").attr("id");
        var data = $("#gridContainer").jqGrid('getRowData', id);
        var $span = $(this.el).find(".paymentAmount");
        var itemAmount = parseFloat($(data.TrainerPay).text());

        if (e.currentTarget.checked) {
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
        var $span = $(this.el).find(".paymentAmount");
        $span.data().total ={amount:0,items:[]};
        if(e.currentTarget.checked){
            var ids = cc.gridMultiSelect.getCheckedBoxes();
            $.each(ids,function(i,id){
                var data = $("#gridContainer").jqGrid('getRowData', id);
                var itemAmount = parseFloat($(data.TrainerPay).text());
                $span.data().total.amount = $span.data().total.amount + itemAmount;
                $span.data().total.items.push({id:id,amount:itemAmount});
            });
        }
        $span.text($span.data().total.amount);
    }
});