/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 8/5/12
 * Time: 9:53 AM
 * To change this template use File | Settings | File Templates.
 */
if (typeof MF == "undefined") {
    var MF = {};
}

if (typeof CC == "undefined") {
    var CC = {};
}

MF.mixins = {};

MF.mixin = function(target, mixin, preserveRender){

    var mixinObj = MF.mixins[mixin];
    for (var prop in mixinObj) {
        if(prop=="render"){
            if(preserveRender!=true){
                target[prop] = mixinObj[prop];
            }
        } else if(target[prop]==null && mixinObj.hasOwnProperty(prop)){
            target[prop] = mixinObj[prop];
        }
    }
};

MF.mixins.modelAndElementsMixin = {
    bindModelAndElements:function(arrayOfIgnoreItems){
        // make sure to apply ids prior to ko mapping.
        var that = this;
        this.mappingOptions ={ ignore:[] };
        _.each(arrayOfIgnoreItems,function(item){ that.mappingOptions.ignore.push(item);});

        this.model = ko.mapping.fromJS(this.rawModel,this.mappingOptions);
        this.extendModel(this.model);
        ko.applyBindings(this.model,this.el);
        this.elementsViewmodel = CC.elementService.getElementsViewmodel(this);

        if(this.boundElementOptions && this.boundElementOptions.modifiers){
            _.each(this.boundElementOptions.modifiers,function(item){
                item(this.elementsViewmodel.collection);
            },this)
        }

        var ignore = _.filter(_.keys(this.model),function(item){
            return (item.indexOf('_') == 0 && item != "__ko_mapping__" );
        });
        _.each(ignore,function(item){
            that.mappingOptions.ignore.push(item);});
        this.renderElements();
        this.mappingOptions.ignore.push("_availableItems");
        this.mappingOptions.ignore.push("_resultsItems");
        this.mappingOptions.ignore = _.uniq(this.mappingOptions.ignore);
        MF.vent.trigger("model:"+this.id+"modelLoaded");
    },
    bindSpecificModelAndElements:function(viewModel){
        var that = this;
        this.mappingOptions ={ ignore:[] };
        this.viewModel = ko.mapping.fromJS(viewModel);
        this.extendModel(this.viewModel);
        ko.applyBindings(this.viewModel,this.el);
        this.elementsViewmodel = CC.elementService.getElementsViewmodel(this);
        var ignore = _.filter(_.keys(this.viewModel),function(item){
            return (item.indexOf('_') == 0 && item != "__ko_mapping__");
        });
        _.each(ignore,function(item){
            that.mappingOptions.ignore.push(item);});
        this.renderElements();
    },
    renderElements:function(){
        var collection = this.elementsViewmodel.collection;
        for(var item in collection){
            collection[item].render();
        }
    },
    extendModel:function(model){
        model._createdText = ko.computed(function() {
            if(model.EntityId()>0 && model.DateCreated()){
                return "Added " + new XDate(model.DateCreated()).toString("MMM d, yyyy");
            }
            return "";
        }, this);
        model._title = ko.computed(function(){
            if(model.EntityId()<=0 && model._Title()){
                return "Add New "+model._Title();
            }
            return model._Title() ? model._Title() : "";
        },this);
    },
    addIdsToModel:function(){
        var rel = MF.State.get("Relationships");
        if(!rel){return;}
        this.model.EntityId(rel.entityId);
        this.model.ParentId(rel.parentId);
        this.model.RootId(rel.rootId);
        this.model.Var(rel.extraVar);
    }
};

MF.mixins.reportMixin = {
    events:{
        'click #viewReport' : 'viewReport'
    },
    successSelector:"#messageContainer",
    errorSelector:"#messageContainer",
    viewReport:function(){
        var isValid = CC.ValidationRunner.runViewModel(this.cid, this.elementsViewmodel,this.errorSelector);
        if(!isValid){return;}
        var url = this.createUrl();
        window.open(url);
    },
    createUrl:function(data){
    }
};

