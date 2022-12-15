using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using vampsdemo.Models;
using vampsdemo.Models.Data_model;
using vampsdemo.Repository;

namespace vampsdemo.Controllers
{
    public class HomeController : Controller
    {
        private IRegister Reg;
        neonmamEntities _context = new neonmamEntities();
        public HomeController()
        {
            this.Reg = new Register(new neonmamEntities());
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult HomePage()
        {
            return View();
        }
        
        public ActionResult UserData()
        {           
            return PartialView(Reg.GetUserDetails());
        }
        [HttpGet]        
        public ActionResult UserProfile(int user_id)
        {
            user_details user = Reg.GetUserId(user_id);
            return PartialView("UserProfile", user);
        }
        public ActionResult Register()
        {
            return PartialView("Register");
        }
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {

            if (ModelState.IsValid)
            {
                var Check = _context.user_details.FirstOrDefault(x => x.email == model.email);
                if (Check == null)
                {
                    user_details user = new user_details()
                    {
                        firstname = model.firstname,
                        lastname = model.lastname,
                        phonenumber = model.phonenumber,
                        email = model.email,
                        birthdate = model.birthdate,
                        password = GetMD5(model.password),
                    };
                    _context.user_details.Add(user);
                    user_role role = new user_role();
                    int Id = _context.user_details
                             .OrderByDescending(x => x.user_id)
                             .Take(0)
                             .Select(x => x.user_id)
                             .FirstOrDefault();
                    role.user_id = Id;
                    role.role_master_id = _context.role_master.Where(x => x.role_type == "User").Select
                    (u => u.role_master_id).SingleOrDefault();
                    _context.user_role.Add(role);
                    _context.Configuration.ValidateOnSaveEnabled = false;
                    _context.SaveChanges();
                    return RedirectToAction("HomePage", "Home");
                }               
            }
            return View();
        }
        public ActionResult Login()
        {
            return PartialView("Login");
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
           /* try
            {*/
                if (ModelState.IsValid)
                {
                    string email = model.email;
                    string password = GetMD5(model.password);
                    var result = Reg.validUser(email, password);
                
                 if (result.user_id == 0)
                    {
                        ViewBag.errormessage = "Invalid Username and Password";
                    }
                    else
                    {
                        var roles = (from users in _context.user_details
                                     join roll in _context.user_role on users.user_id
                                     equals roll.user_id
                                     join mrole in _context.role_master
                                     on roll.role_master_id equals mrole.role_master_id
                                     where users.email == model.email
                                     select mrole.role_type).SingleOrDefault();
                        string Role = Convert.ToString(roles);
                        if (Role == "Admin")
                        {
                            Session["FullName"] = result.firstname +" "+ result.lastname;
                            Session["Email"] = result.email;
                            Session["userId"] = result.user_id;
                            Session["Role1"] = Role;
                            ModelState.Clear();
                            return RedirectToAction("HomePage", "Home");
                        }
                        else if(Role == "User")
                        {
                            Session["FullName"] = result.firstname +" "+ result.lastname;
                            Session["Phone"] = result.phonenumber;
                            Session["Email"] = result.email;
                            Session["BirthDate"] = result.birthdate;
                            Session["userId"] = result.user_id;
                            Session["Role2"] = Role;
                            ModelState.Clear();
                            return RedirectToAction("HomePage", "Home");
                        }
                        else if(Role == "Satellite_User")
                        {
                            Session["Role3"] = Role;
                        Session["FullName"] = result.firstname + " " + result.lastname;
                        Session["Phone"] = result.phonenumber;
                        Session["Email"] = result.email;
                        Session["BirthDate"] = result.birthdate;
                        Session["userId"] = result.user_id;

                        ModelState.Clear();
                        return RedirectToAction("HomePage", "Home");

                    }
                }
                }
                return ViewBag.errorMessage = "modelstate is invalid!!!";          
          /*  catch (Exception ex)
            {
                throw ex;
            }*/
        }
        [HttpGet]
        public ActionResult AddEdit(int user_id)
        {
            user_details user = Reg.GetUserId(user_id);
            return PartialView("AddEdit", user);
        }
        [HttpPost]
        public ActionResult Edit(user_details user)
        {
           
                if (user.user_id > 0)
                {                  
                   /*   user_details user = new user_details();
                        user.firstname = model.firstname;
                        user.lastname = model.lastname;
                        user.phonenumber = model.phonenumber;
                        user.email = model.email;
                        user.birthdate = model.birthdate;
                        user.password = user.password;
                        
                   */   Reg.Updateuser(user);
                        Reg.save();
                        return RedirectToAction("HomePage", "Home");
                    
                }
                return View();
            }
        public JsonResult delete(int id)
        {
            Reg.Delete(id);
            Reg.Deleterole(id);
            Reg.save();
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            return RedirectToAction("HomePage", "Home");
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
        [Authorize(Roles = "User")]
        public ActionResult UserInfo()
        {
            return PartialView("UserInfo");          
        }
        //[Authorize(Roles = "Satellite_User")]
        public ActionResult stProfile()
        {
            return PartialView("stProfile");
        }
    }
}