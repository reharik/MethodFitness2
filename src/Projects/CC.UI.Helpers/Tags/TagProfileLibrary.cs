using CC.Core.Utilities;

namespace CC.UI.Helpers.Tags
{
    public class TagProfileLibrary
    {
        private readonly Cache<string, TagProfile> _profiles =
            new Cache<string, TagProfile>((key => new TagProfile(key)));

        public TagProfile DefaultProfile
        {
            get { return _profiles[TagProfile.DEFAULT]; }
        }

        public TagProfile this[string name]
        {
            get { return _profiles[name]; }
        }

        public void ImportProfile(TagProfile profile)
        {
            _profiles[profile.Name].Import(profile);
        }

        public void ImportRegistry(HtmlConventionRegistry registry)
        {
            foreach (TagProfile tagProfile in registry.Profiles)
            {
                ImportProfile(tagProfile);
            }
        }

        public void Seal()
        {
            TagProfile defaults = DefaultProfile;
            _profiles.Each((p =>
                {
                    if (p == defaults)
                        return;
                    p.Import(defaults);
                }));
        }
    }
}