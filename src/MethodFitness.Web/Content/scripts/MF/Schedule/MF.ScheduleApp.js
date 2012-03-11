/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 2/26/12
 * Time: 11:14 AM
 * To change this template use File | Settings | File Templates.
 */

MF.ScheduleApp = (function(MF, Backbone){
    var ScheduleApp = {};

    ScheduleApp.scheduleViews=function(splat,entityId, parentId){
        var routeToken = _.find(MF.routeTokens,function(item){
            return item.routeSplat == splat;
        });
        if(!routeToken)return;
        // this is so you don't set the id to the routetoken which stays in scope
        var viewOptions = $.extend({},routeToken);
        if(entityId) viewOptions.url +="/"+entityId;
        if(parentId) viewOptions.url +="?ParentId="+parentId;

         var item = new MF.Views[routeToken.viewName](viewOptions);

        if(routeToken.isChild){
            var hasParent = MF.WorkflowManager.addChildView(item);
            if(!hasParent){
                MF.WorkflowManager.cleanAllViews();
                MF.State.set({"currentView":item});
                MF.content.show(item);
            }
        }else{
            MF.WorkflowManager.cleanAllViews();
//            MF.State.set({"application":"portfolio"});
            MF.State.set({"currentView":item});
            MF.content.show(item);
        }
//        if(MF.State.get("application")!="portfolio"){
//            MF.State.set({"application":"portfolio"});
//        }
    };
    //show user settings and hide the menu
    ScheduleApp.show = function(){
        ScheduleApp.Menu.show();
        MF.vent.trigger("route", "schedule/calendar",true);
    };

    MF.State.bind("change:application", function(e,f,g){
        if(MF.State.get("application")=="scheduler") ScheduleApp.show()});
    MF.addInitializer(function(){
        ScheduleApp.Menu.show();
    });

    return ScheduleApp;
})(MF, Backbone);
