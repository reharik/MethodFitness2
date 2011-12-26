/**
 * Created by .
 * User: Harik
 * Date: 11/27/11
 * Time: 2:29 PM
 * To change this template use File | Settings | File Templates.
 */


mf.TrainerController = mf.CrudController.extend({
    events:_.extend({
    }, mf.CrudController.prototype.events),

    registerAdditionalSubscriptions:function(){
        $.subscribe('/contentLevel/form_mainForm/pageLoaded', $.proxy(this.loadTokenizers,this), this.cid);
        $.subscribe('/contentLevel/form_mainForm/pageLoaded', $.proxy(this.loadPlugins,this), this.cid);
    },

    loadTokenizers: function(formOptions){
        var clientOptions = $.extend({el:"#clients"},formOptions.clientOptions);
        var userRoleOptions = $.extend({el:"#userRoles"},formOptions.userRolesOptions);
        this.views.userRoles = new mf.TokenView(userRoleOptions);
        this.views.clients = new mf.TokenView(clientOptions);
    },
    loadPlugins:function(){
        $('#color',"#detailArea").miniColors();
    }
});
