/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 2/26/12
 * Time: 11:11 AM
 * To change this template use File | Settings | File Templates.
 */
MF.Views = {};
MF.Views.View = Backbone.View.extend({
    // Remove the child view and close it
    removeChildView: function(item){
      var view = this.children[item.cid];
      if (view){
        view.close();
        delete this.children[item.cid];
      }
    },

    // Store references to all of the child `itemView`
    // instances so they can be managed and cleaned up, later.
    storeChild: function(view){
      if (!this.children){
        this.children = {};
      }
      this.children[view.cid] = view;
    },

    // Handle cleanup and other closing needs for
    // the collection of views.
    close: function(){
      this.unbind();
      //this.unbindAll();
      this.remove();

      this.closeChildren();

      if (this.onClose){
        this.onClose();
      }
    },

    closeChildren: function(){
      if (this.children){
        _.each(this.children, function(childView){
          childView.close();
        });
      }
    }
  });

MF.Views.BaseFormView = MF.Views.View.extend({
    events:{
        'click #save' : 'saveItem',
        'click .cancel' : 'cancel'
    },

    initialize: function(){
       this.options = $.extend({},MF.formDefaults,this.options);
        this.$el = $(this.el);
    },
    config:function(){
        if(extraFormOptions){
            $.extend(true, this.options, extraFormOptions);
        }

        this.options.crudFormOptions.successHandler = $.proxy(this.successHandler,this);
        $(this.options.crudFormSelector,this.el).crudForm(this.options.crudFormOptions);
        if(this.options.crudFormOptions.additionBeforeSubmitFunc){
            var array = !$.isArray(this.options.crudFormOptions.additionBeforeSubmitFunc)
                ? [this.options.crudFormOptions.additionBeforeSubmitFunc]
                : this.options.crudFormOptions.additionBeforeSubmitFunc;
            $(array).each($.proxy(function(i,item){
                $(this.options.crudFormSelector,this.el).data('crudForm').setBeforeSubmitFuncs(item);
            },this));
        }
        if($(".rte").size()>0){
            $(".rte").cleditor(this.options.editorOptions);
        }
    },
    saveItem:function(){
        $(this.options.crudFormSelector,this.el).submit();
    },
    cancel:function(){
        MF.vent.trigger("form:"+this.id+":cancel");
        if(!this.options.noBubbleUp)
            MF.WorkflowManager.returnParentView();
    },
    successHandler:function(result, form, notification){
        var emh = cc.utilities.messageHandling.messageHandler();
        var message = cc.utilities.messageHandling.mhMessage("success",result.Message,"");
        emh.addMessage(message);
        emh.showAllMessages(notification.getSuccessContainer(),true);
        MF.vent.trigger("form:"+this.id+":success",result);
        if(!this.options.noBubbleUp)
            MF.WorkflowManager.returnParentView(result,true);
    }
});

MF.Views.FormView = MF.Views.BaseFormView.extend({
    render: function(){
        this.config();
        this.viewLoaded();
        MF.vent.trigger("form:"+this.id+":pageLoaded",this.options);
        return this;
    }
});

MF.Views.AjaxFormView = MF.Views.BaseFormView.extend({
    render:function(){
        MF.repository.ajaxGet(this.options.url, this.options.data, $.proxy(function(result){this.renderCallback(result)},this));
    },
    renderCallback:function(result){
        if(result.LoggedOut){
            window.location.replace(result.RedirectUrl);
            return;
        }
        $(this.el).html(result);
        this.config();
        //callback for render
        this.viewLoaded();
        //general notification of pageloaded
        MF.vent.trigger("form:"+this.id+":pageLoaded",this.options);
        $(this.el).find("form :input:visible:enabled:first").focus();
    }
});

MF.Views.AjaxDisplayView = MF.Views.View.extend({
    events:{
        'click .cancel' : 'cancel'
    },
    initialize: function(){
        this.options = $.extend({},MF.displayDefaults,this.options);
         this.$el = $(this.el);
    },
    render:function(){
        MF.repository.ajaxGet(this.options.url, this.options.data, $.proxy(this.renderCallback,this));
    },
    renderCallback:function(result){
        if(result.LoggedOut){
            window.location.replace(result.RedirectUrl);
            return;
        }
        $(this.el).html(result);
        if(extraFormOptions){
            $.extend(true,this.options, extraFormOptions);
        }
        MF.vent.trigger("display:"+this.id+":pageLoaded",this.options);
    },
    cancel:function(){
        MF.vent.trigger("display:"+this.id+":cancel",this.id);
        if(!this.options.noBubbleUp)
            MF.WorkflowManager.returnParentView();
    }
});

