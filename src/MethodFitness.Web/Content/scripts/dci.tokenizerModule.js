/**
 * Created by JetBrains RubyMine.
 * User: RHarik
 * Date: 10/7/11
 * Time: 1:07 PM
 * To change this template use File | Settings | File Templates.
 */

if (typeof mf == "undefined") {
            var mf = {};
}


mf.TokenizerModule  = mf.Module.extend({
    events:_.extend({
    }, mf.Controller.prototype.events),

    initialize:function(){
        $.extend(this,this.defaults());
        this.registerSubscriptions();
        this.views[this.id] = new mf.TokenView(this.options);
    },

    registerSubscriptions: function(){
        $.subscribe("/contentLevel/token_"+this.id+"/addEdit", $.proxy(this.addEditItem,this),this.cid);
        //
        $.subscribe("/contentLevel/ajaxPopupFormModule_" + this.id + "/formSuccess", $.proxy(this.formSuccess,this),this.cid);
        $.subscribe("/contentLevel/ajaxPopupFormModule_" + this.id + "/formCancel", $.proxy(this.formCancel,this),this.cid);

    },

    //from tolkneizer
    addEditItem:function(){
        var formOptions = {
            id:this.id,
            el: "#popupContentDiv",
            url: this.options.tokenizerUrls[this.id+"AddUpdateUrl"],
            crudFormOptions: { errorContainer:"#errorMessagesPU",successContainer:"#errorMessagesForm"}
        };
        $("#detailArea").after("<div id='popupContentDiv'/>");
        this.modules[this.id + "ajaxPopup"] = new mf.AjaxPopupFormModule(formOptions);
    },

    formSuccess:function(result){
        this.views[this.id].successHandler(result);
        this.formCancel();
    },
    formCancel:function(){
        $.publish("/contentLevel/tokenizer_" + this.id + "/formCancel",[this.id]);
        this.modules[this.id + "ajaxPopup"].destroy();
    }
});