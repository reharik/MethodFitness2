/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 7/15/12
 * Time: 10:43 AM
 * To change this template use File | Settings | File Templates.
 */

CC.NotificationService = function(){
    this.successViewmodel = {
        messages: ko.observableArray(),
        fadeOut: function(item){
            if(item.nodeType ==1){
                $(item).hide("slow");
            }
        }
    };
    this.errorViewmodel = {
        messages: ko.observableArray(),
        fadeOut: function(item){
            if(item.nodeType ==1){
                $(item).hide("slow");
            }
        }
    };
};

$.extend(CC.NotificationService.prototype,{
    successSelector:"",
    errorSelector:"",
    render:function(_successSelector, _errorSelector){
        this.successSelector = _successSelector;
        this.errorSelector = _errorSelector;
        ko.applyBindings(this.successViewmodel,this.successSelector);
        ko.applyBindings(this.errorViewmodel,this.errorSelector);
    },
    getCorrectViewModel:function(status){
        if(status=="success"){
            return this.successViewmodel.messages;
        }else if (status == "error"){
            return this.errorViewmodel.messages;
        }
    },
    add:function(msgObject, status){
        var messages=this.getCorrectViewModel(status);
        var exists = _.any(messages,function(msg){
            return msgObject.elementCid() === msg.elementCid() && msgObject.message() === msg.message();
        });
        if(!exists){
            messages.push(msgObject);
            if(msgObject.shouldSelfDestruct){
                msgObject.parent = this;
                msgObject.selfDestruct();
            }
        }
    },
    remove:function(msgObject,status){
        var messages=this.getCorrectViewModel(status);
        messages.remove(function(msg){
            return msg.elementCid()===msgObject.elementCid()
            && msg.message() === msgObject.message();
        });
    },

    removeById:function(cid,status){
        var messages=this.getCorrectViewModel(status);
        messages.remove(function(item){
            return item.elementCid()===cid;
        });
    },

    removeAllErrorsByViewId:function(viewId,status){
        var errorMessages=this.errorViewmodel.messages;
        var successMessages=this.successViewmodel.messages;
        errorMessages.remove(function(item){
            return item.viewId()===viewId&& item.status()==='error';
        });
        successMessages.remove(function(item){
            return item.viewId()===viewId&& item.status()==='success';
        });
    },

    resetSuccessSelector:function(_successSelector){
        ko.cleanNode(this.successSelector[0]);
        this.successSelector = _successSelector;
        ko.applyBindings(this.successViewmodel,this.successSelector);
    },

    resetErrorSelector:function(_errorSelector){
        ko.cleanNode(this.errorSelector[0]);
        this.errorSelector = _successSelector;
        ko.applyBindings(this.errorViewmodel,this.errorSelector);
    },

    handleResult:function(result, cid){
        var that=this;
        if(!result.Success){
            if(result.Message){
                that.add(new CC.NotificationMessage("",cid, result.Message,"error"),"error");
            }
            if(result.Errors){
                _.each(result.Errors,function(item){
                    that.add(new CC.NotificationMessage("",cid, item.ErrorMessage,"error"),"error");
                })
            }
        }else{
            if(result.Message){
                that.add(new CC.NotificationMessage("",cid, result.Message,"success",true),"success");
                that.removeAllErrorsByViewId(cid);
            }
        }
        return result.Success;
    }
});


CC.NotificationMessage = function(elementCid, viewId, message, status, _shouldSelfDestruct){
    this.message = ko.observable(message);
    this.elementCid = ko.observable(elementCid);
    this.viewId = ko.observable(viewId);
    this.status = ko.observable(status);
    this.parent = null;
    this.shouldSelfDestruct = _shouldSelfDestruct;
    this.selfDestruct = function(time){
        var that = this;
        setTimeout(function(){
            that.parent.remove(that);
        }, time ? time : 2000);
    };

};


