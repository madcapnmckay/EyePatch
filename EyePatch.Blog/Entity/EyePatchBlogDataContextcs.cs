using System;
using System.Linq;
using EyePatch.Core.Entity;

namespace EyePatch.Blog.Entity
{
    partial class EyePatchBlogDataContext
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