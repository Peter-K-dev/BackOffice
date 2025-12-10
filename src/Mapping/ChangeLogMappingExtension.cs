using BackOffice.Entities;
using BackOffice.Models.ChangeLog;

namespace BackOffice.Mapping
{
	public static class ChangeLogMappingExtension
	{
		public static ChangeLogResponseModel MapToChangeLogResponseModel(
			this ChangeLog source)
		{
			return new ChangeLogResponseModel
			{
				Id = source.Id,
				Created = source.Created,
				ChangeType = source.ChangeType,
				RuleName = source.RuleName,
				NewValue = source.NewValue,
				OldValue = source.OldValue,

			};
		}
	}
}
