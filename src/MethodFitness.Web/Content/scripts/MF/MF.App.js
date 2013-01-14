/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 2/26/12
 * Time: 10:53 AM
 * To change this template use File | Settings | File Templates.
 */

var MF = new Backbone.Marionette.Application();


MF.addRegions({
    header: "#main-header",
    content:"#contentInner",
    menu:"#left-navigation"
});

  // an initializer to run this functional area
  // when the app starts up
MF.addInitializer(function(){
    $("#ajaxLoading").hide();

    MF.vent.bind("route",function(route,triggerRoute){
        MF.Routing.showRoute(route,triggerRoute);
    });

//    CC.notification = new CC.NotificationService();
//    CC.notification.render($("#successMessageContainer").get(0), $("#errorMessageContainer").get(0));
    Backbone.Marionette.TemplateCache.prototype.loadTemplate = function(templateId){
        return MF.repository.ajaxGet(this.url, this.data);
    };

    // overriding compileTemplate with passthrough function because we are not compiling
    Backbone.Marionette.TemplateCache.prototype.compileTemplate = function(rawTemplate){ return rawTemplate;};

});

MF.bind("initialize:after", function(){
  if (Backbone.history){
    Backbone.history.start();
  }
});

MF.generateRoute = function(route,_entityId,_parentId,_rootId,_var){
    var rel = MF.State.get("Relationships");
    var entityId = _entityId?_entityId:0;
    var parentId = _parentId && _parentId>0 ?_parentId:rel.parentId;
    var rootId = _rootId && _rootId>0?_rootId:rel.rootId;
    var variable = _var?"/"+_var:"";
    return route+"/"+ entityId+ "/"+ parentId+ "/"+rootId+variable;
};

// calling start will run all of the initializers
// this can be done from your JS file directly, or
// from a script block in your HTML
$(function(){
  MF.start();
});