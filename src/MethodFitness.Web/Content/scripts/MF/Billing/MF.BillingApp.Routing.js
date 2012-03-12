/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 2/26/12
 * Time: 11:22 AM
 * To change this template use File | Settings | File Templates.
 */
MF.Routing.BillingApp = (function(MF, Backbone){
    var BillingApp = {};

    // Router
    // ------
    BillingApp.Router = Backbone.Marionette.AppRouter.extend({
          appRoutes: {
              "billing/*path/:entityId/:parentId": "scheduleViews",
              "billing/*path/:entityId": "scheduleViews",
              "billing/*path": "scheduleViews",
          }
      });

    // Initialize the router when the application starts
    MF.addInitializer(function(){
        BillingApp.router = new BillingApp.Router({
            controller: MF.Controller
        });
    });

    return BillingApp;
})(MF, Backbone);
