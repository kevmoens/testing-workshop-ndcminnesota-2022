using FluentAssertions;
using ForeignExchange.Api.Logging;
using ForeignExchange.Api.Models;
using ForeignExchange.Api.Repositories;
using ForeignExchange.Api.Services;
using ForeignExchange.Api.Validation;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace ForeignExchange.Api.Tests.Unit;

public class QuoteServiceTests
{
    private readonly QuoteService _sut;
    private readonly IRatesRepository _ratesRepository = Substitute.For<IRatesRepository>();
    private readonly ILoggerAdapter<QuoteService> _logger = Substitute.For<ILoggerAdapter<QuoteService>>();

    public QuoteServiceTests()
    {
        _sut = new QuoteService(
            _ratesRepository, _logger);
    }
    
    [Theory]
    [InlineData("GBP", "EUR", 100, 1.6, 160)]
    public async Task GetQuoteAsync_ReturnsQuote_WhenCurrenciesAreValid(
        string fromCurrency, string toCurrency, decimal amount, decimal rate, decimal final)
    {
        // Arrange
        var expectedRate = new FxRate
        {
            FromCurrency = fromCurrency,
            ToCurrency = toCurrency,
            TimestampUtc = DateTime.UtcNow,
            Rate = rate
        };
        
        _ratesRepository.GetRateAsync(fromCurrency, toCurrency)
            .Returns(expectedRate);
        
        // Act
        var result = await _sut.GetQuoteAsync(fromCurrency, toCurrency, amount);

        // Assert
        result!.QuoteAmount.Should().Be(final);
    }

    [Fact]
    public async Task GetQuoteAsync_ShouldThrowSameCurrencyException_WhenSameCurrencyIsUsed()
    {
        // Arrange
        var fromCurrency = "GBP";
        var toCurrency = "GBP";
        var amount = 100;
        var expectedRate = new FxRate
        {
            FromCurrency = fromCurrency,
            ToCurrency = toCurrency,
            TimestampUtc = DateTime.UtcNow,
            Rate = 1.6m
        };

        _ratesRepository.GetRateAsync(fromCurrency, toCurrency).Returns(expectedRate);

        // Act
        var action = () => _sut.GetQuoteAsync(fromCurrency, toCurrency, amount);

        // Assert
        await action.Should()
            .ThrowAsync<SameCurrencyException>()
            .WithMessage($"You cannot convert currency {fromCurrency} to itself");
    }

    [Fact]
    public async Task GetQuoteAsync_ShouldLogAppropriateMessage_WhenInvoked()
    {
        // Arrange
        var fromCurrency = "GBP";
        var toCurrency = "USD";
        var amount = 100;
        var expectedRate = new FxRate
        {
            FromCurrency = fromCurrency,
            ToCurrency = toCurrency,
            TimestampUtc = DateTime.UtcNow,
            Rate = 1.6m
        };

        _ratesRepository.GetRateAsync(fromCurrency, toCurrency)
            .Returns(expectedRate);

        // Act
        await _sut.GetQuoteAsync(fromCurrency, toCurrency, amount);
        
        // Assert
        _logger.Received(1).LogInformation("Retrieved quote for currencies {FromCurrency}->{ToCurrency} in {ElapsedMilliseconds}ms",
            Arg.Is<object[]>(x => x[0].ToString() == fromCurrency &&
                                  x[1].ToString() == toCurrency ));
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task GetQuoteAsync_ThrowsException_WhenAmountIsZeroOrNegative(decimal amount)
    {
        // Arrange
        var fromCurrency = "GBP";
        var toCurrency = "USD";
        var expectedRate = new FxRate
        {
            FromCurrency = fromCurrency,
            ToCurrency = toCurrency,
            TimestampUtc = DateTime.UtcNow,
            Rate = 1.6m
        };
        
        _ratesRepository.GetRateAsync(fromCurrency, toCurrency)
            .Returns(expectedRate);
        
        // Act
        var quoteAction = () => _sut.GetQuoteAsync(fromCurrency, toCurrency, amount);

        // Assert
        await quoteAction.Should().ThrowAsync<NegativeAmountException>()
            .WithMessage("You can only convert a positive amount of money");
    }
    
    [Fact]
    public async Task GetQuoteAsync_ShouldReturnNull_WhenNoRateExists()
    {
        // Arrange
        var fromCurrency = "GBP";
        var toCurrency = "USD";
        var amount = 100;

        _ratesRepository.GetRateAsync(fromCurrency, toCurrency)
            .ReturnsNull();
        
        // Act
        var quote = await _sut.GetQuoteAsync(fromCurrency, toCurrency, amount);

        // Assert
        quote.Should().BeNull();
    }
}
