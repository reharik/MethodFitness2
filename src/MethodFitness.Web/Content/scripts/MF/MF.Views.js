/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 2/26/12
 * Time: 11:11 AM
 * To change this template use File | Settings | File Templates.
 */

MF.Views = {};
// use like this
// this._super("testMethod",arguments);

(function(Backbone) {

  // The super method takes two parameters: a method name
  // and an array of arguments to pass to the overridden method.
  // This is to optimize for the common case of passing 'arguments'.
  function _super(methodName, args) {

    // Keep track of how far up the prototype chain we have traversed,
    // in order to handle nested calls to _super.
    this._superCallObjects || (this._superCallObjects = {});
    var currentObject = this._superCallObjects[methodName] || this,
        parentObject  = findSuper(methodName, currentObject);
    this._superCallObjects[methodName] = parentObject;
    if(parentObject[methodName]){
        var result = parentObject[methodName].apply(this, args || []);
    }
    delete this._superCallObjects[methodName];
    return result;
  }

  // Find the next object up the prototype chain that has a
  // different implementation of the method.
  function findSuper(methodName, childObject) {
    var object = childObject;
    while (object[methodName] === childObject[methodName]) {
      object = object.constructor.__super__;
    }
    return object;
  }

  _.each(["Model", "Collection", "View", "Router"], function(klass) {
    Backbone[klass].prototype._super = _super;
  });


})(Backbone);

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
      if(this.elementsViewmodel){
        this.elementsViewmodel.destroy();
      }
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
    },

    viewLoaded:function(){
    }
  });

MF.Views.FormView = MF.Views.View.extend({
    initialize: function(){
        MF.mixin(this, "formMixin");
        MF.mixin(this, "modelAndElementsMixin");
    },
    render: function(){
        this.bindModelAndElements();
        this.viewLoaded();
        MF.vent.trigger("form:"+this.id+":pageLoaded",this.options);
        return this;
    }
});

MF.Views.AjaxFormView = MF.Views.View.extend({
    initialize: function(){
        MF.mixin(this, "formMixin");
        MF.mixin(this, "ajaxFormMixin");
        MF.mixin(this, "modelAndElementsMixin");
    }
});

MF.Views.AjaxDisplayView = MF.Views.View.extend({
    initialize:function(){
        MF.mixin(this, "displayMixin");
        MF.mixin(this, "ajaxDisplayMixin");
        MF.mixin(this, "modelAndElementsMixin");
    }
});

MF.Views.NoMultiSelectGridView= MF.Views.View.extend({
    initialize: function(){
        this.options.gridOptions ={multiselect:false};
        MF.mixin(this, "ajaxGridMixin");
        MF.mixin(this, "setupGridMixin");
        MF.mixin(this, "defaultGridEventsMixin");
        MF.mixin(this, "setupGridSearchMixin");
    },
    viewLoaded:function(){
        this.setupBindings();
    },
    onClose:function(){
        this.unbindBindings();
    }
});


MF.Views.GridView = MF.Views.View.extend({
    initialize: function(){
        MF.mixin(this, "ajaxGridMixin");
        MF.mixin(this, "setupGridMixin");
        MF.mixin(this, "defaultGridEventsMixin");
        MF.mixin(this, "setupGridSearchMixin");
    },
    viewLoaded:function(){
        this.setupBindings();
    },
    onClose:function(){
        this.unbindBindings();
    }
});

MF.Views.AjaxPopupFormModule  = MF.Views.View.extend({
    initialize:function(){
        this.registerSubscriptions();
    },

    render: function(){
        this.options.noBubbleUp=true;
        this.options.isPopup=true;
        this.popupForm = this.options.view && MF.Views[this.options.view] ? new MF.Views[this.options.view](this.options) : new MF.Views.AjaxFormView(this.options);

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
        this.options.noBubbleUp=true;
        this.options.isPopup=true;
        this.popupDisplay = this.options.view && MF.Views[this.options.view] ? new MF.Views[this.options.view](this.options) : new MF.Views.AjaxDisplayView(this.options);
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
        var that = this;
        $(".ui-dialog").remove();

        $(this.el).dialog({
            modal: true,
            width: this.options.width||550,
            buttons:this.options.buttons,
            title: this.options.title,
            close:function(){
                MF.vent.trigger("popup:"+that.options.id+":cancel");
            }
        });
        return this;
    }
});

MF.Views.TemplatedPopupView = MF.Views.View.extend({

    initialize: function(){
        this.options = $.extend({},MF.popupDefaults,this.options);
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

MF.Views.KOPopupView = MF.Views.View.extend({
    initialize: function(){
        this.options = $.extend({},MF.popupDefaults,this.options);
    },
    render:function(){
        var template = $(this.options.template).clone().show();
        $(this.el).append(template);
        ko.applyBindings(this.options.data,this.el);
        this.elementsViewmodel = CC.elementService.getElementsViewmodel(this);
        this.errorSelector = $("#popupMessageContainer",this.el);

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
            MF.vent.trigger("popup:"+id+":save", this);
        };
        var editFunc = function(event) {MF.vent.trigger("popup:"+id+":edit");};
        var cancelFunc = function(){
            $(this).dialog("close");
            MF.vent.trigger("popup:"+id+":cancel");
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
    initialize:function(){
        this.options = $.extend({},MF.tokenDefaults,this.options);
    },
    render: function(){

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
    showSearch:true,
    id:""
};

