MF.Views.DailyPaymentsView = MF.Views.View.extend({
    initialize: function(){
        MF.mixin(this, "ajaxFormMixin");
        MF.mixin(this, "modelAndElementsMixin");
        MF.mixin(this, "reportMixin");
    },
    createUrl:function(){
        return this.model.ReportUrl()+ "?StartDate="+this.model.StartDate()+"&EndDate="+this.model.EndDate();
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