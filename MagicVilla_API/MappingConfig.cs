using AutoMapper;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;

namespace MagicVilla_API
{
    public class MappingConfig :Profile //Hereda Profile
    {
        public MappingConfig()
        {
            //Convertir un objeto en otro de dos manera
            CreateMap<Villa, VillaDto>();
            CreateMap<VillaDto, Villa>();


            //Se crea una copia del objeto vila a villaDto
            CreateMap<Villa,VillaCreateDto>().ReverseMap();
            CreateMap <Villa, VillaUpdateDto>().ReverseMap();
        }
    }
}
