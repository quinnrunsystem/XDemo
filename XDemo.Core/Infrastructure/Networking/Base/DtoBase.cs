using System;
using System.Net;
using System.Collections.Generic;

namespace XDemo.Core.Infrastructure.Networking.Base
{
    public abstract class DtoBase
    {
        public ApiResult Result { get; set; }
        /// <summary>
        /// error message from server
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage { get; internal set; }
    }
}
