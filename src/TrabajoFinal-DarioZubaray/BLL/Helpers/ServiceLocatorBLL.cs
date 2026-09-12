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
        private static IClassroomMPP _classroomMPP;
        private static ICourseMPP _courseMPP;
        private static IEnrollmentMPP _enrollmentMPP;
        private static IAttendanceMPP _attendanceMPP;
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

        public static IClassroomMPP GetClassroomMPP()
        {
            if (_classroomMPP == null)
            {
                _classroomMPP = new ClassroomMPP();
            }

            return _classroomMPP;
        }

        public static ICourseMPP GetCourseMPP()
        {
            if (_courseMPP == null)
            {
                _courseMPP = new CourseMPP();
            }

            return _courseMPP;
        }

        public static IEnrollmentMPP GetEnrollmentMPP()
        {
            if (_enrollmentMPP == null)
            {
                _enrollmentMPP = new EnrollmentMPP();
            }

            return _enrollmentMPP;
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

        public static Interfaces.IActivityBLL CreateActivityBLL()
        {
            return new ActivityBLL(GetActivityMPP());
        }

        public static IClassroomBLL CreateClassroomBLL()
        {
            return new ClassroomBLL(GetClassroomMPP());
        }

        public static ICourseBLL CreateCourseBLL()
        {
            return new CourseBLL(GetCourseMPP());
        }

        public static IEnrollmentBLL CreateEnrollmentBLL()
        {
            return new EnrollmentBLL(GetEnrollmentMPP(), CreateCourseBLL());
        }

        public static IAttendanceMPP GetAttendanceMPP()
        {
            if (_attendanceMPP == null)
            {
                _attendanceMPP = new AttendanceMPP();
            }

            return _attendanceMPP;
        }

        public static IAttendanceBLL CreateAttendanceBLL()
        {
            return new AttendanceBLL(GetAttendanceMPP());
        }
        #endregion
    }
}
