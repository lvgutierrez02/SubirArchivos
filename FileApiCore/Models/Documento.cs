namespace FileApiCore.Models
{
    public class Documento
    {
        public int IdDocumento { get; set; }
        public string Descripcion { get; set; }
        public string Ruta { get; set; }

        public IFormFile Archivo { get; set; }  //permite almacenar el archivo que se envía a traves de la ruta desde el api
    }
}
