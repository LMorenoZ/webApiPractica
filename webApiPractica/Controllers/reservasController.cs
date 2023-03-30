using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webApiPractica.Models;
using Microsoft.EntityFrameworkCore;

namespace webApiPractica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class reservasController : ControllerBase
    {
        private readonly equiposContext _equiposContext;

        public reservasController(equiposContext equiposContext)
        {
            _equiposContext = equiposContext;
        }

        // EndPoint que devuelve todos los registros
        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            var listado_reservas = (from e in _equiposContext.reservas
                                        join eq in _equiposContext.equipos
                                            on e.equipo_id equals eq.id_equipos
                                        join u in _equiposContext.usuarios
                                            on e.usuario_id equals u.usuario_id
                                        join er in _equiposContext.estados_reserva 
                                            on e.estado_reserva_id equals er.estado_res_id
                                        select new
                                        {
                                            e.reserva_id,
                                            e.equipo_id,
                                            equipo = eq.nombre,
                                            e.usuario_id,
                                            usuario = u.nombre,
                                            e.fecha_salida,
                                            e.hora_salida,
                                            e.tiempo_reserva,
                                            e.estado_reserva_id,
                                            estado_reserva = er.estado,
                                            e.fecha_retorno,
                                            e.hora_retorno
                                         }).ToList();

            if (listado_reservas.Count() == 0) { return NotFound(); }

            return Ok(listado_reservas);
        }

        // EndPoint que devuelve un registro identificado por su Id
        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult GetById(int id)
        { 
            var reservas = (from e in _equiposContext.reservas
                                    where e.reserva_id == id
                                    join eq in _equiposContext.equipos
                                        on e.equipo_id equals eq.id_equipos
                                    join u in _equiposContext.usuarios
                                        on e.usuario_id equals u.usuario_id
                                    join er in _equiposContext.estados_reserva
                                        on e.estado_reserva_id equals er.estado_res_id
                                    select new
                                    {
                                        e.reserva_id,
                                        e.equipo_id,
                                        equipo = eq.nombre,
                                        e.usuario_id,
                                        usuario = u.nombre,
                                        e.fecha_salida,
                                        e.hora_salida,
                                        e.tiempo_reserva,
                                        e.estado_reserva_id,
                                        estado_reserva = er.estado,
                                        e.fecha_retorno,
                                        e.hora_retorno
                                    }).FirstOrDefault(); 

            if (reservas == null) { return NotFound(); }

            return Ok(reservas);
        }

        // EndPoint que permite crear un nuevo registro. Usa el método POST
        [HttpPost]
        [Route("Add")]
        public IActionResult GuardarRegistro([FromBody] reservas reservas)
        {
            try
            {
                _equiposContext.reservas.Add(reservas);
                _equiposContext.SaveChanges();
                return Ok(reservas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // EndPoint para modificar un registro identificado por su id
        [HttpPut]
        [Route("actualizar/{id}")]
        public IActionResult ActualizarRegistro(int id, [FromBody] reservas reservas_modificar)
        {
            // Se obtiene el registro original en la db
            reservas? reservas_actual = (from e in _equiposContext.reservas
                                             where e.reserva_id == id
                                             select e).FirstOrDefault();

            // Se verifica que exista ese registro
            if (reservas_actual == null) { return NotFound(); }

            // Una vez obtenido el registro, se procede a cambiar las respectivas propiedades que se actualizarían
            reservas_actual.equipo_id = reservas_modificar.equipo_id;
            reservas_actual.usuario_id = reservas_modificar.usuario_id;
            reservas_actual.fecha_salida = reservas_modificar.fecha_salida;
            reservas_actual.hora_salida = reservas_modificar.hora_salida;
            reservas_actual.tiempo_reserva = reservas_modificar.tiempo_reserva;
            reservas_actual.estado_reserva_id = reservas_modificar.estado_reserva_id;
            reservas_actual.fecha_retorno = reservas_modificar.fecha_retorno;
            reservas_actual.hora_retorno = reservas_modificar.hora_retorno;

            // Se procede a marcar el registro como modificado y se envía a la db los datos actualizados del registro
            _equiposContext.Entry(reservas_actual).State = EntityState.Modified;
            _equiposContext.SaveChanges();

            return Ok(reservas_actual);
        }

        // EndPoint para eliminar un registro identificado por su Id
        [HttpDelete]
        [Route("eliminar/{id}")]
        public IActionResult EliminarRegistro(int id)
        {
            // Obtener el registro de la bd indicado por su id
            reservas? reservas = (from e in _equiposContext.reservas
                                      where e.reserva_id == id
                                      select e).FirstOrDefault();

            // Verificar que haya devuelto un registro, que debe ser el que se va a eliminar
            if (reservas == null) { return NotFound(); }

            // Ejecutar la acción para eliminar
            _equiposContext.reservas.Attach(reservas);
            _equiposContext.reservas.Remove(reservas);
            _equiposContext.SaveChanges();

            return Ok(reservas);

        }
    }
}
