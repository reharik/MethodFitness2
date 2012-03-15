/**
 * Created by JetBrains RubyMine.
 * User: RHarik
 * Date: 10/7/11
 * Time: 9:45 AM
 * To change this template use File | Settings | File Templates.
 */
if (typeof mf == "undefined") {
    var mf = {};
}

mf.EditableTokenView = mf.TokenView.extend({
     events:_.extend({
        'click .tokenEditor' : 'tokenEditor'
    }, mf.TokenView.prototype.events),
    internalTokenMarkup: function(item) {
        var cssClass = "class='selectedItem'";
        return "<p><a " + cssClass + ">" + item.name + "</a><a href='javascript:void(0);' class='tokenEditor' > -- Edit</a> </p>";
    },
    afterTokenSelectedFunction:function(item) {
        if(!$(this.options.inputSelector,this.el).data("selectedItems"))$(this.options.inputSelector,this.el).data("selectedItems",[]);
        $(this.options.inputSelector,this.el).data("selectedItems").push(item);
    },
    deleteToken:function(hidden_input,token_data) {
        var data = $(this.options.inputSelector,this.el).data("selectedItems");
        var idx=0;
        $.each(data,function(i,item){
            if(item.id == hidden_input.id){
                idx=i;
            }
        });
        data.splice(idx,1);
    },
    tokenEditor:function(e){}
});
