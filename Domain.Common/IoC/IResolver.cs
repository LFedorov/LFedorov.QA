using System;

namespace Domain.Common.IoC
{
    public interface IResolver
    {
        object Resolve(Type type);
    }
}