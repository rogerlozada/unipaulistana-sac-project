﻿namespace unipaulistana.web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using unipaulistana.web.Models;
    using unipaulistana.model;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;

    [AllowAnonymous]
    public class HomeController : Controller
    {
        public HomeController(IUsuarioService usuarioService, IDiretivaSegurancaService diretivaSegurancaService)
        {
            this.usuarioService = usuarioService;
            this.diretivaSegurancaService = diretivaSegurancaService;
        }

        readonly IUsuarioService usuarioService;
        readonly IDiretivaSegurancaService diretivaSegurancaService;

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Login dados)
        {
            if (!ModelState.IsValid)
                return View(dados);

            Usuario usuario = this.usuarioService.Login(dados.Email, dados.Senha);

            if (usuario.ContemUsuario())
            {
                IEnumerable<DiretivaSeguranca> diretivas = new List<DiretivaSeguranca>();

                if(usuario.UsuarioAdmin())
                    diretivas = this.diretivaSegurancaService.ObterTodos();
                else
                    diretivas = this.diretivaSegurancaService.ObterDiretivasAssociadasGrupoDoUsuario(usuario.UsuarioID);

                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Sid, usuario.UsuarioID.ToString()),
                        new Claim(ClaimTypes.Name, usuario.Nome),
                        new Claim(ClaimTypes.Email, usuario.Email),
                        new Claim(ClaimTypes.UserData, usuario.Foto)
                    };

                foreach(var item in diretivas)
                    claims.Add(new Claim(item.Nome, ""));     

                var userIdentity = new ClaimsIdentity(claims, "login");
                var principal = new ClaimsPrincipal(userIdentity);

                HttpContext.SignInAsync(principal);
                return RedirectToAction("Index", "Usuario");
            }
            else
            {
                TempData["UserLoginFailed"] = "Falha ao tentar autenticar o usuário.";
                return View();
            }
        }

        public IActionResult AcessoNegado()
        {
            return View();
        }

        [HttpGet]  
        public IActionResult Logout()  
        {  
            HttpContext.SignOutAsync(); 
            return RedirectToAction("Index", "Home");  
        }  
    }
}
