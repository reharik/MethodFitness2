MF.Views.DailyPaymentsView = MF.Views.View.extend({
    initialize: function(){
        MF.mixin(this, "ajaxFormMixin");
        MF.mixin(this, "modelAndElementsMixin");
    },
    viewLoaded:function(){
        $("[name=Date]").val("");
        this.viewReport();
    },
    events:{
        'click #viewReport' : 'viewReport'
    },
    viewReport:function(){
        var date = $("[name=Date]").val();
        var param = "?Date=01/10/1800";
        if(date){
            param = "?Date="+date;
        }
        var url = this.model.ReportUrl()+ param;
        $("#reportBody").attr("src",url);
//        window.location.href(url);
    },
    onClose:function(){
    }


});