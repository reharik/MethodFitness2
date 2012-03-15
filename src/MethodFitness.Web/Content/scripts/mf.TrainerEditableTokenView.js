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

mf.TrainerEditableTokenView = mf.EditableTokenView.extend({
     events:_.extend({
        'click .tokenEditor' : 'tokenEditor'
    }, mf.TokenView.prototype.events),
    internalTokenMarkup: function(item) {
        var cssClass = "class='selectedItem'";
        return "<p><a " + cssClass + ">" + item.name+" ( "+item.percentage + " )</a><a href='javascript:void(0);' class='tokenEditor' >&nbsp;-- Edit</a><input id='itemId' type='hidden' value='"+item.id+"' </p>";
    },
    postSetup:function(){
        $.subscribe("/contentLevel/popup_"+this.id+"/save", $.proxy(this.tokenSave,this));
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
    tokenEditor:function(e){
        this.options.currentlyEditing = $(e.target).prev("a");
        var id = $(e.target).next("input#itemId").val();
        var data = $(this.options.inputSelector,this.el).data("selectedItems");
        var dataItem;
        $.each(data,function(i,item){
            if(item.id == id) dataItem = item;
        });
        var buttons = this.options.buttons?this.options.buttons:mf.popupButtonBuilder.builder(this.id).standardEditButons();
        $("#masterArea").after("<div id='dialogHolder'/>");
        var popupOptions = {
            id:this.id,
            el:$("#dialogHolder"),
            buttons: buttons,
            data:dataItem,
            template:"#percentageTemplate"
        };
        this.popupView = new mf.TemplatedPopupView(popupOptions);
    },
    tokenSave:function(){
        var id = $("#editingId").val();
        var data = $(this.options.inputSelector,this.el).data("selectedItems");
        var dataItem;
        $.each(data,function(i,item){
            if(item.id == id) dataItem = item;
        });
        dataItem.percentage = $("#newTrainerPercentage").val();
        var anchor = $(this.options.currentlyEditing).text();
        var newText = anchor.substr(0,anchor.indexOf('(')) +"( "+$("#newTrainerPercentage").val()+" ) ";
        $(this.options.currentlyEditing).text(newText);
        this.popupView.remove();
    }

});
