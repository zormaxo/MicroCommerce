using Basket.API.IntegrationEvents.Events;
using Basket.API.Model;
using Basket.API.Services;
using EventBus.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Basket.API.Controllers;

[Route("api/v1/[controller]")]
[Authorize]
[ApiController]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository _repository;
    private readonly IIdentityService _identityService;
    private readonly IEventBus _eventBus;
    private readonly ILogger<BasketController> _logger;

    public BasketController(
        ILogger<BasketController> logger,
        IBasketRepository repository,
        IIdentityService identityService,
        IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _identityService = identityService;
        _eventBus = eventBus;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CustomerBasket>> GetBasketByIdAsync(string id)
    {
        var basket = await _repository.GetBasketAsync(id);

        return Ok(basket ?? new CustomerBasket(id));
    }

    [Route("update")]
    [HttpPost]
    [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CustomerBasket>> UpdateBasketAsync([FromBody] CustomerBasket value)
    { return Ok(await _repository.UpdateBasketAsync(value)); }

    [Route("additem")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [HttpPost]
    public async Task<ActionResult> AddItemToBasket([FromBody] BasketItem basketItem)
    {
        var userId = _identityService.GetUserName();

        var basket = await _repository.GetBasketAsync(userId);

        if (basket == null)
        {
            basket = new CustomerBasket(userId);
        }

        basket.Items.Add(basketItem);

        await _repository.UpdateBasketAsync(basket);

        return Ok();
    }

    [Route("checkout")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> CheckoutAsync([FromBody] BasketCheckout basketCheckout)
    {
        var userId = basketCheckout.Buyer;

        var basket = await _repository.GetBasketAsync(userId);

        if (basket == null)
        {
            return BadRequest();
        }

        var userName = _identityService.GetUserName();
        ;

        var eventMessage = new OrderCreatedIntegrationEvent(
            userId,
            userName,
            basketCheckout.City,
            basketCheckout.Street,
            basketCheckout.State,
            basketCheckout.Country,
            basketCheckout.ZipCode,
            basketCheckout.CardNumber,
            basketCheckout.CardHolderName,
            basketCheckout.CardExpiration,
            basketCheckout.CardSecurityNumber,
            basketCheckout.CardTypeId,
            basketCheckout.Buyer,
            basketCheckout.RequestId,
            basket);

        // Once basket is checkout, sends an integration event to
        // ordering.api to convert basket to order and proceeds with
        // order creation process
        try
        {
            _eventBus.Publish(eventMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR Publishing integration event: {IntegrationEventId}", eventMessage.Id);

            throw;
        }

        return Accepted();
    }

    // DELETE api/values/5
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
    public async Task DeleteBasketByIdAsync(string id) { await _repository.DeleteBasketAsync(id); }
}
