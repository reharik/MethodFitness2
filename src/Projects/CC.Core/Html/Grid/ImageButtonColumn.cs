using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using CC.Core.Services;
using HtmlTags;

namespace CC.Core.Html.Grid
{
    public class ImageButtonColumn<ENTITY> : ImageColumn<ENTITY> where ENTITY : IGridEnabledClass
    {
        private readonly SiteConfigurationBase _config;
        private string _actionUrl;
        public string ActionUrl
        {
            get { return _actionUrl; }
            set { _actionUrl = value; }
        }
        private string _action;
        private string _jsonData;
        private string _gridName;
        private string _id;

        public ImageButtonColumn(SiteConfigurationBase config) : base(config)
        {
            _config = config;
        }

        public ImageButtonColumn<ENTITY> ForAction<CONTROLLER>(Expression<Func<CONTROLLER, object>> expression, string gridName = "") where CONTROLLER : Controller
        {
            _id = "gridContainer";
            _gridName = gridName;
            var actionUrl = UrlContext.GetUrlForAction(expression);
            _actionUrl = actionUrl;
            return this;
        }

        public ImageButtonColumn<ENTITY> ToPerformAction(ColumnAction action)
        {
            _action = action.ToString();
            return this;
        }

        public override string BuildColumn(object item, string gridName = "")
        {
            // if a name is given in the controller it overrides the name given in the grid declaration
            if (gridName.IsNotEmpty()) _gridName = gridName;
            var _item = (ENTITY)item;
            var value = FormatValue(_item);
            if (value.IsEmpty()) return null;
            var divTag = BuildDiv();
            divTag.AddClasses(new[] { "imageButtonColumn", _action });
            var anchor = buildAnchor(_item);
            var image = BuildImage();
            divTag.Children.Add(image);
            anchor.Children.Add(divTag);
            return anchor.ToString();
        }

        private HtmlTag buildAnchor(ENTITY item)
        {
            var anchor = new HtmlTag("a");
            var id = _id.IsNotEmpty() ? _id + ":" : "";
            string data = string.Empty;
            if (_jsonData.IsNotEmpty())
            {
                data = "," + _jsonData;
            }
            anchor.Attr("onclick", _config.jsApplicationName+".vent.trigger('" + id + _action + "'," + item.EntityId + data + ")");

            return anchor;
        }

        public void AddDataToEvent(string jsonObject)
        {
            _jsonData = jsonObject;
        }

        public ImageButtonColumn<ENTITY> WithId(string id)
        {
            _id = id;
            return this;
        }
    }
}
