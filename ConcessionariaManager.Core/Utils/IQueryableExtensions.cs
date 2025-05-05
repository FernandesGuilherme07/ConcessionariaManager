using System;
using System.Linq.Expressions;
namespace ConcessionariaManager.Core.Utils
{
    public static class IQueryableExtensions
    {
        /// <summary>
        /// Aplica um filtro Where apenas se a condição for verdadeira.
        /// </summary>
        /// <typeparam name="T">O tipo de entidade no IQueryable.</typeparam>
        /// <param name="query">A query base.</param>
        /// <param name="condition">A condição para aplicar o filtro.</param>
        /// <param name="predicate">A expressão do filtro.</param>
        /// <returns>O IQueryable com o filtro aplicado, se a condição for verdadeira.</returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition ? query.Where(predicate) : query;
        }

        /// <summary>
        /// Aplica um filtro Where apenas se o valor não for nulo.
        /// </summary>
        /// <typeparam name="T">O tipo de entidade no IQueryable.</typeparam>
        /// <typeparam name="TValue">O tipo do valor a ser verificado.</typeparam>
        /// <param name="query">A query base.</param>
        /// <param name="value">O valor que será verificado.</param>
        /// <param name="predicate">A expressão do filtro.</param>
        /// <returns>O IQueryable com o filtro aplicado, se o valor não for nulo.</returns>
        public static IQueryable<T> WhereIf<T, TValue>(this IQueryable<T> query, TValue value, Expression<Func<T, bool>> predicate)
        {
            return value != null ? query.Where(predicate) : query;
        }
    }

}
