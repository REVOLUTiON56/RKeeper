using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RKeeper.Core.Results;

internal class ResultCommonLogic
{
    private readonly ReadOnlyCollection<IError> _errors;
    public IReadOnlyCollection<IError> Errors
    {
        get
        {
            if (!IsFailure)
            {
                throw new InvalidOperationException("No error for success result");
            }

            return _errors;
        }
    }

    public bool IsSuccess => !IsFailure;
    public bool IsFailure { get; }

    public ResultCommonLogic(bool isFailure, IEnumerable<IError>? errors)
    {
        var list = errors as IList<IError> ?? errors?.ToList() ?? new List<IError>(0);

        if (isFailure)
        {
            if (list.Count == 0)
            {
                throw new ArgumentNullException(nameof(errors), "Didn't provide errors for failure result");
            }

            if (list.GroupBy(x => x.Type).Count() > 1)
            {
                throw new InvalidOperationException("Result can't contains different errors types");
            }
        }

        IsFailure = isFailure;
        _errors = new ReadOnlyCollection<IError>(list);
    }
}
