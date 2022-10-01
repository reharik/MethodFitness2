/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 2/26/12
 * Time: 11:24 AM
 * To change this template use File | Settings | File Templates.
 */

MF.Views.CalendarView = MF.Views.View.extend({
	initialize: function () {
		MF.mixin(this, "modelAndElementsMixin");
	},
	events: {
		"change [name=Location]": "resetCalendar",
		"click .legendLabel": "legendLabelClick",
		"click .legendHeader": "legendHeaderClick",
	},
	render: function () {
		$.when(MF.loadTemplateAndModel(this)).then(
			$.proxy(this.renderCallback, this)
		);
		//       MF.repository.ajaxGet(this.options.url, this.options.data).done($.proxy(this.renderCallback,this));
	},
	renderCallback: function () {
		this.model = this.rawModel;
		this.model.id = this.model.CalendarDefinition.id = this.id;
		this.setupLegend();
		$("div.form-scroll-inner").height(window.innerHeight - 160);
		$("#calendar", this.el).asCalendar(this.model.CalendarDefinition);
		this.bindSpecificModelAndElements({
			Location: this.model.Location,
			_LocationList: this.model._LocationList,
			_Title: this.model._Title,
			EntityId: 0,
		});
		//callback for render
		this.viewLoaded();
		$("#calendar", this.el).fullCalendar(
			"option",
			"height",
			$("div.form-scroll-inner").height()
		);
		//general notification of pageloaded
		MF.vent.trigger("calendar:" + this.id + ":pageLoaded", this.options);
		this.calendarBindings();
	},
	calendarBindings: function () {
		MF.vent.trigger("calendar:" + this.id + ":pageLoaded", this.options);
		MF.vent.bind("calendar:" + this.id + ":eventDrop", this.eventDrop, this);
		MF.vent.bind(
			"calendar:" + this.id + ":eventResize",
			this.eventResize,
			this
		);
		MF.vent.bind("calendar:" + this.id + ":dayClick", this.dayClick, this);
		MF.vent.bind("calendar:" + this.id + ":eventClick", this.eventClick, this);
		MF.vent.bind(
			"ajaxPopupFormModule:editModule:success",
			this.formSuccess,
			this
		);
		MF.vent.bind(
			"ajaxPopupFormModule:editModule:cancel",
			this.formCancel,
			this
		);
		MF.vent.bind(
			"ajaxPopupDisplayModule:displayModule:cancel",
			this.displayCancel,
			this
		);
		MF.vent.bind("popup:displayModule:edit", this.displayEdit, this);
		MF.vent.bind("module:displayModule:cancel", this.displayCancel, this);
	},
	onClose: function () {
		MF.vent.unbind("calendar:" + this.id + ":eventDrop");
		MF.vent.unbind("calendar:" + this.id + ":eventResize");
		MF.vent.unbind("calendar:" + this.id + ":eventClick");
		MF.vent.unbind("calendar:" + this.id + ":dayClick");
		MF.vent.unbind("ajaxPopupFormModule:editModule:success");
		MF.vent.unbind("ajaxPopupFormModule:editModule:cancel");
		MF.vent.unbind("ajaxPopupDisplayModule:displayModule:cancel");
		MF.vent.unbind("popup:displayModule:edit");
		MF.vent.unbind("module:displayModule:cancel");
	},
	setupLegend: function () {
		if (this.model.Trainers.length <= 0) {
			$("#legend").hide();
		}
		$("#legendTemplate").tmpl(this.model.Trainers).appendTo("#legendItems");
		$(".legendHeader").addClass("showing");
		$(".legendLabel").each(function (i, item) {
			$(item).addClass("showing");
		});
	},
	eventDrop: function (event, dayDelta, minuteDelta, allDay, revertFunc) {
		const StartTime = $.fullCalendar.formatDate(event.start, "M/d/yyyy hh:mm TT");
		const EndTime = $.fullCalendar.formatDate(event.end, "M/d/yyyy hh:mm TT");
		if (MF.blockedTimes.shouldBounceAppointment(this.model.blockedSlotsByLocation, StartTime, EndTime, event.locationId)) {
			revertFunc();
			return;
		}
		var data = {
			EntityId: event.EntityId,
			ScheduledDate: $.fullCalendar.formatDate(
				event.start,
				"M/d/yyyy hh:mm TT"
			),
			StartTime,
			EndTime,
		};
		MF.repository
			.ajaxGet(this.model.CalendarDefinition.EventChangedUrl, data)
			.then(
				$.proxy(function (result) {
					this.changeEventCallback(result, revertFunc);
				}, this)
			);
	},
	eventResize: function (
		event,
		dayDelta,
		minuteDelta,
		revertFunc,
		jsEvent,
		ui,
		view
	) {
		revertFunc();
	},
	dayClick: function (date, allDay, jsEvent, view) {
		const targetDate = new XDate(date);
		if (
			targetDate.diffHours(new XDate().addHours(6)) > 0 &&
			!this.model.CalendarDefinition.CanEnterRetroactiveAppointments
		) {
			alert("That period is closed");
			return;
		}

		const targetDateString = targetDate.toString("M/d/yyyy h:mm TT");

		const { blockedMsg, blockedLocs } = MF.blockedTimes.shouldBlockAppointment(
			this.model.blockedSlotsByLocation,
			targetDateString,
			this.model.location
		);
		if (blockedMsg.length > 0) {
			alert(blockedMsg);
			if (this.model.location === "1" || this.model.location === "2") {
				return;
            }
		}
		var data = {
			blockedLocs,
			ScheduledDate: new XDate(date).toString("M/d/yyyy"),
			ScheduledStartTime: new XDate(date).toString("hh:mm TT"),
		};
		this.editEvent(this.model.CalendarDefinition.AddUpdateUrl, data);
	},
	eventClick: function (calEvent, jsEvent, view) {
		if (this.displayAppointmentDisabled == true ) {
			return;
		}
		if (
			(calEvent.trainerId != this.model.CalendarDefinition.TrainerId &&
				!this.model.CalendarDefinition.CanSeeOthersAppointments) ||
			calEvent.EntityId ===0 
		) {
			return;
		}
		this.displayAppointmentDisabled = true;

		this.model.CalendarDefinition.canEdit =
			new XDate(calEvent.start).diffHours(new XDate()) < 0 ||
			this.model.CalendarDefinition.CanEditPastAppointments;
		this.currentEventId = calEvent.EntityId;
		var data = { EntityId: calEvent.EntityId };
		var builder = MF.Views.popupButtonBuilder.builder("displayModule");
		if (
			(this.model.CalendarDefinition.CanSeeOthersAppointments
				&& this.model.CalendarDefinition.CanEditOthersAppointments)
			|| calEvent.trainerId === this.model.CalendarDefinition.TrainerId
		) {
			builder.addButton("Delete", $.proxy(this.deleteItem, this));
			builder.addButton("Edit", $.proxy(this.editItem, this));
			builder.addButton("Copy Event", $.proxy(this.copyItem, this));
		}
		builder.addCancelButton();

		var formOptions = {
			id: "displayModule",
			route: this.model.CalendarDefinition.DisplayRoute,
			templateUrl:
				this.model.CalendarDefinition.DisplayUrl + "_Template?Popup=true",
			view: this.options.subViewName
				? "Display" + this.options.subViewName
				: "",
			AddUpdateUrl: this.model.CalendarDefinition.AddUpdateUrl,
			url: this.model.CalendarDefinition.DisplayUrl,
			data: data,
			buttons: builder.getButtons(),
		};
		this.ajaxPopupDisplay = new MF.Views.AjaxPopupDisplayModule(formOptions);
		this.ajaxPopupDisplay.render();
		this.storeChild(this.ajaxPopupDisplay);
		$(this.el).append(this.ajaxPopupDisplay.el);
		this.displayAppointmentDisabled = false;
	},
	editEvent: function (url, data) {
		if (this.appointmentViewDisabled == true) {
			return;
		}
		this.appointmentViewDisabled = true;
		var formOptions = {
			id: "editModule",
			route: this.model.CalendarDefinition.AddUpdateRoute,
			url: url,
			templateUrl: url + "_Template?Popup=true",
			blockedSlotsByLocation: this.model.blockedSlotsByLocation,
			data,
			view: "AppointmentView",
			buttons: MF.Views.popupButtonBuilder
				.builder("editModule")
				.standardEditButons(),
		};
		this.ajaxPopupFormModule = new MF.Views.AjaxPopupFormModule(formOptions);
		this.ajaxPopupFormModule.render();
		this.storeChild(this.ajaxPopupFormModule);
		$(this.el).append(this.ajaxPopupFormModule.el);
		this.dayClickDisabled = false;
		this.eventClickDisabled = false;
	},

	changeEventCallback: function (result, revertFunc) {
		if (!result.Success) {
			alert(result.Message);
			revertFunc();
		}
	},

	copyItem: function () {
		this.appointmentViewDisabled = false;
		var data = { EntityId: this.currentEventId, Copy: true };
		this.editEvent(this.model.CalendarDefinition.AddUpdateUrl, data);
		this.ajaxPopupDisplay.close();
	},

	editItem: function () {
		this.appointmentViewDisabled = false;
		MF.vent.trigger("popup:displayModule:edit");
		this.ajaxPopupDisplay.close();
	},

	deleteItem: function () {
		if (confirm("Are you sure you would like to delete this Item?")) {
			this.appointmentViewDisabled = false;
			MF.repository
				.ajaxGet(this.model.CalendarDefinition.DeleteUrl, {
					EntityId: this.currentEventId,
				})
				.then(
					$.proxy(function (result) {
						this.ajaxPopupDisplay.close();
						if (!result.Success) {
							alert(result.Message);
						} else {
							this.reload();
						}
					}, this)
				);
		}
	},
	displayEdit: function (event) {
		if (!this.model.CalendarDefinition.canEdit) {
			alert("you can't edit retroactively");
			return;
		}
		this.ajaxPopupDisplay.close();
		this.editEvent(this.model.CalendarDefinition.AddUpdateUrl, {
			EntityId: this.currentEventId,
		});
	},

	resetCalendar: function () {
		let model = this.model;
		model.location = this.viewModel.Location();

		var ids = "";
		$(".legendLabel").each(function (i, item) {
			if ($(item).hasClass("showing")) {
				ids += $("#trainerId", $(item).parent()).val() + ",";
			}
		});
		if (ids) {
			ids = ids.substr(0, ids.length - 1);
		} else if ($(".legend").is(":visible")) {
			ids = "NONE";
		}

		let url = model.CalendarDefinition.Url;
		let source = function (start, end, callback) {
			MF.repository
				.ajaxGet(url, {
					Loc: model.location,
					TrainerIds: ids,
					start: ~~(start.getTime() / 1000),
					end: ~~(end.getTime() / 1000),
				})
				.then((response) => {
					model.blockedSlotsByLocation = MF.blockedTimes.calculate(response.BlockedSlotsByLocation);
					callback(response.Events);
				});
		};
		this.replaceSource({ events: source });
		this.reload();
	},
	reload: function () {
		$("#calendar", this.el).fullCalendar("refetchEvents");
	},

	replaceSource: function (source) {
		$("#calendar", this.el).fullCalendar("replaceEventSource", source);
	},
	legendLabelClick: function (e) {
		$(e.target).toggleClass("showing");
		this.resetCalendar();
	},
	legendHeaderClick: function (e) {
		if ($(e.target).hasClass("showing")) {
			$(".legendHeader").removeClass("showing");
			$(".legendLabel").each(function (i, item) {
				$(item).removeClass("showing");
			});
		} else {
			$(".legendHeader").addClass("showing");
			$(".legendLabel").each(function (i, item) {
				if (!$(item).hasClass("showing")) {
					$(item).addClass("showing");
				}
			});
		}
		this.resetCalendar();
	},

	formSuccess: function () {
		this.formCancel();
		this.resetCalendar();
	},
	formCancel: function () {
		this.ajaxPopupFormModule.close();
		this.appointmentViewDisabled = false;
	},
	displayCancel: function () {
		this.ajaxPopupDisplay.close();
	},
});
MF.Views.AppointmentView = MF.Views.View.extend({
	initialize: function () {
		MF.mixin(this, "formMixin");
		MF.mixin(this, "ajaxFormMixin");
		MF.mixin(this, "modelAndElementsMixin");
	},
	events: {
		'change [name="AppointmentType"]': "onApptTypeChange",
		'change [name="LocationEntityId"]': "setDisabledOptionsOnLocation",
		'change [name="StartTimeString"]': "handleTimeChange",
		"click #save": "saveItem",
		"click #cancel": "cancel",
	},
	viewLoaded: function () {
		var startDate = new XDate("1/5/1972t" + this.model.StartTimeString());
		this.setEndTime(startDate);
		MF.vent.bind(
			"ClientsDtos:tokenizer:add",
			this.onApptTypeChange,
			this
		);
		MF.vent.bind(
			"ClientsDtos:tokenizer:remove",
			this.onApptTypeChange,
			this
		);
		this.onApptTypeChange();
		this.setDisabledOptionsOnLocation();
		this.setDisabledOptionsOnStartTime();
		this.setDisabledOptionsOnApptType();
	},

	setCurrentSelectionForAptType: function (isPair) {
		var currentSelection = $("[name='AppointmentType']").val();
		if (isPair) {
			$("[name='AppointmentType']").data.previousSelection = currentSelection;
			$("[name='AppointmentType']").val("Pair");
			this.model.AppointmentType("Pair");
		} else {
			if (currentSelection == "Pair") {
				var previousSelection = $("[name='AppointmentType']").data
					.previousSelection;
				if (previousSelection == "Pair") {
					previousSelection = "Hour";
				}
				$("[name='AppointmentType']").val(previousSelection);
				this.model.AppointmentType(previousSelection);
			}
		}
	},

	onApptTypeChange: function () {
		var isPair = $(this.model.ClientsDtos.selectedItems()).size() > 1;
		this.setCurrentSelectionForAptType(isPair);
		this.disableApptTypeBasedOnSelected(isPair);
		this.handleTimeChange();
		this.setDisabledOptionsOnStartTime();

	},
	disableApptTypeBasedOnSelected: function (isPair) {
		$("[name='AppointmentType'] option").each(function () {
			var $item = $(this);
			if ($item.text() == "Pair") {
				if (isPair) {
					$item.removeAttr("disabled");
				} else {
					$item.attr("disabled", "disabled");
				}
			} else {
				if (isPair) {
					$item.attr("disabled", "disabled");
				} else {
					$item.removeAttr("disabled");
				}
			}
		});
	},
	setDisabledOptionsOnLocation: function () {
		const blockedLocs = this.options.data.blockedLocs;
		$("[name='LocationEntityId'] option").each(function () {
			var $item = $(this);
			if (blockedLocs?.includes($item.val())) {
				$item.attr("disabled", "disabled");
			}
		});
		this.setDisabledOptionsOnStartTime();
		this.setDisabledOptionsOnApptType();
		this.setDisabledOptionsOnStartTime();
	},

	setDisabledOptionsOnApptType: function () {
		const blockedLocs = this.options.blockedSlotsByLocation;
		const date = new XDate(this.model.Date()).toString("M/d/yyyy");
		const blockedSlots = Object.keys(blockedLocs[this.model.LocationEntityId()] || {})
			.filter(x => x.startsWith(date));
		const options = $("[name='StartTimeString'] option");
		if (options.length <= 0) {
			return;
		}
		let startIdx = 0;
		for (var i = 0; i < options.length; i++) {
			if ($(options[i]).text() === this.model.StartTimeString()) {
				startIdx = i;
				break;
			}
		}
		let hourLong = true;
		if (blockedSlots.includes(`${date} ${$(options[startIdx + 2]).text()}`)
			|| blockedSlots.includes(`${date} ${$(options[startIdx + 3]).text()}`)) {
			hourLong = false;
		}
		if (!hourLong) {
			this.model.AppointmentType("Half Hour");
		}
		$("[name='AppointmentType'] option").each(function () {
			var $item = $(this);
			if (!hourLong && ($item.text() == "Pair" || $item.text() == "Hour")) {
				$item.attr("disabled", "disabled");
			} else {
				$item.removeAttr("disabled");
            }
		});
	},
	setDisabledOptionsOnStartTime: function () {
		const blockedLocs = this.options.blockedSlotsByLocation;
		let apptSlots = 2;
		switch (this.model.AppointmentType()) {
			case "Hour":
			case "Pair":
				apptSlots = 4;
				break;
			case "Half Hour":
				apptSlots = 2;
				break;
		}
		const date = new XDate(this.model.Date()).toString("M/d/yyyy");
		const blocked = Object.keys(blockedLocs[this.model.LocationEntityId()] || {})
			.filter(x => x.startsWith(date));
		const options = $("[name='StartTimeString'] option");
		for (var i = 0; i < options.length; i++) {
			if (blocked.includes(`${date} ${$(options[i]).text()}`)) {
				let s = 0;
				while (s < apptSlots && i - s >= 0) {
					$(options[i - s]).attr("disabled", "disabled");
					s = s + 1;
                }
            }
        }
	},
	onClose: function () {
		MF.vent.unbind("ClientsDtos:tokenizer:add");
		MF.vent.unbind("ClientsDtos:tokenizer:remove");
	},
	handleTimeChange: function () {
		var startTime = new XDate(
			this.model.Date().split("T")[0] + "t" + this.model.StartTimeString()
		);
		this.setEndTime(startTime);
		this.setDisabledOptionsOnApptType();

		return startTime;
	},
	setEndTime: function (startTime) {
		var aptMin;
		switch (this.model.AppointmentType()) {
			case "Hour":
			case "Pair":
				aptMin = 60;
				break;
			case "Half Hour":
				aptMin = 30;
				break;
		}
		var endTime = startTime.addMinutes(aptMin);
		var endHour = endTime.getHours();
		var amPm = "AM";
		if (endHour > 12) {
			endHour -= 12;
			amPm = "PM";
		}
		var endMin = endTime.getMinutes().toString();
		if (endMin.length == 1) {
			endMin = "0" + endMin;
		}
		this.model.EndTimeString(endHour + ":" + endMin + " " + amPm);
	},
});
MF.Views.ClientFormView = MF.Views.View.extend({
	events: _.extend({
		"click .client_payment": "payment",
		"click .delete": "deleteItem",
		"click #save": "saveItem",
		"click #cancel": "cancel",
	}),
	initialize: function () {
		MF.mixin(this, "formMixin");
		MF.mixin(this, "ajaxFormMixin");
		MF.mixin(this, "modelAndElementsMixin");
	},
	viewLoaded: function () {
		this._setupBindings();
	},
	_setupBindings: function () {
		MF.vent.bind("delete:" + this.id + ":success", this.deleteSuccess, this);
	},
	_unbindBindings: function () {
		MF.vent.unbind("delete:" + this.id + ":success", this.deleteSuccess, this);
	},
	payment: function () {
		var id = this.model.EntityId();
		MF.vent.trigger("route", MF.generateRoute("clientpurchaselist", id), true);
	},
	deleteItem: function () {
		if (confirm("Are you sure you would like to delete this Item?")) {
			var id = this.model.EntityId();
			MF.repository.ajaxPost(
				this.model._deleteUrl,
				{ EntityId: id },
				$.proxy(this.deleteCallback, this)
			);
		}
	},
	deleteCallback: function (_result) {
		var result = typeof _result == "string" ? JSON.parse(_result) : _result;
		if (!CC.notification.handleResult(result, this.cid)) {
			return;
		}
		MF.vent.trigger("delete:" + this.id + ":success", result);
	},
	deleteSuccess: function (result) {
		MF.WorkflowManager.returnParentView(result, true);
	},
	onClose: function () {
		this._unbindBindings();
		this._super("onClose", arguments);
	},
});
MF.Views.ClientPurchaseListView = MF.Views.View.extend({
	initialize: function () {
		this.options.gridOptions = { multiselect: false };
		MF.mixin(this, "ajaxGridMixin");
		MF.mixin(this, "setupGridMixin");
		MF.mixin(this, "defaultGridEventsMixin");
		MF.mixin(this, "setupGridSearchMixin");
	},
	addNew: function () {
		var parentId = this.options.ParentId;
		MF.vent.trigger(
			"route",
			MF.generateRoute(this.options.addUpdate, 0, parentId),
			true
		);
	},
	editItem: function (id) {
		var parentId = this.options.ParentId;
		MF.vent.trigger(
			"route",
			MF.generateRoute(this.options.addUpdate, id, parentId),
			true
		);
	},
	displayItem: function (id) {
		var parentId = this.options.ParentId;
		MF.vent.trigger(
			"route",
			MF.generateRoute(this.options.display, id, parentId),
			true
		);
	},
	viewLoaded: function () {
		this.setupBindings();
	},
	onClose: function () {
		this.unbindBindings();
	},
});

