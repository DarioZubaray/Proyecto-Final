using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

using DAL;
using BE.Entities;
using MPP.Interfaces;

namespace MPP
{
    public class CourseMPP : ICourseMPP
    {
        #region Propiedades
        private AccessDAL _access;
        #endregion

        #region Constructor
        public CourseMPP() : this(null)
        {
        }

        public CourseMPP(string connectionString)
        {
            _access = new AccessDAL(connectionString);
        }
        #endregion

        #region Métodos Públicos
        public bool Save(CourseBE course)
        {
            if (course.Id == 0)
            {
                return Insert(course);
            }

            return Update(course);
        }

        public bool Delete(CourseBE course)
        {
            string query = @"UPDATE courses
                            SET is_active = 0, last_update = @lastUpdate
                            WHERE id = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@id", course.Id),
                new SqlParameter("@lastUpdate", DateTime.Now)
            };

            return _access.Save(query, parameters);
        }

        public CourseBE FindById(int id)
        {
            string query = @"SELECT c.id, c.name, c.description, c.start_date, c.end_date,
                                    c.classroom_id, c.day_of_week, c.start_time, c.end_time,
                                    c.is_active, c.created_at, c.last_update,
                                    cl.name AS classroom_name
                            FROM courses c
                            LEFT JOIN classrooms cl ON cl.id = c.classroom_id
                            WHERE c.id = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@id", id)
            };

            DataTable table = _access.Read(query, parameters);

            if (table.Rows.Count == 0)
            {
                return null;
            }

