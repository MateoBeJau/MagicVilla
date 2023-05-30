using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        
        private readonly ILogger<VillaController> _logger;
        private readonly ApplicationDbContext _db;
        public VillaController(ILogger<VillaController> logger, ApplicationDbContext db)
        {

            _logger = logger;   
            _db = db;

        }
        [HttpGet]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            _logger.LogInformation("Obtener las villas");
            //return Ok(VillaStore.villaList);
            return Ok(_db.Villas.ToList()); //Es como hacer un select * from villas

        }


        [HttpGet("id:int", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public ActionResult<VillaDto> GetVilla(int id)
        {
            //Si el id es 0 devuelve un badRequest
            if (id == 0)
            {
                _logger.LogInformation($"Id {id} es incorrecto");
                return BadRequest();
            }
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.id == id);
            var villa = _db.Villas.FirstOrDefault(v=> v.id == id);


            //Si es null devuelve un NotFound 404
            if (villa == null)
            {
                return NotFound();
            }

            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)] //diferentes tipos codigos de estado
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDto> CrearVilla([FromBody] VillaDto villaDto) //Se resive de alguien una villaDto
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Validacion personalizada
            if (_db.Villas.FirstOrDefault(v => v.Nombre.ToLower() == villaDto.Nombre.ToLower()) != null)
            {
                ModelState.AddModelError("NombreExiste", "La villa con ese nombre ya existe");
                return BadRequest(ModelState);
            }
            if (villaDto == null)
            {
                return BadRequest(villaDto);
            }
            if (villaDto.id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            //Para crear la villa se le pasa villaDat que son los datos de la villa y con add se agrega a la bdd
            Villa model = new()
            {
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImagenUrl = villaDto.ImagenUrl,
                Ocupante = villaDto.Ocupante,
                Tarifa = villaDto.Tarifa,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                Amenidad = villaDto.Amenidad

            };

            _db.Villas.Add(model); // Se agregar
            _db.SaveChanges(); //Se guarda 

            return CreatedAtRoute("GetVilla", new { id = villaDto.id }, villaDto); //Tiene que devolver la url de la villa creada

        }




        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DelteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = _db.Villas.FirstOrDefault(x => x.id == id);
            if(villa == null)
            {
                return NotFound();
            }
            _db.Villas.Remove(villa);


            return NoContent();
        }

        [HttpPut ("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDto villaDto)
        {
            if (villaDto == null || id!=villaDto.id)
            {
                return BadRequest();
            }
            //var villa = _db.Villas.FirstOrDefault(v=>v.id == id);
            //villa.Nombre = villaDto.Nombre;
            //villa.Ocupante = villaDto.Ocupante;
            //villa.MetrosCuadrados = villaDto.MetrosCuadrados;

            Villa model = new()
            {
                id = villaDto.id,
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImagenUrl = villaDto.ImagenUrl,
                Ocupante = villaDto.Ocupante,
                Tarifa = villaDto.Tarifa,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                Amenidad = villaDto.Amenidad
            };

            _db.Villas.Update (model);
            _db.SaveChanges();

            return NoContent();

        }
        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDto)
        {
            if (patchDto == null || id ==0)
            {
                return BadRequest();
            }
            var villa = _db.Villas.AsNoTracking().FirstOrDefault (v=> v.id ==id);

            VillaDto villaDto = new()
            {
                id = villa.id,
                Nombre = villa.Nombre,
                Detalle = villa.Detalle,
                ImagenUrl = villa.ImagenUrl,
                Ocupante = villa.Ocupante,
                Tarifa = villa.Tarifa,
                MetrosCuadrados = villa.MetrosCuadrados,
                Amenidad = villa.Amenidad
            };
            if (villa == null)  return BadRequest();
            patchDto.ApplyTo(villaDto, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Villa model = new()
            {
                id = villaDto.id,
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImagenUrl = villaDto.ImagenUrl,
                Ocupante = villaDto.Ocupante,
                Tarifa = villaDto.Tarifa,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                Amenidad = villaDto.Amenidad
            };

            _db.Villas.Update(model);
            _db.SaveChanges();

            return NoContent();

        }
    }
}
