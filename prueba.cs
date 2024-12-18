// See https://aka.ms/new-console-template for more information
// Dependencias necesarias
using System.Security.Claims;
using System.Text;
using Tienda.Application.Interfaces;
using Tienda.Core.Entities;
using Tienda.Infrastructure.Data;

// Modelo Cliente
namespace Tienda.Core.Entities
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class Pedido
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public DateTime FechaPedido { get; set; }
        public decimal Total { get; set; }
        public List<PedidoProducto> PedidoProductos { get; set; }
    }

    public class PedidoProducto
    {
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
    }
}

// Interfaces
namespace Tienda.Application.Interfaces
{
    public interface IClienteService
    {
        Task<bool> CrearClienteAsync(Cliente cliente);
    }

    public interface IProductoService
    {
        Task<IEnumerable<Producto>> ObtenerProductosAsync(decimal? precioMin, decimal? precioMax, int? stockMin);
        Task<bool> ActualizarProductoAsync(int id, Producto producto);
    }

    public interface IPedidoService
    {
        Task<bool> CrearPedidoAsync(Pedido pedido);
        Task<Pedido> ObtenerPedidoAsync(int id);
    }
}

// Implementaciones de Servicios
namespace Tienda.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly TiendaDbContext _context;

        public ClienteService(TiendaDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CrearClienteAsync(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class ProductoService : IProductoService
    {
        private readonly TiendaDbContext _context;

        public ProductoService(TiendaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Producto>> ObtenerProductosAsync(decimal? precioMin, decimal? precioMax, int? stockMin)
        {
            var query = _context.Productos.AsQueryable();

            if (precioMin.HasValue) query = query.Where(p => p.Precio >= precioMin.Value);
            if (precioMax.HasValue) query = query.Where(p => p.Precio <= precioMax.Value);
            if (stockMin.HasValue) query = query.Where(p => p.Stock >= stockMin.Value);

            return await query.ToListAsync();
        }

        public async Task<bool> ActualizarProductoAsync(int id, Producto producto)
        {
            var existingProducto = await _context.Productos.FindAsync(id);
            if (existingProducto == null) return false;

            existingProducto.Precio = producto.Precio;
            existingProducto.Stock = producto.Stock;

            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class PedidoService : IPedidoService
    {
        private readonly TiendaDbContext _context;

        public PedidoService(TiendaDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CrearPedidoAsync(Pedido pedido)
        {
            var total = 0m;

            foreach (var pp in pedido.PedidoProductos)
            {
                var producto = await _context.Productos.FindAsync(pp.ProductoId);
                if (producto == null || producto.Stock < pp.Cantidad) return false;

                total += producto.Precio * pp.Cantidad;
                producto.Stock -= pp.Cantidad;
            }

            pedido.Total = total;
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Pedido> ObtenerPedidoAsync(int id)
        {
            return await _context.Pedidos
                .Include(p => p.PedidoProductos)
                .ThenInclude(pp => pp.Producto)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}

// DbContext
namespace Tienda.Infrastructure.Data
{
    public class TiendaDbContext : DbContext
    {
        public TiendaDbContext(DbContextOptions<TiendaDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoProducto> PedidoProductos { get; set; }
        public object DeleteBehavior { get; private set; }

        internal async Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}

// Controladores
namespace Tienda.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        private IActionResult BadRequest(string v)
        {
            throw new NotImplementedException();
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpPut]
        public async Task<IActionResult> ObtenerProductos([FromQuery] decimal? precioMin, [FromQuery] decimal? precioMax, [FromQuery] int? stockMin)
        {
            var productos = await _productoService.ObtenerProductosAsync(precioMin, precioMax, stockMin);
            return Ok(productos);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarProducto(int id, [FromBody] Producto producto)
        {
            var resultado = await _productoService.ActualizarProductoAsync(id, producto);
            if (!resultado) return BadRequest("Error al actualizar el producto.");
            return Ok();
        }

        private IActionResult Ok()
        {
            throw new NotImplementedException();
        }

        private IActionResult BadRequest(string v)
        {
            throw new NotImplementedException();
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        public PedidosController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpPost]
        public async Task<IActionResult> CrearPedido([FromBody] Pedido pedido)
        {
            var resultado = await _pedidoService.CrearPedidoAsync(pedido);
            if (!resultado) return BadRequest("Error al crear el pedido.");
            return Ok();
        }

        private IActionResult Ok(Pedido pedido)
        {
            throw new NotImplementedException();
        }

        private IActionResult BadRequest(string v)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPedido(int id)
        {
            var pedido = await _pedidoService.ObtenerPedidoAsync(id);
            if (pedido == null) return NotFound();
            return Ok(pedido);
        }

        private IActionResult NotFound()
        {
            throw new NotImplementedException();
        }
    }

    internal interface IActionResult
    {
    }

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IConfiguration configuration) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            if (login.Email == "admin@example.com" && login.Password == "password")
            {
                var token = GenerateJwtToken();
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }

        private string GenerateJwtToken()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "Admin"),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

// Modelo para autenticación
public class LoginModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}

