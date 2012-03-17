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

mf.DisplayModule  = mf.Module.extend({
    events:_.extend({
    }, mf.Module.prototype.events),

    initialize:function(){
            $.extend(this,this.defaults());

            this.registerSubscriptions();
            this.views.displayView = this.options.displayViewName ? new mf[this.options.displayViewName](this.options): new mf.AjaxDisplayView(this.options);
            this.views.displayView.render();
        },

        registerSubscriptions: function(){
            $.subscribe("/contentLevel/display_"+this.id+"/cancel", $.proxy(this.formCancel,this), this.cid);
            this.registerAdditionalSubscriptions();
        },
        registerAdditionalSubscriptions:function(){},
        formCancel: function(){
            this.views.displayView.remove();
            $.publish("/contentLevel/displayModule_"+this.id+"/moduleCancel",[this.id]);
        }
});