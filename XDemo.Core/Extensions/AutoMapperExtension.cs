using System;
using System.Collections;
using System.Collections.Generic;
using AutoMapper;

namespace XDemo.Core.Extensions
{
    /// <summary>
    /// Extensions used for Automapper
    /// </summary>
    public static class AutoMapperExtension
    {
        public static List<TResult> MapTo<TResult>(this IEnumerable self)
        {
            return self == null ? null : (List<TResult>)Mapper.Map(self, self.GetType(), typeof(List<TResult>));
        }

        public static TResult MapTo<TResult>(this object self) where TResult : class
        {
            return self == null ? null : (TResult)Mapper.Map(self, self.GetType(), typeof(TResult));
        }

        public static TResult MapTo<TResult>(this object self, TResult dest) where TResult : class
        {
            return self == null ? null : (TResult)Mapper.Map(self, dest, self.GetType(), typeof(TResult));
        }
    }
}
