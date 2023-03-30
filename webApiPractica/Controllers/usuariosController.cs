using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webApiPractica.Models;
using Microsoft.EntityFrameworkCore;

namespace webApiPractica.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class usuariosController : ControllerBase
    {
        public readonly equiposContext _equiposContext;

        public usuariosController(equiposContext equiposContext)
        {
            _equiposContext = equiposContext;
        }

        // EndPoint que devuelve todos los registros
        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            List<usuarios> listado_usuarios = (from e in _equiposContext.usuarios
                                               select e).ToList();

            if (listado_usuarios.Count() == 0) { return NotFound(); }

            return Ok(listado_usuarios);
        }

        // EndPoint que devuelve un registro identificado por su Id
        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            usuarios? usuarios = (from e in _equiposContext.usuarios
                                  where e.usuario_id == id
                                  select e).FirstOrDefault();

            if (usuarios == null) { return NotFound(); }

            return Ok(usuarios);
        }

        // EndPoint que permite crear un nuevo registro. Usa el método POST
        [HttpPost]
        [Route("Add")]
        public IActionResult GuardarRegistro([FromBody] usuarios usuarios)
        {
            try
            {
                _equiposContext.usuarios.Add(usuarios);
                _equiposContext.SaveChanges();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // EndPoint para modificar un registro identificado por su id
        [HttpPut]
        [Route("actualizar/{id}")]
        public IActionResult ActualizarRegistro(int id, [FromBody] usuarios usuarios_modificar)
        {
            // Se obtiene el registro original en la db
            usuarios? usuarios_actual = (from e in _equiposContext.usuarios
                                         where e.usuario_id == id
                                         select e).FirstOrDefault();

            // Se verifica que exista ese registro
            if (usuarios_actual == null) { return NotFound(); }

            // Una vez obtenido el registro, se procede a cambiar las respectivas propiedades que se actualizarían
            usuarios_actual.nombre = usuarios_modificar.nombre;
            usuarios_actual.documento = usuarios_modificar.documento;
            usuarios_actual.tipo = usuarios_modificar.tipo;
            usuarios_actual.carnet = usuarios_modificar.carnet;
            usuarios_actual.carrera_id = usuarios_modificar.carrera_id;

            // Se procede a marcar el registro como modificado y se envía a la db los datos actualizados del registro
            _equiposContext.Entry(usuarios_actual).State = EntityState.Modified;
            _equiposContext.SaveChanges();

            return Ok(usuarios_actual);
        }

        // EndPoint para eliminar un registro identificado por su Id
        [HttpDelete]
        [Route("eliminar/{id}")]
        public IActionResult EliminarRegistro(int id)
        {
            // Obtener el registro de la bd indicado por su id
            usuarios? usuarios = (from e in _equiposContext.usuarios
                                  where e.usuario_id == id
                                  select e).FirstOrDefault();

            // Verificar que haya devuelto un registro, que debe ser el que se va a eliminar
            if (usuarios == null) { return NotFound(); }

            // Ejecutar la acción para eliminar
            _equiposContext.usuarios.Attach(usuarios);
            _equiposContext.usuarios.Remove(usuarios);
            _equiposContext.SaveChanges();

            return Ok(usuarios);

        }
    }
}
