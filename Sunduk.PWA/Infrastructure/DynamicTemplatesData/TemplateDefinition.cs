using System.Collections.Generic;

namespace Sunduk.PWA.Infrastructure.DynamicTemplatesData
{
    public class TemplateDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<VariableDefinition> Variables { get; set; } = [];
        public string Result { get; set; } = string.Empty;
        public bool HasError { get; set; }
        public string Error { get; set; } = string.Empty;

        public void SetCalculationResult(string result, bool hasError, string error)
        {
            Result = result;
            HasError = hasError;
            Error = error;
        }

        public void ClearErrors()
        {
            HasError = false;
            Error = string.Empty;
            foreach (var variable in Variables)
            {
                variable.ClearError();
            }
        }
    }
}