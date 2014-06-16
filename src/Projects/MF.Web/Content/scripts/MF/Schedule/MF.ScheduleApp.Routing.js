/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 2/26/12
 * Time: 11:22 AM
 * To change this template use File | Settings | File Templates.
 */
MF.Routing.ScheduleApp = (function(MF, Backbone){
    var ScheduleApp = {};

    // Router
    // ------
    ScheduleApp.Router = Backbone.Marionette.AppRouter.extend({
          appRoutes: {
              "*path/:entityId/:parentId/:rootId/:string": "showViews",
              "*path/:entityId/:parentId/:rootId": "showViews",
              "*path/:entityId/:parentId": "showViews",
              "*path/:entityId": "showViews",
              "*path": "showViews"
          }
      });

    // Initialization
    // --------------

    // Initialize the router when the application starts
    MF.addInitializer(function(){
        MF.ScheduleApp.router = new MF.Routing.ScheduleApp.Router({
            controller: MF.Controller
        });
    });

    return ScheduleApp;
})(MF, Backbone);
