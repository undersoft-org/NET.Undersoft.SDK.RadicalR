using FluentValidation.Results;

namespace RadicalR
{
    public interface ICommand : IDataIO
    {
        long Id { get; set; }

        object[] Keys { get; set; }

        Entity Entity { get; set; }

        object Data { get; set; }

        ValidationResult Result { get; set; }

        bool IsValid { get; }
    }
}