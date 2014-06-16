/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 2/26/12
 * Time: 11:19 AM
 * To change this template use File | Settings | File Templates.
 */

MF.ScheduleApp.Menu = (function(MF, Backbone, $){
    var Menu = {};

    Menu.show = function(){
        var routeToken = _.find(MF.routeTokens,function(item){
            return item.id == "scheduleMenu";
        });
        var view = new MenuView(routeToken);
        MF.menu.show(view);
        $("#left-navigation").show();
    };

    var MenuView =  MF.Views.View.extend({
        render:function(){
            MF.repository.ajaxGet(this.options.url, this.options.data).done($.proxy(this.renderCallback,this));
        },
        renderCallback:function(result){
            if(result.LoggedOut){
                window.location.replace(result.RedirectUrl);
                return;
            }
            $(this.el).html(result);
            MF.vent.bind("menuItem", this.menuItemClick,this);
            $(this.el).find(".ccMenu").ccMenu({ backLink: false, width : 220 });
            return this;
        },
        menuItemClick:function(name){
            MF.vent.trigger("route",name,true);
        },
        onClose:function(){
            MF.vent.unbind("menuItem", this.menuItemClick,this);
        }
    });

    return Menu;
})(MF, Backbone, jQuery);
