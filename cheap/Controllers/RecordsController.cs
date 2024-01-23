using AutoMapper;
using cheap.Entities;
using cheap.Models;
using cheap.Models.Users;
using cheap.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace cheap.Controllers;

[EnableRateLimiting("fixed")]
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RecordsController : Controller
{
    private IMapper _mapper;
    private IBaseService<Record> _recordService;
    public RecordsController(IMapper mapper, IBaseService<Record> recordService)
    {
        _mapper = mapper;
        _recordService = recordService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateRecord([FromBody] RecordModel recordModel)
    {
        var userId = User.FindFirst("Id")?.Value;
        if (!String.IsNullOrEmpty(userId) && recordModel.UserId != new Guid(userId))
            throw new UnauthorizedAccessException("You are not this person or the ID is missing");
        var record = _mapper.Map<Record>(recordModel);

        var response = await _recordService.Add(new Guid(userId), record);
        if (response.Success)
        {
            return Ok(_mapper.Map<RecordModel>(response.Data));
        }

        return BadRequest(response);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRecord(Guid id)
    {
        var userId = User.FindFirst("Id")?.Value;

        var response = await _recordService.Get(new Guid(userId), id);
        if (response.Success)
        {
            return Ok(_mapper.Map<RecordModel>(response.Data));
        }

        return BadRequest(response);
    }
    
    [HttpPost("{id}")]
    public async Task<IActionResult> UpdateRecord([FromBody] RecordModel recordModel)
    {
        var userId = User.FindFirst("Id")?.Value;
        if (!String.IsNullOrEmpty(userId) && recordModel.UserId != new Guid(userId))
            throw new UnauthorizedAccessException("You are not this person or the ID is missing");
        var record = _mapper.Map<Record>(recordModel);

        var response = await _recordService.Update(new Guid(userId), record);
        if (response.Success)
        {
            return Ok(_mapper.Map<RecordModel>(response.Data));
        }

        return BadRequest(response);
    }
    
}
        