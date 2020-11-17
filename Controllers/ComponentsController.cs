using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Mirjan24.Api.Models;
using Mirjan24.DataAccess.Contact.POCO;
using Mirjan24.DataAccess.Contact.Repositories;
using Serilog;

namespace Mirjan24.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ComponentsController : ControllerBase
    {
        //Dev
        public IDaneDokumentowRepository daneDokumentow { get; }
        //Prod
        public IPodmiotyRepository PodmiotyRepository { get; }
        public IZamowieniaRepository ZamowieniaRepository { get; }
        public IPozycjeZamowieniaRepository PozycjeZamowieniaRepository { get; }

        public ComponentsController(IDaneDokumentowRepository dokumenty, IZamowieniaRepository zamowieniaRepository,
            IPodmiotyRepository podmiotyRepository, IPozycjeZamowieniaRepository pozycjeZamowieniaRepository)
        {
            //Dev
            daneDokumentow = dokumenty;
            //Prod            
            PodmiotyRepository = podmiotyRepository;
            ZamowieniaRepository = zamowieniaRepository;
            PozycjeZamowieniaRepository = pozycjeZamowieniaRepository;
        }

        [HttpGet]
        [Route("GetDane")]
        public IActionResult GetDane(int userId)
        {
            try
            {
                //Dev
                var dane = daneDokumentow.Select(c => c.UserId == userId).ToList();
                //Prod
                //var zamowienia = ZamowieniaRepository.SelectAll();// (c => c.).ToList();//po jakich danych pobierać zamówienia
                Log.Information($"Dokumenty użytkownika zostały pobrane");
                return Ok(dane);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(ex);
            }
        }
        [HttpGet]
        [Route("GetPozycje")]
        public IActionResult GetPozycje(int id)
        {
            try
            {
                //var pozycje = PozycjeZamowieniaRepository.Select(c => c.idZamowienia == id).ToList();

                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet]//wersja Prod
        [Route("GetAdresy")]
        public IActionResult GetAdresy(int userId)
        {
            try
            {
                var podmiotyAdresy = PodmiotyRepository.Select(c => c.idPodmioty == userId).ToList();
                return Ok(podmiotyAdresy);
            }
            catch(Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPost]//wersja Prod
        [Route("UpdateAdress")]
        public IActionResult UpdateAdress([FromBody] CustomerDataModel model)
        {
            try
            {
                var adres = PodmiotyRepository.FindEntity(model.id);

                if (adres == null)
                {
                    Log.Error($"Adresu o numerze Id:{model.id} nie zostały odnalezione");
                    return NotFound();
                }
                else
                {
                    adres.podEmail = model.podEmail;
                    adres.podRegon = model.podRegon;
                    adres.podImie = model.podImie;
                    adres.podKodPocztowy = model.podKodPocztowy;
                    adres.podMiejscowosc = model.podMiejscowosc;
                    adres.podNaziwsko = model.podNaziwsko;
                    adres.podNip = model.podNip;
                    adres.podNumerDomu = model.podNumerDomu;
                    adres.podNumerMieszkania = model.podNumerMieszkania;
                    adres.podTelStacjonarny = model.podTelStacjonarny;
                    adres.podTelKom = model.podTelKom;
                    adres.podUlica = model.podUlica;
                    adres.czynnyPodatnikVat = model.czynnyPodatnikVat;
                    adres.podDateMod = DateTime.Now;
                }
                PodmiotyRepository.Modify(adres);
                PodmiotyRepository.Save();

                Log.Information($"Dane adresu o numerze Id:{adres.id} zostały zmodyikowane");
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("CreateAdress")]
        public IActionResult CreateAdress([FromBody] CustomerDataModel model)//tutaj przemyślec jak dodawać nowe dane 
        {
            try
            {
                //dane wymagane w bazie przypisujemy standardowe?
                var adres = new podmiotyPOCO()
                {
                    podEmail = model.podEmail,
                    podRegon = model.podRegon,
                    podImie = model.podImie,
                    podKodPocztowy = model.podKodPocztowy,
                    podMiejscowosc = model.podMiejscowosc,
                    podNazwa = model.podNazwa,
                    podNaziwsko = model.podNaziwsko,
                    podNip = model.podNip,
                    podNumerDomu = model.podNumerDomu,
                    podNumerMieszkania = model.podNumerMieszkania,
                    podTelStacjonarny = model.podTelStacjonarny,
                    podTelKom = model.podTelKom,
                    podUlica = model.podUlica,
                    czynnyPodatnikVat = model.czynnyPodatnikVat,
                    idPodmioty = model.idPodmioty,
                    podDateAdd = DateTime.Now,
                    podDateMod = DateTime.Now,
                };
                PodmiotyRepository.Add(adres);
                PodmiotyRepository.Save();
            
                Log.Information($"Dane nowego adresu zostały dodane do bazy danych");
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(ex);
            }
        }
    }
}
