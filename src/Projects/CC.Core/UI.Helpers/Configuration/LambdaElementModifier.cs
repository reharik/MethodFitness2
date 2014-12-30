﻿using System;

namespace CC.Core.UI.Helpers.Configuration
{
    public class LambdaElementModifier : IElementModifier
    {
        private readonly Func<AccessorDef, bool> _matches;
        private readonly Func<AccessorDef, TagModifier> _modifierBuilder;

        public LambdaElementModifier(Func<AccessorDef, bool> matches, Func<AccessorDef, TagModifier> modifierBuilder)
        {
            _matches = matches;
            _modifierBuilder = modifierBuilder;
        }

        #region IElementModifier Members

        public TagModifier CreateModifier(AccessorDef accessorDef)
        {
            return _matches(accessorDef) ? _modifierBuilder(accessorDef) : null;
        }

        #endregion
    }
}