            CourseBE course = MapCourse(table.Rows[0]);
            course.Teachers = GetTeachersByCourseId(id);
            return course;
        }

        public List<CourseBE> FindAll()
        {
            string query = @"SELECT c.id, c.name, c.description, c.start_date, c.end_date,
                                    c.classroom_id, c.day_of_week, c.start_time, c.end_time,
                                    c.is_active, c.created_at, c.last_update,
                                    cl.name AS classroom_name
                            FROM courses c
                            LEFT JOIN classrooms cl ON cl.id = c.classroom_id
                            WHERE c.is_active = 1
                            ORDER BY c.id";

            var courses = FindMany(query);
            foreach (var course in courses)
            {
                course.Teachers = GetTeachersByCourseId(course.Id);
            }
            return courses;
        }

        public List<CourseBE> FindAllIncludingInactive()
        {
            string query = @"SELECT c.id, c.name, c.description, c.start_date, c.end_date,
                                    c.classroom_id, c.day_of_week, c.start_time, c.end_time,
                                    c.is_active, c.created_at, c.last_update,
                                    cl.name AS classroom_name
                            FROM courses c
                            LEFT JOIN classrooms cl ON cl.id = c.classroom_id
                            WHERE c.end_date >= CAST(GETDATE() AS DATE)
                            ORDER BY c.is_active DESC, c.id";

            var courses = FindMany(query);
            foreach (var course in courses)
            {
                course.Teachers = GetTeachersByCourseId(course.Id);
            }
            return courses;
        }

        public List<CourseBE> FindByName(string name)
        {
            string query = @"SELECT c.id, c.name, c.description, c.start_date, c.end_date,
                                    c.classroom_id, c.day_of_week, c.start_time, c.end_time,
                                    c.is_active, c.created_at, c.last_update,
                                    cl.name AS classroom_name
                            FROM courses c
                            LEFT JOIN classrooms cl ON cl.id = c.classroom_id
                            WHERE c.name LIKE @name
                            ORDER BY c.is_active DESC, c.id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@name", "%" + name + "%")
            };

            var courses = FindMany(query, parameters);
            foreach (var course in courses)
            {
                course.Teachers = GetTeachersByCourseId(course.Id);
            }
            return courses;
        }

        public bool ExistsClassroomOverlap(int classroomId, DateTime startDate, DateTime endDate, int courseIdToExclude, int? dayOfWeek, TimeSpan? startTime, TimeSpan? endTime)
        {
            string query = @"SELECT COUNT(*)
                            FROM courses
                            WHERE classroom_id = @classroomId
                              AND is_active = 1
                              AND id != @courseIdToExclude
                              AND start_date < @endDate
                              AND end_date > @startDate
                              AND (
                                  (day_of_week IS NOT NULL AND @dayOfWeek IS NOT NULL AND day_of_week = @dayOfWeek)
                                  OR (day_of_week IS NULL AND @dayOfWeek IS NULL)
                              )
                              AND (
                                  (start_time IS NOT NULL AND end_time IS NOT NULL AND @startTime IS NOT NULL AND @endTime IS NOT NULL
                                   AND start_time < @endTime AND end_time > @startTime)
                                  OR (start_time IS NULL AND end_time IS NULL AND @startTime IS NULL AND @endTime IS NULL)
                              )";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@classroomId", classroomId),
                new SqlParameter("@startDate", startDate),
                new SqlParameter("@endDate", endDate),
                new SqlParameter("@courseIdToExclude", courseIdToExclude),
                new SqlParameter("@dayOfWeek", (object)dayOfWeek ?? DBNull.Value),
                new SqlParameter("@startTime", startTime.HasValue ? (object)startTime.Value : DBNull.Value),
                new SqlParameter("@endTime", endTime.HasValue ? (object)endTime.Value : DBNull.Value)
            };

            return _access.ReadScalar(query, parameters) > 0;
        }

        public bool SaveTeachers(int courseId, List<int> teacherIds)
        {
            string deleteQuery = @"UPDATE course_teachers
                                  SET is_active = 0
                                  WHERE course_id = @courseId AND is_active = 1";

            SqlParameter[] deleteParams = new SqlParameter[]
            {
                new SqlParameter("@courseId", courseId)
            };

            _access.Save(deleteQuery, deleteParams);

            foreach (int teacherId in teacherIds)
            {
                string insertQuery = @"INSERT INTO course_teachers (course_id, teacher_id, is_active, created_at)
                                      VALUES (@courseId, @teacherId, 1, @createdAt)";
                SqlParameter[] insertParams = new SqlParameter[]
                {
                    new SqlParameter("@courseId", courseId),
                    new SqlParameter("@teacherId", teacherId),
                    new SqlParameter("@createdAt", DateTime.Now)
                };
                _access.Save(insertQuery, insertParams);
            }

            return true;
        }

        public List<UserBE> GetTeachersByCourseId(int courseId)
        {
            string query = @"SELECT u.id, u.user_name, u.password_hash, u.is_active,
                                    u.retries_count, u.last_update, u.created_at,
                                    u.language, u.theme, u.role_id
                            FROM Users u
                            INNER JOIN course_teachers ct ON ct.teacher_id = u.id
                            WHERE ct.course_id = @courseId AND ct.is_active = 1 AND u.is_active = 1";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@courseId", courseId)
            };

            List<UserBE> teachers = new List<UserBE>();
            DataTable table = _access.Read(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                teachers.Add(MapUser(row));
            }

            return teachers;
        }

        public List<CourseBE> FindByTeacherId(int teacherId)
        {
            string query = @"SELECT c.id, c.name, c.description, c.start_date, c.end_date,
                                    c.classroom_id, c.day_of_week, c.start_time, c.end_time,
                                    c.is_active, c.created_at, c.last_update,
                                    cl.name AS classroom_name
                            FROM courses c
                            INNER JOIN course_teachers ct ON ct.course_id = c.id
                            LEFT JOIN classrooms cl ON cl.id = c.classroom_id
                            WHERE ct.teacher_id = @teacherId AND ct.is_active = 1 AND c.is_active = 1
                            ORDER BY c.id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@teacherId", teacherId)
            };

            return FindMany(query, parameters);
        }

        public List<CourseBE> FindByStudentId(int studentId)
        {
            string query = @"SELECT c.id, c.name, c.description, c.start_date, c.end_date,
                                    c.classroom_id, c.day_of_week, c.start_time, c.end_time,
                                    c.is_active, c.created_at, c.last_update,
                                    cl.name AS classroom_name
                            FROM courses c
                            INNER JOIN course_students cs ON cs.course_id = c.id
                            LEFT JOIN classrooms cl ON cl.id = c.classroom_id
                            WHERE cs.student_id = @studentId AND cs.is_active = 1 AND c.is_active = 1
                            ORDER BY c.id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@studentId", studentId)
            };

            return FindMany(query, parameters);
        }

        public bool ExistsTeacherOverlap(int teacherId, int dayOfWeek, TimeSpan startTime, TimeSpan endTime, int courseIdToExclude)
        {
            string query = @"SELECT COUNT(*)
                            FROM course_teachers ct
                            INNER JOIN courses c ON c.id = ct.course_id
                            WHERE ct.teacher_id = @teacherId
                            AND ct.is_active = 1
                            AND c.is_active = 1
                            AND c.day_of_week = @dayOfWeek
                            AND c.start_time < @endTime
                            AND c.end_time > @startTime
                            AND c.id <> @courseIdToExclude";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@teacherId", teacherId),
                new SqlParameter("@dayOfWeek", dayOfWeek),
                new SqlParameter("@startTime", startTime),
                new SqlParameter("@endTime", endTime),
                new SqlParameter("@courseIdToExclude", courseIdToExclude)
            };

            DataTable table = _access.Read(query, parameters);
            return Convert.ToInt32(table.Rows[0][0]) > 0;
        }
        #endregion

        #region Métodos Privados
        private CourseBE MapCourse(DataRow row)
        {
            return new CourseBE
            {
                Id = Convert.ToInt32(row["id"]),
                Name = row["name"].ToString(),
                Description = row["description"] != DBNull.Value ? row["description"].ToString() : null,
                StartDate = (DateTime)row["start_date"],
                EndDate = (DateTime)row["end_date"],
                ClassroomId = row["classroom_id"] != DBNull.Value ? Convert.ToInt32(row["classroom_id"]) : 0,
                ClassroomName = row["classroom_name"] != DBNull.Value ? row["classroom_name"].ToString() : null,
                DayOfWeek = row["day_of_week"] != DBNull.Value ? Convert.ToInt32(row["day_of_week"]) : (int?)null,
                StartTime = row["start_time"] != DBNull.Value ? (TimeSpan?)TimeSpan.Parse(row["start_time"].ToString()) : (TimeSpan?)null,
                EndTime = row["end_time"] != DBNull.Value ? (TimeSpan?)TimeSpan.Parse(row["end_time"].ToString()) : (TimeSpan?)null,
                IsActive = Convert.ToBoolean(row["is_active"]),
                CreatedAt = (DateTime)row["created_at"],
                LastUpdate = (DateTime)row["last_update"]
            };
        }

        private UserBE MapUser(DataRow row)
        {
            return new UserBE
            {
                Id = Convert.ToInt32(row["id"]),
                UserName = row["user_name"].ToString(),
                PasswordHash = row["password_hash"].ToString(),
                IsActive = Convert.ToBoolean(row["is_active"]),
                RetriesCount = Convert.ToInt32(row["retries_count"]),
                LastUpdate = (DateTime)row["last_update"],
                CreatedAt = (DateTime)row["created_at"],
                Language = row["language"].ToString(),
                Theme = row["theme"] != DBNull.Value ? row["theme"].ToString() : "System",
                RoleId = row["role_id"] != DBNull.Value ? Convert.ToInt32(row["role_id"]) : 0
            };
        }

        private SqlParameter[] CreateCourseParameters(CourseBE course)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@name", course.Name),
                new SqlParameter("@description", (object)course.Description ?? DBNull.Value),
                new SqlParameter("@startDate", course.StartDate),
                new SqlParameter("@endDate", course.EndDate),
                new SqlParameter("@classroomId", course.ClassroomId),
                new SqlParameter("@dayOfWeek", (object)course.DayOfWeek ?? DBNull.Value),
                new SqlParameter("@startTime", course.StartTime.HasValue ? (object)course.StartTime.Value : DBNull.Value),
                new SqlParameter("@endTime", course.EndTime.HasValue ? (object)course.EndTime.Value : DBNull.Value),
                new SqlParameter("@isActive", course.IsActive),
                new SqlParameter("@lastUpdate", course.LastUpdate)
            };
        }

        private bool Insert(CourseBE course)
        {
            string query = @"INSERT INTO courses (name, description, start_date, end_date, classroom_id,
                                    day_of_week, start_time, end_time, is_active, created_at, last_update)
                            VALUES (@name, @description, @startDate, @endDate, @classroomId,
                                    @dayOfWeek, @startTime, @endTime, @isActive, @createdAt, @lastUpdate);
                            SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = CreateCourseParameters(course);
            var allParameters = new SqlParameter[parameters.Length + 1];
            parameters.CopyTo(allParameters, 0);
            allParameters[parameters.Length] = new SqlParameter("@createdAt", course.CreatedAt);

            var newId = _access.ReadScalar(query, allParameters);
            if (newId > 0)
            {
                course.Id = newId;
                return true;
            }
            return false;
        }

        private bool Update(CourseBE course)
        {
            string query = @"UPDATE courses
                            SET name = @name,
                                description = @description,
                                start_date = @startDate,
                                end_date = @endDate,
                                classroom_id = @classroomId,
                                day_of_week = @dayOfWeek,
                                start_time = @startTime,
                                end_time = @endTime,
                                is_active = @isActive,
                                last_update = @lastUpdate
                            WHERE id = @id";

            SqlParameter[] parameters = CreateCourseParameters(course);
            var allParameters = new SqlParameter[parameters.Length + 1];
            parameters.CopyTo(allParameters, 0);
            allParameters[parameters.Length] = new SqlParameter("@id", course.Id);

            return _access.Save(query, allParameters);
        }

        private List<CourseBE> FindMany(string query)
        {
            List<CourseBE> courses = new List<CourseBE>();
            DataTable table = _access.Read(query);

            foreach (DataRow row in table.Rows)
            {
                courses.Add(MapCourse(row));
            }

            return courses;
        }

        private List<CourseBE> FindMany(string query, SqlParameter[] parameters)
        {
            List<CourseBE> courses = new List<CourseBE>();
            DataTable table = _access.Read(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                courses.Add(MapCourse(row));
            }

            return courses;
        }
        #endregion
    }
}