MF.Views.ClientPurchaseView = MF.Views.View.extend({
	initialize: function () {
		MF.mixin(this, "formMixin");
		MF.mixin(this, "ajaxFormMixin");
		MF.mixin(this, "modelAndElementsMixin");
	},
	viewLoaded: function () {
		$("[name='FullHour']").change(
			$.proxy(function (e) {
				this.calculateTotal("FullHour");
			}, this)
		);
		$("[name='HalfHour']").change(
			$.proxy(function (e) {
				this.calculateTotal("HalfHour");
			}, this)
		);
		$("[name='FullHourTenPack']").change(
			$.proxy(function (e) {
				this.calculateTotal("FullHourTenPack");
			}, this)
		);
		$("[name='HalfHourTenPack']").change(
			$.proxy(function (e) {
				this.calculateTotal("HalfHourTenPack");
			}, this)
		);
		$("[name='Pair']").change(
			$.proxy(function (e) {
				this.calculateTotal("Pair");
			}, this)
		);
		$("[name='PairTenPack']").change(
			$.proxy(function (e) {
				this.calculateTotal("PairTenPack");
			}, this)
		);
	},
	calculateTotal: function (type) {
		var number = this.model[type]();
		var itemTotal = this.model._sessionRateDto[type]() * number;
		this.model[type + "Price"](itemTotal);
		var total =
			this.model.FullHourPrice() +
			this.model.HalfHourPrice() +
			this.model.FullHourTenPackPrice() +
			this.model.HalfHourTenPackPrice() +
			this.model.PairPrice() +
			this.model.PairTenPackPrice();
		this.model.PaymentTotal(total);
	},
});

