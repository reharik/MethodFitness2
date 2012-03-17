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
    },
    render:function(){
        $(".ui-dialog").remove();
        var errorMessages = $("div[id*='errorMessages']", this.el);
        if(errorMessages){
            var id = errorMessages.attr("id");
            errorMessages.attr("id","errorMessagesPU").removeClass(id).addClass("errorMessagesPU");
        }
        $(this.el).append($(this.options.template).tmpl(this.options.data));
        $(this.el).dialog({
            modal: true,
            width: this.options.width||550,
            buttons:this.options.buttons,
            title: this.options.title,
            close:function(){
                 MF.vent.trigger("popup:"+id+":cancel");
            }
        });
        return this;
    }
});

