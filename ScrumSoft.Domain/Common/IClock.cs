using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrumSoft.Domain.Common
{
    /// <summary>Reloj del sistema. Se inyecta para poder fijar la hora en los tests.</summary>
    public interface IClock
    {
        /// <summary>Momento actual en UTC.</summary>
        DateTimeOffset UtcNow { get; }
    }
}
