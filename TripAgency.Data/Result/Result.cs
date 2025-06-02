using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Data.Result
{
    // Path: TripAgency.Core/Results/Result.cs (تحديث)

    namespace TripAgency.Core.Results
    {

        public class Result<T>
        {
            public bool IsSuccess { get; }
            public T? Value { get; }
            public string Message { get; } = string.Empty;
            public List<string> Errors { get; } = new List<string>();
            public ResultFailureType FailureType { get; }

          
            protected Result(bool isSuccess, T? value, string message, List<string>? errors, ResultFailureType failureType)
            {
                IsSuccess = isSuccess;
                Value = value;
                Message = message;
                Errors = errors ?? new List<string>();
                FailureType = failureType; // تعيين نوع الفشل
            }

            public static Result<T> Success(T? value)
            {
                return new Result<T>(true, value, string.Empty, null, ResultFailureType.None);
            }

          
            public static Result<T> Failure(string message, List<string>? errors = null, ResultFailureType failureType = ResultFailureType.BadRequest)
            {
                return new Result<T>(false, default, message, errors, failureType);
            }

        
            public static Result<T> NotFound(string message)
            {
                return new Result<T>(false, default, message, new List<string> { message }, ResultFailureType.NotFound);
            }
            public static Result<T> BadRequest(string message)
            {
                return new Result<T>(false, default, message, new List<string> { message }, ResultFailureType.BadRequest);
            }
        }

       
        public class Result
        {
            public bool IsSuccess { get; }
            public string Message { get; } = string.Empty;
            public List<string> Errors { get; } = new List<string>();
            public ResultFailureType FailureType { get; }

            protected Result(bool isSuccess, string message, List<string>? errors, ResultFailureType failureType)
            {
                IsSuccess = isSuccess;
                Message = message;
                Errors = errors ?? new List<string>();
                FailureType = failureType;
            }

            public static Result Success(string message= "")
            {
                return new Result(true, message, null, ResultFailureType.None);
            }

            public static Result Failure(string message, List<string>? errors = null, ResultFailureType failureType = ResultFailureType.BadRequest)
            {
                return new Result(false, message, errors, failureType);
            }

            public static Result NotFound(string message)
            {
                return new Result(false, message, new List<string> { message }, ResultFailureType.NotFound);
            }
            public static Result BadRequest(string message)
            {
                return new Result(false, message, new List<string> { message }, ResultFailureType.BadRequest);
            }
        }
    }
}
