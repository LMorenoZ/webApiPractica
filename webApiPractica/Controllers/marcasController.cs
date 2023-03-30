using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using webApiPractica.Models;
using Microsoft.EntityFrameworkCore;

namespace webApiPractica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class marcasController : ControllerBase
    {
        private readonly equiposContext _equiposContext;
        public marcasController(equiposContext equiposContext)
        {
            _equiposContext = equiposContext;
        }

        // EndPoint que devuelve todas las marcas
        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            List<marcas> listadoMarcas = (from e in _equiposContext.marcas
                                          select e).ToList();

            if (listadoMarcas.Count() == 0) { return NotFound(); }

            return Ok(listadoMarcas);
        }

        // EndPoint que devuelve una marca identificada por su Id
        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            marcas? marca = (from e in _equiposContext.marcas
                             where e.id_marcas == id
                             select e).FirstOrDefault();

            if (marca == null) { return NotFound(); }

            return Ok(marca);
        }

        // EndPoint que permite crear una nueva marca. Usa el petodo POST
        [HttpPost]
        [Route("Add")]
        public IActionResult GuardarMarca([FromBody] marcas marca)
        {
            try
            {
                _equiposContext.marcas.Add(marca);
                _equiposContext.SaveChanges();
                return Ok(marca);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        // EndPoint para modificar el registro de una marca
        [HttpPut]
        [Route("actualizar/{id}")]
        public IActionResult ActualizarMarca(int id, [FromBody] marcas marcaModificar)
        {
            // Se obtiene el registro original en la db
            marcas? marcaActual = (from e in _equiposContext.marcas
                                   where e.id_marcas == id
                                   select e).FirstOrDefault();

            // Se verifica que exista ese registro
            if (marcaActual == null) { return NotFound(); }

            // Una vez obtenido el registro, se procede a cambiar las respectivas propiedades que se actualizarían
            marcaActual.nombre_marca = marcaModificar.nombre_marca;
            marcaActual.estados = marcaModificar.estados;

            // Se procede a marcar el registro como modificado y se envía a la db los datos actualizados del registro
            _equiposContext.Entry(marcaActual).State = EntityState.Modified;
            _equiposContext.SaveChanges();

            return Ok(marcaActual);
        }

        // EndPoint para eliminar una marca identificada por su Id
        [HttpDelete]
        [Route("eliminar/{id}")]
        public IActionResult EliminarMarca(int id)
        {
            // Obtener el registro de la bd indicado por su id
            marcas? marca = (from e in _equiposContext.marcas
                             where e.id_marcas == id
                             select e).FirstOrDefault();

            // Verificar que haya devuelto un registro, que debe ser el que se va a eliminar
            if (marca == null) { return NotFound(); }

            // Ejecutar la acción para eliminar
            _equiposContext.marcas.Attach(marca);
            _equiposContext.marcas.Remove(marca);
            _equiposContext.SaveChanges();

            return Ok(marca);

        }

    }
}
