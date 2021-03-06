using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
//This site developed by Vahit Kuruosman,Gökhan Biçer and Halil Bozkurt
namespace ifs9webapp.Controllers
{
    public class AccountController : Controller
    {
        OracleConnection con;
        OracleCommand cmd;
        string loginName;

        public void sessionLoginnamePassword(string pLoginName,string pPassword){
            HttpContext.Session.SetString("kullaniciAdi", pLoginName);
            HttpContext.Session.SetString("kullaniciParola", pPassword);
        }
        public string oracleConnection(string pLoginName,string pPassword)
        {
            string conString = "User Id="+pLoginName+";"+"Password="+pPassword+";"+
         //How to connect to an Oracle DB without SQL*Net configuration file
         //  also known as tnsnames.ora.
          //  "Data Source=10.6.12.100:1521/PROD";
        "Data Source=10.6.11.102:1521/TEST";
            return conString;
        }
  
        public OracleConnection openConnection(string pConString)
        {
            try{
                con = new OracleConnection(pConString);
                cmd = con.CreateCommand();
                con.Open();
                return con;
            }
            catch (Exception e) { 
                return null;
            }
        }
        public void closeConnection(OracleConnection pCon)
        {
            pCon.Close();
            pCon.Dispose();
        }

        public IActionResult Login(bool error)
        {
            
            HttpContext.Session.Remove("kullaniciAdi");
            HttpContext.Session.Remove("kullaniciParola");

            if (error)
            ViewBag.error = "Hatalı kullanıcı adı veya parola girişi yaptınız.";

            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel lvm)
        {
            
            if (!String.IsNullOrEmpty(lvm.LoginName) && !String.IsNullOrEmpty(lvm.Password)) {
               
                loginName = lvm.LoginName.ToUpper(new CultureInfo("en-US", false));

                sessionLoginnamePassword(loginName, lvm.Password);
                var rCon = openConnection(oracleConnection(loginName, lvm.Password));
                if (rCon != null)
                {

                    var claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Name, lvm.LoginName));
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    var props = new AuthenticationProperties();
                    props.IsPersistent = lvm.RememberMe;
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props).Wait();
                    string loginName = HttpContext.Session.GetString("kullaniciAdi");

                    closeConnection(rCon);

                    return RedirectToAction("Ifs", "Home", new { loginname = loginName });
                }
                else
                return RedirectToAction("Login", "Account", new { error = true });
            }
            else
            return RedirectToAction("Login", "Account", new { error = true });
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}
