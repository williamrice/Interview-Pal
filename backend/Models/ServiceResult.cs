
public class ServiceResult
{
	public bool IsSuccess { get; private set; }
	public IEnumerable<string> Errors { get; private set; }

	protected ServiceResult(bool success, IEnumerable<string> errors)
	{
		IsSuccess = success;
		Errors = errors ?? Array.Empty<string>();
	}

	public static ServiceResult Success()
	{
		return new ServiceResult(true, Array.Empty<string>());
	}

	public static ServiceResult Failure(IEnumerable<string> errors) => new ServiceResult(false, errors);
	public static ServiceResult Failure(string error) => new ServiceResult(false, new[] { error });
}

public class ServiceResult<T> : ServiceResult
{
	public T? Data { get; private set; }

	private ServiceResult(bool success, T? data, IEnumerable<string> errors)
			: base(success, errors)
	{
		Data = data;
	}

	public static ServiceResult<T> Success(T data) => new ServiceResult<T>(true, data, Array.Empty<string>());
	public static ServiceResult<T> Failure(IEnumerable<string> errors) => new ServiceResult<T>(false, default, errors);
	public static ServiceResult<T> Failure(string error) => new ServiceResult<T>(false, default, new[] { error });
}
