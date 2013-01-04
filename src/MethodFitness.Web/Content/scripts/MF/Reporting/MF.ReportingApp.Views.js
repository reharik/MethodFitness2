MF.Views.DailyPaymentsView = MF.Views.View.extend({
    initialize: function(){
        MF.mixin(this, "ajaxFormMixin");
        MF.mixin(this, "modelAndElementsMixin");
    },
    events:{
        'click #viewReport' : 'viewReport'
    },
    viewReport:function(){
        var start = $("[name=StartDate]").val();
        var url = this.model.ReportUrl()+"?Start="+start;
        $("#reportBody").attr("src",url);
    },
    onClose:function(){
    }


});