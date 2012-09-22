/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 9/2/12
 * Time: 10:57 AM
 * To change this template use File | Settings | File Templates.
 */


MF.Views.ResetPasswordView = MF.Views.View.extend({
    initialize: function(){
        MF.mixin(this, "baseFormView");
    },
    render: function(){
        MF.notificationService = new cc.MessageNotficationService();
        this.bindModelAndElements();
        if(this.setBindings){this.setBindings();}
        $("input[name='Password']").focus();
        this.viewLoaded();
        MF.vent.trigger("form:"+this.id+":pageLoaded",this.options);
        return this;
    }
});

MF.Views.LoginView = MF.Views.View.extend({
    registerEvents: function(){ this.events ={
        "click #forgotPasswordLink": "forgotPasswordClick",
        "click .save": "submitClick"
    }},
    initialize: function(){
        MF.mixin(this, "formMixin");
        MF.mixin(this, "modelAndElementsMixin");
        this.registerEvents();
    },
    render: function () {
        this.bindModelAndElements();
        if (this.setBindings) { this.setBindings(); }
        $("input[name='UserName']").focus();
        this.viewLoaded();
        MF.vent.trigger("form:" + this.id + ":pageLoaded", this.options);

        return this;
    },

    forgotPasswordClick: function (e) {
        e.preventDefault();
        var popupOptions = {
            url: this.options.forgotPasswordUrl,
            popupTitle: this.options.popupTitle
        };
        var forgottenPasswordView = new MF.Views.AjaxPopupFormModule(popupOptions);
        forgottenPasswordView.render();

    },

    submitClick: function (e) {
        var isValid = CC.ValidationRunner.runViewModel(this.elementsViewmodel);
        if(!isValid){return;}
        var data = JSON.stringify(ko.mapping.toJS(this.model));
        var promise = MF.repository.ajaxPostModel(this.model._saveUrl(),data);
        promise.done(this.success);
    },
    success:function(result){
        if(result.Success){
            window.location.href = result.RedirectUrl;
        }else{
            CC.notification.handleResult(result);
        }

    }

});
