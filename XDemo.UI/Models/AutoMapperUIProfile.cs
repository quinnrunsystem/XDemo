using System;
using AutoMapper;
using XDemo.Core.BusinessServices.Dtos.Photos;
using XDemo.Core.BusinessServices.Interfaces.Photos;
using XDemo.UI.Models.Photos;
namespace XDemo.UI.Models
{
    /// <summary>
    /// Auto mapper for UI types.
    /// </summary>
    public class AutoMapperUIProfile : Profile
    {
        public AutoMapperUIProfile()
        {
            /* ==================================================================================================
             * todo: Register mapping type on UI level within this ctor
             * ================================================================================================*/
            CreateMap<PhotoDto, Photo>();

            /* ==================================================================================================
             * In case you want to mapping between fields which not match naming convention or difference data type
             * You can do like this:
             * 
             * CreateMap<PhotoDto, Photo>().ForMember(dest => dest.AlbumId, map => map.MapFrom(src => src.AlbumIdOfDto));
             * ================================================================================================*/

        }
    }
}
