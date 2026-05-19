using System.ComponentModel.DataAnnotations;

    namespace MVC.Models
{
    public class Rol
    {
        [Key]
        public int idRol { get; set; }

        public string rol { get; set; }
    }
}
