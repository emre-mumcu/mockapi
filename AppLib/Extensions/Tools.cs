using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MockAPI.AppLib.Extensions
{
    public static class Tools
    {
        public static string ToStringEx(this RouteValueDictionary values)
        {
            try
            {
                return values?.Select(i => $"{i.Key}: {i.Value}")?.Aggregate((i, j) => $"{i}; {j}") ?? string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string ToStringEx(this ModelStateDictionary values)
        {
            try
            {
                string isValid = $"Isvalid: {values.IsValid.ToString()}";
                string errCount = $"ErrorCount: {values.ErrorCount}";
                string errors = values.IsValid ?
                    string.Empty :
                    values?.Select(i => $"{i.Key}: {i.Value}")?.Aggregate((i, j) => $"{i}; {j}") ?? string.Empty;
                
                if (values?.IsValid ?? true) return $"{isValid}; {errCount}";                
                else return $"{isValid}; {errCount}; {errors}";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string ToStringEx(this IQueryCollection values)
        {
            try
            {
                if(values.Count == 0) return string.Empty;
                
                return values?.Select(i => $"{i.Key}: {i.Value}")?.Aggregate((i, j) => $"{i}; {j}") ?? string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}