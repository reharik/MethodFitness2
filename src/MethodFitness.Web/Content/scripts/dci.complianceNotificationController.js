/**
 * Created by .
 * User: RHarik
 * Date: 9/1/11
 * Time: 3:03 PM
 * To change this template use File | Settings | File Templates.
 */

if (typeof mf == "undefined") {
            var mf = {};
}

mf.ComplianceNotificationController  = mf.Controller.extend({
    events:_.extend({
    }, mf.Controller.prototype.events),

    initialize:function(){
        $.extend(this,this.defaults());

        mf.contentLevelControllers["complianceNotificationController"]=this;
        $.unsubscribeByPrefix("/contentLevel");
        this.registerSubscriptions();

        var formOptions = {
            el: "#masterArea",
            id: "complianceSettings"
        };
        this.views.formView = new mf.ComplianceItemsFormView(formOptions);
    },
    registerSubscriptions:function(){
        $.subscribe("/contentLevel/form_complianceSettings/pageLoaded", $.proxy(this.formLoaded,this));
    },
    formLoaded:function(){
        var emailTokenOptions = {
            el: "#emailTokenContainer",
            id: "emailToken",
            inputSelector:$(this.options.emailOptions.inputSelector),
            availableItems:this.options.emailOptions.availableItems,
            selectedItems:this.options.emailOptions.selectedItems
         };
        this.views.emailTokenView = new mf.TokenView(emailTokenOptions);


        var complianceItemTokenOptions = {
            el: "#complianceItemTokenContainer",
            id: "complianceItem",
            inputSelector:$(this.options.ciOptions.inputSelector),
            availableItems:this.options.ciOptions.availableItems,
            selectedItems:this.options.ciOptions.selectedItems
         };
        this.views.complianceItemTokenView = new mf.TokenView(complianceItemTokenOptions);
    }
});


//
//mf.complianceNotificationController = function(container, options){
//    var _container = $(container);
//    var myOptions = $.extend({}, mf.gridDetailDefaults, options || {});
//    var modules = {};
//
//    return{
//        init: function(){
//            var that = this;
//            mf.contentLevelControllers["complianceNotificationController"]=that;
//            var form = mf.formModule("#masterArea",{notAjax:true});
//            var cnm = mf.complianceNotificationModule("#masterArea");
//            var email = mf.tokenModule("#emailTokenContainer",emailOptions);
//            var complianceItems = mf.tokenModule("#complianceItemsTokenContainer",ciOptions);
//            modules.formModule= form;
//            modules.complianceModule= cnm;
//            modules.emailTM= email;
//            modules.complianceItemsTM= complianceItems;
//            $.each(modules,function(i,item){
//               item.init();
//            });
//        },
//        destroy:function(){
//            $.each(modules,function(item,value){
//               if(value){
//                   that.removeModule(item);
//               }
//            });
//            _container.empty();
//        },
//        addFormModule: function(module){
//            that.modules.formModule=module;
//        }
//    }
//};
//
//mf.gridDetailDefaults = {
//};
