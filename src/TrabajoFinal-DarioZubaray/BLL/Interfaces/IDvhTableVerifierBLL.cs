using System.Collections.Generic;

using BE.DTOs;

namespace BLL.Interfaces
{
    public interface IDvhTableVerifierBLL
    {
        string TableName { get; }
        List<IntegrityViolationBE> FindViolations();
        bool AcceptAndRegenerate(IntegrityViolationBE violation);
    }
}
