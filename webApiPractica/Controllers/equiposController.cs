using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webApiPractica.Models;
using Microsoft.EntityFrameworkCore;

namespace webApiPractica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class equiposController : ControllerBase
    {
        private readonly equiposContext _equiposContexto;

        public equiposController(equiposContext equiposContexto)
        {
            _equiposContexto = equiposContexto;
        }

        /// <summary>
        /// EndPoint que retorna el listado de todos los equipos existentes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAll")]

        public IActionResult Get()
        {
            List<equipos> listadoEquipo = (from e in _equiposContexto.equipos
                                            select e).ToList();

            if (listadoEquipo.Count() == 0)
            {
                return NotFound();
            }

            return Ok(listadoEquipo);
        }

        /// EndPoint que retorna ls regisros de una tabla filtrados por su ID
        [HttpGet]
        [Route("GetById/{id}")]

        public IActionResult Get(int id) 
        {
            equipos? equipo = (from e in _equiposContexto.equipos
                               where e.id_equipos == id
                               select e).FirstOrDefault();

            if (equipo == null) 
            {
                return NotFound();
            }

            return Ok(equipo);
        }

        /// EndPoint que retorna los registros de una tabla filtrado por descripción
        [HttpGet]
        [Route("Fild/{filtro}")]

        public IActionResult FindByDescription(String filtro)
        {
            equipos? equipo = (from e in _equiposContexto.equipos
                               where e.descripcion.Contains(filtro)
                               select e).FirstOrDefault();

            if (equipo == null)
            {
                return NotFound();
            }

            return Ok(equipo); 
        }

        /// EndPoint que permite crear registros que serán enviados a la base de datos
        [HttpPost]
        [Route("Add")]

        public IActionResult GuardarEquipo([FromBody] equipos equipo)
        {
            try
            {
                _equiposContexto.equipos.Add(equipo);
                _equiposContexto.SaveChanges();
                return Ok(equipo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// EndPoint que permite modificar un registro. Se utiliza el método PUT
        [HttpPut]
        [Route("actualizar/{id}")]

        public IActionResult ActualizarEquipo (int id, [FromBody] equipos equipoModificar)
        {
            // Para actualizar un registro, se obtiene el registro original de la base de datos, al cual 
            // alteraremos alguna propiedad
            equipos? equipoActual = (from e in _equiposContexto.equipos
                               where e.id_equipos == id
                               select e).FirstOrDefault();

            // Verificamos que exista el registro según su Id
            if (equipoActual == null)
            { return NotFound(); }

            // Si se encuentra el registro, se alteran los campos modificables
            equipoActual.nombre = equipoModificar.nombre;
            equipoActual.descripcion = equipoModificar.descripcion;
            equipoActual.marca_id = equipoModificar.marca_id;
            equipoActual.tipo_equipo_id = equipoModificar.tipo_equipo_id;
            equipoActual.anio_compra = equipoModificar.anio_compra  ;
            equipoActual.costo = equipoModificar.costo;

            // Se marca el registro como modificado en el contexto y se envía la modificación a la base de datos
            _equiposContexto.Entry(equipoActual).State = EntityState.Modified;
            _equiposContexto.SaveChanges();

            return Ok(equipoModificar);
        }

        /// EndPoint que recibe un id para eliminar el registro correspondiente
        [HttpDelete]
        [Route("eliminar/{id}")]

        public IActionResult EliminarEquipo (int id)
        {
            // Para actualizar un registro, se obtiene el registro original de la base de datos
            // al cual eliminaremos

            equipos? equipo = (from e in _equiposContexto.equipos
                               where e.id_equipos == id
                               select e).FirstOrDefault();

            // Verificams que exista el registro según el id especificado
            if (equipo == null) { return NotFound(); }

            // Si estamos en este punto, significa que sí tenemos un registro con ese id. Entonces se eliminará.
            _equiposContexto.equipos.Attach(equipo);
            _equiposContexto.equipos.Remove(equipo);
            _equiposContexto.SaveChanges();

            return Ok(equipo);
        }
    }
}
