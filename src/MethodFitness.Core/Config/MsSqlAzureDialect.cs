﻿using NHibernate.Dialect;

namespace MethodFitness.Core.Config
{
    public class MsSqlAzureDialect : MsSql2008Dialect
    {
        public override string PrimaryKeyString
        {
            get { return "primary key CLUSTERED"; }
        }
    }
}