/**
 * Created by JetBrains RubyMine.
 * User: RHarik
 * Date: 10/13/11
 * Time: 10:52 AM
 * To change this template use File | Settings | File Templates.
 */
if (typeof MF == "undefined") {
    var MF = {};
}

MF.repository= (function(){
    var repositoryCallback = function(result,callback){

        if(result.LoggedOut){
            window.location.replace(result.RedirectUrl);
            return null;
        }
        clearTimeout(MF.throbberTimeout);
        MF.showThrob=false;
        $("#ajaxLoading").hide();
        return result;
    };
    var throbber = function(){
        MF.showThrob=true;
        if(!MF.throbberTimeout){
            MF.throbberTimeout = setTimeout(function() {
                if(MF.showThrob) {
                    $("#ajaxLoading").show();
                }
            }, 500);
        }
    };
    return {
        ajaxPost:function(url, data){
            throbber();
            $.post(url,data).done(repositoryCallback);
        },
        ajaxGet:function(url, data){
            throbber();
            return $.get(url,data).done(repositoryCallback);
        },
        ajaxPostModel:function(url, data){
            throbber();
            return $.ajax({
                type:"post",
                url: url,
                data:data,
                contentType:  "application/json; charset=utf-8",
                traditional:true
            }).done(repositoryCallback);
        }
    }
}());
