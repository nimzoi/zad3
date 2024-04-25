using AnimalRestAPI.Animals;
using Microsoft.AspNetCore.Mvc;

namespace AnimaAPI.Animals;

[ApiController]
[Route("/api/animals")]
public class AnimalController(IAnimalService service) : ControllerBase
{
    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType( StatusCodes.Status400BadRequest)]
    public IActionResult GetAllAnimals([FromQuery] string? orderBy)
    {
        orderBy ??= "name";
        if (!AnimalRepository.ValidOrderParameters.Contains(orderBy))
        {
            return BadRequest($"Cannot sort by: {orderBy}");
        }

        var animals = service.GetAllAnimals(orderBy);
        return Ok(animals);
    }

    
    [HttpPost("")]
    [ProducesResponseType( StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    
    public IActionResult CreateAnimal([FromBody] CreateAnimalDTO dto)
    {
        var createdAnimal = service.AddAnimal(dto);
        return createdAnimal != null
            ? Created("", createdAnimal)
            : Conflict("Could not create a new Animal");
    }

    [HttpPut("{idAnimal:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateAnimal([FromRoute] int idAnimal, [FromBody] UpdateAnimalDTO dto)
    {

        var animal = service.GetAnimalById(idAnimal);
        if (animal == null)
        {
            return NotFound($"Animal with id: {idAnimal} not found");
        }

        var updatedAnimal = service.UpdateAnimal(idAnimal, dto);
        return updatedAnimal != null
            ? Ok(new { message = $"Updated Animal with id: {idAnimal}", animal = updatedAnimal })
            : Conflict($"Could not update Animal with id: {idAnimal}");
    }
    [HttpDelete("{idAnimal:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType( StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    
    public IActionResult DeleteAnimal([FromRoute] int idAnimal)
    {
        var animal = service.GetAnimalById(idAnimal);
        if (animal == null)
        {
            return NotFound($"Animal with id: {idAnimal} not found");
        }

        var deletedAnimal = service.DeleteAnimal(idAnimal);
        return deletedAnimal != null
            ? Ok(new { message = $"Removed Animal with id: {idAnimal}", animal = deletedAnimal })
            : Conflict($"Could not remove Animal with id: {idAnimal}");
    }
}