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

cc.object = function(o){
    function F(){}
    F.prototype = o;
    return new F();
};

cc.namespace("cc.utilities");

cc.utilities.toggleForm = function(type, onlyShowNoHide) {
    if (onlyShowNoHide && $("#" + type + "Collapsible .mf_container:visible").length > 0) return;
    var element = $("#" + type + "Collapsible .mf_container");
    if (!element.is(":hidden")) {
        element.hide();
    } else if (element.is(":hidden")) {
        element.show();
    }
};

cc.utilities.clearInputs = function(element){
    var formFields = "input, checkbox, select, textarea";
    $(element).find(formFields).each(function(){
        if(this.tagName == "SELECT" ){
            $(this).find("option:selected").removeAttr("selected");
        } else if($(this).text()) {
            $(this).text("");
        } else if ( this.type == "radio") {
            $(this).attr("checked",false);
        } else if ( this.type == "checkbox") {
            $(this).attr("checked",false);
        } else if($(this).val() && this.type != "radio") {
            $(this).val("");
        }
    })
};

 cc.utilities.findAndRemoveItem = function(array, item, propertyOnItem){
    if(!$.isArray(array)) return false;
    var indexOfItem;
    $(array).each(function(idx,x){
        if(propertyOnItem){
            if(x[propertyOnItem] == item){
                 indexOfItem=idx;
            }
        }else{
            if(x == item){
                indexOfItem=idx;
            }
        }
    });
    if(indexOfItem >= 0){
        array.splice(indexOfItem,1);
    }
 };

 cc.utilities.cleanAndHideErrorMessageDiv = function(element){
    $(element).html("");
     $(element).hide();
};

 cc.utilities.openDocInNewWindow = function(url){
     window.open(url,'_blank','');
 };

 cc.straightNotification = function(result,form,notification){
    notification.result(result);
};

(function($) {
    $.fn.convertToListItems = function(value, display) {
        var listItems = [];
        this.each(function(i, item) {
            var selectedItem = "";
            if (item.IsDefault) {
                selectedItem = "selected=\"selected\"";
            }
            var option = '<option value="' + item[value] + '" ' + selectedItem + '> ' + item[display] + ' </option>';
            listItems.push(option);
        });
        return listItems;
    };


})(jQuery);


cc.utilities.trim = function(stringValue){
    return stringValue.replace(/(^\s*|\s*$)/, "");
};

cc.utilities.fixedWidthDropdown = function(){
    if($.browser.msie){
        $('select.mf_fixedWidthDropdown').css("width","auto");
    }else{
        $('select.mf_fixedWidthDropdown').ieSelectStyle({ applyStyle : false });
    }
};

(function($) {
    $.fn.extend({
        exclusiveCheck: function() {
            var checkboxes = $(this).find("input:checkbox");
            checkboxes.each(function(i,item){
                $(item).click(function(){
                    if(this.checked){
                        checkboxes.each(function() {
                            if ($(this)[0]!==$(item)[0]) this.checked = false;
                        });
                    }
                })
            });
        }});
})(jQuery);


$.fn.clearForm = function() {
  return this.each(function() {
    var type = this.type, tag = this.tagName.toLowerCase();
    if (tag != 'input')
      return $(':input',this).clearForm();
    if (type == 'text' || type == 'password' || tag == 'textarea')
      this.value = '';
    else if (type == 'checkbox' || type == 'radio')
      this.checked = false;
    else if (tag == 'select')
      this.selectedIndex = -1;
  });
};
