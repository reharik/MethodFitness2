using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using CC.Core.Domain;
using CC.Core.DomainTools;
using CC.Core.Localization;
using CC.Core.Utilities;

namespace CC.Core.Services
{
    public interface ISelectListItemService
    {
        IEnumerable<SelectListItem> CreateList<ENTITY>(IEnumerable<ENTITY> entityList,
                                                       Expression<Func<ENTITY, object>> text,
                                                       Expression<Func<ENTITY, object>> value, bool addSelectItem);

        IEnumerable<SelectListItem> CreateList<ENTITY>(Expression<Func<ENTITY, object>> text,
                                                       Expression<Func<ENTITY, object>> value, bool addSelectItem)
            where ENTITY : IPersistableObject;


        IEnumerable<SelectListItem> SetSelectedItemByValue(IEnumerable<SelectListItem> entityList,
                                                           string value);

        IEnumerable<SelectListItem> CreateListWithConcatinatedText<ENTITY>(IEnumerable<ENTITY> entityList,
                                                                           Expression<Func<ENTITY, object>> text1,
                                                                           Expression<Func<ENTITY, object>> text2,
                                                                           string seperator,
                                                                           Expression<Func<ENTITY, object>> value,
                                                                           bool addSelectItem);

        IEnumerable<SelectListItem> CreateListWithConcatinatedText<ENTITY>(Expression<Func<ENTITY, object>> text1,
                                                                           Expression<Func<ENTITY, object>> text2,
                                                                           string seperator,
                                                                           Expression<Func<ENTITY, object>> value,
                                                                           bool addSelectItem)
            where ENTITY : IPersistableObject;

        IEnumerable<SelectListItem> CreateLookupList<ENTITY>(IEnumerable<ENTITY> entityList,
                                                             Expression<Func<ENTITY, object>> text,
                                                             Expression<Func<ENTITY, object>> value,
                                                             bool addSelectItem)
            where ENTITY : ILookupType;

        string SetModelValueBySelected(IEnumerable<SelectListItem> list, string value);
        IEnumerable<SelectListItem> CreateList<ENUM>(bool onlyKey = false, bool addSelectItem = false, ENUM defaultEnum = null) where ENUM : Enumeration, new();
    }

    public class SelectListItemService : ISelectListItemService
    {
        private readonly IRepository _repository;

