using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrumSoft.Application.Common
{
    public interface IUsuarioActual
    {
        Guid Id { get; }

        bool EstaAutenticado { get; }

        bool EsAdministrador { get; }
    }
}