MF.Views.GridView = MF.Views.View.extend({
    events:{
        'click .new' : 'addNew',
        'click .delete' : 'deleteItems'
    },
    initialize: function(){
        this.options = $.extend({},MF.gridDefaults,this.options);
        this.id=this.options.id;
    },
    render:function(){
        if(this.onPreRender)this.onPreRender();
        MF.repository.ajaxGet(this.options.url, this.options.data, $.proxy(function(result){this.renderCallback(result)},this));
    },
    renderCallback:function(result){
        if(result.LoggedOut){
            window.location.replace(result.RedirectUrl);
            return;
        }
        $(this.el).html($("#gridTemplate").tmpl(result));
        $.extend(this.options,result);
//        if(gridControllerOptions){
//            $.extend(true, this.options, gridControllerOptions);
//        }
        $.each(this.options.headerButtons,function(i,item){
            $("."+item).show();
        });

        var gridContainer = this.options.gridContainer;
        $(gridContainer,this.el).AsGrid(this.options.gridDef, this.options.gridOptions);
        $(window).bind('resize', function() { cc.gridHelper.adjustSize(gridContainer); }).trigger('resize');
        $(this.el).gridSearch({onClear:$.proxy(this.removeSearch,this),onSubmit:$.proxy(this.search,this)});
        //callback for render
        this.viewLoaded();
        //general notification of pageloaded
        MF.vent.trigger("grid:"+this.id+":pageLoaded",this.options);
        MF.vent.bind("AddUpdateItem",this.editItem,this);
        MF.vent.bind("DisplayItem",this.displayItem,this);
    },
    addNew:function(){
        MF.vent.trigger("route",this.options.addUpate,true);
        //$.publish('/contentLevel/grid_'+this.id+'/AddUpdateItem', [this.options.addUpdateUrl]);
    },
    editItem:function(id){
        MF.vent.trigger("route",this.options.addUpate+"/"+id,true);
    },
    displayItem:function(id){
        MF.vent.trigger("route",this.options.display+"/"+id,true);
    },
    deleteItems:function(){
        if (confirm("Are you sure you would like to delete this Item?")) {
            var ids = cc.gridMultiSelect.getCheckedBoxes();
            MF.repository.ajaxGet(this.options.deleteMultipleUrl,
                $.param({"EntityIds":ids},true),
                $.proxy(function(){this.reloadGrid()},this));
        }
    },
    search:function(v){
        var searchItem = {"field": this.options.searchField ,"data": v };
        var filter = {"group":"AND",rules:[searchItem]};
        var obj = {"filters":""  + JSON.stringify(filter) + ""};
        $(this.options.gridContainer).jqGrid('setGridParam',{postData:obj});
        this.reloadGrid();
    },
    removeSearch:function(){
        delete $(this.options.gridContainer).jqGrid('getGridParam' ,'postData')["filters"];
        this.reloadGrid();
        return false;
    },
    reloadGrid:function(){
        MF.vent.unbind("AddUpdateItem");
        $(this.options.gridContainer).trigger("reloadGrid");
        MF.vent.bind("AddUpdateItem",this.editItem,this);
    },
    callbackAction:function(){
        this.reloadGrid();
    },
    onClose:function(){
        MF.vent.unbind("AddUpdateItem");
    }

});

MF.Views.AjaxPopupFormModule  = MF.Views.View.extend({
    initialize:function(){
        this.registerSubscriptions();
    },

    render: function(){
        this.options.noBubbleUp=true;
        this.popupForm = this.options.view ? new MF.Views[this.options.view](this.options) :  new MF.Views.AjaxFormView(this.options);
        this.popupForm.render();
        this.storeChild(this.popupForm);
        $(this.el).append(this.popupForm.el);
    },
    registerSubscriptions: function(){
        MF.vent.bind("form:"+this.id+":pageLoaded", this.loadPopupView, this);
        MF.vent.bind("popup:"+this.id+":cancel", this.popupCancel, this);
        MF.vent.bind("popup:"+this.id+":save", this.formSave, this);
        MF.vent.bind("form:"+this.id+":success", this.formSuccess, this);
    },
    onClose:function(){
         MF.vent.unbind("form:"+this.id+":pageLoaded");
         MF.vent.unbind("popup:"+this.id+":cancel");
        MF.vent.unbind("popup:"+this.id+":save");
        MF.vent.unbind("form:"+this.id+":success");
    },
    loadPopupView:function(formOptions){
        var buttons = formOptions.buttons?formOptions.buttons:MF.Views.popupButtonBuilder.builder(formOptions.id).standardEditButons();
        var popupOptions = {
            id:this.id,
            el:this.el, // we pass the el here so we can call the popup on it
            buttons: buttons,
            title:formOptions.title
        };
        var view = new MF.Views.PopupView(popupOptions);
        view.render();
        this.storeChild(view);
    },
    formSave:function(){
        this.popupForm.saveItem();
    },
    //just catching and re triggering with the module name
    formSuccess:function(result){
        MF.vent.trigger("ajaxPopupFormModule:"+this.id+":success",result);
    },
    popupCancel:function(){
        MF.vent.trigger("ajaxPopupFormModule:"+this.id+":cancel",[]);
    }
});

