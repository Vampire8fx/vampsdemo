using System;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using vampsdemo.Models;
using vampsdemo.Models.Data_model;
using vampsdemo.Repository;

namespace vampsdemo.Controllers
{
    public class UserController : Controller
    {

        private IRegister Reg;
        neonmamEntities _vamps = new neonmamEntities();
        // GET: User
        public UserController()

        {
            this.Reg = new Register(new neonmamEntities());
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var data = from m in Reg.GetUserDetails()
                       select m;
            return View(data);
        }
        [Authorize(Roles = "Admin")]     
        //new crude 
        
        [HttpPost]
        public ActionResult UserDetails(int id)
        {
            user_details user = Reg.GetUserId(id);
            return PartialView("UserDetails", user);
        }
        [AllowAnonymous]
        public ActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Registration(RegisterModel user)
        {

            if (ModelState.IsValid)
            {
                var check = _vamps.user_details.FirstOrDefault(s => s.email == user.email);
                if (check == null)
                {
                    user_details obj = new user_details()
                    {
                        firstname = user.firstname,
                        lastname = user.lastname,
                        phonenumber = user.phonenumber,
                        email = user.email,
                        birthdate = user.birthdate,
                        password = GetMD5(user.password),
                    };
                    _vamps.Configuration.ValidateOnSaveEnabled = false;
                    _vamps.user_details.Add(obj);
                    user_role role = new user_role();
                    int Id = _vamps.user_details
                             .OrderByDescending(x => x.user_id)
                             .Take(0)
                             .Select(x => x.user_id)
                             .FirstOrDefault();
                    role.user_id = Id;
                    role.role_master_id = _vamps.role_master.Where(x => x.role_type == "User").Select
                    (u => u.role_master_id).SingleOrDefault();
                    _vamps.user_role.Add(role);
                    _vamps.SaveChanges();                  

                    /*                         user.password = GetMD5(user.password);
                                            _vamps.Configuration.ValidateOnSaveEnabled = false;
                                            Reg.Insert(user);

                                            user_role role = new user_role();
                                            int Id = _vamps.UserDetail
                                                     .OrderByDescending(x => x.user_id)
                                                     .Take(0)
                                                     .Select(x => x.user_id)
                                                     .FirstOrDefault();
                                            role.user_id = Id;
                                            role.role_master_id = _vamps.Rolemaster.Where(x => x.role_type == "User").Select
                                            (u => u.role_master_id).SingleOrDefault();
                                            Reg.AddUserToRole(role);
                                            Reg.save();
                                            return RedirectToAction("Login");*/
                }
                else
                {
                    ViewBag.error = "Email already exists";
                    return RedirectToAction("Login");
                }

            }

            /*   catch (DbEntityValidationException dbEx)
               {
                   string message = string.Empty;
                   foreach (var validationErrors in dbEx.EntityValidationErrors)
                   {
                       foreach (var validationError in validationErrors.ValidationErrors)
                       {
                           message += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                       }
                   }
   */
            return View();
        }     
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string email = user.email;
                    string password = GetMD5(user.password);
                    var result = Reg.validUser(email, password);
                    if (result.user_id == 0 || result.user_id < 1)
                    {
                        ViewBag.errormessage = "Invalid Username and Password";
                    }
                    else
                    {
                        var roles = (from users in _vamps.user_details
                                     join roll in _vamps.user_role on users.user_id
                                     equals roll.user_id
                                     join mrole in _vamps.role_master
                                     on roll.role_master_id equals mrole.role_master_id
                                     where users.email == user.email
                                     select mrole.role_type).SingleOrDefault();
                        string Role = Convert.ToString(roles);
                        if(Role == "Admin")
                        {
                            Session["FullName"] = result.firstname + result.lastname;
                            Session["Email"] = result.email;
                            Session["userId"] = result.user_id;
                            return RedirectToAction("Index", "User");
                        }
                        if(Role == "User")
                        {
                            Session["FullName"] = result.firstname + result.lastname;
                            Session["Phone"] = result.phonenumber;
                            Session["Email"] = result.email;
                            Session["BirthDate"] = result.birthdate;
                            Session["userId"] = result.user_id;
                            return RedirectToAction("Profile");
                        }
                        if(Role == "Satellite_user")
                        {
                            return RedirectToAction("Sattelite");
                        }
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.errormessage = "Big Error";
            }
            return View();
        }
        //pending
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Update(int id)
        {
            user_details user = Reg.GetUserId(id);
            return PartialView(user);
        }
        //pending
        [HttpPost]
        public ActionResult Updatedata(int id, user_details user)
        {

            user = Reg.GetUserId(id);
            if (user != null)
            {
                Reg.Updateuser(user);
                Reg.save();

            }
            return RedirectToAction("Index");
        }
        //peding
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Reg.Delete(id);
            Reg.Deleterole(id);
            Reg.save();
            return RedirectToAction("Index");
            /*user = Reg.GetUserId(id);
            try
            {
                Reg.Delete(id);
                Reg.Deleterole(id);
                _vamps.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbEntityValidationException dbEx)
            {
                string message = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        message += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }

            }
            return RedirectToAction("Index");*/
        }
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            return RedirectToAction("Login");

        }
        [Authorize(Roles = "User")]
        public new ActionResult Profile()
        {
            return View();
        }
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;
            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }
            return byte2String;
        }
      
      
    }
}