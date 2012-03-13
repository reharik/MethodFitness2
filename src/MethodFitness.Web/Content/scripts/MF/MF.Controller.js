/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 3/11/12
 * Time: 6:48 PM
 * To change this template use File | Settings | File Templates.
 */

MF.Controller = (function(MF, Backbone){
    var Controller = {};

       Controller.showViews=function(splat,entityId, parentId){
           var routeToken = _.find(MF.routeTokens,function(item){
               return item.route == splat;
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
               MF.State.set({"currentView":item});
               MF.content.show(item);
           }
       };

       MF.addInitializer(function(){
           MF.ScheduleApp.Menu.show();
       });

       return Controller;
   })(MF, Backbone);
