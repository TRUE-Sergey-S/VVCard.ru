using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace vvcard.Controllers
{
    public class CustomAccountController : Controller
    {
        ApplicationContext db;
        public CustomAccountController(ApplicationContext context)
        {
            db = context;
        }
        [AcceptVerbs("Get", "Post")]
        public IActionResult CheckEmail(string email)
        {
            string emailDb = db.Users.Select(e => e.Email).FirstOrDefault(x => x == email);
            if (emailDb != null)
            {
                return Json(false);
            }
            return Json(true);
        }
        [AcceptVerbs("Get", "Post")]
        public IActionResult CheckUserName(string userName)
        {
            string userNameDb = db.Users.Select(e => e.UserName).FirstOrDefault(x => x == userName);
            if (userNameDb != null)
            {
                return Json(false);
            }
            return Json(true);
        }
        /*
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(x => x.UserName == model.UserName && x.PasswordHash == model.Password);
                if (user != null)
                {
                    await Authenticate(model.UserName);
                    return RedirectToAction("ShowPersonalCard", "Card");
                }
                ModelState.AddModelError("", "Логин или пароль неверный");
            }
            return View(model);
        }
        
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(x => x.UserName == model.UserName && x.Email == model.Email);
                if (user == null)
                {
                    Random rnd = new Random();
                    int checkEmailCode = rnd.Next(10000, 90000);
                    db.Users.Add(new User { UserName = model.UserName, Password = model.Password, Email = model.Email, CheckEmailCode = checkEmailCode });
                    await db.SaveChangesAsync();
                    await Authenticate(model.UserName);
                    return RedirectToAction("ShowPersonalCard", "Card");
                }
                else { ModelState.AddModelError("", "Логин или почта уже используется"); }
            }
            return View(model);
        }

        public async Task<IActionResult> logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
        */
    }
}

