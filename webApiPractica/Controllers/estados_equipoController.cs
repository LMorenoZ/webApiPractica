using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webApiPractica.Models;
using Microsoft.EntityFrameworkCore;

namespace webApiPractica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class estados_equipoController : ControllerBase
    {
        private readonly equiposContext _equiposContext;

        public estados_equipoController(equiposContext equiposContext)
        {
            _equiposContext = equiposContext;
        }

        // EndPoint que devuelve todos los registros
        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            List<estados_equipo> listado_estados_equipo = (from e in _equiposContext.estados_equipo
                                                    select e).ToList();

            if (listado_estados_equipo.Count() == 0) { return NotFound(); }

            return Ok(listado_estados_equipo);
        }

        // EndPoint que devuelve un registro identificado por su Id
        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            estados_equipo? estados_equipo = (from e in _equiposContext.estados_equipo
                                        where e.id_estados_equipo == id
                                        select e).FirstOrDefault();

            if (estados_equipo == null) { return NotFound(); }

            return Ok(estados_equipo);
        }

        // EndPoint que permite crear un nuevo registro. Usa el método POST
        [HttpPost]
        [Route("Add")]
        public IActionResult GuardarRegistro([FromBody] estados_equipo estados_equipo)
        {
            try
            {
                _equiposContext.estados_equipo.Add(estados_equipo);
                _equiposContext.SaveChanges();
                return Ok(estados_equipo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // EndPoint para modificar un registro identificado por su id
        [HttpPut]
        [Route("actualizar/{id}")]
        public IActionResult ActualizarRegistro(int id, [FromBody] estados_equipo estados_equipo_modificar)
        {
            // Se obtiene el registro original en la db
            estados_equipo? estados_equipo_actual = (from e in _equiposContext.estados_equipo
                                              where e.id_estados_equipo == id
                                              select e).FirstOrDefault();

            // Se verifica que exista ese registro
            if (estados_equipo_actual == null) { return NotFound(); }

            // Una vez obtenido el registro, se procede a cambiar las respectivas propiedades que se actualizarían
            estados_equipo_actual.descripcion = estados_equipo_modificar.descripcion;
            estados_equipo_actual.estado = estados_equipo_modificar.estado;

            // Se procede a marcar el registro como modificado y se envía a la db los datos actualizados del registro
            _equiposContext.Entry(estados_equipo_actual).State = EntityState.Modified;
            _equiposContext.SaveChanges();

            return Ok(estados_equipo_actual);
        }

        // EndPoint para eliminar un registro identificado por su Id
        [HttpDelete]
        [Route("eliminar/{id}")]
        public IActionResult EliminarRegistro(int id)
        {
            // Obtener el registro de la bd indicado por su id
            estados_equipo? estados_equipo = (from e in _equiposContext.estados_equipo
                                              where e.id_estados_equipo == id
                                              select e).FirstOrDefault();

            // Verificar que haya devuelto un registro, que debe ser el que se va a eliminar
            if (estados_equipo == null) { return NotFound(); }

            // Ejecutar la acción para eliminar
            _equiposContext.estados_equipo.Attach(estados_equipo);
            _equiposContext.estados_equipo.Remove(estados_equipo);
            _equiposContext.SaveChanges();

            return Ok(estados_equipo);

        }
    }
}
