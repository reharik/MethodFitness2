using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MF.Core.Services
{
    public class CompletedAppointment
    {
        public int AppointmentId { get; set; }
        public int ClientId { get; set; }
        public int TrainerId { get; set; }
        public string AppointmentType { get; set; }
        public bool InArrears { get; set; }
    }

    public interface ISessionCompletionService
    {
        /// <summary>
        /// Marks every appointment that has ended as complete, using up one of each
        /// client's unused sessions of that type, or recording a session in arrears
        /// when the client has none.
        /// </summary>
        IList<CompletedAppointment> CompleteDueAppointments();
    }

    /// <summary>
    /// A port of the "sessionManagement" Lambda (github.com/reharik/methodFitLambdas,
    /// v1.0.8), which is what completes sessions in production. The statements,
    /// their order and the values written are the Lambda's; only the string-built
    /// SQL has become parameters. Production still runs the Lambda: nothing calls
    /// this on a schedule.
    /// </summary>
    public class SessionCompletionService : ISessionCompletionService
    {
        // The Lambda stamps its rows as this user and company.
        private const int SystemUserId = 17;
        private const int CompanyId = 1;

        // The Lambda treats "now" as UTC minus five hours (Central Daylight Time) all
        // year. Kept identical here so both behave the same; whether that is right
        // in winter is an open question for the Lambda too.
        private const int HoursBehindUtc = 5;

        private readonly ILogger _logger;

        public SessionCompletionService(ILogger logger)
        {
            _logger = logger;
        }

        public IList<CompletedAppointment> CompleteDueAppointments()
        {
            var now = DateTime.UtcNow.AddHours(-HoursBehindUtc);
            var completed = new List<CompletedAppointment>();
            var connectionString = ConfigurationManager.AppSettings["MethodFitness.sql_server_connection_string"];

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var unresolved = GetUnresolvedAppointments(connection, now);
                _logger.LogInfo("Session completion: {0} appointment/client pair(s) due.", unresolved.Count);

                // Like the Lambda: first everyone who has a session to use, then the ones in arrears.
                var inArrears = new List<CompletedAppointment>();
                foreach (var item in unresolved)
                {
                    var sessionId = FindUnusedSession(connection, item);
                    if (sessionId == null)
                    {
                        inArrears.Add(item);
                        continue;
                    }
                    UseSession(connection, item, sessionId.Value, now);
                    completed.Add(item);
                }

                foreach (var item in inArrears)
                {
                    RecordSessionInArrears(connection, item, now);
                    item.InArrears = true;
                    completed.Add(item);
                }
            }

            _logger.LogInfo("Session completion: {0} completed, {1} of them in arrears.", completed.Count,
                            completed.FindAll(x => x.InArrears).Count);
            return completed;
        }

        private static List<CompletedAppointment> GetUnresolvedAppointments(SqlConnection connection, DateTime now)
        {
            const string sql = @"
select distinct
    a.entityId as AppointmentId,
    ac.ClientId,
    a.TrainerId,
    a.AppointmentType
from Appointment a
    inner join appointment_Client ac on a.EntityId = ac.AppointmentID
        and a.Completed = 0
        and a.EndTime < @now";
            var result = new List<CompletedAppointment>();
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@now", SqlDbType.DateTime2).Value = now;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new CompletedAppointment
                        {
                            AppointmentId = Convert.ToInt32(reader["AppointmentId"]),
                            ClientId = Convert.ToInt32(reader["ClientId"]),
                            TrainerId = Convert.ToInt32(reader["TrainerId"]),
                            AppointmentType = Convert.ToString(reader["AppointmentType"])
                        });
                    }
                }
            }
            return result;
        }

        private static int? FindUnusedSession(SqlConnection connection, CompletedAppointment item)
        {
            const string sql = @"
select top 1 EntityId from session
where ClientID = @clientId
    and AppointmentType = @appointmentType
    and SessionUsed = 0
order by createddate";
            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@clientId", SqlDbType.Int).Value = item.ClientId;
                command.Parameters.Add("@appointmentType", SqlDbType.NVarChar, 255).Value = item.AppointmentType;
                var value = command.ExecuteScalar();
                return value == null || value == DBNull.Value ? (int?)null : Convert.ToInt32(value);
            }
        }

        private static void UseSession(SqlConnection connection, CompletedAppointment item, int sessionId, DateTime now)
        {
            const string sql = @"
update Session set
    SessionUsed = 1,
    appointmentId = @appointmentId,
    trainerId = @trainerId,
    changedDate = @now,
    changedById = @userId
where entityId = @sessionId;

update Appointment set
    Completed = 1,
    ChangedById = @userId,
    ChangedDate = @now
where entityId = @appointmentId;";
            InTransaction(connection, command =>
            {
                command.CommandText = sql;
                command.Parameters.Add("@sessionId", SqlDbType.Int).Value = sessionId;
                AddCommonParameters(command, item, now);
            });
        }

        private static void RecordSessionInArrears(SqlConnection connection, CompletedAppointment item, DateTime now)
        {
            const string sql = @"
insert into Session (
    IsDeleted, CompanyId, Cost, AppointmentType, SessionUsed, TrainerPaid, TrainerCheckNumber, InArrears,
    ClientID, AppointmentId, TrainerId, TrainerVerified, CreatedDate, CreatedById, ChangedDate, ChangedById)
values (
    0, @companyId, 0, @appointmentType, 1, 0, 0, 1,
    @clientId, @appointmentId, @trainerId, 0, @now, @userId, @now, @userId);

update Appointment set
    Completed = 1,
    ChangedById = @userId,
    ChangedDate = @now
where entityId = @appointmentId;";
            InTransaction(connection, command =>
            {
                command.CommandText = sql;
                command.Parameters.Add("@companyId", SqlDbType.Int).Value = CompanyId;
                command.Parameters.Add("@appointmentType", SqlDbType.NVarChar, 255).Value = item.AppointmentType;
                command.Parameters.Add("@clientId", SqlDbType.Int).Value = item.ClientId;
                AddCommonParameters(command, item, now);
            });
        }

        private static void AddCommonParameters(SqlCommand command, CompletedAppointment item, DateTime now)
        {
            command.Parameters.Add("@appointmentId", SqlDbType.Int).Value = item.AppointmentId;
            command.Parameters.Add("@trainerId", SqlDbType.Int).Value = item.TrainerId;
            command.Parameters.Add("@now", SqlDbType.DateTime2).Value = now;
            command.Parameters.Add("@userId", SqlDbType.Int).Value = SystemUserId;
        }

        private static void InTransaction(SqlConnection connection, Action<SqlCommand> prepare)
        {
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        prepare(command);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
