using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webApiPractica.Models;
using Microsoft.EntityFrameworkCore;

namespace webApiPractica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class facultadesController : ControllerBase
    {
        private readonly equiposContext _equiposContext;

        public facultadesController(equiposContext equiposContext)
        {
            _equiposContext = equiposContext;
        }

        // EndPoint que devuelve todos los registros
        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            List<facultades> listado_facultades = (from e in _equiposContext.facultades
                                               select e).ToList();

            if (listado_facultades.Count() == 0) { return NotFound(); }

            return Ok(listado_facultades);
        }

        // EndPoint que devuelve un registro identificado por su Id
        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            facultades? facultades = (from e in _equiposContext.facultades
                                  where e.facultad_id == id
                                  select e).FirstOrDefault();

            if (facultades == null) { return NotFound(); }

            return Ok(facultades);
        }

        // EndPoint que permite crear un nuevo registro. Usa el método POST
        [HttpPost]
        [Route("Add")]
        public IActionResult GuardarRegistro([FromBody] facultades facultades)
        {
            try
            {
                _equiposContext.facultades.Add(facultades);
                _equiposContext.SaveChanges();
                return Ok(facultades);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // EndPoint para modificar un registro identificado por su id
        [HttpPut]
        [Route("actualizar/{id}")]
        public IActionResult ActualizarRegistro(int id, [FromBody] facultades facultades_modificar)
        {
            // Se obtiene el registro original en la db
            facultades? facultades_actual = (from e in _equiposContext.facultades
                                         where e.facultad_id == id
                                         select e).FirstOrDefault();

            // Se verifica que exista ese registro
            if (facultades_actual == null) { return NotFound(); }

            // Una vez obtenido el registro, se procede a cambiar las respectivas propiedades que se actualizarían
            facultades_actual.nombre_facultad = facultades_modificar.nombre_facultad;

            // Se procede a marcar el registro como modificado y se envía a la db los datos actualizados del registro
            _equiposContext.Entry(facultades_actual).State = EntityState.Modified;
            _equiposContext.SaveChanges();

            return Ok(facultades_actual);
        }

        // EndPoint para eliminar un registro identificado por su Id
        [HttpDelete]
        [Route("eliminar/{id}")]
        public IActionResult EliminarRegistro(int id)
        {
            // Obtener el registro de la bd indicado por su id
            facultades? facultades = (from e in _equiposContext.facultades
                                  where e.facultad_id == id
                                  select e).FirstOrDefault();

            // Verificar que haya devuelto un registro, que debe ser el que se va a eliminar
            if (facultades == null) { return NotFound(); }

            // Ejecutar la acción para eliminar
            _equiposContext.facultades.Attach(facultades);
            _equiposContext.facultades.Remove(facultades);
            _equiposContext.SaveChanges();

            return Ok(facultades);

        }
    }
}
