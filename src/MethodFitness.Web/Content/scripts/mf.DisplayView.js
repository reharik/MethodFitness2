/**
 * Created by JetBrains RubyMine.
 * User: RHarik
 * Date: 10/7/11
 * Time: 3:17 PM
 * To change this template use File | Settings | File Templates.
 */


mf.PortfolioLandingPageView = Backbone.View.extend({
    events:{
        'click #buildNewPortfolio': "buildNewPortfolio"
    },
    initialize:function(){
        this.render();
    },
    render:function(){
        $(this.el).show();
    },

    buildNewPortfolio: function(){
        $.publish("/contentLevel/grid/AddUpdateItem",[this.options.addEditUrl]);
    }

});

mf.AjaxDisplayView = Backbone.View.extend({
    events:{
        'click .cancel' : 'cancel'
    },
    initialize: function(){
        this.options = $.extend({},mf.displayDefaults,this.options);
        this.id=this.options.id;
    },
    render:function(){
        mf.repository.ajaxGet(this.options.url, this.options.data, $.proxy(this.renderCallback,this));
    },
    renderCallback:function(result){
        if(result.LoggedOut){
            window.location.replace(result.RedirectUrl);
            return;
        }
        $(this.el).html(result);
        if(extraFormOptions){
            $.extend(true,this.options, extraFormOptions);
        }
        if(typeof this.options.runAfterRenderFunction == 'function'){
            this.options.runAfterRenderFunction.apply(this,[this.el]);
        }
        $.publish("/contentLevel/display_"+this.id+"/pageLoaded",[this.options]);
    },
    cancel:function(){
        $.publish("/contentLevel/display_"+this.id+"/cancel",[this.id]);
    }
});

mf.AddToPortfolioView = mf.AjaxDisplayView.extend({
    events:_.extend({
        'click .portfolioClick' : 'portfolioClick'
    }, mf.AjaxDisplayView.prototype.events),

    portfolioClick:function(e){
        $.publish("/contentLevel/display_"+this.id+"/portfolioChosen",[$(e.currentTarget).attr("id")]);
    }

});


mf.displayDefaults = {
    id:"",
    data:{},
    runAfterRenderFunction: null
};