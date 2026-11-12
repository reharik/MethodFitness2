/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 8/31/12
 * Time: 11:30 PM
 * To change this template use File | Settings | File Templates.
 */
MF.loadTemplateAndModel = async function (view) {
    const html = await Backbone.Marionette.TemplateCache.get(view.options.route, view.options.templateUrl, view.options.data);

    const data = await MF.repository.ajaxGet(view.options.url, view.options.data);
    view.$el.html(html);
    view.rawModel = data;
    return view
};