MF.Views.TrainerFormView = MF.Views.View.extend({
	initialize: function () {
		MF.mixin(this, "formMixin");
		MF.mixin(this, "ajaxFormMixin");
		MF.mixin(this, "modelAndElementsMixin");
	},
	events: {
		"click #trainerPayments": "trainerPayments",
		"click #payTrainer": "payTrainer",
		"click #save": "saveItem",
		"click #cancel": "cancel",
		"click .tokenEditor": "tokenEditor",
	},
	viewLoaded: function () {
		var that = this;
		$("#colorPickerInput", this.el).miniColors({
			change: function (hex) {
				that.model.Color(hex);
			},
		});
		MF.vent.bind("popup:templatePopup:save", this.tokenSave, this);
		MF.vent.bind("popup:templatePopup:cancel", this.tokenCancel, this);
	},
	onClose: function () {
		MF.vent.unbind("popup:templatePopup:save", this.tokenSave, this);
		MF.vent.unbind("popup:templatePopup:cancel", this.tokenCancel, this);
	},
	multiSelectModifier: function (ccElement) {
		if (ccElement.name == "ClientsDtos") {
			ccElement.multiSelectOptions = {
				internalTokenMarkup: function () {
					var anchor = $("<a>")
						.addClass("selectedItem")
						.attr("data-bind", "text:display");
					var anchor2 = $("<a>")
						.addClass("tokenEditor stdTxtColor")
						.text(" --Edit")
						.attr("href", "javascript:void(0);")
						.attr("data-bind", "attr:{rel:percentage}");
					return $("<p>").append(anchor).append(anchor2);
				},
				beforeTokenAddedFunction: $.proxy(this.beforeTokenAddedFunction, this),
			};
		}
	},
	tokenEditor: function (e) {
		var $li = $(e.target).closest("li");
		this.currentlyEditing = $li;
		var id = $li.attr("id");
		var percentage = $(e.target).attr("rel");
		var dataItem = { id: id, percentage: percentage };
		var buttons = this.options.buttons
			? this.options.buttons
			: MF.Views.popupButtonBuilder
					.builder("templatePopup")
					.standardEditButons();
		var popupOptions = {
			id: "templatePopup",
			buttons: buttons,
			data: dataItem,
			template: "#percentageTemplate",
		};
		this.templatedPopupView = new MF.Views.TemplatedPopupView(popupOptions);
		this.templatedPopupView.render();
		this.storeChild(this.templatedPopupView);
	},
	tokenSave: function () {
		var id = $("#editingId").val();
		var newVal = $("#newTrainerPercentage").val();
		_.find(
			this.model.ClientsDtos.selectedItems(),
			function (item) {
				if (item.id() == id) {
					var anchorText = $(this.currentlyEditing.find("a").first()).text();
					item.display(
						anchorText.substring(0, anchorText.indexOf("(")) +
							"( " +
							newVal +
							" ) "
					);
					item.percentage(newVal);
					return true;
				}
			},
			this
		);
		this.templatedPopupView.close();
	},
	tokenCancel: function () {
		this.templatedPopupView.close();
	},
	beforeTokenAddedFunction: function (item) {
		item.percentage = ko.observable(this.model.ClientRateDefault());
		item.display = ko.observable(item.name() + "(" + item.percentage() + ")");
	},
	trainerPayments: function () {
		var rel = MF.State.get("Relationships");
		MF.vent.trigger("route", "trainerpaymentlist/" + rel.entityId, true);
	},
	payTrainer: function () {
		var rel = MF.State.get("Relationships");
		MF.vent.trigger("route", "paytrainerlist/" + rel.entityId, true);
	},
});

