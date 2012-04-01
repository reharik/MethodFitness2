/**
 * Created by JetBrains RubyMine.
 * User: RHarik
 * Date: 10/13/11
 * Time: 10:52 AM
 * To change this template use File | Settings | File Templates.
 */
if (typeof mf == "undefined") {
    var mf = {};
}

MF.repository= (function(){
    var repositoryCallback = function(result,callback){
        if(result.LoggedOut){
            window.location.replace(result.RedirectUrl);
            return;
        }
        callback(result);
    };
    return {
        ajaxPost:function(url, data, callback){
             MF.showThrob=true;
            $.post(url,data,function(result){ repositoryCallback(result,callback)});
        },
        ajaxGet:function(url, data, callback){
             MF.showThrob=true;
            $.get(url,data,function(result){repositoryCallback(result,callback);
            });
        }
    }
}());
