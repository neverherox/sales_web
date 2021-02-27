using AutoMapper;
using Ninject.Extensions.Logging;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Task5.BL.DTO;
using Task5.BL.Services.Contracts;
using Task5.Web.Models;
using Task5.Web.Models.Client;
using Task5.Web.Models.Filters;
using Task5.Web.Util;

namespace Task5.Web.Controllers
{
    [Authorize(Roles = "user")]
    public class ClientController : Controller
    {
        private IClientService _clientService;
        private IOrderService _orderService;
        private IMapper _mapper;
        private ILogger _logger;
        public ClientController(IClientService clientService, IOrderService orderService, ILogger logger)
        {
            _clientService = clientService;
            _orderService = orderService;
            _logger = logger;
            _mapper = new Mapper(AutoMapperWebConfig.Configure());
        }
        [HttpGet]
        public ActionResult Index(int? page)
        {
            ClientFilter filter = (ClientFilter)Session["clientFilter"];
            ViewBag.CurrentPage = page ?? 1;
            if (filter != null)
            {
                return View(filter);
            }
            return View(new ClientFilter());
        }
        [HttpGet]
        public ActionResult Clients(int? page)
        {
            try
            {
                var clients = _mapper.Map<IEnumerable<ClientDTO>, IEnumerable<ClientViewModel>>(_clientService.GetClients());
                ClientFilter filter = (ClientFilter)Session["clientFilter"];
                if (filter != null)
                {
                    clients = GetFilteredClients(clients, filter);
                }
                ViewBag.CurrentPage = page;
                return PartialView(clients.ToPagedList(page ?? 1, 4));
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult ApplyFilter(ClientFilter model)
        {
            Session["clientFilter"] = model;
            return RedirectToAction("Clients");
        }

        [HttpGet]
        public ActionResult ClearFilter()
        {
            Session["clientFilter"] = null;
            return RedirectToAction("Clients");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Create(int? page)
        {
            try
            {
                var createClient = new CreateClientViewModel();
                ViewBag.CurrentPage = page;
                return View(createClient);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateClientViewModel model, int? page)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var clientDTO = _mapper.Map<CreateClientViewModel, ClientDTO>(model);
                _clientService.Create(clientDTO);
                return RedirectToAction("Index", new { page = page });
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.Message);
                return View("Error");
            }
        }
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id, int? page)
        {
            try
            {
                var clientDTO = _clientService.GetClient(x => x.Id == id);
                var editClient = new EditClientViewModel
                {
                    Name = clientDTO.Name,
                    PhoneNumber = clientDTO.PhoneNumber
                };
                ViewBag.CurrentPage = page;
                return View(editClient);
            }
            catch(Exception ex)
            {
                _logger.Warn(ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditClientViewModel model, int? page)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var clientDTO = _clientService.GetClient(x => x.Id == model.Id);
                clientDTO.Name = model.Name;
                clientDTO.PhoneNumber = model.PhoneNumber;
                _clientService.Update(clientDTO);
                return RedirectToAction("Index", new { page = page });
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.Message);
                return View("Error");
            }
        }
        [HttpGet]
        public ActionResult Details(int? id, int? page)
        {
            try
            {
                var client = _mapper.Map<ClientDTO, ClientViewModel>(_clientService.GetClient(x => x.Id == id));
                ViewBag.CurrentPage = page;
                return View(client);
            }
            catch(Exception ex)
            {
                _logger.Warn(ex.Message);
                return View("Error");
            }
        }
        [HttpGet]
        public ActionResult ClientSales(int? id, int? page)
        {
            try
            {
                var orders = _mapper.Map<IEnumerable<OrderDTO>, IEnumerable<OrderViewModel>>
                                       (_orderService.GetOrders(x => x.ClientId == id))
                                       .ToPagedList(page ?? 1, 4);
                ViewBag.ClientId = id;
                return PartialView(orders);
            }
            catch(Exception ex)
            {
                _logger.Warn(ex.Message);
                return View("Error");
            }
        }
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id, int? page, bool? saveChangesError = false)
        {
            try
            {
                var client = _mapper.Map<ClientDTO, ClientViewModel>(_clientService.GetClient(x => x.Id == id));
                ViewBag.CurrentPage = page;
                return View(client);
            }
            catch(Exception ex)
            {
                _logger.Warn(ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id, int? page)
        {
            try
            {
                var clientDTO = _clientService.GetClient(x => x.Id == id);
                _clientService.Remove(clientDTO);
                return RedirectToAction("Index", new { page = page });
            }
            catch(Exception ex)
            {
                _logger.Warn(ex.Message);
                return View("Error");
            }
        }

        [HttpGet]
        public JsonResult CheckClientName(string Name)
        {
            var client = _clientService.GetClient(x => x.Name == Name);
            var result = (client == null);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CheckClientPhoneNumber(string PhoneNumber)
        {
            var client = _clientService.GetClient(x => x.PhoneNumber == PhoneNumber);
            var result = (client == null);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetChartData()
        {
            var clients = _clientService.GetClients();
            var data = clients.Select(x => new object[] { x.Name, _orderService.GetOrders(y => y.ClientId == x.Id).Count() }).ToArray();
            return Json(data.ToArray(), JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<ClientViewModel> GetFilteredClients(IEnumerable<ClientViewModel> clients, ClientFilter filter)
        {
            var expression = filter.ToExpression();
            var predicate = expression.Compile();
            if (filter.Name != null)
            {
                clients = clients.Where(x => predicate(x.Name, filter.Name));
            }
            if (filter.PhoneNumber != null)
            {
                clients = clients.Where(x => predicate(x.PhoneNumber, filter.PhoneNumber));
            }
            return clients;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && _clientService != null)
            {
                _clientService.Dispose();
                _orderService.Dispose();
                _orderService = null;
                _clientService = null;
            }
            base.Dispose(disposing);
        }
    }
}