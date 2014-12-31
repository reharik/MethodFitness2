﻿using StructureMap.Configuration.DSL;

namespace CC.Core.DataValidation
{
    public class DataValidationRegistry : Registry
    {
        public DataValidationRegistry()
        {
            Scan(x =>
            {
                x.TheCallingAssembly();
                x.WithDefaultConventions();
            });
        }
    }
}