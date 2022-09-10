/**
 * Created by Owner on 11/25/13.
 */

define(["jquery", "underscore"],function($,_){

    var model = {};
    var getModel =
    // constructor
    function Validator(view){
        var that = this;
        that.init = function(){
            var model = new CC.ElementCollection();
            view.$el.find("[eltype]").each(function(i,item){
                var element = new CC.Elements[$(item).attr("eltype")]($(item));
                element.init(view);
                model.add(element);
            });
            return model;
        };

    }

});