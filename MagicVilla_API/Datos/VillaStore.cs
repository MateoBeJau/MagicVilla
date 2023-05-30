using MagicVilla_API.Modelos.Dto;

namespace MagicVilla_API.Datos
{
    public static class VillaStore
    {
        public static List<VillaDto> villaList = new List<VillaDto>
        {
            new VillaDto{id=1, Nombre="Vista a la piscina", Ocupante= 3, MetrosCuadrados= 50},
            new VillaDto{id=2,Nombre="Vista a la playa",Ocupante= 4, MetrosCuadrados=80}
        };
    }
}
