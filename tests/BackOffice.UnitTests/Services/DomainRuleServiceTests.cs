using BackOffice.Database;
using BackOffice.Entities;
using BackOffice.Exceptions;
using BackOffice.Repositories;
using BackOffice.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Data;
using System.IO;

namespace BackOffice.UnitTests.Services
{
	[TestFixture]
	public class DomainRuleServiceTests
	{
		private DomainRuleService _domainRuleService;
		private Mock<IDomainRuleRepository> _domainRuleRepositoryMock;
		private Mock<ILogger<DomainRuleService>> _loggerMock;
		private Mock<INotificationRepository> _notificationRepositoryMock;
		private MemoryDbContext _dbContext;



		[SetUp]
		public void Setup()
		{
			var options = new DbContextOptionsBuilder<MemoryDbContext>()
				.UseInMemoryDatabase("db--" + Guid.NewGuid())
				.Options;

			_domainRuleRepositoryMock = new Mock<IDomainRuleRepository>();

			_dbContext = new MemoryDbContext(options);

			_notificationRepositoryMock = new Mock<INotificationRepository>();
			_loggerMock = new Mock<ILogger<DomainRuleService>>();

			_domainRuleService = new DomainRuleService(_domainRuleRepositoryMock.Object, _dbContext, _loggerMock.Object, _notificationRepositoryMock.Object);
		}

		[TearDown]
		public void Cleanup()
		{
			_dbContext?.Dispose();
		}

		[Test]
		public Task CreateRuleAsync_Should_ReturnError_WhenUseSameName()
		{
			//Arrange

			var rule = new DomainRule
			{
				Name = "creditLimits",
				Data = "{\"minCredit\" : 5000}"
			};

			_domainRuleRepositoryMock.Setup(x => x.GetByNameAsync(rule.Name)).ReturnsAsync(new DomainRule() { Name = rule.Name, Id = 1, Data = rule.Data });

			//Assert

			Assert.ThrowsAsync<DomainRuleExistsException>(() => _domainRuleService.CreateAsync(rule), "Should not create DomainRule with same name");
			return Task.CompletedTask;
		}

		[Test]
		public async Task CreateRuleAsync_ShouldPersist_AndCreateChangeLog()
		{
			//Arrange
			var rule = new DomainRule
			{
				Name = "creditLimits-" + Guid.NewGuid(),
				Data = "{\"minCredit\" : 5000}"
			};

			var notification = new Notification { CorrelationId = rule.Name, Data = "" };

			_domainRuleRepositoryMock.Setup(x => x.GetByNameAsync(rule.Name)).ReturnsAsync((DomainRule)null!);
			_domainRuleRepositoryMock.Setup(x => x.CreateAsync(rule)).ReturnsAsync(new DomainRule() { Id = 1, Name = rule.Name, Data = rule.Data });

			_notificationRepositoryMock.Setup(n => n.CreateAsync(notification)).ReturnsAsync(new Notification());

			//Act

			var result = await _domainRuleService.CreateAsync(rule);

			var changeLogResult = _dbContext.ChangeLogs.FirstOrDefaultAsync(c => c.RuleName == rule.Name);


			//Assert
			Assert.That(result, Is.EqualTo(1), "DomainRule not created");
			Assert.That(changeLogResult, Is.Not.Null, "Should create ChangeLog");
			_notificationRepositoryMock.Verify(n => n.CreateAsync(It.IsAny<Notification>()), Times.Never, "Should not be a Critical Change");
		}

		[Test]
		public async Task CreateRuleAsync_ShouldPersist_AndCreateChangeLog_AndCreateCriticalNotification()
		{
			//Arrange
			var rule = new DomainRule
			{
				Name = "creditLimits" + Guid.NewGuid(),
				Data = "{\"minCredit\" : 500}"
			};

			var notification = new Notification { CorrelationId = rule.Name, Data = "" };

			_domainRuleRepositoryMock.Setup(x => x.GetByNameAsync(rule.Name)).ReturnsAsync((DomainRule)null!);
			_domainRuleRepositoryMock.Setup(x => x.CreateAsync(rule)).ReturnsAsync(new DomainRule() { Id = 1, Name = rule.Name, Data = rule.Data });
			_notificationRepositoryMock.Setup(n => n.CreateAsync(notification)).ReturnsAsync(new Notification());

			//Act

			var result = await _domainRuleService.CreateAsync(rule);

			var changeLogResult = _dbContext.ChangeLogs.FirstOrDefaultAsync(c => c.RuleName == rule.Name).Result;


			//Assert

			Assert.That(result, Is.EqualTo(1), "DomainRule not created");
			Assert.That(changeLogResult, Is.Not.Null, "should create ChangeLog");
			_notificationRepositoryMock.Verify(n => n.CreateAsync(It.IsAny<Notification>()), Times.AtLeastOnce, "should be a critical ");

		}

		[Test]
		public Task UpdateRuleAsync_Should_ReturnError_WhenNotExists()
		{
			//Arrange
			var rule = new DomainRule()
			{
				Name = "creditLimits",
				Data = "{\"minCredit\" : 5000}",
				RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }
			};

			_domainRuleRepositoryMock.Setup(x => x.GetByIdAsync(rule.Id)).ReturnsAsync((DomainRule)null!);

			//Assert

			Assert.ThrowsAsync<DomainRuleNotFoundException>(() => _domainRuleService.UpdateAsync(rule));
			return Task.CompletedTask;
		}

		[Test]
		public Task UpdateRuleAsync_Should_ReturnError_WhenModified()
		{
			//Arrange
			var rule = new DomainRule()
			{
				Name = "creditLimits",
				Data = "{\"minCredit\" : 5000}",
				RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }
			};

			_domainRuleRepositoryMock.Setup(x => x.GetByIdAsync(rule.Id)).ReturnsAsync(new DomainRule
			{
				Name = "creditLimits",
				Data = "{\"minCredit\" : 5000}",
				RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 2 }
			});

			//Assert

			Assert.ThrowsAsync<DomainRuleModifiedException>(() => _domainRuleService.UpdateAsync(rule));
			return Task.CompletedTask;
		}

	}
}
