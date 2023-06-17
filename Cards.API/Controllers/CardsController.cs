using Cards.API.Data;
using Cards.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cards.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardsController : Controller
    {
        private readonly CardsDbContext cardsDbContext;

        public CardsController(CardsDbContext cardsDbContext)
        {
            this.cardsDbContext = cardsDbContext;
        }

        // get all card
        [HttpGet]
        public async Task<IActionResult> GetAllCards()
        {
             var cards = await cardsDbContext.Cards.ToListAsync();
            return Ok(cards);
        }

        // get single card
        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetCard")]
        public async Task<IActionResult> GetCard([FromRoute] Guid id)
        {
            var card = await cardsDbContext.Cards.FirstOrDefaultAsync(c => c.Id == id);
            if (card != null )
            {
               return Ok(card);
            }
            return NotFound("card not found");
        }

        // Add card
        [HttpPost]
        public async Task<IActionResult> AddCard([FromBody] Card card)
        {
            if (card == null)
            {
                return BadRequest("Invalid card data");
            }

            card.Id = Guid.NewGuid();
            await cardsDbContext.Cards.AddAsync(card);
            await cardsDbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCard), new { id = card.Id }, card);
        }


        // Update card
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCard([FromRoute] Guid id, [FromBody] Card card)
        {
            var existingCard = await cardsDbContext.Cards.FirstOrDefaultAsync(c => c.Id == id);

            if (existingCard == null)
            {
                return NotFound("Card not found");
            }

            if (card == null)
            {
                return BadRequest("Invalid card data");
            }

            // Update the properties of the existing card with the values from the provided card object
            existingCard.CardHolderName = card.CardHolderName;
            existingCard.CardNumber = card.CardNumber;
            existingCard.CardExpiryMonth = card.CardExpiryMonth;
            existingCard.CardExpiryYear = card.CardExpiryYear;
            existingCard.CVC = card.CVC;

            await cardsDbContext.SaveChangesAsync();

            return Ok(existingCard);
        }

        // Remove card
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> RemoveCard([FromRoute] Guid id)
        {
            var existingCard = await cardsDbContext.Cards.FirstOrDefaultAsync(c => c.Id == id);

            if (existingCard == null)
            {
                return NotFound("Card not found");
            }

            cardsDbContext.Cards.Remove(existingCard);
            await cardsDbContext.SaveChangesAsync();

            return NoContent();
        }


    }
}
