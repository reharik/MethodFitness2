MF.Views.DailyPaymentsView = MF.Views.View.extend({
    initialize: function(){
        MF.mixin(this, "ajaxFormMixin");
        MF.mixin(this, "modelAndElementsMixin");
        MF.mixin(this, "reportMixin");
    },
    createUrl:function(){
        var trainerId = this.model.Trainer()>0?this.model.Trainer():0;
        var clientId = this.model.Client()>0?this.model.Client():0;
        return this.model.ReportUrl()+ "?StartDate="+this.model.StartDate()+"&EndDate="+this.model.EndDate() +
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
        var startDate = this.model.StartDate();
        var endDate = this.model.EndDate();
        var trainerId = this.model.TrainerEntityId();
        return this.model.ReportUrl()+ "?TrainerId="+trainerId+"&EndDate="+endDate+"&StartDate="+startDate;
    }
});

MF.Views.ActivityView = MF.Views.View.extend({
    initialize: function(){
        MF.mixin(this, "ajaxFormMixin");
        MF.mixin(this, "modelAndElementsMixin");
        MF.mixin(this, "reportMixin");
    },
    createUrl:function(){
        var trainerId = this.model.Trainer()>0?this.model.Trainer():0;
        var clientId = this.model.Client()>0?this.model.Client():0;
        var locationId = this.model.Location()>0?this.model.Location():0;
        return this.model.ReportUrl()+ "?StartDate="+this.model.StartDate()+"&EndDate="+this.model.EndDate() +
            "&TrainerId="+trainerId+"&ClientId="+clientId+"&LocationId="+locationId;
    }
});

MF.Views.ManagerView = MF.Views.View.extend({
    initialize: function(){
        MF.mixin(this, "ajaxFormMixin");
        MF.mixin(this, "modelAndElementsMixin");
        MF.mixin(this, "reportMixin");
    },
    createUrl:function(){
        var trainerId = this.model.Trainer()>0?this.model.Trainer():0;
        var clientId = this.model.Client()>0?this.model.Client():0;
        return this.model.ReportUrl()+ "?StartDate="+this.model.StartDate()+"&EndDate="+this.model.EndDate() +
            "&TrainerId="+trainerId+"&ClientId="+clientId;
    }
});