MF.Views.TrainerEditableTokenView = MF.Views.EditableTokenView.extend({
	events: _.extend(
		{
			"click .tokenEditor": "tokenEditor",
		},
		MF.Views.EditableTokenView.prototype.events
	),
	internalTokenMarkup: function (item) {
		var cssClass = "class='selectedItem'";
		return (
			"<p><a " +
			cssClass +
			">" +
			item.name +
			" ( " +
			item.percentage +
			" )</a><a href='javascript:void(0);' class='tokenEditor' >&nbsp;-- Edit</a><input id='itemId' type='hidden' value='" +
			item.id +
			"' </p>"
		);
	},
	render: function () {
		MF.vent.bind("popup:templatePopup:save", this.tokenSave, this);
		MF.vent.bind("popup:templatePopup:cancel", this.tokenCancel, this);
	},
	onClose: function () {
		MF.vent.unbind("popup:templatePopup:save");
		MF.vent.unbind("popup:templatePopup:cancel");
	},
	afterTokenSelectedFunction: function (item) {
		if (!$(this.options.inputSelector, this.el).data("selectedItems"))
			$(this.options.inputSelector, this.el).data("selectedItems", []);
		$(this.options.inputSelector, this.el).data("selectedItems").push(item);
	},
	deleteToken: function (hidden_input, token_data) {
		var data = $(this.options.inputSelector, this.el).data("selectedItems");
		var idx = 0;
		$.each(data, function (i, item) {
			if (item.id == hidden_input.id) {
				idx = i;
			}
		});
		data.splice(idx, 1);
	},
	tokenEditor: function (e) {
		this.options.currentlyEditing = $(e.target).prev("a");
		var id = $(e.target).next("input#itemId").val();
		var data = $(this.options.inputSelector, this.el).data("selectedItems");
		var dataItem;
		$.each(data, function (i, item) {
			if (item.id == id) dataItem = item;
		});
		var buttons = this.options.buttons
			? this.options.buttons
			: MF.Views.popupButtonBuilder
					.builder("templatePopup")
					.standardEditButons();
		var popupOptions = {
			id: "templatePopup",
			buttons: buttons,
			data: dataItem,
			template: "#percentageTemplate",
		};
		this.templatedPopupView = new MF.Views.TemplatedPopupView(popupOptions);
		this.templatedPopupView.render();
		this.storeChild(this.templatedPopupView);
	},
	tokenSave: function () {
		var id = $("#editingId").val();
		var data = $(this.options.inputSelector, this.options.el).data(
			"selectedItems"
		);
		var dataItem;
		$.each(data, function (i, item) {
			if (item.id == id) dataItem = item;
		});
		dataItem.percentage = $("#newTrainerPercentage").val();
		var anchor = $(this.options.currentlyEditing).text();
		var newText =
			anchor.substr(0, anchor.indexOf("(")) +
			"( " +
			$("#newTrainerPercentage").val() +
			" ) ";
		$(this.options.currentlyEditing).text(newText);
		//        MF.vent.unbind("popup:templatePopup:save");
		this.templatedPopupView.close();
	},
	tokenCancel: function () {
		this.templatedPopupView.close();
	},
});

