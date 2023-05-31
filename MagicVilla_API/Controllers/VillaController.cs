using AutoMapper;
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
        private readonly IMapper _mapper;
        public VillaController(ILogger<VillaController> logger, ApplicationDbContext db, IMapper mapper)
        {

            _logger = logger;   
            _db = db;
            _mapper = mapper;

        }
        [HttpGet]
        public async Task< ActionResult<IEnumerable<VillaDto>>> GetVillas()
        {
            _logger.LogInformation("Obtener las villas");
            //return Ok(VillaStore.villaList);

            IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<VillaDto>>(villaList)); //Devuelve una lista de villaDto de villaList QUE ES UNA LISTA DE VILLAS

        }


        [HttpGet("id:int", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<ActionResult<VillaDto>> GetVilla(int id)
        {
            //Si el id es 0 devuelve un badRequest
            if (id == 0)
            {
                _logger.LogInformation($"Id {id} es incorrecto");
                return BadRequest();
            }
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.id == id);
            var villa = await _db.Villas.FirstOrDefaultAsync(v=> v.id == id);


            //Si es null devuelve un NotFound 404
            if (villa == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IEnumerable <VillaDto>>(villa));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)] //diferentes tipos codigos de estado
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task <ActionResult<VillaDto>> CrearVilla([FromBody] VillaCreateDto createVillaDto) //Se resive de alguien una villaDto
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Validacion personalizada
            if (await _db.Villas.FirstOrDefaultAsync(v => v.Nombre.ToLower() == createVillaDto.Nombre.ToLower()) != null)
            {
                ModelState.AddModelError("NombreExiste", "La villa con ese nombre ya existe");
                return BadRequest(ModelState);
            }
            if (createVillaDto == null)
            {
                return BadRequest(createVillaDto);
            }

            Villa model = _mapper.Map<Villa>(createVillaDto);
            //Para crear la villa se le pasa villaDat que son los datos de la villa y con add se agrega a la bdd


            await _db.Villas.AddAsync(model); // Se agregar
            await _db.SaveChangesAsync(); //Se guarda 

            return CreatedAtRoute("GetVilla", new { id = model.id }, model); //Tiene que devolver la url de la villa creada

        }




        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task <IActionResult> DelteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = await _db.Villas.FirstOrDefaultAsync(x => x.id == id);
            if(villa == null)
            {
                return NotFound();
            }
            _db.Villas.Remove(villa);
            await _db.SaveChangesAsync();


            return NoContent();
        }

        [HttpPut ("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task <IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateVillaDto)
        {
            if (updateVillaDto == null || id!= updateVillaDto.id)
            {
                return BadRequest();
            }


            Villa model = _mapper.Map<Villa>(updateVillaDto);



            _db.Villas.Update (model);
            await _db.SaveChangesAsync();

            return NoContent();

        }
        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public  async Task <IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if (patchDto == null || id ==0)
            {
                return BadRequest();
            }
            var villa =await _db.Villas.AsNoTracking().FirstOrDefaultAsync (v=> v.id ==id);


            VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(patchDto);

            if (villa == null)  return BadRequest();
            patchDto.ApplyTo(villaDto, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Villa model = _mapper.Map<Villa>(villaDto);


            _db.Villas.Update(model);
            await _db.SaveChangesAsync();

            return NoContent();

        }
    }
}
