/**
 * Created by JetBrains RubyMine.
 * User: RHarik
 * Date: 10/6/11
 * Time: 4:52 PM
 * To change this template use File | Settings | File Templates.
 */
if (typeof mf == "undefined") {
            var mf = {};
}


mf.ClientController  = mf.CrudController.extend({
    events:_.extend({
    }, mf.CrudController.prototype.events),

    initialize:function(options){
        $.extend(this,this.defaults());
        mf.contentLevelControllers["ClientController"]=this;
        $.unsubscribeByPrefix("/contentLevel");
        this.id="clientController";
        this.registerSubscriptions();

        var _options = $.extend({},this.options, options);
        _options.el="#masterArea";
        this.views.gridView = new mf.GridView(_options);
    },

    registerAdditionalSubscriptions: function(){
        $.subscribe('/contentLevel/formModule_paymentForm/moduleSuccess',$.proxy(this.paymentModuleSuccess,this),this.cid);
        $.subscribe('/contentLevel/formModule_paymentForm/moduleCancel',$.proxy(this.paymentModuleCancel,this), this.cid);
        $.subscribe('/contentLevel/form_mainForm/payment',$.proxy(this.payment,this), this.cid);
        $.subscribe("/contentLevel/form_mainForm/paymentPageLoaded", $.proxy(this.paymentFormLoaded,this),this.cid);
    },

    addPayment: function(url){
            var formOptions = {
                el: "#paymentArea",
                id: "paymentForm",
                url: url
            };
            $("#masterArea","#contentInner").after("<div id='paymentArea'/>");
            this.modules.paymnentFormModule = new mf.FormModule(formOptions);
        },

    payment:function(){
        var id = $("#EntityId","#detailArea").val();
        this.addPayment(this.options.paymentUrl+"?ParentId="+id);
    },
    paymentFormLoaded:function(){
        $("#detailArea").hide();
    },

    paymentModuleSuccess:function(){
       this.paymentModuleCancel();
        //reload payments if they are shown on client
    },
    paymentModuleCancel: function(){
       this.modules.paymnentFormModule.destroy();
       $("#detailArea").show();
    }
});
