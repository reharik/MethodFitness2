﻿using System;
using System.Collections.Generic;
using CC.Core.Reflection;
using CC.Core.UI.Helpers.Tags;

namespace CC.Core.UI.Helpers
{
    public class HtmlConventionRegistry : TagProfileExpression
    {
        private readonly Cache<string, TagProfile> _profiles =
            new Cache<string, TagProfile>(name => new TagProfile(name));

        public HtmlConventionRegistry() : base(new TagProfile(TagProfile.DEFAULT))
        {
            _profiles[TagProfile.DEFAULT] = profile;
        }

        public IEnumerable<TagProfile> Profiles
        {
            get { return _profiles.GetAll(); }
        }

        public void Profile(string profileName, Action<TagProfileExpression> configure)
        {
            var profileExpression = new TagProfileExpression(_profiles[profileName]);
            configure(profileExpression);
        }
    }
}