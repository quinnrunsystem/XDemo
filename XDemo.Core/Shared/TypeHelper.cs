using System;
using System.Collections;
namespace XDemo.Core.Shared
{
    public static class TypeHelper
    {
        /// <summary>
        /// check if a type is a collection type, ie: List, ObservableCollection, Array...
        /// </summary>
        /// <param name="inputType">Input type.</param>
        public static bool IsEnumerableType(Type inputType)
        {
            if (inputType == null)
                throw new ArgumentNullException(nameof(inputType), $"Your input type can not be null!");

            /* ==================================================================================================
             * Why use IEnumerable instead of others? => bc this is highest supper class of all collection types
             * ================================================================================================*/
            return inputType.GetInterface(nameof(IEnumerable)) != null;
        }
        /// <summary>
        /// Count the specified Enumerable src.
        /// </summary>
        /// <returns>The count.</returns>
        /// <param name="src">Source.</param>
        public static int Count(IEnumerable src)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src), "Can not count a null source");

            var rs = 0;
            var enumerator = src.GetEnumerator();
            while (enumerator.MoveNext())
                rs++;
            /* ==================================================================================================
             * Cleanup. We can not use 'using' directly here. Just manual cast
             * bc IEnumberable does not implement IDisposable like IEnumerable<>
             * ================================================================================================*/
            var disposable = enumerator as IDisposable;
            disposable?.Dispose();
            return rs;
        }
    }
}
