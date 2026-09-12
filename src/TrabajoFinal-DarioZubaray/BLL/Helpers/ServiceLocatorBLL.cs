using BLL.Interfaces;
using BLL.Services;
using MPP;
using MPP.Interfaces;

namespace BLL.Helpers
{
    public static class ServiceLocatorBLL
    {
        #region Propiedades
        private static IUserMPP _userMPP;
        private static IRoleMPP _roleMPP;
        private static IActivityMPP _activityMPP;
        private static IClassroomMPP _aulaMPP;
        private static ICourseMPP _cursoMPP;
        private static IEnrollmentMPP _inscripcionMPP;
        private static IAttendanceMPP _asistenciaMPP;
        #endregion

        #region Métodos
        public static IUserMPP GetUserMPP()
        {
            if (_userMPP == null)
            {
                _userMPP = new UserMPP();
            }

            return _userMPP;
        }

        public static IRoleMPP GetRoleMPP()
        {
            if (_roleMPP == null)
            {
                _roleMPP = new RoleMPP();
            }

            return _roleMPP;
        }

        public static IActivityMPP GetActivityMPP()
        {
            if (_activityMPP == null)
            {
                _activityMPP = new ActivityMPP();
            }

            return _activityMPP;
        }

        public static IClassroomMPP GetAulaMPP()
        {
            if (_aulaMPP == null)
            {
                _aulaMPP = new ClassroomMPP();
            }

            return _aulaMPP;
        }

        public static ICourseMPP GetCursoMPP()
        {
            if (_cursoMPP == null)
            {
                _cursoMPP = new CourseMPP();
            }

            return _cursoMPP;
        }

        public static IEnrollmentMPP GetInscripcionMPP()
        {
            if (_inscripcionMPP == null)
            {
                _inscripcionMPP = new EnrollmentMPP();
            }

            return _inscripcionMPP;
        }

        public static IAuthBLL CreateAuthBLL()
        {
            return new AuthBLL(GetUserMPP());
        }

        public static IUserBLL CreateUserBLL()
        {
            return new UserBLL(GetUserMPP());
        }

        public static PermissionBLL CreatePermissionBLL()
        {
            return new PermissionBLL(GetRoleMPP());
        }

        public static IRoleBLL CreateRoleBLL()
        {
            return new RoleBLL(GetRoleMPP());
        }

        public static IActivityBLL CreateActivityBLL()
        {
            return new ActivityBLL(GetActivityMPP());
        }

        public static IClassroomBLL CreateAulaBLL()
        {
            return new ClassroomBE(GetAulaMPP());
        }

        public static ICourseBLL CreateCursoBLL()
        {
            return new CourseBLL(GetCursoMPP());
        }

        public static IEnrollmentBLL CreateInscripcionBLL()
        {
            return new EnrollmentBLL(GetInscripcionMPP(), CreateCursoBLL());
        }

        public static IAttendanceMPP GetAsistenciaMPP()
        {
            if (_asistenciaMPP == null)
            {
                _asistenciaMPP = new AttendanceMPP();
            }

            return _asistenciaMPP;
        }

        public static IAttendanceBLL CreateAsistenciaBLL()
        {
            return new AttendanceBLL(GetAsistenciaMPP());
        }
        #endregion
    }
}
