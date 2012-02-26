if (typeof mf == "undefined") {
            var mf = {};
}

mf.crudHelpers = {
    documentHandler: function(arr){
        if($("#DocumentInput").val())
        {
            arr.push({"name":"DocumentTokenViewModel.DocumentInput","value":$("#DocumentInput").val()})
        }
    },
    photoHandler: function(arr){
        if($("#PhotoInput").val())
        {
            arr.push({"name":"PhotoTokenViewModel.PhotoInput","value":$("#PhotoInput").val()})
        }
    },
    membershipPositionHandler: function(arr){
        // this is to handle blank rows
        var indx = 0;
        $(".multirow-input-tbl tr").each(function(i,item){
            var name = $(item).find("td [name='MembershipPosition.Name']").val();
            var heldFrom = $(item).find("td [name='MembershipPosition.HeldFrom']").val();
            var heldTo = $(item).find("td [name='MembershipPosition.HeldTo']").val();
            var entityId = $(item).find("td [name='MembershipPosition.EntityID']").val()?$(item).find("td[name='MembershipPosition.EntityID']").val():0;
            if(name){
                var objName = "MembershipDtos["+ indx +"]";
                indx++;
                arr.push({"name":objName+".Name","value":name});
                arr.push({"name":objName+".HeldFrom","value":heldFrom});
                arr.push({"name":objName+".HeldTo","value":heldTo});
                arr.push({"name":objName+".EntityId","value":entityId});
            }
        });
    }
};

$(document).ready(function() {
    // this must be on the form view if it's retrieved by ajax. don't know how to make it a delegate.
//    $('.mf_CRUD').crudForm({});
    $(".mf_datepicker").datepicker({ changeMonth: true, changeYear: true, yearRange: '1950:' + new Date().getFullYear() });
    });

    if (typeof cc == "undefined") {
        var cc = {};
    }

    cc.namespace = function() {
        var a = arguments, o = null, i, j, d;
        for (i = 0; i < a.length; i = i + 1) {
            d = a[i].split(".");
            o = window;
            for (j = 0; j < d.length; j = j + 1) {
                o[d[j]] = o[d[j]] || {};
                o = o[d[j]];
            }
        }
        return o;
    };
    cc.namespace("cc.utilities");

(function($) {

    $.fn.crudForm = function(options) {
        return this.each(function()
        {
            var elem = $(this);
            if (!elem.data('crudForm')) {
              elem.data('crudForm', new CrudFunction(options, elem));
            }
        });
    };

    var CrudFunction = function(options, elem){
        var myOptions = $.extend({}, $.fn.crudForm.defaults, options || {});
        var errorContainer = myOptions.errorContainer;
        var successContainer = myOptions.successContainer?myOptions.successContainer:myOptions.errorContainer;
        // do not move this into crudForm.defaults.  it end up using the same one for every call.
        var notification = myOptions.notification ? myOptions.notification : cc.utilities.messageHandling.notificationResult();
        var mySubmitHandler = function(form) {
            var ajaxOptions = {dataType: 'json',
                success: function(result){
                    notification.result(result,form)
                },
                beforeSubmit:  beforSubmitCallback
            };
            $(form).ajaxSubmit(ajaxOptions);
        };

        $(errorContainer).hide();
        $(successContainer).hide();
        if(myOptions.successHandler){
            notification.setSuccessHandler(myOptions.successHandler)
        }
        notification.setErrorContainer(myOptions.errorContainer);
        notification.setSuccessContainer(myOptions.successContainer);

        if(myOptions.submitHandler){
            mySubmitHandler=myOptions.submitHandler;
        }
        var beforSubmitCallback = function(arr, form, options){
            var _arr = arr;
            $(myOptions.beforeSubmitCallbackFunctions).each(function(i,item){
                if(typeof(item) === 'function') item(_arr);
            });
        };

        this.setBeforeSubmitFuncs = function(beforeSubmitFunc){
             var array = !$.isArray(beforeSubmitFunc) ? [beforeSubmitFunc] : beforeSubmitFunc;
            $(array).each(function(i,item){
                if($.inArray(item,myOptions.beforeSubmitCallbackFunctions)<=0){
                    myOptions.beforeSubmitCallbackFunctions.push(item);
                }
            });};

        $(elem).validate({
            submitHandler: mySubmitHandler,
            errorContainer: $(errorContainer),
            errorLabelContainer: $(errorContainer).find("ul"),
            wrapper: 'li',
            validClass: "valid_field",
            errorClass: "invalid_field"

        });
    };

    $.fn.crudForm.defaults = {
        dataType: 'json',
        errorContainer: '#errorMessagesForm',
        beforeSubmitCallbackFunctions: [mf.crudHelpers.documentHandler,mf.crudHelpers.photoHandler]
    };
})(jQuery);

