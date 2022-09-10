/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 2/26/12
 * Time: 10:55 AM
 * To change this template use File | Settings | File Templates.
 */
MF.WorkflowManager = (function(MF, Backbone){
    var WFM =  Backbone.Model.extend({
        defaults: {
            parentStack: []
        },
        addChildView:function(child){
            var parent = MF.State.get("currentView");
            if(!parent)return null;
            MF.State.set({"currentView":child});
            MF.State.set({"childView":child});
            var stack =  this.get("parentStack");
            stack.push(parent);
            $.when(child.render()).then(function () {
                $(parent.el).after(child.el);
            });
            $(parent.el).hide();
            return child;
        },
        returnParentView:function(result, triggerCallback){
            var stack =  this.get("parentStack");
            var parent = stack.pop();
            MF.State.get("currentView").close();
            if(!parent){return;}
            if(triggerCallback&&parent.callbackAction){
                parent.callbackAction(result);
            }
            $(parent.el).show();
            MF.vent.trigger("route",parent.options.route,false);
            MF.State.set({"currentView":parent});
        },
        loadBottomLevel:function(url){
            var last = _.last(this.get("parentStack"));
            if(last && last.options.url == url){
                this.returnParentView("",false);
                return false;
            }
            return true;
        },

        cleanAllViews:function(){
            var currentView = MF.State.get("currentView");
            if(currentView){currentView.close();}
            var stack =  this.get("parentStack");
            while (stack.length>0){
                stack.pop().close();
            }
        }
    });
    return new WFM();

})(MF, Backbone);

