using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mirjan24.DataAccess.Contact.Repositories;
using Serilog;

namespace Mirjan24.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UzytkownicyController : ControllerBase
    {
        public IPodmiotyRepository PodmiotyRepository { get; }
        

        public UzytkownicyController(IPodmiotyRepository podmiotyRepository)
        {
            PodmiotyRepository = podmiotyRepository;
        }

        [HttpPost]
        public IActionResult Login([FromQuery] string login, [FromQuery] string password)
        {
            try
            {
                var user = PodmiotyRepository.Select(p => p.podEmailSys == login).SingleOrDefault();
                Log.Information($"Znaleziono użytkownika {login}");
                if (user != null)
                {
                    if (user.podHaslo == password)// HashHelper.CalculateSHA2hash(user.RowUID + password).ToLower())
                    {
                        return Ok(user);
                    }
                    Log.Error($"Użytkownik {login} wprowadził błędne hasło");
                    return BadRequest("Błędne hasło");
                }
                Log.Error($"Użytkownik {login} niezostał odnaleziony w bazie");
                return BadRequest("Uzytkownik o podanym loginie nie został odnaleziony w bazie");
            }
            catch(Exception ex)
            {
                Log.Error(ex.ToString());
                return BadRequest(ex);
            }
            
        }

        [HttpPost]
        [Route("InitializePasswordChange")]
        public IActionResult InitializePasswordChange([FromQuery] string login, string password)
        {
            try
            {
                Log.Information($"Uzytkownik {login} rozpoczął procedure zmiany hasła");
                //sprawdzenie czy użytkownik jest w bazie
                var user = PodmiotyRepository.Select(p => p.podEmailSys == login).SingleOrDefault();
                if (user == null)
                {
                    Log.Error($"Użytkownik {login} nie został odnaleziony w bazie");
                    return BadRequest("Użytkownik o podanym loginie nie został odnaleziony w bazie");
                }
                else
                {
                    //jeśli istnieje to zmień hasło
                    user.podHaslo = password;
                    PodmiotyRepository.Modify(user);
                    PodmiotyRepository.Save();

                    Log.Information("Zmiana hasła przebiegła pomyślnie");
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(ex); 
            }
        }
    }
}
