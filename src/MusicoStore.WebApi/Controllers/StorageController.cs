using Microsoft.AspNetCore.Mvc;
using MusicoStore.Domain.DTOs.Stock;
using MusicoStore.Domain.DTOs.Storage;
using MusicoStore.Domain.Interfaces.Service;
using MusicoStore.Domain.Constants;

namespace MusicoStore.WebApi.Controllers;

public class StorageController(IStorageService storageService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        List<StorageDTO> storages = await storageService.FindAllAsync(ct);
        return Ok(storages);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        if (!storageService.DoesExistById(id))
        {
            return NotFound(string.Format(ErrorMessages.NotFoundFormat, "Storage", $"id '{id}'"));
        }

        StorageDTO storage = await storageService.FindByIdAsync(id, ct);
        return Ok(storage);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateStorageDTO req, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        StorageDTO created = await storageService.CreateAsync(req, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.StorageId }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateStorageDTO req, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!storageService.DoesExistById(id))
        {
            return NotFound(string.Format(ErrorMessages.NotFoundFormat, "Storage", $"id '{id}'"));
        }

        await storageService.UpdateAsync(id, req, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        if (!storageService.DoesExistById(id))
        {
            return NotFound(string.Format(ErrorMessages.NotFoundFormat, "Storage", $"id '{id}'"));
        }

        await storageService.DeleteByIdAsync(id, ct);
        return NoContent();
    }

    [HttpGet("{storageId:int}/stocks")]
    public async Task<IActionResult> GetStocks(int storageId, CancellationToken ct)
    {
        if (!storageService.DoesExistById(storageId))
        {
            return NotFound(string.Format(ErrorMessages.NotFoundFormat, "Storage", $"id '{storageId}'"));
        }

        List<StockSummaryForStorageDTO> stocks = await storageService.FindStocksByStorageIdAsync(storageId, ct);
        return Ok(stocks);
    }

    [HttpPost("{storageId:int}/stocks")]
    public async Task<IActionResult> AddOrUpdateStock(int storageId, StockUpdateDTO req, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!storageService.DoesExistById(storageId))
        {
            return NotFound(string.Format(ErrorMessages.NotFoundFormat, "Storage", $"id '{storageId}'"));
        }

        try
        {
            StockSummaryForStorageDTO updatedStock = await storageService.AddOrUpdateStockAsync(storageId, req, ct);
            return Ok(updatedStock);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message); // 400
        }
    }
}
