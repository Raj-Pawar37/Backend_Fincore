namespace Backend_Fincore.Response
{
   
        public class ApiResponse<T>
        {
            public bool Success { get; set; }

            public string Message { get; set; } = string.Empty;

            public T? Data { get; set; }

            public object? Error { get; set; }
        }
 }