MF.Views.AjaxPopupDisplayModule  = MF.Views.View.extend({
    initialize:function(){
        this.registerSubscriptions();
    },
    render: function(){
        this.popupDisplay = this.options.view ? new this.options.view(this.options) : new MF.Views.AjaxDisplayView(this.options);
        this.popupDisplay.render();
        this.storeChild(this.popupDisplay);
        $(this.el).append(this.popupDisplay.el);
    },

    registerSubscriptions: function(){
        MF.vent.bind("display:"+this.id+":pageLoaded", this.loadPopupView, this);
        MF.vent.bind("popup:"+this.id+":cancel", this.popupCancel, this);
    },
    onClose:function(){
         MF.vent.unbind("display:"+this.id+":pageLoaded");
         MF.vent.unbind("popup:"+this.id+":cancel");
    },

    loadPopupView:function(formOptions){
        var buttons = formOptions.buttons?formOptions.buttons:MF.Views.popupButtonBuilder.builder(formOptions.id).standardEditButons();
        var popupOptions = {
            id:this.id,
            el:this.el, // we pass the el here so we can call the popup on it
            buttons: buttons,
            width:this.options.popupWidth,
            title:formOptions.title
        };
        var view = new MF.Views.PopupView(popupOptions);
        view.render();
        this.storeChild(view);
    },
    popupCancel:function(){
        MF.vent.trigger("module:"+this.id+":cancel",[]);
    }
});

MF.Views.PopupView = MF.Views.View.extend({
    render:function(){
        $(".ui-dialog").remove();
        var errorMessages = $("div[id*='errorMessages']", this.el);
        if(errorMessages){
            var id = errorMessages.attr("id");
            errorMessages.attr("id","errorMessagesPU").removeClass(id).addClass("errorMessagesPU");
        }

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
    },
    close:function(){
        $(this.el).dialog("close");
    }
});


MF.Views.TemplatedPopupView = MF.Views.View.extend({

    initialize: function(){
        this.options = $.extend({},mf.popupDefaults,this.options);
    },
    render:function(){
        $(this.el).append($(this.options.template).tmpl(this.options.data));
        var popupOptions = {
            id:this.id,
            el:this.el, // we pass the el here so we can call the popup on it
            buttons: this.options.buttons,
            width:this.options.popupWidth,
            title:this.options.title
        };
        var view = new MF.Views.PopupView(popupOptions);
        view.render();
        this.storeChild(view);

    }
});



MF.Views.popupButtonBuilder = (function(){
    return {
        builder: function(id){
        var buttons = {};
        var _addButton = function(name,func){ buttons[name] = func; };
        var saveFunc = function() {
            MF.vent.trigger("popup:"+id+":save");
        };
        var editFunc = function(event) {MF.vent.trigger("popup:"+id+":edit");};
        var cancelFunc = function(){
                            MF.vent.trigger("popup:"+id+":cancel");
                            $(this).dialog("close");
                            $(".ui-dialog").remove();
                        };
        return{
            getButtons:function(){return buttons;},
            getSaveFunc:function(){return saveFunc;},
            getCancelFunc:function(){return cancelFunc;},
            addSaveButton:function(){_addButton("Save",saveFunc); return this},
            addEditButton:function(){_addButton("Edit",editFunc);return this},
            addCancelButton:function(){_addButton("Cancel",cancelFunc);return this},
            addButton:function(name,func){_addButton(name,func);return this},
            clearButtons:function(){buttons = {};return this},
            standardEditButons: function(){
                _addButton("Save",saveFunc);
                _addButton("Cancel",cancelFunc);
                return buttons;
            },
            standardDisplayButtons: function(){
                _addButton("Cancel",cancelFunc);
                return buttons;
            }
        };
    }
    }
}());

