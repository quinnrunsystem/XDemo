using System;
using AutoMapper;
namespace XDemo.Core.BusinessServices
{
    /// <summary>
    /// Auto mapper between types in core project, not include UI types
    /// </summary>
    public class AutoMapperCoreProfile : Profile
    {
        public AutoMapperCoreProfile()
        {
            /* ==================================================================================================
            * todo: Register mapping type on Core level within this ctor
            * ie: map input dto -> request for api
            * ================================================================================================*/

        }
    }
}
