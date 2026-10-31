using System.Collections.Generic;

using BE.Entities;

namespace MPP.Interfaces
{
    public interface IEvaluationMPP
    {
        int Save(EvaluationBE evaluation);
        void SaveGrades(int evaluationId, List<GradeBE> grades);
        List<EvaluationBE> FindByCourseId(int courseId);
        List<GradeBE> FindGradesByEvaluationId(int evaluationId);
    }
}
