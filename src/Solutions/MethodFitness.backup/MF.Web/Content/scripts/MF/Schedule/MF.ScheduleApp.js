/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 2/26/12
 * Time: 11:14 AM
 * To change this template use File | Settings | File Templates.
 */

MF.ScheduleApp = (function(MF, Backbone){
    var ScheduleApp = {};

    //show user settings and hide the menu
    ScheduleApp.show = function(){
        ScheduleApp.Menu.show();
        MF.vent.trigger("route", "calendar",true);
    };

    MF.State.bind("change:application", function(e,f,g){
        if(MF.State.get("application")=="scheduler") ScheduleApp.show()});
    MF.addInitializer(function(){
    });

    return ScheduleApp;
})(MF, Backbone);
