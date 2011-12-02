/**
 * Created by .
 * User: Harik
 * Date: 11/27/11
 * Time: 2:29 PM
 * To change this template use File | Settings | File Templates.
 */

var mf = mf || {};

mf.TrainerController = mf.CrudController.extend({
    events:_.extend({
    }, mf.CrudController.prototype.events),

    additionalSubscriptions:function(){
        $.subscribe('/form_editModule/pageLoaded', $.proxy(this.loadTokenizers,this), this.cid);
    },

    loadTokenizers: function(formOptions){
        var options = $.extend({},formOptions,{el:"#dialogHolder"});
        this.views.roles = new kyt.TokenView(formOptions.rolesOptions);
    }
});
