using System;

namespace EyePatch.Core.Audit
{
    public interface IAuditModifiedDate
    {
        DateTime? LastModified { get; set; }
    }
}