        public SelectListItemService(IRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<SelectListItem> CreateList<ENTITY>(IEnumerable<ENTITY> entityList,
                                                              Expression<Func<ENTITY, object>> text,
                                                              Expression<Func<ENTITY, object>> value, bool addSelectItem)
        {

            IList<SelectListItem> items = new List<SelectListItem>();
            Accessor textAccessor = text.ToAccessor();
            Accessor valueAccessor = value.ToAccessor();
            if (addSelectItem)
            {
                items.Add(new SelectListItem { Text = CCCoreLocalizationKeys.SELECT_ITEM.ToString(), Value = "" });
            }
            entityList.ForEachItem(x =>
            {
                if (textAccessor.GetValue(x) != null && valueAccessor.GetValue(x) != null)
                {
                    items.Add(new SelectListItem
                    {
                        Text = textAccessor.GetValue(x).ToString(),
                        Value = valueAccessor.GetValue(x).ToString()
                    });
                }
            });
            return items.OrderBy(x => x.Text);
        }

        public IEnumerable<SelectListItem> CreateListWithConcatinatedText<ENTITY>(IEnumerable<ENTITY> entityList,
                                                                                  Expression<Func<ENTITY, object>> text1,
                                                                                  Expression<Func<ENTITY, object>> text2,
                                                                                  string seperator,
                                                                                  Expression<Func<ENTITY, object>> value,
                                                                                  bool addSelectItem)
        {

            IList<SelectListItem> items = new List<SelectListItem>();
            Accessor text1Accessor = text1.ToAccessor();
            Accessor text2Accessor = text2.ToAccessor();
            Accessor valueAccessor = value.ToAccessor();
            if (addSelectItem)
            {
                items.Add(new SelectListItem { Text = CCCoreLocalizationKeys.SELECT_ITEM.ToString(), Value = "" });
            }
            entityList.ForEachItem(x =>
            {
                if (text1Accessor.GetValue(x) != null && text2Accessor.GetValue(x) != null &&
                    valueAccessor.GetValue(x) != null)
                {
                    items.Add(new SelectListItem
                    {
                        Text =
                            text1Accessor.GetValue(x).ToString() + seperator +
                            text2Accessor.GetValue(x),
                        Value = valueAccessor.GetValue(x).ToString()
                    });
                }
            });
            return items.OrderBy(x => x.Text);
        }

        public IEnumerable<SelectListItem> CreateListWithConcatinatedText<ENTITY>(
            Expression<Func<ENTITY, object>> text1, Expression<Func<ENTITY, object>> text2, string seperator,
            Expression<Func<ENTITY, object>> value, bool addSelectItem) where ENTITY : IPersistableObject
        {
            var enumerable = _repository.FindAll<ENTITY>();
            return CreateListWithConcatinatedText(enumerable, text1, text2, seperator, value, addSelectItem);
        }

        public IEnumerable<SelectListItem> CreateList<ENTITY>(Expression<Func<ENTITY, object>> text,
                                                              Expression<Func<ENTITY, object>> value, bool addSelectItem) where ENTITY : IPersistableObject
        {
            var enumerable = _repository.FindAll<ENTITY>();
            return CreateList(enumerable, text, value, addSelectItem);
        }

        public IEnumerable<SelectListItem> CreateList<ENUM>(bool onlyKey = false, bool addSelectItem = false, ENUM defaultEnum = null) where ENUM : Enumeration, new()
        {
            IEnumerable<Enumeration> enumerations = Enumeration.GetAllActive<ENUM>();
            if (enumerations == null) return null;
            var items =
                enumerations.Select(x => new SelectListItem { Text = x.Key, Value = onlyKey || x.Value.IsEmpty() ? x.Key : x.Value })
                    .ToList();
            if (enumerations.Any(x => x.Index > 0))
            { enumerations.OrderBy(x => x.Index); }
            else { enumerations.OrderBy(item => item.Key); }
          
            if (addSelectItem)
            {
                items.Insert(0, new SelectListItem { Text = CCCoreLocalizationKeys.SELECT_ITEM.ToString(), Value = "" });
            }

            return items;
        }

        public string SetModelValueBySelected(IEnumerable<SelectListItem> list, string value)
        {
            return (value.IsEmpty() && list.Any(x => x.Selected)) ? list.First(x => x.Selected).Value : value;
        }

        public IEnumerable<SelectListItem> SetSelectedItemByValue(IEnumerable<SelectListItem> entityList, string value)
        {
            SelectListItem selectListItem = entityList.FirstOrDefault(x => x.Value == value);
            if (selectListItem != null)
            {
                selectListItem.Selected = true;
            }
            return entityList;
        }

        public IEnumerable<SelectListItem> CreateLookupList<ENTITY>(IEnumerable<ENTITY> entityList,
                                                                    Expression<Func<ENTITY, object>> text,
                                                                    Expression<Func<ENTITY, object>> value,
                                                                    bool addSelectItem)
            where ENTITY : ILookupType
        {

            IList<SelectListItem> items = new List<SelectListItem>();
            Accessor textAccessor = text.ToAccessor();
            Accessor valueAccessor = value.ToAccessor();
            if (addSelectItem)
            {
                items.Add(new SelectListItem { Text = CCCoreLocalizationKeys.SELECT_ITEM.ToString(), Value = "" });
            }
            entityList.ForEachItem(x =>
            {
                if (textAccessor.GetValue(x) != null && valueAccessor.GetValue(x) != null)
                {
                    items.Add(new SelectListItem
                    {
                        Text = textAccessor.GetValue(x).ToString(),
                        Value = valueAccessor.GetValue(x).ToString()
                    });
                }
            });
            return items;
        }
    }
}
