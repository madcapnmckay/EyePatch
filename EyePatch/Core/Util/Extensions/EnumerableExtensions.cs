using System;
using System.Collections.Generic;
using System.Linq;

namespace EyePatch.Core.Util.Extensions
{
    public static class EnumerableExtensions
    {
        private static IEnumerable<HierarchyNode<TEntity>>
            CreateHierarchy<TEntity, TProperty>(
            IEnumerable<TEntity> allItems,
            TEntity parentItem,
            Func<TEntity, TProperty> idProperty,
            Func<TEntity, TProperty> parentIdProperty,
            object rootItemId,
            int maxDepth,
            int depth) where TEntity : class
        {
            IEnumerable<TEntity> childs;

            if (rootItemId != null)
            {
                childs = allItems.Where(i => idProperty(i).Equals(rootItemId));
            }
            else
            {
                if (parentItem == null)
                {
                    childs = allItems.Where(i => parentIdProperty(i).Equals(default(TProperty)));
                }
                else
                {
                    childs = allItems.Where(i => parentIdProperty(i).Equals(idProperty(parentItem)));
                }
            }

            if (childs.Count() > 0)
            {
                depth++;

                if ((depth <= maxDepth) || (maxDepth == 0))
                {
                    foreach (var item in childs)
                        yield return
                            new HierarchyNode<TEntity>
                                {
                                    Entity = item,
                                    ChildNodes =
                                        CreateHierarchy(allItems.AsEnumerable(), item, idProperty, parentIdProperty,
                                                        null, maxDepth, depth),
                                    Depth = depth,
                                    Parent = parentItem
                                };
                }
            }
        }

        /// <summary>
        ///   LINQ to Objects (IEnumerable) AsHierachy() extension method
        /// </summary>
        /// <typeparam name = "TEntity">Entity class</typeparam>
        /// <typeparam name = "TProperty">Property of entity class</typeparam>
        /// <param name = "allItems">Flat collection of entities</param>
        /// <param name = "idProperty">Func delegete to Id/Key of entity</param>
        /// <param name = "parentIdProperty">Func delegete to parent Id/Key</param>
        /// <returns>Hierarchical structure of entities</returns>
        public static IEnumerable<HierarchyNode<TEntity>> AsHierarchy<TEntity, TProperty>(
            this IEnumerable<TEntity> allItems,
            Func<TEntity, TProperty> idProperty,
            Func<TEntity, TProperty> parentIdProperty) where TEntity : class
        {
            return CreateHierarchy(allItems, default(TEntity), idProperty, parentIdProperty, null, 0, 0);
        }

        /// <summary>
        ///   LINQ to Objects (IEnumerable) AsHierachy() extension method
        /// </summary>
        /// <typeparam name = "TEntity">Entity class</typeparam>
        /// <typeparam name = "TProperty">Property of entity class</typeparam>
        /// <param name = "allItems">Flat collection of entities</param>
        /// <param name = "idProperty">Func delegete to Id/Key of entity</param>
        /// <param name = "parentIdProperty">Func delegete to parent Id/Key</param>
        /// <param name = "rootItemId">Value of root item Id/Key</param>
        /// <returns>Hierarchical structure of entities</returns>
        public static IEnumerable<HierarchyNode<TEntity>> AsHierarchy<TEntity, TProperty>(
            this IEnumerable<TEntity> allItems,
            Func<TEntity, TProperty> idProperty,
            Func<TEntity, TProperty> parentIdProperty,
            object rootItemId) where TEntity : class
        {
            return CreateHierarchy(allItems, default(TEntity), idProperty, parentIdProperty, rootItemId, 0, 0);
        }

        /// <summary>
        ///   LINQ to Objects (IEnumerable) AsHierachy() extension method
        /// </summary>
        /// <typeparam name = "TEntity">Entity class</typeparam>
        /// <typeparam name = "TProperty">Property of entity class</typeparam>
        /// <param name = "allItems">Flat collection of entities</param>
        /// <param name = "idProperty">Func delegete to Id/Key of entity</param>
        /// <param name = "parentIdProperty">Func delegete to parent Id/Key</param>
        /// <param name = "rootItemId">Value of root item Id/Key</param>
        /// <param name = "maxDepth">Maximum depth of tree</param>
        /// <returns>Hierarchical structure of entities</returns>
        public static IEnumerable<HierarchyNode<TEntity>> AsHierarchy<TEntity, TProperty>(
            this IEnumerable<TEntity> allItems,
            Func<TEntity, TProperty> idProperty,
            Func<TEntity, TProperty> parentIdProperty,
            object rootItemId,
            int maxDepth) where TEntity : class
        {
            return CreateHierarchy(allItems, default(TEntity), idProperty, parentIdProperty, rootItemId, maxDepth, 0);
        }

        public static T FirstOrNull<T>(this IEnumerable<T> list)
        {
            if (list.Any())
                return list.First();
            return default(T);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            return source.Where(element => seenKeys.Add(keySelector(element)));
        }
    }

    public class HierarchyNode<T> where T : class
    {
        public T Entity { get; set; }
        public IEnumerable<HierarchyNode<T>> ChildNodes { get; set; }
        public int Depth { get; set; }
        public T Parent { get; set; }
    }
}