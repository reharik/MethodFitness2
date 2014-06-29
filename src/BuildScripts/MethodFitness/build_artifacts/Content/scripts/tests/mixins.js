/**
 * Created by JetBrains RubyMine.
 * User: Owner
 * Date: 10/15/12
 * Time: 8:04 PM
 * To change this template use File | Settings | File Templates.
 */



var viewModel={
    EntityId:1,
    DateCreated:"1/5/1972",
    _Title:"someTitle",
    _someList:"someList",
    _someUrl:"someUrl"
};

var koViewModel={
    EntityId:ko.observable(1),
    DateCreated:ko.observable("1/5/1972"),
    _Title:ko.observable("someTitle"),
    _someList:ko.observable("someList"),
    _someUrl:ko.observable("someUrl")
};

var mix ={
    rawModel:viewModel,
    el:"#someSelector"
};
MF.mixin(mix, "modelAndElementsMixin");



describe("bindModelAndElements", function() {
    beforeEach(function() {
        ko.mapping.fromJS = jasmine.createSpy('fromJS').andReturn(koViewModel);
        spyOn(ko, "applyBindings");
        spyOn(mix, "extendModel");
        spyOn(CC.elementService, "getElementsViewmodel");
        spyOn(mix, "renderElements");
        MF.vent = jasmine.createSpy('vent');
        MF.vent.trigger = jasmine.createSpy('trigger');
        mix.bindModelAndElements(["_someList","_someUrl"]);
    });
    it("should call ko.mapping.fromJs", function() {
        expect(ko.mapping.fromJS).toHaveBeenCalledWith(viewModel,mix.mappingOptions);
    });
    it("should call ko.applyBindings", function() {
        expect(ko.applyBindings).toHaveBeenCalledWith(koViewModel,mix.el);
    });
    it("should call mix.extendModel", function() {
        expect(mix.extendModel).toHaveBeenCalledWith(koViewModel);
    });
    it("should call CC.elementService.getElementsViewmodel", function() {
        expect(CC.elementService.getElementsViewmodel).toHaveBeenCalled();
    });
    it("should call mix.renderElements", function() {
        expect(mix.renderElements).toHaveBeenCalled();
    });
    it("should push ignore elements into array", function() {
        expect(mix.mappingOptions.ignore).toContain("_someList");
        expect(mix.mappingOptions.ignore).toContain("_someUrl");
    });
    it("should filter out any items in ignore or that start with _",function(){
        expect(mix.mappingOptions.ignore).totalCount(5);
    })
});