using System.ComponentModel.DataAnnotations;

namespace MagicVilla_API.Modelos.Dto
{
    public class VillaUpdateDto
    {
        [Required]
        public int id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Nombre { get; set; }
        public string Detalle { get; set; }
        [Required]
        public double Tarifa { get; set; }
        [Required]

        public int Ocupante { get; set; }
        [Required]

        public int MetrosCuadrados { get; set; }
        [Required]

        public string ImagenUrl { get; set; }
        [Required]

        public string Amenidad { get; set; }
    }
}
