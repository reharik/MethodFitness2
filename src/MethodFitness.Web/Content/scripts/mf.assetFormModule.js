/**
 * Created by JetBrains RubyMine.
 * User: RHarik
 * Date: 10/10/11
 * Time: 9:17 AM
 * To change this template use File | Settings | File Templates.
 */

if (typeof mf == "undefined") {
            var mf = {};
}

mf.AssetFormModule  = mf.FormModule.extend({
    events:_.extend({
    }, mf.FormModule.prototype.events),

    initialize:function(){
            $.extend(this,this.defaults());
            this.registerSubscriptions();
            this.views.formView = new mf.AssetFormView(this.options);
            this.views.formView.render();
        },

        registerAdditionalSubscriptions: function(){
            $.subscribe("/contentLevel/form_"+this.id+"/pageLoaded", $.proxy(this.setupTokenInputViews,this), this.cid);
            $.subscribe("/contentLevel/tokenizer_" + this.id + "/formCancel",$.proxy(this.formCancelTokenizer,this), this.cid);
        },
        setupTokenInputViews: function(formOptions){
            var tokenOptions = {
                el:"#documentTokenContainer",
                id:"Document",
                tokenizerUrls:formOptions.tokenizerUrls
            };
            $.extend(true, tokenOptions, formOptions.docOptions);
            this.modules.tokenizerModule = new mf.TokenizerModule(tokenOptions);
        },
        formCancelTokenizer:function(){
            this.modules.tokenizerModule.destroy();
            delete this.modules.tokenizerModule;
        }

});