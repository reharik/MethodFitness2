/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 8/31/12
 * Time: 11:30 PM
 * To change this template use File | Settings | File Templates.
 */
MF.loadTemplateAndModel = function(view){
    var d = new $.Deferred();
    $.when(Backbone.Marionette.TemplateCache.get(view.options.route,view.options.templateUrl, view.options.data),
        MF.repository.ajaxGet(view.options.url, view.options.data))
    .then(function(html,data){
            view.$el.html(html);
            view.rawModel = data[0];
                d.resolve();
        });
    return d.promise();
};
