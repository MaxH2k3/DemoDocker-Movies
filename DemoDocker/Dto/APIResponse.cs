using System.Net;

namespace DemoDocker.Dto
{
	public class APIResponse
	{
		public APIResponse()
		{
		}

		public APIResponse(HttpStatusCode status, string v)
		{
			this.Status = status;
			this.Message = v;
		}

		public APIResponse(HttpStatusCode? status, string? message, object? data)
		{
			Status = status;
			Message = message;
			Data = data;
		}

		public HttpStatusCode? Status { get; set; }
		public string? Message { get; set; }
		public object? Data { get; set; }


	}
}
