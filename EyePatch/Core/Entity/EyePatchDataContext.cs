using System;
using System.Linq;

namespace EyePatch.Core.Entity
{
    partial class EyePatchDataContext
    {
        public override void SubmitChanges(System.Data.Linq.ConflictMode failureMode)
        {
            var changeSet = this.GetChangeSet();

            foreach (var created in changeSet.Inserts.OfType<IAuditCreatedDate>())
                created.Created = DateTime.UtcNow;

            foreach (var created in changeSet.Inserts.OfType<IAuditModifiedDate>())
                created.LastModified = DateTime.UtcNow;

            foreach (var updated in changeSet.Updates.OfType<IAuditModifiedDate>())
                updated.LastModified = DateTime.UtcNow;
            
            base.SubmitChanges(failureMode);
        }
    }
}