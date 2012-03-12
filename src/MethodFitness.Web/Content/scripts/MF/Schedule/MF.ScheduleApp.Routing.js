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
              "schedule/*path/:entityId/:parentId": "scheduleViews",
              "schedule/*path/:entityId": "scheduleViews",
              "schedule/*path": "scheduleViews",
              "":"show"
          }
      });

    MF.vent.bind("route",function(route,triggerRoute){
        MF.Routing.showRoute(route,triggerRoute);
    });

    MF.vent.bind("home", function(){
        MF.Routing.showRoute("home");
    });


    // Initialization
    // --------------

    // Initialize the router when the application starts
    MF.addInitializer(function(){
        ScheduleApp.router = new ScheduleApp.Router({
            controller: MF.Controller
        });
    });

    return ScheduleApp;
})(MF, Backbone);
