using Cassette;
using Cassette.Scripts;
using Cassette.Stylesheets;

namespace MethodFitness.Web
{
    using System;
    using System.Collections.Generic;

    using MethodFitness.Web.Config;

    /// <summary>
    /// Configures the Cassette asset bundles for the web application.
    /// </summary>
    public class CassetteBundleConfiguration : IConfiguration<BundleCollection>
    {
        public void Configure(BundleCollection bundles)
        {
            // TODO: Configure your bundles here...
            // Please read http://getcassette.net/documentation/configuration

            // This default configuration treats each file as a separate 'bundle'.
            // In production the content will be minified, but the files are not combined.
            // So you probably want to tweak these defaults!

            bundles.Add<ScriptBundle>("content/scripts", this.JavascriptFileList());
            bundles.Add<StylesheetBundle>("Content/css", CssFileList(), bundle => bundle.EmbedImages());

            // To combine files, try something like this instead:
            //   bundles.Add<StylesheetBundle>("Content");
            // In production mode, all of ~/Content will be combined into a single bundle.
            
            // If you want a bundle per folder, try this:
            //   bundles.AddPerSubDirectory<ScriptBundle>("Scripts");
            // Each immediate sub-directory of ~/Scripts will be combined into its own bundle.
            // This is useful when there are lots of scripts for different areas of the website.
        }
        public List<string> CssFileList()
        {
            var fileNames = new List<String>();
            fileNames.Add("bootstrap.css");
            fileNames.Add("jquery.miniColors.css");
            fileNames.Add("fullcalendar.css");
            fileNames.Add("token-input.css");
            fileNames.Add("smoothness/jquery-ui-1.10.0.custom.css");
            fileNames.Add("fileinput.css");
            fileNames.Add("ui.jqgrid.css");
            fileNames.Add("fg.menu.css");
            fileNames.Add("main.css");
            fileNames.Add("mf.css");
            fileNames.Add("CC.css");

            return fileNames;
        } 
        public List<string> JavascriptFileList()
        {
            var fileNames = new List<String>();
            fileNames.Add("jqueryPlugins/jquery-1.9.0.js");
            fileNames.Add("jqueryPlugins/jquery-migrate-1.0.0.js");
            fileNames.Add("jqueryPlugins/jquery-ui-1.10.0.custom.js");
            fileNames.Add("jqueryPlugins/cc.tokeninput2.js");
            fileNames.Add("jqueryPlugins/grid.locale-en.js");
            fileNames.Add("jqueryPlugins/jquery.jqGrid.src.js");
            fileNames.Add("jqueryPlugins/jquery-tmpl.js");
            fileNames.Add("jqueryPlugins/jquery.miniColors.js");
            fileNames.Add("jqueryPlugins/jquery.fileinput.js");

            fileNames.Add("jqueryPlugins/noty/jquery.noty.js");
            fileNames.Add("jqueryPlugins/noty/inline.js");
            fileNames.Add("jqueryPlugins/noty/default.js");

            fileNames.Add("externalHelpers/fullcalendar.js");
            fileNames.Add("externalHelpers/underscore.js");
            fileNames.Add("externalHelpers/backbone.js");
            fileNames.Add("externalHelpers/backbone.marionette.js");
            fileNames.Add("externalHelpers/knockout-2.1.0.debug.js");
            fileNames.Add("externalHelpers/knockout.mapping-lastest.debug.js");

            fileNames.Add("MF/MF.App.js");
            fileNames.Add("MF/MF.Controller.js");
            fileNames.Add("MF/MF.Mixins.js");
            fileNames.Add("MF/MF.Header.js");
            fileNames.Add("MF/MF.Views.js");
            fileNames.Add("MF/MF.OneOffViews.js");
            fileNames.Add("MF/MF.Routing.js");
            fileNames.Add("MF/MF.State.js");
            fileNames.Add("MF/MF.Services.js");
            fileNames.Add("MF/MF.WorkflowManager.js");
            fileNames.Add("MF/Schedule/MF.ScheduleApp.js");
            fileNames.Add("MF/Schedule/MF.ScheduleApp.Menu.js");
            fileNames.Add("MF/Schedule/MF.ScheduleApp.Routing.js");
            fileNames.Add("MF/Schedule/MF.ScheduleApp.Views.js");
            fileNames.Add("MF/Billing/MF.BillingApp.Routing.js");
            fileNames.Add("MF/Billing/MF.BillingApp.Views.js");

            fileNames.Add("MF/Reporting/MF.ReportingApp.Routing.js");
            fileNames.Add("MF/Reporting/MF.ReportingApp.Views.js");

            fileNames.Add("MF/CC.ModelBinding/CC.CustomBindings.js");
            fileNames.Add("MF/CC.ModelBinding/CC.Elements.js");
            fileNames.Add("MF/CC.ModelBinding/CC.NotificationService.js");
            fileNames.Add("MF/CC.ModelBinding/CC.ModelService.js");
            fileNames.Add("MF/CC.ModelBinding/CC.ValidationRules.js");
            fileNames.Add("MF/CC.ModelBinding/CC.ValidationRunner.js");
            fileNames.Add("MF/CC.ModelBinding/CC.AjaxFileUpload.js");

            fileNames.Add("externalHelpers/superfish.js");
            fileNames.Add("externalHelpers/ba-debug.js");
            fileNames.Add("internalHelpers/cc.ccMenu.js");
            fileNames.Add("internalHelpers/jquery.cc.grid.js");
            fileNames.Add("internalHelpers/cc.grid.columnService.js");
            fileNames.Add("internalHelpers/cc.gridHelpers.js");
            fileNames.Add("internalHelpers/mf.gridSearch.js");
            fileNames.Add("internalHelpers/MF.repository.js");
            fileNames.Add("internalHelpers/jquery.mf.calendar.js");

            fileNames.Add("externalHelpers/json2.js");
            fileNames.Add("externalHelpers/xdate.js");

            return fileNames;
        }
    }
}