﻿// This file was automatically generated by the PetaPoco T4 Template
// Do not make changes directly to this file - edit the template instead
// 
// The following connection settings were used to generate this file
// 
//     Provider:               `System.Data.SqlClient`
//     Connection String:      `Data Source=CannibalCoder.cloudapp.net;Initial Catalog=MethodFitness_PROD;User ID=methodfitness;password=**zapped**;`
//     Schema:                 ``
//     Include Views:          `False`

//     Factory Name:          `SqlClientFactory`
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using FluentMigrator;

namespace Migrations
{
    [Migration(10070)]
    public class Create_DailyPayments_Sproc : Migration
    {
        public override void Up()
        {
            string sql = System.IO.File.ReadAllText(@"src\dbfluentmigration\CreateDailyPaymentsSproc.sql");
            Execute.Sql(sql);
        }

        public override void Down()
        {
            
        }
    }
}