MF.Views.TrainerGridView = MF.Views.View.extend({
	initialize: function () {
		this.options.gridOptions = { multiselect: false };
		MF.mixin(this, "ajaxGridMixin");
		MF.mixin(this, "setupGridMixin");
		MF.mixin(this, "defaultGridEventsMixin");
		MF.mixin(this, "setupGridSearchMixin");
	},
	addNew: function () {
		var parentId = this.options.ParentId;
		MF.vent.trigger(
			"route",
			MF.generateRoute(this.options.addUpdate, 0, parentId),
			true
		);
	},
	editItem: function (id) {
		var parentId = this.options.ParentId;
		MF.vent.trigger(
			"route",
			MF.generateRoute(this.options.addUpdate, id, parentId),
			true
		);
	},
	viewLoaded: function () {
		this.setupBindings();
		MF.vent.bind(this.options.gridId + ":Redirect", this.showPayGrid, this);
		MF.vent.bind(this.options.gridId + ":Other", this.archiveTrainer, this);
	},
	_setupBindings: function () {
		//MF.vent.bind("Redirect",this.showPayGrid,this);
		MF.vent.bind("gridComplete", this.modifyArchiveColumn, this);
		//        MF.vent.bind("grid:"+this.id+":pageLoaded",this.modifyArchiveColumn,this);
	},
	_unbindBindings: function () {
		MF.vent.unbind(this.options.gridId + ":Redirect", this.showPayGrid, this);
		MF.vent.unbind(this.options.gridId + ":Other", this.archiveTrainer, this);
		MF.vent.unbind("gridComplete", this.modifyArchiveColumn, this);
	},
	modifyArchiveColumn: function () {
		var grid = $("#" + this.options.gridId);
		var rows = grid.jqGrid("getRowData");
		grid
			.find("tr")
			.not(".jqgfirstrow")
			.each(function (idx, row) {
				var span = $($(row).find("td").last().find("span"));
				if (span.text() === "True" || span.text() === "UnArchive") {
					span.text("UnArchive");
					$(row).addClass("gridRowGrey");
				} else if (span.text() === "False" || span.text() === "Archive") {
					span.text("Archive");
				}
			});
	},
	showPayGrid: function (id) {
		MF.vent.trigger("route", "paytrainerlist/" + id, true);
	},
	archiveTrainer: function (id) {
		MF.repository
			.ajaxGet(this.options.ArchiveTrainerUrl, { EntityId: id })
			.then($.proxy(this.successHandler, this));
	},
	onClose: function () {
		this.unbindBindings();
	},
});
MF.Views.ClientGridView = MF.Views.View.extend({
	initialize: function () {
		this.options.gridOptions = { multiselect: false };
		MF.mixin(this, "ajaxGridMixin");
		MF.mixin(this, "setupGridMixin");
		MF.mixin(this, "defaultGridEventsMixin");
		MF.mixin(this, "setupGridSearchMixin");
	},
	viewLoaded: function () {
		MF.vent.bind(this.options.gridId + ":Redirect", this.showPayGrid, this);
		MF.vent.bind(this.options.gridId + ":Other", this.archiveClient, this);
		this.setupBindings();
	},
	_setupBindings: function () {
		MF.vent.bind("gridComplete", this.modifyArchiveColumn, this);
	},
	_unbindBindings: function () {
		MF.vent.unbind(this.options.gridId + ":Other", this.archiveClient, this);
		MF.vent.unbind("gridComplete", this.modifyArchiveColumn, this);
	},
	modifyArchiveColumn: function () {
		var grid = $("#" + this.options.gridId);
		var rows = grid.jqGrid("getRowData");
		grid
			.find("tr")
			.not(".jqgfirstrow")
			.each(function (idx, row) {
				var span = $($(row).find("td").last().find("span"));
				if (span.text() === "True" || span.text() === "UnArchive") {
					span.text("UnArchive");
					$(row).addClass("gridRowGrey");
				} else if (span.text() === "False" || span.text() === "Archive") {
					span.text("Archive");
				}
			});
	},
	archiveClient: function (id) {
		MF.repository
			.ajaxGet(this.options.ArchiveClientUrl, { EntityId: id })
			.then($.proxy(this.successHandler, this));
	},
	onClose: function () {
		MF.vent.unbind(this.options.gridId + ":Redirect", this.showPayGrid, this);
		this.unbindBindings();
	},
	showPayGrid: function (id) {
		MF.vent.trigger("route", MF.generateRoute("clientpurchaselist", id), true);
	},
});
