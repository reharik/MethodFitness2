/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 3/11/12
 * Time: 6:48 PM
 * To change this template use File | Settings | File Templates.
 */

MF.Controller = (function(MF, Backbone){
    var Controller = {};

       Controller.showViews=function(splat,entityId, parentId,rootId,_var) {
           var routeToken = _.find(MF.routeTokens,function(item){
               return item.route == splat;
           });
           if(!routeToken)return;
           // this is so you don't set the id to the routetoken which stays in scope
           var viewOptions = $.extend({},routeToken);
           viewOptions.templateUrl = viewOptions.url+"_Template";
            if (entityId) {
              viewOptions.url += "/" + entityId;
              viewOptions.route += "/" + entityId;
            }
            if (parentId) {
              viewOptions.url += "?ParentId=" + parentId;
              viewOptions.route += "/" + parentId;
            }
            if (rootId) {
              viewOptions.url += "&RootId=" + rootId;
              viewOptions.route += "/" + rootId;
            }
            if (_var) {
              viewOptions.url += "&Var=" + _var;
              viewOptions.route += "/" + _var;
            }
            MF.State.set({"Relationships":
            {
              "entityId":entityId ? entityId : 0,
              "parentId":parentId ? parentId : 0,
              "rootId":rootId ? rootId : 0,
              "extraVar":_var ? _var : ""
            }
            });
            var item;
            if(routeToken.itemName && MF.Views[routeToken.itemName]){
              item = new MF.Views[routeToken.itemName](viewOptions);
            }else{
              item = new MF.Views[routeToken.viewName](viewOptions);
            }


           if(routeToken.isChild){
               var hasParent = MF.WorkflowManager.addChildView(item);
               if(!hasParent){
                   MF.WorkflowManager.cleanAllViews();
                   MF.State.set({"currentView":item});
                   MF.content.show(item);
               }
           }else{
               MF.WorkflowManager.cleanAllViews();
               MF.State.set({"currentView":item});
               MF.content.show(item);
           }
       };

       MF.addInitializer(function(){
           MF.ScheduleApp.Menu.show();
       });

       return Controller;
   })(MF, Backbone);
