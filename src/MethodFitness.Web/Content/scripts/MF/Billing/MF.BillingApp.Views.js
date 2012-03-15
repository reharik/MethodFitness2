/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 2/26/12
 * Time: 11:24 AM
 * To change this template use File | Settings | File Templates.
 */

MF.Views.TrainerPaymentGridView = MF.Views.GridView.extend({
    onPreRender:function(){
        this.options.gridOptions={loadComplete : function(){
            var ids = $("#gridContainer").jqGrid('getDataIDs');
            for (var i = 0, l = ids.length; i < l; i++) {
                var rowid = ids[i];
                var rowData =$("#gridContainer").jqGrid('getRowData',ids[i]);
                if (parseInt($(rowData.TrainerPay).text())<=0) {
                    var row = $('#' + rowid, this.el);
                    row.addClass('gridRowStrikeThrough');
                   // row.find("td").addClass('gridRowStrikeThrough');
                    row.find("td:first input").hide();
                }
            }
        }}
    },
    viewLoaded:function(){
        MF.vent.bind("Redirect",this.showPayGrid,this);
        $(this.el).find(".content-header").append("<button class='delete'></button>");
    },
    showPayGrid:function(id){
        MF.vent.trigger("route","trainerpaymentlist/"+id,true);
    }
});