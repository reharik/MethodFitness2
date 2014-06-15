using System;
using Castle.Components.Validator;

namespace CC.Core.Domain
{
    [Serializable]
    public class LocalizedProperty : Entity, ILocalizedItem
    {
        [ValidateLength(500)]
        public virtual string Name { get; set; }
        [ValidateLength(500)]
        public virtual string ParentType { get; set; }
        public virtual string Culture { get; set; }
        [ValidateLength(500)]
        public virtual string Text { get; set; }
        [ValidateLength(500)]
        public virtual string Tooltip { get; set; }
    }
}