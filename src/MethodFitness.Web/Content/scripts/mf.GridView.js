/**
 * Created by JetBrains RubyMine.
 * User: RHarik
 * Date: 10/6/11
 * Time: 3:24 PM
 * To change this template use File | Settings | File Templates.
 */
if (typeof mf == "undefined") {
    var mf = {};
}


mf.GridView = Backbone.View.extend({
    events:{
        'click .new' : 'addNew',
        'click .delete' : 'deleteItems'
    },
    initialize: function(){
        this.options = $.extend({},mf.gridDefaults,this.options);
        this.id=this.options.id;
        this.render();
    },
    render: function(){
        $("#gridContainer",this.el).AsGrid(this.options.gridDef, this.options.gridOptions);
        $(window).bind('resize', function() { cc.gridHelper.adjustSize("#gridContainer"); }).trigger('resize');
        $(this.el).gridSearch({onClear:$.proxy(this.removeSearch,this),onSubmit:$.proxy(this.search,this)});
        return this;
    },
    addNew:function(){
        $.publish('/contentLevel/grid/AddEditItem', [this.options.addEditUrl]);
    },
    deleteItems:function(){
        if (confirm("Are you sure you would like to delete this Item?")) {
            var ids = cc.gridMultiSelect.getCheckedBoxes();
            mf.repository.ajaxGet(this.options.deleteMultipleUrl,
                $.param({"EntityIds":ids},true),
                $.proxy(function(){this.reloadGrid()},this));
        }
    },

    search:function(v){
        var searchItem = {"field": this.options.searchField ,"data": v };
        var filter = {"group":"AND",rules:[searchItem]};
        var obj = {"filters":""  + JSON.stringify(filter) + ""};
        $("#gridContainer").jqGrid('setGridParam',{postData:obj});
        this.reloadGrid();
    },
    removeSearch:function(){
        delete $("#gridContainer").jqGrid('getGridParam' ,'postData')["filters"];
        this.reloadGrid();
        return false;
    },
    reloadGrid:function(){
        $("#gridContainer").trigger("reloadGrid");
    }
});


mf.AssetGridView = mf.GridView.extend({
    events:_.extend({
       'click .copy' : 'copyItems',
       'click .addToPortfolio' : 'addToPortfolio'
    }, mf.GridView.prototype.events),
    copyItems:function(){
        var ids = cc.gridMultiSelect.getCheckedBoxes();
        mf.repository.ajaxGet(this.options.copyItemsUrl,$.param({"EntityIds":ids},true),$.proxy(function(){this.reloadGrid()},this));
    },
    addToPortfolio:function(){
        $.publish('/contentLevel/grid/addToPortfolio', []);
    },
    handleAddToPortfolio:function(portfolioId){
            var ids = cc.gridMultiSelect.getCheckedBoxes();
            mf.repository.ajaxGet(this.options.handleAddItemsToPortfolioUrl,$.param({"EntityIds":ids,"EntityId":portfolioId},true),$.proxy(this.handleAddToPortfolioCallback,this));
        },
    handleAddToPortfolioCallback:function(result){
        $.subscribe("/pageLoaded", $.proxy(function(){
            $.publish('/contentLevel/grid/AddEditItem', [this.options.addEditPortfolioUrl +"/"+result.EntityId]);
            $.unsubscribe("/pageLoaded")
        },this));
        $.publish("/contentLevel/popup_addToPortfolio/cancel",[]);
        $.address.value(this.options.portfolioListUrl);
    }
});

mf.gridDefaults = {
    searchField:"Name",
    showSearch:true
};