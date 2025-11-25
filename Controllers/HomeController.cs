using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionEmpleados.Models;
using System.Diagnostics;

namespace GestionEmpleados.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Datos para el dashboard
                ViewBag.TotalEmpleados = await _context.Empleados.CountAsync();
                ViewBag.EmpleadosActivos = await _context.Empleados.CountAsync(e => e.Activo);
                ViewBag.NuevosEsteMes = await _context.Empleados
                    .CountAsync(e => e.FechaCreacion.Month == DateTime.Now.Month);

                // Calcular salario promedio
                var salarioPromedio = await _context.Empleados
                    .Where(e => e.Activo)
                    .AverageAsync(e => (double?)e.Salario) ?? 0;
                ViewBag.SalarioPromedio = salarioPromedio.ToString("N0");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar datos del dashboard");
                // Valores por defecto en caso de error
                ViewBag.TotalEmpleados = 0;
                ViewBag.EmpleadosActivos = 0;
                ViewBag.NuevosEsteMes = 0;
                ViewBag.SalarioPromedio = "0";
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}