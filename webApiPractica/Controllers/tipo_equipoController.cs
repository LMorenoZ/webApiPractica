using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webApiPractica.Models;
using Microsoft.EntityFrameworkCore;

namespace webApiPractica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class tipo_equipoController : ControllerBase
    {
        private readonly equiposContext _equiposContext;

        public tipo_equipoController(equiposContext equiposContexto)
        {
            _equiposContext = equiposContexto;
        }

        // EndPoint que devuelve todas los tipo_equipo
        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            List<tipo_equipo> listadoTipo_equipo = (from e in _equiposContext.tipo_equipo
                                          select e).ToList();

            if (listadoTipo_equipo.Count() == 0) { return NotFound(); }

            return Ok(listadoTipo_equipo);
        }

        // EndPoint que devuelve un tipo_equipo identificada por su Id
        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            tipo_equipo? tipo_equipo = (from e in _equiposContext.tipo_equipo
                             where e.id_tipo_equipo == id
                             select e).FirstOrDefault();

            if (tipo_equipo == null) { return NotFound(); }

            return Ok(tipo_equipo);
        }

        // EndPoint que permite crear un nuevo tipo_equipo. Usa el método POST
        [HttpPost]
        [Route("Add")]
        public IActionResult GuardarTipo_equipo([FromBody] tipo_equipo tipo_equipo)
        {
            try
            {
                _equiposContext.tipo_equipo.Add(tipo_equipo);
                _equiposContext.SaveChanges();
                return Ok(tipo_equipo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // EndPoint para modificar el registro de un tipo_equipo
        [HttpPut]
        [Route("actualizar/{id}")]
        public IActionResult ActualizarMarca(int id, [FromBody] tipo_equipo tipo_equipoModificar)
        {
            // Se obtiene el registro original en la db
            tipo_equipo? tipo_equipoActual = (from e in _equiposContext.tipo_equipo
                                   where e.id_tipo_equipo == id
                                   select e).FirstOrDefault();

            // Se verifica que exista ese registro
            if (tipo_equipoActual == null) { return NotFound(); }

            // Una vez obtenido el registro, se procede a cambiar las respectivas propiedades que se actualizarían
            tipo_equipoActual.descripcion = tipo_equipoModificar.descripcion;
            tipo_equipoActual.estado = tipo_equipoModificar.estado;

            // Se procede a marcar el registro como modificado y se envía a la db los datos actualizados del registro
            _equiposContext.Entry(tipo_equipoActual).State = EntityState.Modified;
            _equiposContext.SaveChanges();

            return Ok(tipo_equipoActual);
        }

        // EndPoint para eliminar un tipo_equipo identificado por su Id
        [HttpDelete]
        [Route("eliminar/{id}")]
        public IActionResult EliminarTipo_equipo(int id)
        {
            // Obtener el registro de la bd indicado por su id
            tipo_equipo? tipo_equipo = (from e in _equiposContext.tipo_equipo
                             where e.id_tipo_equipo == id
                             select e).FirstOrDefault();

            // Verificar que haya devuelto un registro, que debe ser el que se va a eliminar
            if (tipo_equipo == null) { return NotFound(); }

            // Ejecutar la acción para eliminar
            _equiposContext.tipo_equipo.Attach(tipo_equipo);
            _equiposContext.tipo_equipo.Remove(tipo_equipo);
            _equiposContext.SaveChanges();

            return Ok(tipo_equipo);

        }
    }
}
