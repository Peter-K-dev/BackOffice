using BackOffice.Entities;
using BackOffice.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using BackOffice.Mapping;
using BackOffice.Models.DomainRule;

namespace BackOffice.UnitTests.Mapping
{
	internal class DomainRuleMappingExtensionTests
	{
		[Test]
		public Task MapToDomainRule_Should_ReturnSuccess()
		{
			//Arrange
			var dateTime = DateTime.UtcNow;
			var rule = new DomainRule
			{
				Id = 1,
				Name = "creditLimits",
				Data = "{\"minCredit\" : 5000}",
				RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 },
				Created = dateTime
			};

			var ruleResponseModel = new DomainRuleResponseModel
			{
				Id = 1,
				Name = "creditLimits",
				Data = JsonNode.Parse(rule.Data)?.AsObject()!,
				RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 },
				Created = dateTime
			};

			//Act
			var ruleMapped = rule.MapToDomainRuleResponseModel();


			//Assert

			Assert.That(ruleMapped, Is.EqualTo(ruleResponseModel).UsingPropertiesComparer());
			return Task.CompletedTask;
		}
	}
}
