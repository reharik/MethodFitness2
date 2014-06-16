﻿using CC.Security.Model;
using FluentNHibernate.Mapping;

namespace MF.Core.Domain.Persistence
{
    public class OperationsMap : ClassMap<Operation>
    {
        public OperationsMap()
        {
            Cache.ReadWrite().Region("cc-security");
            Table("Operations");
            Id(x => x.EntityId);

            Map(x => x.Comment).Length(1000);
            Map(x => x.Name).Length(255).Unique().Not.Nullable().Not.Update();
            References(x => x.Parent).Column("ParentId");
            HasMany(x => x.Children).AsSet().KeyColumn("ParentId").LazyLoad().Inverse().Cache.ReadWrite().Region("cc-security");
        }

    }
}