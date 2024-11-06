using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class EquipmentInfoController : ControllerBase
{
    private readonly Connection _context;

    public EquipmentInfoController(Connection context)
    {
        _context = context;
    }

    [HttpGet("EquipmentTypes")]
    public async Task<IActionResult> GetEquipmentTypes()
    {
        try
        {
            var types = await _context.Type.ToListAsync();
            return Ok(types);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("EquipmentStatuses")]
    public async Task<IActionResult> GetEquipmentStatuses()
    {
        try
        {
            var statuses = await _context.Status.ToListAsync();
            return Ok(statuses);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