MF.mixins.formMixin = {
    events:{
        'click #save' : 'saveItem',
        'click #cancel' : 'cancel'
    },
    successSelector:"#messageContainer",
    errorSelector:"#messageContainer",
    saveItem:function(){
        var isValid = CC.ValidationRunner.runViewModel(this.cid, this.elementsViewmodel,this.errorSelector);
        if(!isValid){return;}
        var data;
        var fileInputs = $('input:file', this.$el);
        if(fileInputs.length > 0 && _.any(fileInputs, function(item){return $(item).val();})){
            var that = this;
            data = ko.mapping.toJS(this.model,this.mappingOptions);
            var ajaxFileUpload = new CC.AjaxFileUpload(fileInputs[0],{
                action:that.model._saveUrl(),
                onComplete:function(file,response){that.successHandler(response);}
            });
            ajaxFileUpload.setData(data);
            ajaxFileUpload.submit()
        }
        else{
            data = JSON.stringify(ko.mapping.toJS(this.model,this.mappingOptions));
            var promise = MF.repository.ajaxPostModel(this.model._saveUrl(),data);
            promise.done($.proxy(this.successHandler,this));
        }
        this.$el.find("#save").attr("disabled","disabled");
    },
    cancel:function(){
        MF.vent.trigger("form:"+this.id+":cancel");
        if(!this.options.noBubbleUp) {MF.WorkflowManager.returnParentView();}
    },
    successHandler:function(_result){
        var that = this;
        var result = typeof _result =="string" ? JSON.parse(_result) : _result;
        if(!result.Success){
            if(result.Message && !$.noty.getByViewIdAndElementId(this.cid)){
                $(this.errorSelector).noty({type: "error", text: result.Message, viewId:this.cid, closeWith:['button']});
            }
            if(result.Errors && !$.noty.getByViewIdAndElementId(this.cid)){
                _.each(result.Errors,function(item){
                    $(that.errorSelector).noty({type: "error", text:item.ErrorMessage, viewId:that.cid, closeWith:['button']});
                })
            }
        }else{
            if(result.Message){
                var note = $(this.successSelector).noty({type: "success", text:result.Message, viewId:this.cid});
                note.setAnimationSpeed(1000);
                note.setTimeout(3000);
                $.noty.closeAllErrorsByViewId(this.cid);
            }
            MF.vent.trigger("form:"+this.id+":success",result);
            if(!this.options.noBubbleUp){MF.WorkflowManager.returnParentView(result,true);}
        }
        this.$el.find("#save").removeAttr("disabled");
    }
};

MF.mixins.displayMixin = {
    events:{
        'click #cancel' : 'cancel'
    },
    cancel:function(){
        MF.vent.trigger("display:"+this.id+":cancel");
        if(!this.options.noBubbleUp) {MF.WorkflowManager.returnParentView();}
    }
};

MF.mixins.ajaxDisplayMixin = {
    render:function(){
        $.when(MF.loadTemplateAndModel(this))
            .done($.proxy(this.renderCallback,this));
    },
    renderCallback:function(){
        this.bindModelAndElements();
        $("div.form-scroll-inner").height( window.innerHeight - 180);
        this.viewLoaded();
        MF.vent.trigger("display:"+this.id+":pageLoaded",this.options);
    }
};

MF.mixins.ajaxFormMixin = {
    render:function(){
        $.when(MF.loadTemplateAndModel(this))
            .done($.proxy(this.renderCallback,this));
    },
    renderCallback:function(){
        this.bindModelAndElements();
        $("div.form-scroll-inner").height( window.innerHeight - 220);
        this.viewLoaded();
        MF.vent.trigger("form:"+this.id+":pageLoaded",this.options);
    }
};

MF.mixins.ajaxGridMixin = {
    render:function(){
        MF.repository.ajaxGet(this.options.url, this.options.data)
            .done($.proxy(this.renderCallback,this));
    },
    renderCallback:function(result){
        $(this.el).html($("#gridTemplate").tmpl(result));
        $.extend(this.options,result,MF.gridDefaults);
        this.setupGrid();
        this.viewLoaded();
        MF.vent.trigger("grid:"+this.id+":pageLoaded",this.options);

    }
};

MF.mixins.setupGridMixin = {
    setupGrid: function() {
        $.each(this.options.headerButtons, $.proxy(function(i, item) {
            $(this.el).find("." + item).show();
        }, this));
        // if we have more then one grid, jqgrid doesn't scope so we need different names.
        if (this.options.gridId) {
            this.$el.find("#gridContainer").attr("id", this.options.gridId);
        } else {
            this.options.gridId = "gridContainer";
        }
        if(this.options.NoMultiSelectGridView){
            this.options.gridOptions = this.options.gridOptions
                ?this.options.gridOptions.multiselect = false
                :this.options.gridOptions={multiselect:false};
        }
        this.options.searchField = this.options.gridDef.SearchField ||this.options.searchField;
        $("#" + this.options.gridId, this.el).AsGrid(this.options.gridDef, this.options.gridOptions);
        ///////
        $(this.el).gridSearch({onClear:$.proxy(this.removeSearch, this),onSubmit:$.proxy(this.search, this)});
    }
};