MF.Views.TokenizerModule = MF.Views.View.extend({
    render:function() {
        this.registerSubscriptions();
        this.tokenView = new MF.Views.TokenView(this.options);
        this.storeChild(this.tokenView);
        },
    registerSubscriptions: function() {
        MF.vent.bind("token:" + this.id + ":addUpdate", this.addUpdateItem, this);
        MF.vent.bind("ajaxPopupFormModule:" + this.id + ":success", this.formSuccess, this);
        MF.vent.bind("ajaxPopupFormModule:" + this.id + ":cancel", this.formCancel, this);
    },
    onClose:function(){
         MF.vent.unbind("token:" + this.id + ":addUpdate");
         MF.vent.unbind("ajaxPopupFormModule:" + this.id + ":success");
        MF.vent.unbind("ajaxPopupFormModule:" + this.id + ":cancel");
    },
//from tolkneizer
    addUpdateItem:function() {
        var options = {
            id: this.id,
            url: this.options.tokenizerUrls[this.id + "AddUpdateUrl"],
            crudFormOptions: { errorContainer:"#errorMessagesPU",successContainer:"#errorMessagesForm"}
        };
        this.popupModule = new MF.Views.AjaxPopupFormModule(options);
        this.popupModule.render();
        this.storeChild(this.popupModule);
        $(this.el).append(this.popupModule.el);
    },
    formSuccess:function(result) {
        this.tokenView.successHandler(result);
        this.popupModule.close();
        this.formCancel();
        return false;
    },
    formCancel:function() {
        this.popupModule.close();
        return false;
// $.publish("/contentLevel/tokenizer_" + this.id + "/formCancel",[this.id]);
    }
});

MF.Views.TokenView = MF.Views.View.extend({
    events:{
        'click #addNew' : 'addNew'
    },
    initialize: function(){
        this.options = $.extend({},MF.tokenDefaults,this.options);

        if(!this.options.availableItems || this.options.availableItems.length==0) {
            $("#noAssets",this.el).show();
            $("#hasAssets",this.el).hide();
        }else{
            $("#noAssets",this.el).hide();
            $("#hasAssets",this.el).show();
            this.inputSetup();
        }
    },
    inputSetup:function(){
        $(this.options.inputSelector, this.el).tokenInput(this.options.availableItems, {prePopulate: this.options.selectedItems,
            internalTokenMarkup:$.proxy(this.internalTokenMarkup,this),
            afterTokenSelectedFunction:$.proxy(this.afterTokenSelectedFunction,this),
            onDelete:$.proxy(this.deleteToken,this)
        });
        this.options.instantiated = true;
        if(this.postSetup)this.postSetup();
    },
    internalTokenMarkup:function(item) {
        var cssClass = "class='selectedItem'";
        return "<p><a " + cssClass + ">" + item.name + "</a></p>";
    },
    afterTokenSelectedFunction:function(item) {},
    deleteToken:function(hidden_input,token_data) {},
    addNew:function(e){
        e.preventDefault();
        MF.vent.trigger("token:"+this.id+":addUpdate",this.id);
    },
    successHandler: function(result){
        if(!this.options.instantiated){
            $("#noAssets",this.el).hide();
            $("#hasAssets",this.el).show();
            this.inputSetup();
        }
        $(this.options.inputSelector,this.el).tokenInput("add",{id:result.EntityId, name:result.Variable});
        $(this.options.inputSelector,this.el).tokenInput("addToAvailableList",{id:result.EntityId, name:result.Variable});
    }
});

MF.Views.EditableTokenView = MF.Views.TokenView.extend({
     events:_.extend({
        'click .tokenEditor' : 'tokenEditor'
    }, MF.Views.TokenView.prototype.events),
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


MF.tokenDefaults = {
    availableItems:[],
    selectedItems: [],
    tooltipAjaxUrl:"",
    inputSelector:"#Input"
};

MF.popupDefaults = {
    title:""
};

MF.gridDefaults = {
    searchField:"Name",
    gridContainer: "#gridContainer",
    showSearch:true,
    id:""
};

MF.formDefaults = {
    id:"",
    data:{},
    crudFormSelector:"#CRUDForm",
    crudFormOptions:{
        errorContainer:"#errorMessagesForm",
        successContainer:"#errorMessagesGrid",
        additionBeforeSubmitFunc:null
    },
    runAfterRenderFunction: null
};

MF.displayDefaults = {
    id:"",
    data:{},
    runAfterRenderFunction: null
};