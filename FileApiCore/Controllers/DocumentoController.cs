using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using FileApiCore.Models;
using System.Data.SqlClient;
using System.Data;

namespace FileApiCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentoController : ControllerBase
    {
        private readonly string _rutaServidor;
        private readonly string _cadenaSql;
        public DocumentoController(IConfiguration config)  //Obtiene información del archivo appsettings.json
        {  

            _rutaServidor = config.GetSection("Configuracion").GetSection("RutaServidor").Value;
            _cadenaSql = config.GetConnectionString("CadenaSQL");
        }


        //REFERENCIAS 
        //MODELS
        [HttpPost]
        [Route("Subir")]
        [DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)] //OMITIR ESTO //configura el tamaño del documento
        public IActionResult Subir([FromForm]Documento objeto) //recibe un documento desde el cuerpo de un formulario
        {

            string rutaDocumento = Path.Combine(_rutaServidor, objeto.Archivo.FileName); //combinar rutas (concatena la ruta del servidor junto con el nombre con el que se guarda el doc)

            try
            {
                //System.IO
                using (FileStream newFile = System.IO.File.Create(rutaDocumento)) //el documento recibido se va a guardar en la ruta del servidor
                {
                    objeto.Archivo.CopyTo(newFile);
                    newFile.Flush();
                }

                //system.data.sqlclient
                using (var conexion = new SqlConnection(_cadenaSql)) //declara conexion con la Db
                {
                    conexion.Open();//Abrir la Db
                    var cmd = new SqlCommand("sp_guardar_documento", conexion); //declarar comando 
                    cmd.Parameters.AddWithValue("descripcion", objeto.Descripcion); //al comando se le envian parametros 
                    cmd.Parameters.AddWithValue("ruta", rutaDocumento); //se envia la ruta en donde se va a guardar por medio del parametro
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery(); // para que la operacion del comando sea ejecutada
                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Guardado" });//codigo de todo ha sido bien
            }
            catch (Exception error) {

                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message }); //Enviar error
            }
            


            
        }
    }
}
