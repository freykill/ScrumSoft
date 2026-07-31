using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrumSoft.Domain.Common
{
    /// <summary>Error de regla de negocio. Lo lanza el dominio, la Api lo traduce a un 400.</summary>
    public sealed class DomainException(string mensaje) : Exception(mensaje);
}
