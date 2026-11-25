if (typeof cc == 'undefined') {
    cc = function() {
    }
}

if (typeof cc.grid == 'undefined') {
    cc.grid = function() {
    }
}

(function($) {
   $.fn.AsGrid = function(gridDefinition, userOptions) {
        if(!this)return;
        var colModel = cc.grid.columnService.columnModel(gridDefinition);
        // Phones show lists in larger type (mobile.css), so each column gets more room.
        if (MF.isPhone()) {
            $.each(colModel, function(i, col) {
                col.width = Math.round((parseInt(col.width, 10) || 100) * 1.35);
            });
        }
        var gridDefaultOptions = {
            url: gridDefinition.Url,
            datatype: 'json',
            mtype: 'GET',
            colNames: cc.grid.columnService.columnNames(gridDefinition),
            colModel: colModel,
            rowNum: 10000,
            scroll:false,
            altRows:true,
//            height:"100%",
            width:"100%",
            autowidth:true,
            // On a phone the columns keep a readable width and the list scrolls sideways,
            // rather than every column being squeezed into the screen.
            shrinkToFit: !MF.isPhone(),
            multiselect: true,
            // here we move the value of the entityId to ParentId since it's the Id of the containing element
            // and set EntityId to RowId as RowId is for the Particular entity we want to change
            beforeSubmitCell: function(rowid, celname, value, iRow, iCol) { return { RootId: rootId, ParentId: entityId, EntityId: rowid, rowId: rowid, cellName: celname, cellValue: value} },
            afterSubmitCell: function(serverresponse, rowid, cellname, value, iRow, iCol) {
                eval("var cellEditResult =" + serverresponse.responseText);
                return [cellEditResult.success, cellEditResult.message];
            },
            loadtext:"",
            //emptyrecords:"aint go nothin",
            gridComplete:function(){
                $(this).find(".cbox").parent().addClass("jqg_cb");
                $("div.form-scroll-inner").height( window.innerHeight-180);
                $(this).setGridHeight($("div.form-scroll-inner").height()-36  );
                MF.vent.trigger("gridComplete");
            },
            sortorder: "asc",
            sortname:gridDefinition.DefaultSortColumn ? gridDefinition.DefaultSortColumn :"", //  cc.grid.columnService.defaultSortColumnName(gridDefinition),
            onSortCol:function(index,iCol,sortorder) {cc.gridHelper.adjustSortStyles(index,iCol,sortorder);},
            jsonReader: {
                repeatitems: true,
                root: "items",
                cell: "cell",
                id: "id"
            }
        };

        if (gridDefinition.EditUrl) {
            $.extend(gridDefaultOptions, { cellEdit: true, cellurl: gridDefinition.EditUrl} || {});
        }

        cc.gridHelper.adjustSortStyles();

        var gridOptions = $.extend(gridDefaultOptions, userOptions || {});
        var grid = this;
        grid.jqGrid(gridOptions);

        return grid;
    }
})(jQuery);
