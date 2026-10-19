using System.Collections.Generic;

using BE.DTOs;

namespace BLL.Interfaces
{
    public interface IIntegrityBLL
    {
        List<IntegrityViolationBE> Scan();

        List<IntegrityViolationBE> GetViolations();

        bool AcceptAndRegenerate(IntegrityViolationBE violation);
    }
}
