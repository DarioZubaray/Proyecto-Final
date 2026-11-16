using System;
using System.Collections.Generic;
using System.Linq;

using BE.Entities;
using BLL.Helpers;
using BLL.Interfaces;
using MPP.Interfaces;

namespace BLL.Services
{
    public class EvaluationBLL : IEvaluationBLL
    {
        #region Propiedades
        private readonly IEvaluationMPP _evaluationMPP;
        private readonly ICourseMPP _courseMPP;
        #endregion

        #region Constructor
        public EvaluationBLL(IEvaluationMPP evaluationMPP, ICourseMPP courseMPP)
        {
            _evaluationMPP = evaluationMPP;
            _courseMPP = courseMPP;
        }
        #endregion

        #region Métodos
        public bool Save(EvaluationBE evaluation, List<GradeBE> grades)
        {
            if (evaluation == null)
            {
                throw new ArgumentNullException(nameof(evaluation));
            }

            if (string.IsNullOrWhiteSpace(evaluation.Name))
            {
                throw new InvalidOperationException("El nombre de la evaluación es obligatorio. (Código: BIZ-013)");
            }

            evaluation.Name = evaluation.Name.Trim();

            if (grades == null)
            {
                grades = new List<GradeBE>();
            }

            foreach (GradeBE grade in grades)
            {
                if (grade.Grade < 0 || grade.Grade > 10)
                {
                    throw new InvalidOperationException("La nota debe estar entre 0 y 10. (Código: BIZ-015)");
                }
            }

            List<CourseBE> teacherCourses = _courseMPP.FindByTeacherId(evaluation.CreatedBy);
            if (!teacherCourses.Any(c => c.Id == evaluation.CourseId))
            {
                throw new InvalidOperationException("El curso seleccionado no le pertenece. (Código: BIZ-014)");
            }

            CourseBE course = teacherCourses.First(c => c.Id == evaluation.CourseId);

            if (evaluation.Date.Date < course.StartDate.Date ||
                evaluation.Date.Date > course.EndDate.Date)
            {
                throw new InvalidOperationException(
                    "La fecha de la evaluación debe estar dentro del período del curso. (Código: BIZ-016)");
            }

            evaluation.CreatedAt = DateTime.Now;
            evaluation.LastUpdate = DateTime.Now;

            int evaluationId = _evaluationMPP.Save(evaluation);
            if (evaluationId <= 0)
            {
                return false;
            }

            evaluation.Id = evaluationId;

            foreach (GradeBE grade in grades)
            {
                grade.EvaluationId = evaluationId;
            }

            _evaluationMPP.SaveGrades(evaluationId, grades);

            if (!DVHHelperBLL.RegenerateEvaluation(_evaluationMPP, evaluationId))
            {
                return false;
            }

            List<GradeBE> savedGrades = _evaluationMPP.FindGradesByEvaluationId(evaluationId)
                ?? new List<GradeBE>();

            foreach (GradeBE grade in savedGrades)
            {
                if (!DVHHelperBLL.Regenerate(_evaluationMPP, grade))
                {
                    return false;
                }
            }

            return true;
        }

        public List<EvaluationBE> FindByCourseId(int courseId)
        {
            return _evaluationMPP.FindByCourseId(courseId);
        }

        public List<GradeBE> FindGradesByEvaluationId(int evaluationId)
        {
            return _evaluationMPP.FindGradesByEvaluationId(evaluationId);
        }
        #endregion
    }
}
