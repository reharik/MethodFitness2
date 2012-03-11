/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 2/26/12
 * Time: 10:54 AM
 * To change this template use File | Settings | File Templates.
 */


MF.State = (function(MF, Backbone){
    var State =  Backbone.Model.extend({
        defaults: {
            parentStack: []
        },
        application:"",
        currentView:""
    });
    return new State();
})(MF, Backbone);



