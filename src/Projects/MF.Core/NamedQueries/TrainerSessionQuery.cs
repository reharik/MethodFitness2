using System;
using CC.Core.Html.Grid;

namespace MF.Core.NamedQueries
{
//    public interface ITrainerSessionQuery
//    {
//        IEnumerable<SessionViewDto> Execute(int userId, DateTime? endDate);
//    }
//
//    public class TrainerSessionQuery : ITrainerSessionQuery
//    {
//        private readonly IUnitOfWork _unitOfWork;
//
//        public TrainerSessionQuery(IUnitOfWork unitOfWork)
//        {
//            _unitOfWork = unitOfWork;
//        }
//
//        public IEnumerable<SessionViewDto> Execute(int userId, DateTime? endDate)
//        {
//            Session sessionAlias = null;
//            Client clientAlias = null;
//            Client clientAlias2 = null;
//            SessionViewDto sessionViewDto= null;
//            TrainerClientRate tcrTrainerAlias= null;
//
//
//
//
////            var tcrFuture = _unitOfWork.CurrentSession.QueryOver<Appointment>()
////                                        .JoinAlias(x => x.Clients, () => clientAlias, JoinType.LeftOuterJoin)
////                                        .JoinAlias(() => tcrClientAlias.Client, () => tcrClientAlias, JoinType.LeftOuterJoin)
////                                        .JoinAlias(() => tcrClientAlias.Trainer, () => tcrTrainerAlias, JoinType.LeftOuterJoin)
////                                        .Where(x => tcrTrainerAlias.EntityId == userId)
////                                        .TransformUsing(Transformers.DistinctRootEntity).Future();
//
//
//
//            var userFuture = _unitOfWork.CurrentSession.QueryOver<Appointment>()
//                            .JoinAlias(x => x.Sessions, () => sessionAlias, JoinType.LeftOuterJoin)
//                            .JoinAlias(() => sessionAlias.Client, () => clientAlias, JoinType.LeftOuterJoin)
//                            .Where(x => x.Trainer.EntityId == userId && !sessionAlias.TrainerPaid && x.Date <= endDate)
//                            .SelectList(list=>list
//                                .Select(x => x..EntityId)
//                                .Select( x => clientAlias.FirstName)
//                                .Select( x => clientAlias.LastName)
//                                .Select( x => x.Date).WithAlias(() => sessionViewDto.AppointmentDate)
//                                .Select(x=>x.AppointmentType).WithAlias(() => sessionViewDto.Type)
//                                .Select(x => sessionAlias.InArrears).WithAlias(() => sessionViewDto.InArrears)
//                                ).TransformUsing(Transformers.AliasToBean<SessionViewDto>()).List<SessionViewDto>();
//            
// 
//            return userFuture;
//        }
//    }
    public class SessionViewDto : IGridEnabledClass
    {
        public int EntityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return FirstName + " " + LastName; } }
        public DateTime? AppointmentDate { get; set; }
        public string Type { get; set; }
        public double PricePerSession { get; set; }
        public int TrainerPercentage { get; set; }
        public double TrainerPay { get; set; }
        public bool InArrears { get; set; }
    }
}
