using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webApiPractica.Models;
using Microsoft.EntityFrameworkCore;

namespace webApiPractica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class estados_reservaController : ControllerBase
    {
        private readonly equiposContext _equiposContext;

        public estados_reservaController(equiposContext equiposContext)
        {
            _equiposContext = equiposContext;
        }

        // EndPoint que devuelve todos los registros
        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            List<estados_reserva> listado_estados_reserva = (from e in _equiposContext.estados_reserva
                                                             select e).ToList();

            if (listado_estados_reserva.Count() == 0) { return NotFound(); }

            return Ok(listado_estados_reserva);
        }

        // EndPoint que devuelve un registro identificado por su Id
        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            estados_reserva? estados_reserva = (from e in _equiposContext.estados_reserva
                                              where e.estado_res_id == id
                                              select e).FirstOrDefault();

            if (estados_reserva == null) { return NotFound(); }

            return Ok(estados_reserva);
        }

        // EndPoint que permite crear un nuevo registro. Usa el método POST
        [HttpPost]
        [Route("Add")]
        public IActionResult GuardarRegistro([FromBody] estados_reserva estados_reserva)
        {
            try
            {
                _equiposContext.estados_reserva.Add(estados_reserva);
                _equiposContext.SaveChanges();
                return Ok(estados_reserva);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // EndPoint para modificar un registro identificado por su id
        [HttpPut]
        [Route("actualizar/{id}")]
        public IActionResult ActualizarRegistro(int id, [FromBody] estados_reserva estados_reserva_modificar)
        {
            // Se obtiene el registro original en la db
            estados_reserva? estados_reserva_actual = (from e in _equiposContext.estados_reserva
                                                       where e.estado_res_id == id
                                                     select e).FirstOrDefault();

            // Se verifica que exista ese registro
            if (estados_reserva_actual == null) { return NotFound(); }

            // Una vez obtenido el registro, se procede a cambiar las respectivas propiedades que se actualizarían
            estados_reserva_actual.estado = estados_reserva_modificar.estado;

            // Se procede a marcar el registro como modificado y se envía a la db los datos actualizados del registro
            _equiposContext.Entry(estados_reserva_actual).State = EntityState.Modified;
            _equiposContext.SaveChanges();

            return Ok(estados_reserva_actual);
        }

        // EndPoint para eliminar un registro identificado por su Id
        [HttpDelete]
        [Route("eliminar/{id}")]
        public IActionResult EliminarRegistro(int id)
        {
            // Obtener el registro de la bd indicado por su id
            estados_reserva? estados_reserva = (from e in _equiposContext.estados_reserva
                                                where e.estado_res_id == id
                                                select e).FirstOrDefault();

            // Verificar que haya devuelto un registro, que debe ser el que se va a eliminar
            if (estados_reserva == null) { return NotFound(); }

            // Ejecutar la acción para eliminar
            _equiposContext.estados_reserva.Attach(estados_reserva);
            _equiposContext.estados_reserva.Remove(estados_reserva);
            _equiposContext.SaveChanges();

            return Ok(estados_reserva);

        }
    }
}
    