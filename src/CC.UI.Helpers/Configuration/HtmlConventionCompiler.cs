//using System;
//using CCUIHelpers.Tags;
//
//namespace CCUIHelpers.Configuration
//{
//    public interface IConfigurationAction
//    {
//        void Configure(BehaviorGraph graph);
//    }
//
//    public class HtmlConventionCompiler : IConfigurationAction
//    {
//        public void Configure(BehaviorGraph graph)
//        {
//            TagProfileLibrary tagProfileLibrary = new TagProfileLibrary();
//            foreach (var registry in graph.Services.FindAllValues<HtmlConventionRegistry>())
//            {
//                tagProfileLibrary.ImportRegistry(registry);
//            }
//
//
//            tagProfileLibrary.ImportRegistry((HtmlConventionRegistry)new DefaultHtmlConventions());
//            tagProfileLibrary.Seal();
//            graph.Services.ClearAll<HtmlConventionRegistry>();
//            graph.Services.ReplaceService<TagProfileLibrary>(tagProfileLibrary);
//            graph.Services.SetServiceIfNone<IElementNamingConvention, DefaultElementNamingConvention>();
//            graph.Services.SetServiceIfNone<ILabelAndFieldLayout, DefinitionListLabelAndField>();
//        }
//    }
//}
