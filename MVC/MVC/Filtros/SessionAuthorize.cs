using Microsoft.AspNetCore.Mvc; // <--- ESTA ES LA QUE FALTA
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http; // Para el GetString

namespace appWeb2.Filtros
{
    public class SessionAuthorize : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var usuario = context.HttpContext.Session.GetString("Usuario");
            if (string.IsNullOrEmpty(usuario))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }

            base.OnActionExecuting(context);
        }
    }
}