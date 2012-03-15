/**
 * Created by JetBrains RubyMine.
 * User: RHarik
 * Date: 10/7/11
 * Time: 7:44 AM
 * To change this template use File | Settings | File Templates.
 */

if (typeof mf == "undefined") {
    var mf = {};
}

mf.BaseFormView = Backbone.View.extend({
    events:{
        'click #save' : 'saveItem',
        'click .cancel' : 'cancel',
        'click .delete' : 'deleteItem'
    },

    initialize: function(){
       this.options = $.extend({},mf.formDefaults,this.options);
       this.id=this.options.id;
    },
    config:function(){
        if(extraFormOptions){
            $.extend(true, this.options, extraFormOptions);
        }

        this.options.crudFormOptions.successHandler = $.proxy(this.successHandler,this);
        $(this.options.crudFormSelector,this.el).crudForm(this.options.crudFormOptions);
        if(this.options.crudFormOptions.additionBeforeSubmitFunc){
            var array = !$.isArray(this.options.crudFormOptions.additionBeforeSubmitFunc) ? [this.options.crudFormOptions.additionBeforeSubmitFunc] : this.options.crudFormOptions.additionBeforeSubmitFunc;
            $(array).each($.proxy(function(i,item){
                $(this.options.crudFormSelector,this.el).data('crudForm').setBeforeSubmitFuncs(item);
            },this));
        }
        $(".rte").cleditor();
    },
    saveItem:function(){
        $(this.options.crudFormSelector,this.el).submit();
    },
    cancel:function(){
        $.publish("/contentLevel/form_"+this.id+"/cancel",[this.id]);
    },
    deleteItem:function(){
        if (confirm("Are you sure you would like to delete this Item?")) {
            var id = $("#EntityId",this.el).val();

            mf.repository.ajaxGet(this.options.deleteUrl,
                $.param({"EntityId":id},true),
                $.proxy(function(result){
                    var notification = cc.utilities.messageHandling.notificationResult();
                    notification.setErrorContainer("#errorMessagesForm");
                    notification.setSuccessContainer("#errorMessagesGrid");
                    notification.result(result);
                    $.publish("/contentLevel/form_"+this.id+"/delete",[result]);
                },this));
        }
    },


    successHandler:function(result, form, notification){
        var emh = cc.utilities.messageHandling.messageHandler();
        var message = cc.utilities.messageHandling.mhMessage("success",result.Message,"");
        emh.addMessage(message);
        emh.showAllMessages(notification.getSuccessContainer());
        $.publish("/contentLevel/form_"+this.id+"/success",[result,form]);
    }
});

mf.FormView = mf.BaseFormView.extend({
    initialize: function(){
        this.options = $.extend({},mf.formDefaults,this.options);
        this.id=this.options.id;
        this.config();
        this.render();
    },
    render: function(){
        $.publish("/contentLevel/form_"+this.id+"/pageLoaded",[this.options]);
        return this;
    }
});
mf.AjaxFormView = mf.BaseFormView.extend({
    render:function(){
        mf.repository.ajaxGet(this.options.url, this.options.data, $.proxy(function(result){this.renderCallback(result)},this));
    },
    renderCallback:function(result){
        if(result.LoggedOut){
            window.location.replace(result.RedirectUrl);
            return;
        }
        $(this.el).html(result);
        this.config();
        //callback for render
        this.viewLoaded();
        //general notification of pageloaded
        $.publish("/contentLevel/form_"+this.id+"/pageLoaded",[this.options]);
    }
});

mf.ClientFormView = mf.AjaxFormView.extend({
    events:_.extend({
        'click .payment':'payment'
    }, mf.AjaxFormView.prototype.events),
    payment:function(){
        $.publish("/contentLevel/form_"+this.id+"/payment",[this.options.paymentListUrl]);
    }
});

mf.PaymentFormView = mf.AjaxFormView.extend({
    events:_.extend({
    },mf.AjaxFormView.prototype.events),
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

mf.formDefaults = {
    id:"",
    data:{},
    crudFormSelector:"#CRUDForm",
    crudFormOptions:{
        errorContainer:"#errorMessagesForm",
        successContainer:"#errorMessagesGrid",
        additionBeforeSubmitFunc:null
    },
    runAfterRenderFunction: null
};