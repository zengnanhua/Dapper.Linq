using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.LinqExtensions.EntityFramework.Entities
{
    public enum LockType
    {
        FOR_UPADTE,
        LOCK_IN_SHARE_MODE,
        UPDLOCK,
        NOLOCK
    }
}
