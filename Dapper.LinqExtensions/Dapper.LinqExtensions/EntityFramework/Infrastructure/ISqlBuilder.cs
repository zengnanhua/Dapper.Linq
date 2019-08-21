using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.LinqExtensions.EntityFramework.Infrastructure
{
    public interface ISqlBuilder
    {
        string Build(Dictionary<string, object> values, string prefix);
    }
}
