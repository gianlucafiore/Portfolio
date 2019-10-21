using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API
{
    public class Usuario
    {
        public int Id{get;set;}
        public string NombreUsuario{get;set;}
        public string Contrasenia{get;set;}
        public DateTime FechaAlta{get;set;}
        public DateTime FechaBaja{get;set;}
    }

    [Route("[controller]")]
    public class AcountController : Controller
    {
        Db db;
        IConfiguration config;
        public AcountController(Db dbi, IConfiguration cnf)
        {
            db = dbi;
            config = cnf;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Listo");
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]    
        [HttpGet("p")]
        public IActionResult GetP()
        {
            return Ok("Listo protegido");
        }

        [HttpPost("login")]
        public IActionResult Login(string user, string pass)
        {
            var usuario = db.Usuarios
                            .FirstOrDefault(u => u.NombreUsuario == user && u.Contrasenia == GenerateHash(pass));
            if(usuario is null)
                return BadRequest("Credenciales incorrectas");
            var claims = new[]
            {
                new Claim("IdAcount", usuario.Id.ToString()),
                new Claim("UserName2", usuario.NombreUsuario),
                new Claim("UserName1", usuario.NombreUsuario),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue<string>("Apikey")));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(12);
             JwtSecurityToken token = new JwtSecurityToken(
                issuer : "localhost",
                audience: "localhost",
                claims : claims,
                expires: expiration,
                signingCredentials: credenciales
            );
            return Ok(new{
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = expiration
            });
        }

        static string GenerateHash(string randomString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}