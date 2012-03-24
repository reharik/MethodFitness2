using System.Diagnostics;
using NHibernate;

namespace MethodFitness.Core.Domain.Tools
{
    public class SqlStatementInterceptor : EmptyInterceptor
    {
        public override NHibernate.SqlCommand.SqlString OnPrepareStatement(NHibernate.SqlCommand.SqlString sql)
        {
            Debug.WriteLine(sql.ToString());
            return sql;
        }
    }
}