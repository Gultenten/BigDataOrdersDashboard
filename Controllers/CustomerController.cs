using BigDataOrdersDashboard.Context;
using BigDataOrdersDashboard.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace BigDataOrdersDashboard.Controllers
{
    public class CustomerController : Controller
    {
        private readonly BigDataOrderDbContext _context;

        public CustomerController(BigDataOrderDbContext context)
        {
            _context = context;
        }

        public IActionResult CustomerList(int page = 1)
        {
            int pageSize = 12;
            var values = _context.Customers
                .OrderBy(p => p.CustomerId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)                
                .ToList();
            int totalCustomers = _context.Customers.Count();
            ViewBag.TotalPages = (int)Math.Ceiling(totalCustomers / (double)pageSize);
            ViewBag.CurrentPage = page;

            return View(values);
        }

        [HttpGet]
        public IActionResult CreateCustomer()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
            return RedirectToAction("CustomerList");
        }


        public IActionResult DeleteCustomer(int id)
        {
            var value = _context.Customers.Find(id);
            _context.Customers.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("CustomerList");
        }

        [HttpGet]
        public IActionResult UpdateCustomer(int id)
        {

            var value = _context.Customers.Find(id);
            return View(value);

        }

        [HttpPost]
        public IActionResult UpdateCustomer(Customer customer)
        {
            _context.Customers.Update(customer);
            _context.SaveChanges();
            return RedirectToAction("CustomerList");

        }
    }
    }

