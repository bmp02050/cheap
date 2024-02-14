using cheap.Entities;

namespace cheap.Models;

public class Response<T>
{
    public T Data { get; set; }
    public Boolean Success { get; set; }
    public String? ErrorMessage { get; set; }

    public Response(Boolean success, T data)
    {
        Data = data;
        Success = success;
    }

    public Response(Boolean success, String errorMessage)
    {
        Success = success;
        ErrorMessage = errorMessage;
    }

 
}