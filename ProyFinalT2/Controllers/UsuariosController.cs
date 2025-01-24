using ProyFinalT2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProyFinalT2.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using ProyFinalT2.Servicios;
using Microsoft.EntityFrameworkCore;

namespace ProyFinalT2.Controllers
{
    public class UsuariosController : Controller
    {
        // UserManager es una clase que se encarga de administrar los usuarios de Identity
        private readonly UserManager<IdentityUser> userManager;
        // SignInManager es una clase que se encarga de administrar el inicio de sesión de los usuarios
        private readonly SignInManager<IdentityUser> signInManager;
        // ApplicationDbContext es la clase que se encarga de interactuar con la base de datos
        private readonly ApplicationDbContext context;

        // se inyectan las dependencias necesarias para el controlador 
        public UsuariosController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
        }

        // este método se encarga de mostrar la vista de registro
        // [AllowAnonymous] es un atributo que indica que el método puede ser accedido por usuarios anónimos
        [AllowAnonymous]
        public IActionResult Registro()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(RegistroViewModel modelo)
        {
            //ModelState.IsValid valida si los campos requeridos están llenos
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            // usuario  guarda el correo y el nombre de usuario del usuario que se va a registrar
            var usuario = new IdentityUser() { Email = modelo.Email, UserName = modelo.Email };

            //resultado  contiene el resultado de la creación del usuario
            // lo guarda en la tabla de usuarios de la base de datos
            var resultado = await userManager.CreateAsync(usuario, password: modelo.Password);


            // si el resultado es exitoso , se inicia sesión y se redirige al usuario a la página de inicio
            if (resultado.Succeeded)
            {
                await signInManager.SignInAsync(usuario, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }
            else
            {  // si el resultado no es exitoso , se agregan los errores al modelo y se muestra la vista de registro
                foreach (var error in resultado.Errors)
                {
                    //ModelState.AddModelError agrega un error al modelo
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(modelo);
            }

        }




        [AllowAnonymous]
        public IActionResult Login(string mensaje = null)
        {
            // Si se ha proporcionado un mensaje (no es nulo)...
            if (mensaje is not null)
            {
                // Agrega el mensaje a los datos de la vista para mostrarlo al usuario.
                ViewData["mensaje"] = mensaje;
            }


            // Devuelve la vista de login, que ahora podría contener el mensaje si se pasó uno.
            return View();
        }



        [HttpPost]
        [AllowAnonymous] // Permite que este método sea llamado sin autenticación.
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            // Verifica si el estado del modelo es válido. Esto significa que se cumplen todas las anotaciones de datos del modelo.
            if (!ModelState.IsValid)
            {
                // Si el estado del modelo no es válido, devuelve la vista con los mismos datos del modelo para la corrección de errores.
                return View(modelo);
            }


            // Intenta iniciar sesión al usuario usando el SignInManager con el email y contraseña proporcionados.
            // El parámetro 'Recuerdame' decide si el login debe persistir, y 'lockoutOnFailure' está configurado en falso para evitar el bloqueo de la cuenta en caso de fallo.
            var resultado = await signInManager.PasswordSignInAsync(modelo.Email,
                modelo.Password, modelo.Recuerdame, lockoutOnFailure: false);


            // Si el inicio de sesión fue exitoso...
            if (resultado.Succeeded)
            {
                // Redirige al usuario a la acción 'Index' del controlador 'Home'.
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Si no, añade un error al ModelState con un mensaje genérico sobre credenciales incorrectas.
                ModelState.AddModelError(string.Empty, "Nombre de usuario o contraseña incorrecto.");
                // Devuelve la vista con el modelo original para que el usuario pueda intentar de nuevo.
                return View(modelo);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }



        //----------->  Login con proveedores externos


        //Este método permite iniciar sesión con un proveedor externo (Google, Facebook, etc.).
        [AllowAnonymous] // 1. Permite el acceso sin autenticación.
        [HttpGet] // 2. Responde a solicitudes GET.
        public ChallengeResult LoginExterno(string proveedor, string urlRetorno = null)
        {
            // 3. Define la URL a la que se redirigirá después del login externo.
            var urlRedireccion = Url.Action("RegistrarUsuarioExterno", values: new { urlRetorno });

            // 4. Configura las propiedades de autenticación con el proveedor externo.
            var propiedades = signInManager.ConfigureExternalAuthenticationProperties(proveedor, urlRedireccion);

            // 5. Inicia el proceso de autenticación con el proveedor externo.
            return new ChallengeResult(proveedor, propiedades);
        }

        //Este método maneja la finalización del proceso de autenticación y registra un usuario si es necesario

        [AllowAnonymous] // 1. Permite el acceso sin autenticación.
        public async Task<IActionResult> RegistrarUsuarioExterno(string urlRetorno = null, string remoteError = null)
        {
            urlRetorno = urlRetorno ?? Url.Content("~/"); // 2. Define una URL de retorno predeterminada.
            var mensaje = "";

            if (remoteError is not null)
            {
                // 3. Maneja errores del proveedor externo.
                mensaje = $"Error del proveedor externo: {remoteError}";
                return RedirectToAction("login", routeValues: new { mensaje });
            }

            var info = await signInManager.GetExternalLoginInfoAsync(); // 4. Obtiene la información del login externo.
            if (info is null)
            {
                // 5. Maneja el error si no se pudo cargar la información del login.
                mensaje = "Error cargando la data de login externo";
                return RedirectToAction("login", routeValues: new { mensaje });
            }

            var resultadoLoginExterno = await signInManager.ExternalLoginSignInAsync(
                info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

            if (resultadoLoginExterno.Succeeded)
            {
                // 6. Redirige al usuario si ya existe una cuenta vinculada.
                return LocalRedirect(urlRetorno);
            }

            string email = "";

            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                // 7. Obtiene el email del usuario si está disponible.
                email = info.Principal.FindFirstValue(ClaimTypes.Email);
            }
            else
            {
                // 8. Maneja el error si no se pudo leer el email del usuario.
                mensaje = "Error leyendo el email del usuario del proveedor";
                return RedirectToAction("login", routeValues: new { mensaje });
            }

            var usuario = new IdentityUser { Email = email, UserName = email };

            var resultadoCrearUsuario = await userManager.CreateAsync(usuario); // 9. Crea un nuevo usuario.
            if (!resultadoCrearUsuario.Succeeded)
            {
                // 10. Maneja errores al crear el usuario.
                mensaje = resultadoCrearUsuario.Errors.First().Description;
                return RedirectToAction("login", routeValues: new { mensaje });
            }

            var resultadoAgregarLogin = await userManager.AddLoginAsync(usuario, info); // 11. Vincula el login externo al usuario.
            if (resultadoAgregarLogin.Succeeded)
            {
                // 12. Inicia sesión automáticamente.
                await signInManager.SignInAsync(usuario, isPersistent: true, info.LoginProvider);
                return LocalRedirect(urlRetorno);
            }

            // 13. Maneja errores al agregar el login externo.
            mensaje = "Ha ocurrido un error agregando el login";
            return RedirectToAction("login", routeValues: new { mensaje });
        }


        //------> Manejo de Roles

        //Este método obtiene una lista de usuarios para mostrar en la vista.
        [HttpGet] // 1. Permite responder a solicitudes GET.
      // [Authorize(Roles = Constantes.RolAdmin)] // 2. Restringe el acceso solo a usuarios con rol de administrador.
        public async Task<IActionResult> Listado(string mensaje = null)
        {
            // 3. Obtiene la lista de usuarios desde la base de datos, seleccionando solo el email.
            var usuarios = await context.Users.Select(u => new UsuarioViewModel
            {
                Email = u.Email // 4. Asigna el correo electrónico a un modelo de vista.
            }).ToListAsync();

            // 5. Crea un modelo de vista para enviar a la vista correspondiente.
            var modelo = new UsuariosListadoViewModel
            {
                Usuarios = usuarios,
                Mensaje = mensaje
            };

            // 6. Devuelve la vista con el modelo completo.
            return View(modelo);
        }


        //Este método asigna el rol de administrador a un usuario.
        [HttpPost] // 1. Responde a solicitudes POST.
        //[Authorize(Roles = Constantes.RolAdmin)] // 2. Restringe la acción a administradores.
        public async Task<IActionResult> HacerAdmin(string email)
        {
            // 3. Busca el usuario con el email proporcionado.
            var usuario = await context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (usuario is null)
            {
                // 4. Devuelve un error 404 si el usuario no existe.
                return NotFound();
            }

            // 5. Agrega el rol de administrador al usuario.
            await userManager.AddToRoleAsync(usuario, Constantes.RolAdmin);

            // 6. Redirige a la lista de usuarios con un mensaje de confirmación.
            return RedirectToAction("Listado", new { mensaje = "Rol asignado correctamente a " + email });
        }



        //Este método elimina el rol de administrador de un usuario.
        [HttpPost] // 1. Responde a solicitudes POST.
        //[Authorize(Roles = Constantes.RolAdmin)] // 2. Restringe la acción a administradores.
        public async Task<IActionResult> RemoverAdmin(string email)
        {
            // 3. Busca el usuario con el email proporcionado.
            var usuario = await context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (usuario is null)
            {
                // 4. Devuelve un error 404 si el usuario no existe.
                return NotFound();
            }

            // 5. Elimina el rol de administrador del usuario.
            await userManager.RemoveFromRoleAsync(usuario, Constantes.RolAdmin);

            // 6. Redirige a la lista de usuarios con un mensaje de confirmación.
            return RedirectToAction("Listado", new { mensaje = "Rol removido correctamente a " + email });
        }









    }
}
