using System.Collections.Generic;

using BE.Entities;

namespace BLL.Interfaces
{
    public interface IEvaluationBLL
    {
        bool Save(EvaluationBE evaluation, List<GradeBE> grades);
        List<EvaluationBE> FindByCourseId(int courseId);
        List<GradeBE> FindGradesByEvaluationId(int evaluationId);
    }
}
