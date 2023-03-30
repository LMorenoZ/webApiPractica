using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webApiPractica.Models;
using Microsoft.EntityFrameworkCore;

namespace webApiPractica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class carrerasController : ControllerBase
    {
        private readonly equiposContext _equiposContext;

        public carrerasController(equiposContext equiposContext)
        {
            _equiposContext = equiposContext;
        }

        // EndPoint que devuelve todos los registros
        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            List<carreras> listado_carreras = (from e in _equiposContext.carreras
                                                             select e).ToList();

            if (listado_carreras.Count() == 0) { return NotFound(); }

            return Ok(listado_carreras);
        }

        // EndPoint que devuelve un registro identificado por su Id
        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            carreras? carreras = (from e in _equiposContext.carreras
                                 where e.carrera_id == id
                                 select e).FirstOrDefault();

            if (carreras == null) { return NotFound(); }

            return Ok(carreras);
        }

        // EndPoint que permite crear un nuevo registro. Usa el método POST
        [HttpPost]
        [Route("Add")]
        public IActionResult GuardarRegistro([FromBody] carreras carreras)
        {
            try
            {
                _equiposContext.carreras.Add(carreras);
                _equiposContext.SaveChanges();
                return Ok(carreras);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // EndPoint para modificar un registro identificado por su id
        [HttpPut]
        [Route("actualizar/{id}")]
        public IActionResult ActualizarRegistro(int id, [FromBody] carreras carreras_modificar)
        {
            // Se obtiene el registro original en la db
            carreras? carreras_actual = (from e in _equiposContext.carreras
                                                       where e.carrera_id == id
                                                       select e).FirstOrDefault();

            // Se verifica que exista ese registro
            if (carreras_actual == null) { return NotFound(); }

            // Una vez obtenido el registro, se procede a cambiar las respectivas propiedades que se actualizarían
            carreras_actual.nombre_carrera = carreras_modificar.nombre_carrera;
            carreras_actual.facultad_id = carreras_modificar.facultad_id;

            // Se procede a marcar el registro como modificado y se envía a la db los datos actualizados del registro
            _equiposContext.Entry(carreras_actual).State = EntityState.Modified;
            _equiposContext.SaveChanges();

            return Ok(carreras_actual);
        }

        // EndPoint para eliminar un registro identificado por su Id
        [HttpDelete]
        [Route("eliminar/{id}")]
        public IActionResult EliminarRegistro(int id)
        {
            // Obtener el registro de la bd indicado por su id
            carreras? carreras = (from e in _equiposContext.carreras
                                                where e.carrera_id == id
                                                select e).FirstOrDefault();

            // Verificar que haya devuelto un registro, que debe ser el que se va a eliminar
            if (carreras == null) { return NotFound(); }

            // Ejecutar la acción para eliminar
            _equiposContext.carreras.Attach(carreras);
            _equiposContext.carreras.Remove(carreras);
            _equiposContext.SaveChanges();

            return Ok(carreras);

        }
    }
}
