using AutoMapper;
using WidgetAndCo.Business;
using WidgetAndCo.Core;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Tests;

public class ReviewServiceTests
{
    private Mock<IReviewRepository> reviewRepositoryMock;
    private Mock<IMapper> mapperRepositoryMock;

    private ReviewService reviewService;


    [SetUp]
    public void Setup()
    {
        reviewRepositoryMock = new Mock<IReviewRepository>();
        mapperRepositoryMock = new Mock<IMapper>();

        reviewService = new ReviewService(reviewRepositoryMock.Object, mapperRepositoryMock.Object);
    }

    public Review GetReview()
    {
        return new Review
        {
            UserId = Guid.Parse("96278634-3b8c-4eef-879d-cfeaa07988f0"),
            Title = "Test Title",
            Description = "Test Description",
            Rating = 5
        };
    }

    public ReviewRequestDto GetReviewRequestDto()
    {
        return new ReviewRequestDto
        {
            ProductId = Guid.Parse("50348bf1-5e5b-4139-88c0-14ba8288d906"),
            Title = "Test Title",
            Description = "Test Description",
            Rating = 5
        };
    }

    public ReviewResponseDto GetReviewResponseDto()
    {
        return new ReviewResponseDto
        {
            UserId = Guid.Parse("96278634-3b8c-4eef-879d-cfeaa07988f0"),
            Title = "Test Title",
            Description = "Test Description",
            Rating = 5
        };
    }

    [Test]
    public async Task StoreReview_StoresReviewInRepository()
    {
        // Arrange
        var reviewRequest = GetReviewRequestDto();
        var userId = Guid.NewGuid();

        // Act
        await reviewService.StoreReview(reviewRequest, userId);

        // Assert
        reviewRepositoryMock.Verify(x => x.StoreReviewAsync(It.IsAny<ReviewDelegateDto>()), Times.Once);
    }

    [Test]
    public async Task GetReviews_ReturnsReviewsFromRepository()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var reviews = new List<Review>
        {
            GetReview(),
            GetReview(),
            GetReview()
        };

        reviewRepositoryMock.Setup(x => x.GetReviewsAsync(productId)).ReturnsAsync(reviews);

        // Act
        var result = await reviewService.GetReviews(productId);

        // Assert
        Assert.AreEqual(reviews.Count, result.Count());
    }

    [Test]
    public async Task GetReview_ReturnsReviewFromRepository()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var reviewId = Guid.NewGuid();
        var review = GetReview();

        reviewRepositoryMock.Setup(x => x.GetReviewAsync(productId, reviewId)).ReturnsAsync(review);
        mapperRepositoryMock.Setup(x => x.Map<ReviewResponseDto>(review)).Returns(GetReviewResponseDto());

        // Act
        var result = await reviewService.GetReview(productId, reviewId);

        // Assert
        Assert.AreEqual(GetReviewResponseDto(), result);
    }

}