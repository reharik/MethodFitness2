/**
 * Created by JetBrains RubyMine.
 * User: RHarik
 * Date: 10/7/11
 * Time: 10:13 AM
 * To change this template use File | Settings | File Templates.
 */
if (typeof mf == "undefined") {
    var mf = {};
}

mf.TemplatedPopupView = Backbone.View.extend({
    
    initialize: function(){
        this.options = $.extend({},mf.popupDefaults,this.options);
        this.id=this.options.id;
        $(".ui-dialog").remove();
        this.render()
    },
    render:function(){
        var that = this;
        $(this.el).append($(this.options.template).tmpl(this.options.data));
        $(this.el).dialog({
            modal: true,
            width: this.options.width||550,
            buttons:this.options.buttons,
            title: this.options.title,
            close:function(){
                $.publish("/contentLevel/popup_"+that.id+"/cancel",[]);
                $(".ui-dialog").remove();
            }
        });
        return this;
    }
});

