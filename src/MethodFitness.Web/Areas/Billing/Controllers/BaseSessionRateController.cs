using System.Linq;
using System.Web.Mvc;
using CC.Core.CoreViewModelAndDTOs;
using CC.Core.DomainTools;
using CC.Core.Html;
using CC.Core.Services;
using Castle.Components.Validator;
using MethodFitness.Core.Domain;
using MethodFitness.Web.Config;
using MethodFitness.Web.Controllers;

namespace MethodFitness.Web.Areas.Billing.Controllers
{
    public class BaseSessionRateController : MFController
    {
        private readonly IRepository _repository;
        private readonly ISaveEntityService _saveEntityService;

        public BaseSessionRateController(IRepository repository,
            ISaveEntityService saveEntityService)
        {
            _repository = repository;
            _saveEntityService = saveEntityService;
        }

        public ActionResult AddUpdate_Template(ViewModel input)
        {
            return View("AddUpdate", new BaseSessionRateViewModel());
        }

        public ActionResult AddUpdate(ViewModel input)
        {
            var baseSessionRate = _repository.Query<BaseSessionRate>().FirstOrDefault();
            var model = new BaseSessionRateViewModel
                {
                    FullHour = baseSessionRate.FullHour,
                    HalfHour = baseSessionRate.HalfHour,
                    FullHourTenPack = baseSessionRate.FullHourTenPack,
                    HalfHourTenPack = baseSessionRate.HalfHourTenPack,
                    Pair = baseSessionRate.Pair,
                    PairTenPack = baseSessionRate.PairTenPack,
                    _Title = WebLocalizationKeys.BASE_RATES.ToString(),
                    _saveUrl = UrlContext.GetUrlForAction<BaseSessionRateController>(x => x.Save(null))
                };
            return new CustomJsonResult(model);
        }

        public ActionResult Save(BaseSessionRateViewModel input)
        {
            var orig = _repository.Query<BaseSessionRate>().FirstOrDefault();
            orig.FullHour = input.FullHour;
            orig.HalfHour = input.HalfHour;
            orig.FullHourTenPack = input.FullHourTenPack;
            orig.HalfHourTenPack = input.HalfHourTenPack;
            orig.Pair = input.Pair;
            orig.PairTenPack = input.PairTenPack;
            var crudManager = _saveEntityService.ProcessSave(orig);
            var notification = crudManager.Finish();
            return new CustomJsonResult(notification){ ContentType = "text/plain" };
        }
    }

    public class BaseSessionRateViewModel:ViewModel
    {
        [ValidateNonEmpty]
        public virtual double FullHour { get; set; }
        [ValidateNonEmpty]
        public virtual double HalfHour { get; set; }
        [ValidateNonEmpty]
        public virtual double FullHourTenPack { get; set; }
        [ValidateNonEmpty]
        public virtual double HalfHourTenPack { get; set; }
        [ValidateNonEmpty]
        public virtual double Pair { get; set; }
        [ValidateNonEmpty]
        public virtual double PairTenPack { get; set; }
    }
}