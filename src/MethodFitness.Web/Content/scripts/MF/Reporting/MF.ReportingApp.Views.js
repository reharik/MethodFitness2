MF.Views.DailyPaymentsView = MF.Views.View.extend({
    initialize: function(){
        MF.mixin(this, "ajaxFormMixin");
        MF.mixin(this, "modelAndElementsMixin");
        MF.mixin(this, "reportMixin");
    },
    viewLoaded:function(){
        $("[name=Date]").val("");
        this.viewReport();
    },
    createUrl:function(){
        var date = this.model.Date();
        var param = "?Date=01/01/1800";
        if(date){
            param = "?Date="+date;
        }
        var url = this.model.ReportUrl()+ param;
        return url;
    }
});

MF.Views.TrainerMetricView = MF.Views.View.extend({
    initialize: function(){
        MF.mixin(this, "ajaxFormMixin");
        MF.mixin(this, "modelAndElementsMixin");
        MF.mixin(this, "reportMixin");
    },
    createUrl:function(){
        var startDate = this.model.StartDate();
        var endDate = this.model.EndDate();
        var trainerId = this.model.TrainerEntityId();
        return this.model.ReportUrl()+ "?TrainerId="+trainerId+"&EndDate="+endDate+"&StartDate="+startDate;
    }
});