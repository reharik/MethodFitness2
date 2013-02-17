MF.Views.DailyPaymentsView = MF.Views.View.extend({
    initialize: function(){
        MF.mixin(this, "ajaxFormMixin");
        MF.mixin(this, "modelAndElementsMixin");
        MF.mixin(this, "reportMixin");
    },
    createUrl:function(){
        var trainerId = this.model.Trainer()>0?this.model.Trainer():0;
        var clientId = this.model.Client()>0?this.model.Client():0;
        return this.model.ReportUrl()+ "?StartDate="+this.model.StartDate()+" 00:01&EndDate="+this.model.EndDate()+" 23:59" +
            "&TrainerId="+trainerId+"&ClientId="+clientId;
    }
});

MF.Views.TrainerMetricView = MF.Views.View.extend({
    initialize: function(){
        MF.mixin(this, "ajaxFormMixin");
        MF.mixin(this, "modelAndElementsMixin");
        MF.mixin(this, "reportMixin");
    },
    createUrl:function(){
        var startDate = this.model.StartDate()+" 00:01";
        var endDate = this.model.EndDate()+" 23:59";
        var trainerId = this.model.TrainerEntityId();
        return this.model.ReportUrl()+ "?TrainerId="+trainerId+"&EndDate="+endDate+"&StartDate="+startDate;
    }
});