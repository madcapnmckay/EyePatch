using System;

namespace EyePatch.Core.Entity
{
    public interface IAuditModifiedDate
    {
        DateTime? LastModified { get; set; }
    }
}