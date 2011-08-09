using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace EyePatch.Core.Util.Extensions
{
    public static class QueryableExtensions
    {
        private static Func<Expression, bool> CanBeEvaluatedLocally
        {
            get
            {
                return expression =>
                           {
                               // don't evaluate parameters
                               if (expression.NodeType == ExpressionType.Parameter)
                                   return false;

                               // can't evaluate queries
                               if (typeof (IQueryable).IsAssignableFrom(expression.Type))
                                   return false;

                               return true;
                           };
            }
        }

        public static void InvalidateCache<T>(this IQueryable<T> query)
        {
            HttpContext.Current.Cache.Remove(query.CacheKey());
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// The cache entry has a one minute sliding expiration with normal priority.
        /// </summary>
        public static T SingleFromCache<T>(this IQueryable<T> query)
        {
            return query.SingleFromCache(CacheItemPriority.Normal, TimeSpan.FromMinutes(5));
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// </summary>
        public static T SingleFromCache<T>(this IQueryable<T> query,
                                                  CacheItemPriority priority,
                                                  TimeSpan duration)
        {
            // try to get the query result from the cache
            var key = query.CacheKey();
            var result = (T)HttpRuntime.Cache.Get(key);

            if (result == null)
            {
                // todo: ... ensure that the query results do not
                // hold on to resources for your particular data source
                //
                // for entity framework queries, set to NoTracking
                var entityQuery = query as ObjectQuery<T>;
                if (entityQuery != null)
                {
                    entityQuery.MergeOption = MergeOption.NoTracking;
                }

                // materialize the query
                result = query.SingleOrDefault();

                StoreResultInCache(key, result, duration, priority);
            }

            return result;
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// The cache entry has a one minute sliding expiration with normal priority.
        /// </summary>
        public static IEnumerable<T> ListFromCache<T>(this IQueryable<T> query)
        {
            return query.ListFromCache(CacheItemPriority.Normal, TimeSpan.FromMinutes(5));
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// </summary>
        public static IEnumerable<T> ListFromCache<T>(this IQueryable<T> query,
                                                  CacheItemPriority priority,
                                                  TimeSpan duration)
        {
            // try to get the query result from the cache
            var key = query.CacheKey();
            var result = HttpRuntime.Cache.Get(key) as List<T>;

            if (result == null)
            {
                // todo: ... ensure that the query results do not
                // hold on to resources for your particular data source
                //
                // for entity framework queries, set to NoTracking
                var entityQuery = query as ObjectQuery<T>;
                if (entityQuery != null)
                {
                    entityQuery.MergeOption = MergeOption.NoTracking;
                }

                // materialize the query
                result = query.ToList();
                
                if (result != null)
                    StoreResultInCache(key, result, duration, priority);
            }

            return result;
        }

        private static void StoreResultInCache(string key, object result, TimeSpan duration, CacheItemPriority priority)
        {
            HttpRuntime.Cache.Insert(
                key,
                result,
                null, // no cache dependenc
                DateTime.Now.Add(duration),
                Cache.NoSlidingExpiration,
                priority,
                null); // no removal notification
        }

        private static string CacheKey<T>(this IQueryable<T> query)
        {
            // locally evaluate as much of the query as possible
            var expression = Evaluator.PartialEval(
                query.Expression,
                CanBeEvaluatedLocally);

            // use the string representation of the query for the cache key
            string key = expression.ToString();

            // the key is potentially very long, so use an md5 fingerprint
            // (fine if the query result data isn't critically sensitive)
            return key.ToMd5Fingerprint();
        }

        /// <summary>
        /// Creates an MD5 fingerprint of the string.
        /// </summary>
        private static string ToMd5Fingerprint(this string s)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(s.ToCharArray());
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(bytes);

            // concat the hash bytes into one long string
            return hash.Aggregate(new StringBuilder(32),
                                  (sb, b) => sb.Append(b.ToString("X2")))
                .ToString();
        }
    }

    /// <summary>
    /// Enables the partial evalutation of queries.
    /// From http://msdn.microsoft.com/en-us/library/bb546158.aspx
    /// </summary>
    public static class Evaluator
    {
        /// <summary>
        /// Performs evaluation & replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <param name="fnCanBeEvaluated">A function that decides whether a given expression node can be part of the local function.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression PartialEval(Expression expression, Func<Expression, bool> fnCanBeEvaluated)
        {
            return new SubtreeEvaluator(new Nominator(fnCanBeEvaluated).Nominate(expression)).Eval(expression);
        }

        /// <summary>
        /// Performs evaluation & replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression PartialEval(Expression expression)
        {
            return PartialEval(expression, CanBeEvaluatedLocally);
        }

        private static bool CanBeEvaluatedLocally(Expression expression)
        {
            return expression.NodeType != ExpressionType.Parameter;
        }

        #region Nested type: Nominator

        /// <summary>
        /// Performs bottom-up analysis to determine which nodes can possibly
        /// be part of an evaluated sub-tree.
        /// </summary>
        private class Nominator : ExpressionVisitor
        {
            private readonly Func<Expression, bool> fnCanBeEvaluated;
            private HashSet<Expression> candidates;
            private bool cannotBeEvaluated;

            internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
            {
                this.fnCanBeEvaluated = fnCanBeEvaluated;
            }

            internal HashSet<Expression> Nominate(Expression expression)
            {
                candidates = new HashSet<Expression>();
                Visit(expression);
                return candidates;
            }

            public override Expression Visit(Expression expression)
            {
                if (expression != null)
                {
                    bool saveCannotBeEvaluated = cannotBeEvaluated;
                    cannotBeEvaluated = false;
                    base.Visit(expression);
                    if (!cannotBeEvaluated)
                    {
                        if (fnCanBeEvaluated(expression))
                        {
                            candidates.Add(expression);
                        }
                        else
                        {
                            cannotBeEvaluated = true;
                        }
                    }
                    cannotBeEvaluated |= saveCannotBeEvaluated;
                }
                return expression;
            }
        }

        #endregion

        #region Nested type: SubtreeEvaluator

        /// <summary>
        /// Evaluates & replaces sub-trees when first candidate is reached (top-down)
        /// </summary>
        private class SubtreeEvaluator : ExpressionVisitor
        {
            private readonly HashSet<Expression> candidates;

            internal SubtreeEvaluator(HashSet<Expression> candidates)
            {
                this.candidates = candidates;
            }

            internal Expression Eval(Expression exp)
            {
                return Visit(exp);
            }

            public override Expression Visit(Expression exp)
            {
                if (exp == null)
                {
                    return null;
                }
                if (candidates.Contains(exp))
                {
                    return Evaluate(exp);
                }
                return base.Visit(exp);
            }

            private static Expression Evaluate(Expression e)
            {
                if (e.NodeType == ExpressionType.Constant)
                {
                    return e;
                }
                LambdaExpression lambda = Expression.Lambda(e);
                Delegate fn = lambda.Compile();
                return Expression.Constant(fn.DynamicInvoke(null), e.Type);
            }
        }

        #endregion
    }
}