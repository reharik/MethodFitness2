/**
 * Created by JetBrains RubyMine.
 * User: RHarik
 * Date: 10/6/11
 * Time: 4:52 PM
 * To change this template use File | Settings | File Templates.
 */
if (typeof mf == "undefined") {
            var mf = {};
}


mf.TimeSheetController  = mf.Controller.extend({
    events:_.extend({
    }, mf.Controller.prototype.events),

    initialize:function(options){
        $.extend(this,this.defaults());
        mf.contentLevelControllers["TimeSheetController"]=this;
        $.unsubscribeByPrefix("/contentLevel");
        this.id="TimeSheetController";
        this.registerSubscriptions();

        var _options = $.extend({},this.options, options);
        _options.el="#masterArea";
        $("#viewReport").click($.proxy(function(){
            var trainerId = $("[name='Trainer']").val();
            var start = $("[name=StartDate]").val();
            var end = $("[name=EndDate]").val();
            var url = this.options.reportUrl +"?EntityId="+trainerId+"&Start="+start+"&End="+end;
            $("#reportBody").attr("src",url);
        },this));
    },

    registerSubscriptions: function(){
    }

});
