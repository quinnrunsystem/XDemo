using System;
using System.Net;
using System.Collections.Generic;

namespace XDemo.Core.Infrastructure.Networking.Base
{
    public abstract class DtoBase
    {
        public ApiResult Result { get; set; }
        public string ErrorMessage { get; internal set; }
    }

    public sealed class ListDtoBase<T>:DtoBase
    {
        public ListDtoBase()
        {
            Items = new List<T>();
        }
        public List<T> Items { get; set; }
    }
}