MF.mixins.defaultGridEventsMixin = {
    events: {
        'click .new': 'addNew',
        'click .delete': 'deleteItems',
        'click .toggleArchive': 'toggleArchived'
    },
    successSelector:"#messageContainer",
    errorSelector:"#messageContainer",
    setupBindings: function () {
        MF.vent.bind(this.options.gridId + ":AddUpdateItem", this.editItem, this);
        MF.vent.bind(this.options.gridId + ":DisplayItem", this.displayItem, this);
        if ($.isFunction(this._setupBindings)) {
            this._setupBindings();
        }
    },
    unbindBindings: function () {
        MF.vent.unbind(this.options.gridId + ":AddUpdateItem", this.editItem, this);
        MF.vent.unbind(this.options.gridId + ":DisplayItem", this.displayItem, this);
        if ($.isFunction(this._unbindBindings)) {
            this._unbindBindings();
        }
    },
    addNew: function () {
        MF.vent.trigger("route", MF.generateRoute(this.options.addUpdate), true);
    },
    editItem: function (id) {
        MF.vent.trigger("route", MF.generateRoute(this.options.addUpdate, id), true);
    },
    displayItem: function (id) {
        MF.vent.trigger("route", MF.generateRoute(this.options.display, id), true);
    },
    deleteItems: function () {
        if (confirm("Are you sure you would like to delete this Item?")) {
            var ids = cc.gridMultiSelect.getCheckedBoxes(this.options.gridId);
            MF.repository.ajaxGet(this.options.deleteMultipleUrl, $.param({ "EntityIds": ids }, true))
                .done($.proxy(function (result) { this.successHandler(result) }, this));
        }
    },
    toggleArchived: function () {
        var currentState = $(".toggleArchived").attr("data-togglestate");
        var action;
        if(currentState === "showAll"){
            action = "hide";
        } else if(currentState === "hide") {
            action = "showAll";
        }
        var filter = {"group": "AND", rules: [{"op":"toggle", "field": "Archived", "data": action || "showAll" }]};
        var obj = {"filters":""  + JSON.stringify(filter) + ""};
        $("#"+this.options.gridId).jqGrid('setGridParam',{postData:obj});
        this.reloadGrid();
        $(".toggleArchived").attr("data-togglestate", action || "showAll" );
    },
    successHandler:function(_result){
        var that = this;
        var result = typeof _result =="string" ? JSON.parse(_result) : _result;
        if(!result.Success){
            if(result.Message && !$.noty.getByViewIdAndElementId(this.cid)){
                var note = $(that.errorSelector).noty({type: "error", text: result.Message, viewId:that.cid, closeWith:['button']});
                note.setAnimationSpeed(1000);
                note.setTimeout(3000);
                $.noty.closeAllErrorsByViewId(this.cid);
            }
            if(result.Errors && !$.noty.getByViewIdAndElementId(this.cid)){
                _.each(result.Errors,function(item){
                    var note = $(that.errorSelector).noty({type: "error", text:item.ErrorMessage, viewId:that.cid, closeWith:['button']});
                    note.setAnimationSpeed(1000);
                    note.setTimeout(3000);
                    $.noty.closeAllErrorsByViewId(this.cid);
                })
            }
        }else{
            if(result.Message){
                var note = $(that.successSelector).noty({type: "success", text:result.Message, viewId:this.cid});
                note.setAnimationSpeed(1000);
                note.setTimeout(3000);
                $.noty.closeAllErrorsByViewId(this.cid);
            }
            this.reloadGrid();
        }

    },

    bulkDeleteSuccess:function(result){

        this.reloadGrid();
    },
    reloadGrid: function () {
        MF.vent.unbind(this.options.gridId + ":AddUpdateItem", this.editItem, this);
        MF.vent.unbind(this.options.gridId + ":DisplayItem", this.displayItem, this);
        $("#" + this.options.gridId).trigger("reloadGrid");
        MF.vent.bind(this.options.gridId + ":AddUpdateItem", this.editItem, this);
        MF.vent.bind(this.options.gridId + ":DisplayItem", this.displayItem, this);
    },
    // used by children to update parent grid
    callbackAction: function () {
        this.reloadGrid();
    }
};

MF.mixins.setupGridSearchMixin = {
    search:function(v){
        var searchItem = {"field": this.options.searchField ,"data": v };
        var filter = {"group":"AND",rules:[searchItem]};
        var obj = {"filters":""  + JSON.stringify(filter) + ""};
        $("#"+this.options.gridId).jqGrid('setGridParam',{postData:obj});
        this.reloadGrid();
    },
    removeSearch:function(){
        delete $("#"+this.options.gridId).jqGrid('getGridParam' ,'postData')["filters"];
        this.reloadGrid();
        return false;
    }
};

