using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication66.Data;
using WebApplication66.DTOs;
using WebApplication66.Models;

namespace WebApplication66.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrdersController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] OrderQueryParameters queryParams)
        {
            var query = _context.Orders.AsQueryable();
            if (!string.IsNullOrWhiteSpace(queryParams.Number))
            {
                query = query.Where(o => o.Number.Contains(queryParams.Number));
            }

            if (queryParams.MinTotal.HasValue)
            {
                query = query.Where(o => o.Total >= queryParams.MinTotal.Value);
            }

            if (queryParams.MaxTotal.HasValue)
            {
                query = query.Where(o => o.Total <= queryParams.MaxTotal.Value);
            }
            query = queryParams.SortBy?.ToLower() switch
            {
                "number" => query.OrderBy(o => o.Number),
                "number_desc" => query.OrderByDescending(o => o.Number),
                "total" => query.OrderBy(o => o.Total),
                "total_desc" => query.OrderByDescending(o => o.Total),
                "date" => query.OrderBy(o => o.CreatedAt),
                _ => query.OrderByDescending(o => o.CreatedAt)
            };
            int totalCount = await query.CountAsync();

            var orders = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<OrderReadDto>>(orders);

            var result = new PagedResultDto<OrderReadDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize
            };

            return Ok(result);
        }
    }
}
