using BackOffice.Common;
using System.ComponentModel.DataAnnotations;
namespace BackOffice.Exceptions;


public class DomainRuleNotFoundException(): ProblemDetailException(StatusCodes.Status404NotFound, "Not Found", "Rule Item Not Found", "Not Found");

public class DomainRuleExistsException(): ProblemDetailException(StatusCodes.Status400BadRequest, "Rule exists", "Rule with same name already exists", "Bad Request");

public class DomainRuleInvalidJsonException() : ProblemDetailException(StatusCodes.Status400BadRequest, "JSON parse error", "The JSON value could not be converted to System.Text.Json.Nodes.JsonObject", "Bad Request");
public class DomainRuleJsonDataEmptyException() : ProblemDetailException(StatusCodes.Status400BadRequest, "Rule data empty", "The JSON value could not be empty object", "Bad Request");
public class DomainRuleModifiedException() : ProblemDetailException(StatusCodes.Status400BadRequest, "Rule modified", "Rule was modified before update", "Bad Request");

public class DomainRuleValidationException(List<ValidationResult> errors)
	: ProblemDetailException(StatusCodes.Status400BadRequest, "One or more validation errors occurred.", errors,
		"Bad Request");

